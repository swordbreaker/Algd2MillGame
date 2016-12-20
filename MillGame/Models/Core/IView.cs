using System;

namespace MillGame.Models.Core
{
    /**
     * 
     * @author Christoph Stamm
     * @version 14.9.2010
     *
     */
    public interface IView
    {
        /**
         * Show action and update game board
         * @param s New game state
         * @param a Action
         * @param isComputerAction True, if it is a computer action
         */
        void UpdateBoard(State s, Actions.Action a, bool isComputerAction);

        /**
         * Prepare game board for a new game
         */
        void PrepareBoard();

        /**
         * Set computer player name
         * @param name Computer player name
         */
        void SetComputerName(String name);

        /**
         * Set human player name
         * @param name Human player name
         */
        void SetHumanName(String name);
    }

}
