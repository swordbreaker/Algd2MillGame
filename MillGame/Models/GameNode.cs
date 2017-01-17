using System;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;
using Action = MillGame.Models.Core.Actions.Action;

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


        /// <summary>
        /// Creates the childrens of the Node with alpha–beta pruning
        /// </summary>
        /// <param name="curHeight">current subtree height</param>
        /// <param name="height">height Subtree height</param>
        /// <param name="color">Color of next actions</param>
        /// <param name="root">Subtree root</param>
        /// <param name="rootState">Game state at root</param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns>The </returns>
        public int Create(int curHeight, int height, sbyte color, GameNode root, State rootState, int alpha, int beta)
        {
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
                    var childNode = Create(nextAction, root);
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
                                    var takingNode = Create(takingAction, root);

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
                    var moves = (rootState.JumpingPhase(color)) ? State.TRANSPOSED : State.MOVES[i];
                    foreach (byte to in moves)
                    {
                        if (!rootState.IsValidMove(i, to, color)) continue;
                        var nextAction = new Moving(color, i, to);
                        var childNode = Create(nextAction, root);
                        var newState = rootState.Clone();
                        childNode.Data().Update(newState);
                        if (newState.InMill(to, color))
                        {
                            if (newState.TakingIsPossible(State.OppositeColor(color)))
                            {
                                foreach (byte takingPosition in State.TRANSPOSED)
                                {
                                    if (!newState.IsValidTake(takingPosition, State.OppositeColor(color))) continue;
                                    var takingNode = Create(new Taking(nextAction, takingPosition), root);
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
                                    //UpdateScore(takingNode, v);
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
                            //UpdateScore(childNode, newState.Score());
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
                var maxNode = (GameNode)node.m_children.Peek();
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
                var minNode = (GameNode)node.m_children.Peek();
                if (minNode.m_score > score)
                {
                    node.m_score = minNode.m_score;
                    return;
                }
            }
            node.m_score = score;
        }

        /**
	     * Create new node and add it to this as a child node.
	     * @param a Action
	     * @param score Score
	     */
        public GameNode Add(Action a, int score)
        {
            var node = (this is MinNode) ? (GameNode) new MaxNode(a, score) : new MinNode(a, score);
            node.m_parent = this;
            m_children.Enqueue(node);
            return node;
        }

        public GameNode Add(Action a)
        {
            var node = (this is MinNode) ? (GameNode)new MaxNode(a) : new MinNode(a);
            node.m_parent = this;
            m_children.Enqueue(node);
            return node;
        }

        /// <summary>
        /// Create a new Node as MinNode or MaxNode
        /// </summary>
        /// <param name="a">Action in Node</param>
        /// <param name="root">To determine if it should be MaxNode or MinNode</param>
        /// <returns>The new GameNode</returns>
        public GameNode Create(Action a, GameNode root)
        {
            var node = (root is MinNode) ? (GameNode)new MaxNode(a) : new MinNode(a);
            node.m_parent = this;
            return node;
        }

        /*
         * Removes now unused subtrees
         * O(n)
         */
        public GameNode RemoveUnusedChilds(Action a)
        {
            var node = Create(a, this);
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
            if (this is MinNode)
            {
                return ((MinNode) this).CompareTo(other);
            }
            else
            {
                return ((MaxNode) this).CompareTo(other);
            }
            //this.CompareTo(other);

            //var gameNodeOther = (GameNode)other;
            //return m_score - gameNodeOther.m_score;
        }
    }
}
