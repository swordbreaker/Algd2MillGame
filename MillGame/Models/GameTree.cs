using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;
using Action = MillGame.Models.Core.Actions.Action;

namespace MillGame.Models
{
    /**
     * Black is a minimum player (minimizer); White is a maximum player (maximizer).
     * 
     * @author christoph.stamm
     * @version 14.9.2010
     *
     */
    public class GameTree : IGameTree
    {
        Tree<IAction> m_gameTree;
        GameNode m_currentNode;

        /**
        * Creates a new game tree: the first action is white, on the next level plays black.
        * White is a maximizer, black is a minimizer. 
        * @param pa null if computer plays white, first action if human plays white
        */
        public void Create(int height, Placing pa)
        {
            if(pa == null)
            {
                // First move made by computer
                m_currentNode = new GameNode(ComputerPlayer());
            } else
            {
                m_currentNode = new GameNode(pa);
            }
            m_gameTree = new Tree<IAction>(m_currentNode);
        }

        /**
	     * Return current game state.
	     * @return Current game state.
	     */
        public State CurrentState()
        {
            throw new NotImplementedException();
        }

        /**
	     * Update tree (remove subtrees), current node, and current state for the human player
	     * @param a Action
	     */
        public void HumanPlayer(Action a)
        {
            throw new NotImplementedException();
        }

        /**
         * Compute best next node at current node, update tree (remove subtrees), current node, 
         * and current state for the computer player
         * @return Best action or null
         */
        public Action ComputerPlayer()
        {
            throw new NotImplementedException();
        }

        public void Print()
        {
            throw new NotImplementedException();
        }

        public int Size()
        {
            throw new NotImplementedException();
        }
    }
}
