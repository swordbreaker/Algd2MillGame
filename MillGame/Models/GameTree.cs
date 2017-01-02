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
     * @author Yanick Schraner
     * @version 14.9.2010
     *
     */
    public class GameTree : Tree<IAction>, IGameTree
    {
        private new GameNode m_root;
        private GameNode m_currentNode;
        private State m_baseState;
        private int m_height;

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
            m_height = height;
            m_root = m_currentNode;
        }

        /**
	     * Return current game state.
	     * @return Current game state.
	     */
        public State CurrentState()
        {
            return m_currentNode.ComputeState(m_baseState, m_root);
        }

        /**
	     * Update tree (remove subtrees), current node, and current state for the human player
	     * @param a Action
	     */
        public void HumanPlayer(Action a)
        {
            m_currentNode = m_currentNode.RemoveUnusedChilds(a);
        }

        /**
         * Compute best next node at current node, update tree (remove subtrees), current node, 
         * and current state for the computer player
         * @return Best action or null
         */
        public Action ComputerPlayer()
        {
            int maxScore = int.MinValue;
            Action bestAction = null;
            foreach(GameNode child in m_currentNode.m_children)
            {
                if (maxScore < child.Score())
                {
                    maxScore = child.Score();
                    bestAction = child.Data();
                }
            }
            if (bestAction != null) m_currentNode = m_currentNode.RemoveUnusedChilds(bestAction);
            return bestAction;
        }
    }
}
