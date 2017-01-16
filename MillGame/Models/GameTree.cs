using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private bool _firstTurn;
        private bool inMovingPhase;

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
                var placing = new Placing(IController.WHITE, 10);
                m_currentNode = new MaxNode(placing);
                _firstTurn = true;
                //m_currentNode = new MaxNode(ComputerPlayer());
            } else
            {
                m_currentNode = new MaxNode(pa);
            }
            m_root = m_currentNode;
            m_currentState = new State();
            m_currentNode.Data().Update(m_currentState);
            m_root.Create(0, height, IController.BLACK, m_root, m_currentState);
            m_height = height;
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
            m_currentState = m_currentNode.ComputeState(m_currentState, m_currentNode);
            //m_currentState = m_currentNode.ComputeState(m_currentState, oldNode);

        }

        /**
         * Compute best next node at current node, update tree (remove subtrees), current node, 
         * and current state for the computer player
         * @return Best action or null
         */
        public Action ComputerPlayer()
        {
            if (!inMovingPhase && m_currentState.MovingPhase(IController.WHITE) && m_currentState.MovingPhase(IController.BLACK))
            {
                inMovingPhase = true;
                m_height = m_height*2;
            }
            var opColor = State.OppositeColor(m_currentNode.Data().Color());
            m_currentNode.Create(0, m_height, opColor, m_currentNode, m_currentState);

            if (_firstTurn)
            {
                _firstTurn = false;
                return m_currentNode.Data();
            }

            Action bestAction = null;

            if (m_currentNode.Data().Color() == IController.BLACK)
            {
                int maxScore = int.MinValue;
                
                foreach (GameNode child in m_currentNode.m_children)
                {
                    if (maxScore < child.Score())
                    {
                        maxScore = child.Score();
                        bestAction = child.Data();
                    }
                }
            }
            else
            {
                int minScore = int.MaxValue;
                foreach (GameNode child in m_currentNode.m_children)
                {
                    if (minScore > child.Score())
                    {
                        minScore = child.Score();
                        bestAction = child.Data();
                    }
                }
            }

            if (bestAction != null)
            {
                m_currentNode = m_currentNode.RemoveUnusedChilds(bestAction);
                m_currentState = m_currentNode.ComputeState(m_currentState, m_currentNode);
                Debug.WriteLine("Score is: " + m_currentNode.Score());
                Debug.WriteLine("COMPUTER");
                Debug.WriteLine(m_currentState.Infomations.ToString((byte)m_currentNode.Data().Color()));
                Debug.WriteLine("PLAYER");
                Debug.WriteLine(m_currentState.Infomations.ToString((byte)State.OppositeColor(m_currentNode.Data().Color())));
                //m_currentState = m_currentNode.ComputeState(m_currentState, oldNode);
                var oppColor = State.OppositeColor(bestAction.Color());
                m_currentNode.Create(0, 1, oppColor, m_currentNode, m_currentState);
            }
            return bestAction;
        }
    }
}
