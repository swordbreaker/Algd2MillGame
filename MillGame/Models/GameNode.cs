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
 * 
 * @author christoph.stamm
 * @author yanick.schraner
 * @version 04.01.2017
 *
 */
    public class GameNode : Node<Core.Actions.Action>, IGameNode
    {
        protected int m_score;

        public GameNode(Core.Actions.Action action) : base(action)
        {
        }

        public GameNode(Core.Actions.Action action, int score) : base(action)
        {
            m_score = score;
        }

        /**
        * Create new nodes recursively.
        * Approximate O(24*(height-curHeight))
        * @param curHeight current subtree height
        * @param height Subtree height
        * @param color Color of next actions
        * @param root Subtree root
        * @param rootState Game state at root
        * @return Number of created nodes
        */
        public int Create(int curHeight, int height, sbyte color, GameNode root, State rootState)
        {
            int numberOfCreatedNodes = 0;
            if (curHeight != height && !rootState.Finished())
            {
                if (rootState.PlacingPhase(color) || rootState.JumpingPhase(color))
                {
                    foreach (byte position in State.TRANSPOSED)
                    {
                        if (rootState.IsValidPlace(position, color))
                        {
                            GameNode childNode = root.Add(new Placing(color, position), color);
                            State newState = rootState.clone();
                            childNode.Data().Update(newState);
                            numberOfCreatedNodes++;
                            numberOfCreatedNodes += Create(curHeight + 1, height, State.OppositeColor(color), childNode, newState);
                        }
                    }
                }
                else if (rootState.MovingPhase(color))
                {
                    for (int i = 0; i < State.TRANSPOSED.Length; i++)
                    {
                        foreach (byte to in State.MOVES[i])
                        {
                            if (rootState.IsValidMove(State.TRANSPOSED[i], to, color))
                            {
                                GameNode childNode = root.Add(new Moving(color, State.TRANSPOSED[i], to), color);
                                State newState = rootState.clone();
                                childNode.Data().Update(newState);
                                numberOfCreatedNodes++;
                                numberOfCreatedNodes += Create(curHeight + 1, height, State.OppositeColor(color), childNode, newState);
                            }
                        }
                    }
                }
            }

            return numberOfCreatedNodes;
        }

        /**
	     * Create new minimizer or maximizer node and add it to this as a child node.
	     * @param a Action
	     * @param score Score
	     */
        public GameNode Add(Action a, int score, sbyte color)
        {
            GameNode node = null;
            if(color == IController.BLACK)
            {
                // Black is minimizer
                node = new MinNode(a, score);
                node.m_parent = this;
                m_children.Enqueue(node);
            } else
            {
                // White is maximizer
                node = new MaxNode(a, score);
                node.m_parent = this;
                m_children.Enqueue(node);
            }
            return node;
        }

        /**
	     * Create new minimizer or maximizer node and add it to this as a child node.
	     * @param a Action
	     * @param score Score
	     */
        public GameNode Add(Action a, sbyte color)
        {
            GameNode node = null;
            if (color == IController.BLACK)
            {
                // Black is minimizer
                node = new MinNode(a);
                node.m_parent = this;
                m_children.Enqueue(node);
            }
            else
            {
                // White is maximizer
                node = new MaxNode(a);
                node.m_parent = this;
                m_children.Enqueue(node);
            }
            return node;
        }

        /**
	     * Create new node and add it to this as a child node.
	     * @param a Action
	     * @param score Score
	     */
        public GameNode Add(Action a, int score)
        {
            // This method doesn't different between minimizer and maximizer!
            GameNode node = new GameNode(a, score);
            node.m_parent = this;
            m_children.Enqueue(node);
            return node;
        }

        public GameNode Add(Action a)
        {
            // This method doesn't different between minimizer and maximizer!
            GameNode node = new GameNode(a);
            node.m_parent = this;
            m_children.Enqueue(node);
            return node;
        }

        /*
         * Removes now unused subtrees
         * O(n)
         */
        public GameNode RemoveUnusedChilds(Action a)
        {
            GameNode node = null;

            //for (int i = 0; i < m_children.Count; i++)
            //{
            //    var curr = m_children.Deque();
            //    if (curr.Data() == a)
            //    {
            //        node = (GameNode)curr;
            //    }
            //}

            foreach (GameNode child in m_children)
            {
                if (child.Data().Equals(a))
                {
                    node = child;
                }
            }

            m_children.Clear();
            m_children.Enqueue(node);
            //foreach (GameNode child in m_children)
            //{
            //    if (child.Data() != a)
            //    {
            //        m_children.Remove(child);
            //    } else
            //    {
            //        node = child;
            //    }
            //}
            return node;
        }

        /**
	     * Compute game state at this node
	     * @param s Game state at given node v
	     * @param v Game node v must be parent of this
	     * @return Game state at this node
	     */
        public State ComputeState(State s, GameNode v)
        {
            State computedState = s.clone();
            v.Data().Update(computedState);
            return computedState;
        }

        public int GetWinnerScore()
        {
            // Will be taken per default from minimizer or maximizer. It shouldn't happen that a GameNode itself will be created
            return 0;
        }

        public int Score()
        {
            return m_score;
        }

        /**
         * If this score > other score  then return > 0
         * If this score < other score  then return < 0
         * If this score == other score  then return 0
         */
        public override int CompareTo(Node<Action> other)
        {
            GameNode gameNodeOther = (GameNode) other;
            return m_score - gameNodeOther.m_score;
        }
    }
}
