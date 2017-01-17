using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MillGame.Models.Core;

namespace MillGame.Models
{
    /**
     * Client used in client-server games.
     * 
     * @author  Christoph Stamm
     * @version  14.9.2010
     * 
     */

    public class GameClient
    {
        private const int s_port = 18181;
        //private StreamReader m_inFromServer;
        private Stream m_outToServer;
        private Game m_game;
        private String m_playerName;

        public Socket Socket { get; private set; }
        public IController Controller { get; private set; }

        /**
         * Constructor
         * @param controller
         */

        public GameClient(IController controller)
        {
            Controller = controller;
        }

        /**
         * Open connection to server
         * @return true if a connection has been established
         * @throws IOException
         */

        public bool OpenConnection()
        {
            try
            {
                String hostName;

                try
                {
                    // read server address and player name
                    var reader = new StreamReader(new FileStream("config.txt", FileMode.CreateNew));
                    hostName = reader.ReadLine();
                    m_playerName = reader.ReadLine();
                    reader.Close();
                }
                catch (FileNotFoundException)
                {
                    Debug.WriteLine("Warning: config.txt not found.");
                    hostName = "localhost";
                    m_playerName = "Miller";
                }

                var ipe = new IPEndPoint(IPAddress.Parse(hostName), s_port);
                Socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // removes blanks in player's name
                StringBuilder sb = new StringBuilder(m_playerName);
                for (int i = 0; i < sb.Length; i++)
                {
                    if (sb[i] == ' ') sb[i] = '_';
                }

                // send registration message
                Socket.Send(Encoding.ASCII.GetBytes("REGISTER " + sb.ToString() + '\n'));

                // read answer from server
                byte[] buffer = new byte[1024];
                int iRx = Socket.Receive(buffer);
                char[] chars = new char[iRx];

                var d = Encoding.ASCII.GetDecoder();
                d.GetChars(buffer, 0, iRx, chars, 0);

                var s = new string(chars);
                if (s.Equals("OK"))
                {
                    Controller.SetPlayerName(m_playerName);
                    // registration successfully done
                    return true;
                }
            }
            catch (SocketException)
            {
            }

            return false;
        }

        /**
         * Start a new game. The connection has to be established in advanced.
         */

        public void StartGame()
        {
            if (!(Socket != null && Socket.Connected && Socket.IsBound)) throw new Exception();
            Controller.SetPlayerName(m_playerName);
            m_game = new Game(this);
            m_game.Run();
        }

        /**
         * Stop a running game.
         * @param winner The winner of the game.
         * @throws IOException
         */

        public void StopGame(int winner)
        {
            if (!(Socket != null && Socket.Connected && Socket.IsBound)) throw new Exception();

            switch (winner)
            {
                case IController.BLACK:
                    Socket.Send(Encoding.ASCII.GetBytes("FINISH BLACK\n"));
                    break;
                case IController.WHITE:
                    Socket.Send(Encoding.ASCII.GetBytes("FINISH WHITE\n"));
                    break;
                default:
                    Socket.Send(Encoding.ASCII.GetBytes("FINISH\n"));
                    break;
            }
        }

        /**
         * Close an open connection.
         * @throws IOException
         */

        public void CloseConnection()
        {
            if (Socket != null)
            {
                Socket.Send(Encoding.ASCII.GetBytes("Stop\n"));
                Socket.Close();
            }
        }
    }

    /**
     * Parallel client thread.
     */

    class Game
    {
        private IController.Status m_status;
        public Exception m_error;

        private GameClient m_client;

        public Game(GameClient gc)
        {
            m_client = gc;
        }

        public void Run()
        {
            Debug.WriteLine("Thread starts");

            try
            {
                do
                {
                    m_status = ReadAndWait();
                } while (m_status == IController.Status.OK || m_status == IController.Status.CLOSEDMILL);
            }
            catch (IOException ex)
            {
                m_error = ex;
            }
            Debug.WriteLine("Thread terminates");
        }

        private IController.Status ReadAndWait()
        {
            if (m_client.Socket == null) throw new Exception();

            var buffer = new byte[1024];
            int iRx = m_client.Socket.Receive(buffer);
            char[] chars = new char[iRx];

            var d = Encoding.ASCII.GetDecoder();
            d.GetChars(buffer, 0, iRx, chars, 0);

            var s = new string(chars);

            String[]
                token = s.Split(' ');

            if (token[0].Equals("PLAY"))
            {
                // I have to play: compute move
                var a = m_client.Controller.Compute();
                var status = m_client.Controller.GetStatus();
                if (status == IController.Status.FINISHED)
                {
                   m_client.StopGame(m_client.Controller.GetWinner());
                }
                else if (status == IController.Status.OK && a != null)
                {
                    // send move to server
                    a.Writeln(new NetworkStream(m_client.Socket, true));
                }
                return status;
            }
            else if (token[0].Equals("WHITE"))
            {
                // I will play white stones and open the game
                m_client.Controller.SetStarter(true);
                return IController.Status.OK;
            }
            else if (token[0].Equals("BLACK"))
            {
                // I will play black stones
                m_client.Controller.SetStarter(false);
                return IController.Status.OK;
            }
            else if (token[0].Equals("STOP"))
            {
                // close connection
                m_client.Controller.CloseConnection();
                return IController.Status.FINISHED;
            }
            else if (token[0].Equals("FINISH"))
            {
                m_client.StopGame(IController.NONE);
                return IController.Status.FINISHED;
            }
            else
            {
                // opponent plays action a
                var a = Core.Actions.Action.Readln(token);
                if (a != null)
                {
                    IController.Status status = m_client.Controller.Play(a);
                    if (status == IController.Status.FINISHED) m_client.StopGame(m_client.Controller.GetWinner());
                    return status;
                }
                else
                {
                    return IController.Status.INVALIDACTION;
                }
            }
        }
    }
}