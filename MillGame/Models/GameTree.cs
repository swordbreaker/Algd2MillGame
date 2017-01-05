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
    public class GameTree : Tree<Action>, IGameTree
    {
        private new GameNode m_root;
        private GameNode m_currentNode;
        private State m_currentState;
        private int m_height;

        /**
        * Creates a new game tree: the first action is white, on the next level plays black.
        * White is a maximizer, black is a minimizer. 
        * @param pa null if computer plays white, first action if human plays white
        */
        public void Create(int height, Placing pa)
        {
            // TODO Create Tree
            if (pa == null)
            {
                // First move made by computer
                m_currentNode = new MaxNode(ComputerPlayer());
            } else
            {
                m_currentNode = new MaxNode(pa);
            }
            m_currentState = new State();
            m_currentNode.Data().Update(m_currentState);
            m_height = height;
            m_root = m_currentNode;
            m_root.Create(0, height, IController.BLACK, m_root, m_currentState);
        }

        /**
	     * Return current game state.
	     * @return Current game state.
	     */
        public State CurrentState()
        {
            return m_currentState;
        }

        /**
	     * Update tree (remove subtrees), current node, and current state for the human player
	     * @param a Action
	     */
        public void HumanPlayer(Action a)
        {
            GameNode oldNode = m_currentNode;
            m_currentNode = m_currentNode.RemoveUnusedChilds(a);
            m_currentState = m_currentNode.ComputeState(m_currentState, oldNode);
            m_currentNode.Create(0, m_height, m_currentNode.Data().Color(), m_currentNode, m_currentState);
        }

        /**
         * Compute best next node at current node, update tree (remove subtrees), current node, 
         * and current state for the computer player
         * @return Best action or null
         */
        public Action ComputerPlayer()
        {
            //TODO what should the bot do at the first turn?
            if(m_currentNode == null) return new Placing(1, 10);
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
            if (bestAction != null)
            {
                GameNode oldNode = m_currentNode;
                m_currentNode = m_currentNode.RemoveUnusedChilds(bestAction);
                m_currentState = m_currentNode.ComputeState(m_currentState, oldNode);
                m_currentNode.Create(0, m_height, m_currentNode.Data().Color(), m_currentNode, m_currentState);
            }
            return bestAction;
        }
    }
}
