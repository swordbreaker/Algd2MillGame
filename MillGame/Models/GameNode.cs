using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;
using Action = MillGame.Models.Core.Actions.Action;
using System.Diagnostics;

namespace MillGame.Models
{
    /**
    * 
    * @author christoph.stamm
    * @author yanick.schraner
    * @author tobias.bollinger
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

        public int Create(int curHeight, int height, sbyte color, GameNode root, State rootState)
        {
            var alpha = int.MinValue;
            var beta = int.MaxValue;
            return Create(curHeight, height, color, root, rootState, alpha, beta);
        }

        public int Create(int curHeight, int height, sbyte color, GameNode root, State rootState, int alpha, int beta)
        {
            int numberOfCreatedNodes = 0;
            if (curHeight == height || rootState.Finished())
            {
                return rootState.Score();
            }
            var v = (color == IController.BLACK) ? int.MinValue : int.MaxValue;
            if (rootState.PlacingPhase(color))
            {
                foreach (byte position in State.TRANSPOSED)
                {
                    if (!rootState.IsValidPlace(position, color)) continue;
                    var nextAction = new Placing(color, position);
                    var newState = rootState.Clone();
                    nextAction.Update(newState);
                    var childNode = Create(nextAction);
                    numberOfCreatedNodes++;
                    if (newState.InMill(position, color))
                    {
                        if (newState.TakingIsPossible(State.OppositeColor(color)))
                        {
                            foreach (byte takingPosition in State.TRANSPOSED)
                            {
                                if (newState.IsValidTake(takingPosition, State.OppositeColor(color)))
                                {
                                    var takeState = rootState.Clone();
                                    var takingAction = new Taking(nextAction, takingPosition);
                                    takingAction.Update(takeState);
                                    var takingNode = Create(takingAction);

                                    //Minimizer
                                    if (color == IController.BLACK)
                                    {
                                        v = Math.Max(v,
                                            Create(curHeight + 1, height, State.OppositeColor(color), takingNode,
                                                takeState, alpha, beta));

                                        alpha = Math.Max(alpha, v);
                                    }
                                    else //Maximizer
                                    {
                                        v = Math.Min(v, Create(curHeight + 1, height, State.OppositeColor(color), takingNode,
                                            takeState, alpha, beta));
                                        beta = Math.Min(beta, v);
                                    }
                                           
                                    UpdateScore(takingNode, takeState.Score());
                                    root.m_children.Enqueue(takingNode);
                                    if (beta <= alpha) break;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Minimizer
                        if (color == IController.BLACK)
                        {
                            v = Math.Max(v,
                                Create(curHeight + 1, height, State.OppositeColor(color), childNode,
                                    newState, alpha, beta));

                            alpha = Math.Max(alpha, v);
                        }
                        else //Maximizer
                        {
                            v = Math.Min(v, Create(curHeight + 1, height, State.OppositeColor(color), childNode,
                                newState, alpha, beta));
                            beta = Math.Min(beta, v);
                        }

                        UpdateScore(childNode, newState.Score());
                        root.m_children.Enqueue(childNode);
                        if (beta <= alpha) break;
                    }
                }
            }
            else if (rootState.MovingPhase(color) || rootState.JumpingPhase(color))
            {
                for (byte i = 0; i < rootState.Board.Length; i++)
                {
                    foreach (byte to in State.MOVES[i])
                    {
                        if (!rootState.IsValidMove(i, to, color)) continue;
                        var nextAction = new Moving(color, i, to);
                        var childNode = Create(nextAction);
                        var newState = rootState.Clone();
                        numberOfCreatedNodes++;
                        childNode.Data().Update(newState);
                        if (newState.InMill(to, color))
                        {
                            if (newState.TakingIsPossible(State.OppositeColor(color)))
                            {
                                foreach (byte takingPosition in State.TRANSPOSED)
                                {
                                    if (!newState.IsValidTake(takingPosition, State.OppositeColor(color))) continue;
                                    var takingNode = Create(new Taking(nextAction, takingPosition));
                                    var takeState = rootState.Clone();
                                    takingNode.Data().Update(takeState);

                                    //Minimizer
                                    if (color == IController.BLACK)
                                    {
                                        v = Math.Max(v,
                                            Create(curHeight + 1, height, State.OppositeColor(color), takingNode,
                                                takeState, alpha, beta));

                                        alpha = Math.Max(alpha, v);
                                    }
                                    else //Maximizer
                                    {
                                        v = Math.Min(v, Create(curHeight + 1, height, State.OppositeColor(color), takingNode,
                                            takeState, alpha, beta));
                                        beta = Math.Min(beta, v);
                                    }

                                    UpdateScore(takingNode, takeState.Score());
                                    root.m_children.Enqueue(takingNode);
                                    if (beta <= alpha) break;
                                }
                            }
                        }
                        else
                        {
                            //Minimizer
                            if (color == IController.BLACK)
                            {
                                v = Math.Max(v,
                                    Create(curHeight + 1, height, State.OppositeColor(color), childNode,
                                        newState, alpha, beta));

                                alpha = Math.Max(alpha, v);
                            }
                            else //Maximizer
                            {
                                v = Math.Min(v, Create(curHeight + 1, height, State.OppositeColor(color), childNode,
                                    newState, alpha, beta));
                                beta = Math.Min(beta, v);
                            }

                            UpdateScore(childNode, newState.Score());
                            root.m_children.Enqueue(childNode);
                            if (beta <= alpha) break;
                        }
                    }
                }
            }
            return v;
        }

        private void UpdateScore(GameNode node, int score)
        {
            var minNode = node as MinNode;
            var maxNode = node as MaxNode;

            if (minNode != null)
            {
                UpdateScore(minNode, score);
            }
            else
            {
                UpdateScore(maxNode, score);
            }
        }

        private void UpdateScore(MinNode node, int score)
        {
            if (!node.m_children.IsEmpty)
            {
                var maxNode = (GameNode) node.m_children.Peek();
                if (maxNode.m_score < score)
                {
                    node.m_score = maxNode.m_score;
                    return;
                }
            }
            node.m_score = score;
        }
        private void UpdateScore(MaxNode node, int score)
        {
            if (!node.m_children.IsEmpty)
            {
                var minNode = (GameNode) node.m_children.Peek();
                if (minNode.m_score > score)
                {
                    node.m_score = minNode.m_score;
                    return;
                }
            }
            node.m_score = score;
        }

        /**
	     * Create new minimizer or maximizer node and add it to this as a child node.
	     * @param a Action
	     * @param score Score
	     */
        public GameNode Add(Action a, int score, sbyte color)
        {
            GameNode node = null;
            if (color == IController.BLACK)
            {
                // Black is minimizer
                node = new MinNode(a, score);
                node.m_parent = this;
                m_children.Enqueue(node);
            }
            else
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
            var isMinimizer = this is MinNode;
            // This method doesn't different between minimizer and maximizer!
            GameNode node = (isMinimizer) ? (GameNode) new MinNode(a, score) : new MaxNode(a, score);
            //GameNode node = new GameNode(a, score);
            node.m_parent = this;
            m_children.Enqueue(node);
            return node;
        }

        public GameNode Add(Action a)
        {
            var isMinimizer = this is MinNode;
            // This method doesn't different between minimizer and maximizer!
            GameNode node = (isMinimizer) ? (GameNode)new MinNode(a) : new MaxNode(a);
            //var node = new GameNode(a);
            node.m_parent = this;
            m_children.Enqueue(node);
            return node;
        }

        /// <summary>
        /// Create a new Node as MinNode or MaxNode
        /// </summary>
        /// <param name="a">Action in Node</param>
        /// <returns>The new GameNode</returns>
        public GameNode Create(Action a)
        {
            var isMinimizer = this is MinNode;
            var node = (isMinimizer) ? (GameNode)new MinNode(a) : new MaxNode(a);
            node.m_parent = this;
            return node;
        }

        /*
         * Removes now unused subtrees
         * O(n)
         */
        public GameNode RemoveUnusedChilds(Action a)
        {
            var node = Create(a);
            m_children.Clear();
            m_children.Enqueue(node);
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
            var computedState = s.Clone();
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
            var gameNodeOther = (GameNode)other;
            return m_score - gameNodeOther.m_score;
        }
    }
}
