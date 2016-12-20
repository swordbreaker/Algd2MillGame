using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillGame.Models.Core
{
    /**
     * 
     * @author christoph.stamm
     * @version 14.9.2010
     *
     */
    public abstract class IController
    {
        // colors
        public const sbyte NONE = -1;
        public const sbyte BLACK = 0;
        public const sbyte WHITE = 1;

        // return status flag used in controller actions
        public enum Status { OK, INVALIDACTION, CLOSEDMILL, FINISHED };
        // everything is ok
        // action is invalid
        // a mill have been closed
        // game over

        /**
         * Determines the player who opens the game.
         * @param computer If computer is true, then this computer player will open the game.
         */
        public abstract void SetStarter(bool computer);

        /**
         * Sets a name for this computer player.
         * @param name Computer player name
         */
        public abstract void SetPlayerName(String name);

        /**
         * Play action a and return status
         * @param a Action
         */
        public abstract Status Play(Core.Actions.Action a);

        /**
         * Compute new move and return it
         * @return computed move
         */
        public abstract Core.Actions.Action Compute();

        /**
         * Invokes the parallel computer player just for one move
         */
        public abstract void ComputeAsync();

        /**
         * Use this method to check the status after compute()
         * @return current controller status
         */
        public abstract Status GetStatus();

        /**
         * Returns the winner of the game
         * @return NONE: no winner, BLACK: black, WHITE: white
         */
        public abstract int GetWinner();

        /**
         * The server asks for closing the connection to it
         */
        public abstract void CloseConnection();

        /**
         * Return color of human player.
         * @return Color of human player or IController.NONE if not initialized yet.
         */
        public abstract sbyte HumanColor();

        /**
         * Return true if a human player plays in this game.
         * @return false if both players are computers using a game server.
         */
        public abstract bool HumanPlayer();

    }

}
