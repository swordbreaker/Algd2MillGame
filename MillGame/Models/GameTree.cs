using System.Diagnostics;
using System.Linq;
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

        /**
        * Creates a new game tree: the first action is white, on the next level plays black.
        * White is a maximizer, black is a minimizer. 
        * @param pa null if computer plays white, first action if human plays white
        */
        public void Create(int height, Placing pa)
        {
            if (pa == null)
            {
                // First move made by computer
                var placing = new Placing(IController.WHITE, 10);
                m_currentNode = new MaxNode(placing);
                _firstTurn = true;
            }
            else
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
            m_currentNode = m_currentNode.RemoveUnusedChilds(a);
            m_currentState = m_currentNode.ComputeState(m_currentState, m_currentNode);
        }

        /**
         * Compute best next node at current node, update tree (remove subtrees), current node, 
         * and current state for the computer player
         * @return Best action or null
         */
        public Action ComputerPlayer()
        {
            var height = m_height;
            if (m_currentState.JumpingPhase(IController.BLACK) || m_currentState.JumpingPhase(IController.WHITE))
            {
                height = 4;
            }
            var opColor = State.OppositeColor(m_currentNode.Data().Color());
            m_currentNode.Create(0, height, opColor, m_currentNode, m_currentState);

            if (_firstTurn)
            {
                _firstTurn = false;
                return m_currentNode.Data();
            }
            if (m_currentNode.m_children.IsEmpty) return null;
            Action bestAction = null;

            bestAction = m_currentNode.m_children.Peek().Data();

            if (bestAction != null)
            {
                Debug.WriteLine("Score is: " + ((GameNode)m_currentNode.m_children.Peek()).Score());
                Debug.WriteLine("COMPUTER");
                Debug.WriteLine(m_currentState.Infomations.ToString((byte)m_currentNode.Data().Color()));
                Debug.WriteLine("PLAYER");
                Debug.WriteLine(m_currentState.Infomations.ToString((byte)State.OppositeColor(m_currentNode.Data().Color())));

                m_currentNode = m_currentNode.RemoveUnusedChilds(bestAction);
                m_currentState = m_currentNode.ComputeState(m_currentState, m_currentNode);
                var oppColor = State.OppositeColor(bestAction.Color());
                m_currentNode.Create(0, 1, oppColor, m_currentNode, m_currentState);
            }
            return bestAction;
        }
    }
}
