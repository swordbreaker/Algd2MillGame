using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;

namespace MillGame.Models
{
    /**
     * The game controller controls the game control flow.
     * The control flow is the following:
     * - choose randomly the player who begins the game
     * - create game tree
     * - start loop
     * - if computer plays: choose best known action from game tree
     * - if human plays: get action from user interface
     * - update game tree and game state with last action
     * - loop until game has been finished
     * 
     * @author  Christoph Stamm
     * @version  14.9.2010
     */
    public class Controller : IController
    {
        // constants
        public static int TREEDEPTH = 2;        // relative game tree height measured in full-moves
        public static bool VERBOSE = false;  // print additional output
                                             // testing
        public static int[] s_scores = new int[State.NPOS];

        private IView m_view;           // GUI
        private IGameTree m_gameTree;   // game tree
        private sbyte m_humanColor;      // IController.WHITE or IController.BLACK
        private bool m_humanStarts;  // human starts the game
        private GameClient m_gc;        // client/server module
        private bool m_serverGame;   // server sends human moves
        private ComputerPlayer m_compi; // asynchronous computer player used in human game

        /**
         * Constructor
         * @param view Graphical user interface
         */
        public Controller(IView view)
        {
            m_view = view;
            m_humanColor = IController.NONE;
            m_serverGame = false;
        }

        /**
         * Start a new game between a human and a computer player
         */
        public void StartHumanGame()
        {
            if (m_serverGame) StopServerGame();

            // set player's name
            m_view.SetComputerName("Computer");
            m_view.SetHumanName("Player");

            // start computer player
            m_compi = new ComputerPlayer(this);

            // choose beginning player
            bool b;

            if (m_humanColor == IController.NONE)
            {
                b = new Random().NextDouble() < 0.5;
            }
            else
            {
                // computer will begin every second game
                b = (m_humanColor == IController.WHITE);
            }

            SetStarter(b);
            Task.Run(() => m_compi.Run());

            //m_compi.Start();

            if (b)
            {
                m_compi.Play();
            }
        }

        /**
         * Start a new game between two computer players using a game server
         */
        public void StartServerGame()
        {
            if (!m_serverGame && m_compi != null) m_compi.Finish();

            // start client thread
            m_gc.StartGame();
            m_view.PrepareBoard();
            m_serverGame = true;
        }

        /**
         * Stop a game server game
         */
        public void StopServerGame()
        {
            // stop client thread
            try
            {
                m_gc.StopGame((m_humanColor == IController.NONE) ? IController.WHITE : m_humanColor);
            }
            catch (IOException ex)
            {
                if (VERBOSE) Debug.WriteLine("Error: IO exeception.\n" + ex.Message);
            }
            m_serverGame = false;
        }

        /**
         * Connect to a game server.
         * @return Returns true if the connection has been established
         */
        public bool ConnectToServer()
        {
            bool retValue = false;
            m_gc = new GameClient(this);

            // the server will decide who will open the game
            m_humanColor = IController.NONE;

            if (VERBOSE) Debug.WriteLine("Client tries to connect to server.");
            try
            {
                if (m_gc.OpenConnection())
                {
                    if (VERBOSE) Debug.WriteLine("Connection established, registration done, and game initialized.");
                    retValue = true;
                }
                else
                {
                    if (VERBOSE) Debug.WriteLine("Connection or registration failed.");
                }

            }
            catch (IOException ex)
            {
                if (VERBOSE) Debug.WriteLine("Error: IO exeception.\n" + ex.Message);
            }

            return retValue;
        }

        /**
         * Set player who opens the game.
         * called by parallel thread in a server game
         */
        public override void SetStarter(bool b)
        {
            if (b)
            {
                // computer starts
                if (VERBOSE) Debug.WriteLine("Computer starts");
                m_humanColor = IController.BLACK;
                m_humanStarts = false;
            }
            else
            {
                // human starts
                if (VERBOSE) Debug.WriteLine("Human starts");
                m_humanColor = IController.WHITE;
                m_humanStarts = true;
            }

            // refresh gui: must be called after beginning player have been chosen
            m_view.PrepareBoard();

            // create new game tree
            m_gameTree = new GameTree();
            if (b)
            {
                // computer will open the game
                m_gameTree.Create(TREEDEPTH, null);
                if (VERBOSE) Debug.WriteLine("\tgame tree created; tree size: " + m_gameTree.Size());
                //m_gameTree.print();
            }
        }

        /**
         * Set names of players
         */
        public override void SetPlayerName(String name)
        {
            m_view.SetComputerName(name);
            m_view.SetHumanName("Opponent");
        }

        /**
         * Exit application.
         */
        public void Exit()
        {
            if (m_serverGame) CloseConnection();
            App.Current.Shutdown();
        }

        /**
         * Return color of human player.
         * @return Color of human player or IController.NONE if not initialized yet.
         */
        public override sbyte HumanColor()
        {
            return m_humanColor;
        }

        /**
         * Return true if human player opens the game.
         * @return True if human player starts.
         */
        public bool HumanStarts()
        {
            return m_humanStarts;
        }

        /**
         * Human plays action a
         */
        public override Status Play(Core.Actions.Action a)
        {
            Contract.Assert(a != null);
            Status status;

            var pm = a as ActionPM;
            if (pm != null)
            {
                status = Human(pm);
            }
            else if (a is Taking)
            {
                status = Human((Taking)a);
            }
            else
            {
                Contract.Assert(false);
                //assert false;
                status = Status.INVALIDACTION;
            }
            return status;
        }

        /**
         * Play human player action 
         * @param a Action
         * @return status flag
         */
        private Status Human(ActionPM a)
        {
            State s = m_gameTree.CurrentState();

            // play human action
            if (s == null)
            {
                if (a is Placing)
                {
                    Contract.Assert(m_humanColor == IController.WHITE, "wrong human player color");
                    m_gameTree.Create(TREEDEPTH, (Placing)a);
                    s = m_gameTree.CurrentState();
                    if (VERBOSE) Debug.WriteLine("Human has played\n\ttree size: " + m_gameTree.Size());
                    //m_gameTree.print();

                }
                else
                {
                    m_view.UpdateBoard(m_gameTree.CurrentState(), a, false);
                    return Status.INVALIDACTION;
                }
            }
            else
            {
                // check if a is a valid human player action
                if (a.IsValid(s))
                {
                    var sCopy = s.clone();

                    // update temporary state with user action
                    a.Update(sCopy);

                    // check if a mill has been closed
                    if (sCopy.InMill(a.EndPosition, a.Color()))
                    {
                        // action is not yet played, because it is part of a taking action
                        if (VERBOSE) Debug.WriteLine("Human closed mill\n\ttree size: " + m_gameTree.Size());
                        // redraw game board
                        m_view.UpdateBoard(sCopy, a, false);
                        return Status.CLOSEDMILL;
                    }
                    else
                    {
                        // play human player action a
                        m_gameTree.HumanPlayer(a);
                        if (VERBOSE) Debug.WriteLine("Human has played\n\ttree size: " + m_gameTree.Size());
                        //m_gameTree.print();
                    }
                }
                else
                {
                    if (VERBOSE) Debug.WriteLine("Human played an invalid action");
                    m_view.UpdateBoard(m_gameTree.CurrentState(), a, false);
                    return Status.INVALIDACTION;
                }
            }

            if (s.Finished())
            {
                if (VERBOSE) Debug.WriteLine("Human has won");
                m_view.UpdateBoard(m_gameTree.CurrentState(), a, false);
                return Status.FINISHED;
            }
            else
            {
                m_view.UpdateBoard(m_gameTree.CurrentState(), a, false);
                return Status.OK;
            }
        }

        /**
         * Play human taking action
         * @param a Action
         * @return Status flag
         */
        private Status Human(Taking a)
        {
            State s = m_gameTree.CurrentState();

            // human take action
            if (s == null)
            {
                m_view.UpdateBoard(m_gameTree.CurrentState(), a, false);
                return Status.INVALIDACTION;
            }
            else
            {
                // first check if a taking action is possible at all
                if (s.TakingIsPossible(State.OppositeColor(a.Color())))
                {
                    // now check if a is a valid taking action
                    if (a.IsValid(s))
                    {
                        // play human player action a
                        m_gameTree.HumanPlayer(a);
                        if (VERBOSE) Debug.WriteLine("Human has played\n\ttree size: " + m_gameTree.Size());
                        //m_gameTree.print();

                        m_view.UpdateBoard(m_gameTree.CurrentState(), a, false);
                        if (s.Finished())
                        {
                            if (VERBOSE) Debug.WriteLine("Human has won");
                            return Status.FINISHED;
                        }
                        else
                        {
                            return Status.OK;
                        }
                    }
                    else
                    {
                        // ActionPM part of a is valid, just the taking is invalid
                        State sCopy = s.clone();

                        // update state with user action
                        a.Action.Update(sCopy);

                        // redraw game board
                        m_view.UpdateBoard(sCopy, a.Action, false);
                        return Status.INVALIDACTION;
                    }
                }
                else
                {
                    m_view.UpdateBoard(m_gameTree.CurrentState(), a, false);
                    return Status.OK;
                }
            }
        }

        /**
         * Invokes the parallel computer player just for one move
         */
        public override void ComputeAsync()
        {
            Contract.Assert(!m_serverGame && m_compi != null);
            m_compi.Play();
        }

        /**
         * Play computer player action
         * @return Status flag
         */
        public override Core.Actions.Action Compute()
        {
            // compute computer player action
            Core.Actions.Action a = m_gameTree.ComputerPlayer();
            if (VERBOSE) Debug.WriteLine("Computer has played\n\ttree size: " + m_gameTree.Size());
            //m_gameTree.print();	

            // redraw game board: current game tree state is the state after computer played
            m_view.UpdateBoard(m_gameTree.CurrentState(), a, true);

            return a;
        }

        /**
         * @return Returns current controller status.
         */
        public override Status GetStatus()
        {
            if (m_gameTree.CurrentState().Finished())
            {
                if (VERBOSE) Debug.WriteLine("Game has been finished");
                return Status.FINISHED;
            }
            else
            {
                return Status.OK;
            }
        }

        /**
         * Returns the winner of the game
         * @return NONE: no winner, BLACK: black, WHITE: white
         */
        public override int GetWinner()
        {
            return (m_gameTree != null && m_gameTree.CurrentState() != null) ? m_gameTree.CurrentState().Winner : NONE;
        }

        /**
         * The server asks for closing its connection
         */
        public override void CloseConnection()
        {
            try
            {
                m_gc.CloseConnection();
                if (VERBOSE) Debug.WriteLine("Connection closed");
            }
            catch (IOException ex)
            {
                if (VERBOSE) Debug.WriteLine("Error: IO exeception.\n" + ex.Message);
            }
        }

        public override bool HumanPlayer()
        {
            return !m_serverGame;
        }
    }
}
