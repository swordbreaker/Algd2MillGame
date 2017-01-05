using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using System.Windows;
using MillGame.Models.Core.Actions;

namespace MillGame.Models.Core
{
    /**
     * 
     * @author Christoph Stamm
     * @version 16.7.2009
     *
     * board positions
     * ---------------
     * 00       01       02
     *    03    04    05
     *       06 07 08
     * 09 10 11    12 13 14
     *       15 16 17
     *    18    19    20
     * 21       22       23      
     *   
     */
    public class State
    {
        // scores
        public const int WHITEWINS = int.MaxValue;
        public const int BLACKWINS = int.MinValue;

        // positions
        public const sbyte INVALID = -1;  // invalid position
        public const byte NPOS = 24;     // number of board positions

        // transposed board positions
        public static readonly byte[] TRANSPOSED = new byte[] { 0, 9, 21, 3, 10, 18, 6, 11, 15, 1, 4, 7, 16, 19, 22, 8, 12, 17, 5, 13, 20, 2, 14, 23 };

        // valid moves for each board position
        public static readonly byte[][] MOVES = new byte[][]
        {
            new byte[]{ 1, 9 },
            new byte[]{ 0, 2, 4 },
            new byte[]{ 1, 14 },
            new byte[]{ 4, 10 },
            new byte[]{ 1, 3, 5, 7 },
            new byte[]{ 4, 13 },
            new byte[]{ 7, 11 },
            new byte[]{ 4, 6, 8 },
            new byte[]{ 7, 12 },
            new byte[]{ 0, 10, 21 },
            new byte[]{ 3, 9, 11, 18 },
            new byte[]{ 6, 10, 15 },
            new byte[]{ 8, 13, 17 },
            new byte[]{ 5, 12, 14, 20 },
            new byte[]{ 2, 13, 23 },
            new byte[]{ 11, 16 },
            new byte[]{ 15, 17, 19 },
            new byte[]{ 12, 16 },
            new byte[]{ 10, 19 },
            new byte[]{ 16, 18, 20, 22 },
            new byte[]{ 13, 19 },
            new byte[]{ 9, 22 },
            new byte[]{ 19, 21, 23 },
            new byte[]{ 14, 22 }
        };

        // board coordinates for each board position
        public static readonly Point[] BOARD = {
        new Point(0, 0),
        new Point(3, 0),
        new Point(6, 0),
        new Point(1, 1),
        new Point(3, 1),
        new Point(5, 1),
        new Point(2, 2),
        new Point(3, 2),
        new Point(4, 2),
        new Point(0, 3),
        new Point(1, 3),
        new Point(2, 3),
        new Point(4, 3),
        new Point(5, 3),
        new Point(6, 3),
        new Point(2, 4),
        new Point(3, 4),
        new Point(4, 4),
        new Point(1, 5),
        new Point(3, 5),
        new Point(5, 5),
        new Point(0, 6),
        new Point(3, 6),
        new Point(6, 6)

    };

        // class methods

        /**
         * Return opposite player's color
         */
        public static sbyte OppositeColor(sbyte color)
        {
            if (!(color == IController.WHITE || color == IController.BLACK)) throw new Exception();
            //Contract.Requires<ArgumentException>(color == IController.WHITE || color == IController.BLACK);
            //assert color == IController.WHITE || color == IController.BLACK : "wrong color";
            return (sbyte)(IController.WHITE - color);
        }

        // instance variables
        private sbyte[] m_board = new sbyte[NPOS];    // valid positions 0..23
        private sbyte[] m_stonesOnBoard = { 0, 0 };  // number of stones on board
        private sbyte[] m_unplacedStones = { 9, 9 }; // number of unplaced stones
        private sbyte m_winner = IController.NONE;               // IController.NONE, IController.WHITE, IController.BLACK	

        public State()
        {
            // initialize board: all positions are empty
            for (int i = 0; i < NPOS; i++)
            {
                m_board[i] = IController.NONE;
            }
        }

        // instance methods

        /**
         * Creates deep copy.
         */
        public State clone()
        {
            State s = new State();
            s.m_board = (sbyte[])m_board.Clone();
            s.m_stonesOnBoard = (sbyte[])m_stonesOnBoard.Clone();
            s.m_unplacedStones = (sbyte[])m_unplacedStones.Clone();
            s.m_winner = m_winner;
            return s;
        }

        /**
         * Return number of unplaced stones
         * @param color color Player's color
         * @return number of unplaced stones
         */
        public int UnplacedStones(byte color)
        {
            if (!(color == IController.WHITE || color == IController.BLACK)) throw new Exception();
            //Contract.Requires<ArgumentException>(color == IController.WHITE || color == IController.BLACK);
            return m_unplacedStones[color];
        }

        /**
         * Return true during placing phase
         * @param color Player's color
         * @return True if the player is in its placing phase
         */
        public bool PlacingPhase(sbyte color)
        {
            if (!(color == IController.WHITE || color == IController.BLACK)) throw new Exception();
            //Contract.Requires<ArgumentException>(color == IController.WHITE || color == IController.BLACK);
            return m_unplacedStones[color] > 0;
        }

        /**
         * Return true during moving phase
         * @param color Player's color
         * @return True if the player is in its moving phase
         */
        public bool MovingPhase(sbyte color)
        {
            if (!(color == IController.WHITE || color == IController.BLACK)) throw new Exception();
            //Contract.Requires<ArgumentException>(color == IController.WHITE || color == IController.BLACK);

            return m_unplacedStones[color] == 0;
        }

        /**
         * Return true during jumping phase
         * @param color Player's color
         * @return True if the player is in its jumping phase
         */
        public bool JumpingPhase(sbyte color)
        {
            if (!(color == IController.WHITE || color == IController.BLACK)) throw new Exception();
            //Contract.Requires<ArgumentException>(color == IController.WHITE || color == IController.BLACK);
            return MovingPhase(color) && m_stonesOnBoard[color] == 3;
        }

        /**
         * Set winner
         * @param color Color of winner
         */
        public void SetWinner(sbyte color)
        {
            if (!(color == IController.WHITE || color == IController.BLACK)) throw new Exception();
            //Contract.Requires<ArgumentException>(color == IController.WHITE || color == IController.BLACK);
            m_winner = color;
        }

        /**
         * Return true if the game has been finished
         * @return True if the game has been finished
         */
        public bool Finished()
        {
            return m_winner != IController.NONE;
        }

        /**
         * Return winner's color or IController.NONE
         * @return Color of the winner
         */
        public sbyte Winner => m_winner;

        /**
         * Return stone color at given position or IController.NONE
         * @param pos Board position
         * @return Stone color at position pos or IController.NONE
         */
        public sbyte color(sbyte pos)
        {
            if (!(pos >= 0 && pos < State.NPOS)) throw new Exception();
            //Contract.Requires<ArgumentException>(pos >= 0 && pos < State.NPOS);
            //assert pos >= 0 && pos < State.NPOS : "wrong board position";

            return m_board[pos];
        }

        /**
         * Return true if a stone of given color is part of a mill at given position
         * @param pos Position
         * @param color Color
         * @return True if a stone of given color is part of a mill at given position
         */
        public bool InMill(int pos, sbyte color)
        {
            if (!(pos >= 0 && pos < State.NPOS)) throw new Exception("wrong board position");
            if (!(color == IController.WHITE || color == IController.BLACK || color == IController.NONE)) throw new Exception("wrong color");

            //Contract.Requires<ArgumentException>(pos >= 0 && pos < State.NPOS, "wrong board position");
            //Contract.Requires<ArgumentException>(color == IController.WHITE || color == IController.BLACK || color == IController.NONE, "wrong color");
            //assert pos >= 0 && pos < State.NPOS : "wrong board position";
            //assert color == IController.WHITE || color == IController.BLACK || color == IController.NONE : "wrong color";

            if (color == IController.NONE) return false;

            // horizontal mills
            var p1 = pos - (pos % 3);
            var p2 = p1 + 1;
            var p3 = p2 + 1;

            if (m_board[p1] == color && m_board[p2] == color && m_board[p3] == color)
            {
                return true;
            }

            // vertical mills
            int t = TRANSPOSED[pos];
            p1 = t - (t % 3);
            p2 = p1 + 1;
            p3 = p2 + 1;

            if (m_board[TRANSPOSED[p1]] == color && m_board[TRANSPOSED[p2]] == color && m_board[TRANSPOSED[p3]] == color)
            {
                return true;
            }

            return false;
        }

        /**
         * Return true if taking a stone of given color is possible at all
         * @param color Color of stone to be taken
         * @return True if taking a stone of given color is possible
         */
        public bool TakingIsPossible(sbyte color)
        {
            if (JumpingPhase(color))
            {
                return true;
            }
            else
            {
                // placing or moving phase
                for (byte pos = 0; pos < NPOS; pos++)
                {
                    if (m_board[pos] == color && !InMill(pos, color)) return true;
                }
                return false;
            }
        }

        /**
         * Checks if a placing is possible
         * @param pos Position
         * @param color Color
         * @return True if placing is possible
         */
        public bool IsValidPlace(byte pos, sbyte color)
        {
            if (!(pos >= 0 && pos < State.NPOS)) throw new Exception("wrong board position");
            if (!(color == IController.WHITE || color == IController.BLACK)) throw new Exception("wrong color");
            //Contract.Requires<ArgumentException>(pos >= 0 && pos < State.NPOS, "wrong board position");
            //Contract.Requires<ArgumentException>(color == IController.WHITE || color == IController.BLACK, "wrong color");
            //assert pos >= 0 && pos < State.NPOS : "wrong board position";
            //assert color == IController.WHITE || color == IController.BLACK : "wrong color";

            return PlacingPhase(color) && m_board[pos] == IController.NONE;
        }

        /**
         * Place stone s on board
         * @param a Action
         */
        public void Update(Placing a)
        {
            if (a == null) throw new Exception("action is null");
            //Contract.Requires<ArgumentException>(a != null, "action is null");

            byte pos = a.EndPosition;
            sbyte color = a.Color();

            if (!IsValidPlace(pos, color)) throw new Exception("invalid action");
            //Contract.Requires<ArgumentException>(IsValidPlace(pos, color), "invalid action");


            m_board[pos] = color;
            m_unplacedStones[color]--;
            m_stonesOnBoard[color]++;
        }

        /**
         * Checks if a move is possible
         * @param from Start position
         * @param to End position
         * @param color Color
         * @return True if mode is possible
         */
        public bool IsValidMove(byte from, byte to, sbyte color)
        {
            if (!(from >= 0 && from < State.NPOS)) throw new Exception("wrong board position");
            if (!(to >= 0 && to < State.NPOS)) throw new Exception("wrong board position");
            if (!(color == IController.WHITE || color == IController.BLACK)) throw new Exception("wrong color");

            //Contract.Requires<ArgumentException>(from >= 0 && from < State.NPOS, "wrong board position");
            //Contract.Requires<ArgumentException>(to >= 0 && to < State.NPOS, "wrong board position");
            //Contract.Requires<ArgumentException>(color == IController.WHITE || color == IController.BLACK, "wrong color");

            if (MovingPhase(color) && from != to && m_board[from] == color && m_board[to] == IController.NONE)
            {
                if (m_stonesOnBoard[color] > 3)
                {
                    int i = 0;
                    int len = MOVES[from].Length;
                    while (i < len && MOVES[from][i] != to) i++;
                    return i < len;
                }
                else
                {
                    // jumping allowed
                    return true;
                }
            }
            return false;
        }

        /**
         * Move stone on board
         * @param a Action
         */

        public void Update(Moving a)
        {
            if (a == null) throw new Exception("action is null");
            //Contract.Requires<ArgumentException>(a != null, "action is null");

            byte from = a.StartPosition;
            byte to = a.EndPosition;
            sbyte color = a.Color();

            if(!IsValidMove(from, to, color)) throw new Exception("invalid action");
            //Contract.Requires<ArgumentException>(IsValidMove(from, to, color), "invalid action");

            m_board[from] = IController.NONE;
            m_board[to] = color;
        }

        /**
         * Checks if a take at given position of given color is possible
         * @param pos Position where a stone will be taken
         * @param color Color of a stone to be taken
         * @return True if action is possible
         */
        public bool IsValidTake(byte pos, sbyte color)
        {
            if (!(pos >= 0 && pos < State.NPOS)) throw new Exception("wrong board position");
            if (!(color == IController.WHITE || color == IController.BLACK)) throw new Exception("wrong color");
            //Contract.Requires<ArgumentException>(pos >= 0 && pos < State.NPOS, "wrong board position");
            //Contract.Requires<ArgumentException>(color == IController.WHITE || color == IController.BLACK, "wrong color");


            bool valid = false;

            if (m_board[pos] == color)
            {
                if (MovingPhase(color))
                {
                    if (m_stonesOnBoard[color] > 3)
                    {
                        // moving phase
                        valid = !InMill(pos, color);
                    }
                    else
                    {
                        // jumping phase
                        valid = true;
                    }
                }
                else
                {
                    // start phase
                    valid = !InMill(pos, color);
                }
            }
            return valid;
        }

        /**
         * Take stone and update winner
         * @param a Action
         */
        public void Update(Taking a)
        {
            if (a == null) throw new Exception("action is null");
            //Contract.Requires<ArgumentException>(a != null, "action is null");

            byte pos = a.TakePosition;
            var color = a.TakeColor; // color of taken stone

            if (!IsValidTake(pos, color)) throw new Exception("invalid action");
            //Contract.Requires<ArgumentException>(IsValidTake(pos, color), "invalid action");

            m_board[pos] = IController.NONE;
            m_stonesOnBoard[color]--;

            if (MovingPhase(color) && m_stonesOnBoard[color] < 3) m_winner = a.Color();
        }

        /**
         * ASCII Art of game board
         */

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < m_board.Length; i++)
            {

                if (i == 1 || i == 2 || i == 22 || i == 23 || i == 6 || i == 15)
                {
                    str.Append("      ");
                }
                else if (i == 3 || i == 4 || i == 5 || i == 20 || i == 19 || i == 18 || i == 12)
                {
                    str.Append("   ");
                }

                if (m_board[i] == IController.BLACK)
                {
                    str.Append(" b ");
                }
                else if (m_board[i] == IController.WHITE)
                {
                    str.Append(" w ");
                }
                else
                {
                    str.Append(" - ");
                }

                if ((i + 1) % 3 == 0 && i != 11)
                {
                    str.Append("\r\n");
                }
            }

            return str.ToString();
        }

        /**
         * Compute score of this game state: Black is a minimizer, White a maximizer.
         * If this state has already a winner, then one of the predefined values BLACKWINS or
         * WHITEWINS should be returned.
         * @return Score of this game state
         */
        public int Score()
        {
            sbyte winner = Winner;

            if (winner != IController.NONE)
            {
                // there is a winner
                return (winner == IController.BLACK) ? BLACKWINS : WHITEWINS;
            }
            else
            {
                // compute score
                throw new NotImplementedException();
            }
        }
    }

}
