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
    * @version 04.01.2017
    *
    */
    public class GameNode : Node<Core.Actions.Action>, IGameNode
    {
        protected int m_score;
        protected int m_alpha;
        protected int m_beta;

        public GameNode(Core.Actions.Action action) : base(action)
        {
            m_alpha = int.MinValue;
            m_beta = int.MaxValue;
        }

        public GameNode(Core.Actions.Action action, int score) : base(action)
        {
            m_score = score;
            m_alpha = int.MinValue;
            m_beta = int.MaxValue;
        }

        public GameNode(Core.Actions.Action action, int score, int alpha, int beta) : base(action)
        {
            m_score = score;
            m_alpha = alpha;
            m_beta = beta;
        }

        public int Create(int curHeight, int height, sbyte color, GameNode root, State rootState)
        {
            return Create(curHeight, height, color, root, rootState, int.MinValue, int.MaxValue);
        }

        public int Create(int curHeight, int height, sbyte color, GameNode root, State rootState, int alpha = int.MinValue, int beta = int.MaxValue)
        {
            int numberOfCreatedNodes = 0;
            if (curHeight != height && !rootState.Finished())
            {
                if (rootState.PlacingPhase(color))
                {
                    foreach (byte position in State.TRANSPOSED)
                    {
                        if (rootState.IsValidPlace(position, color))
                        {
                            ActionPM nextAction = new Placing(color, position);
                            var childNode = root.Add(nextAction, color);
                            var newState = rootState.Clone();
                            numberOfCreatedNodes++;
                            childNode.Data().Update(newState);
                            if (newState.InMill(position, color))
                            {
                                if (newState.TakingIsPossible(State.OppositeColor(color)))
                                {
                                    foreach (byte takingPosition in State.TRANSPOSED)
                                    {
                                        if (newState.IsValidTake(takingPosition, State.OppositeColor(color)))
                                        {
                                            var takingNode = root.Add(new Taking(nextAction, takingPosition));
                                            var takeState = rootState.Clone();
                                            takingNode.Data().Update(takeState);
                                            numberOfCreatedNodes += Create(curHeight + 1, height, State.OppositeColor(color), takingNode, takeState, takingNode.m_alpha, takingNode.m_beta);
                                            if (curHeight + 1 == height)
                                            {
                                                takingNode.m_score = takeState.Score();
                                            }
                                            else
                                            {
                                                if (takingNode.Data().Color() == IController.BLACK)
                                                {
                                                    int maxScore = int.MinValue;

                                                    foreach (GameNode child in takingNode.m_children)
                                                    {
                                                        if (maxScore < child.Score())
                                                        {
                                                            maxScore = child.Score();
                                                        }
                                                    }
                                                    takingNode.m_score = maxScore;
                                                }
                                                else
                                                {
                                                    int minScore = int.MaxValue;
                                                    foreach (GameNode child in takingNode.m_children)
                                                    {
                                                        if (minScore > child.Score())
                                                        {
                                                            minScore = child.Score();
                                                        }
                                                    }
                                                    takingNode.m_score = minScore;
                                                }
                                            }
                                            // Alpha Beta Pruning
                                            if (color == IController.BLACK)
                                            {
                                                // Black is minimizer
                                                if (takeState.Score() < takingNode.m_alpha)
                                                {
                                                    // Alpha cut
                                                    return numberOfCreatedNodes;
                                                }
                                            }
                                            else
                                            {
                                                // White is maximizer
                                                if (takeState.Score() > takingNode.m_beta)
                                                {
                                                    // Beta Cut
                                                    return numberOfCreatedNodes;
                                                }
                                            }
                                        }
                                    }
                                    childNode.Remove();
                                }
                            }
                            else
                            {
                                numberOfCreatedNodes += Create(curHeight + 1, height, State.OppositeColor(color), childNode, newState, childNode.m_alpha, childNode.m_beta);
                                if (curHeight + 1 == height)
                                {
                                    childNode.m_score = newState.Score();
                                }
                                else
                                {
                                    if (childNode.Data().Color() == IController.BLACK)
                                    {
                                        int maxScore = int.MinValue;

                                        foreach (GameNode child in childNode.m_children)
                                        {
                                            if (maxScore < child.Score())
                                            {
                                                maxScore = child.Score();
                                            }
                                        }
                                        childNode.m_score = maxScore;
                                    }
                                    else
                                    {
                                        int minScore = int.MaxValue;
                                        foreach (GameNode child in childNode.m_children)
                                        {
                                            if (minScore > child.Score())
                                            {
                                                minScore = child.Score();
                                            }
                                        }
                                        childNode.m_score = minScore;
                                    }
                                }
                                // Alpha Beta Pruning
                                if (color == IController.BLACK)
                                {
                                    // Black is minimizer
                                    if (newState.Score() < childNode.m_alpha)
                                    {
                                        // Alpha cut
                                        return numberOfCreatedNodes;
                                    }
                                }
                                else
                                {
                                    // White is maximizer
                                    if (newState.Score() > childNode.m_beta)
                                    {
                                        // Beta Cut
                                        return numberOfCreatedNodes;
                                    }
                                }
                            }
                            // Set siblings alpha or beta value
                            if (color == IController.BLACK)
                            {
                                // Black ist minimizer
                                alpha = childNode.Score();
                            }
                            else
                            {
                                // White is maximizer
                                beta = childNode.Score();
                            }
                        }
                    }
                }
                else if (rootState.MovingPhase(color) || rootState.JumpingPhase(color))
                {
                    for (byte i = 0; i < rootState.Board.Length; i++)
                    {
                        foreach (byte to in State.MOVES[i])
                        {
                            if (rootState.IsValidMove(i, to, color))
                            {
                                ActionPM nextAction = new Moving(color, i, to);
                                GameNode childNode = root.Add(nextAction, color);
                                State newState = rootState.Clone();
                                numberOfCreatedNodes++;
                                childNode.Data().Update(newState);
                                if (newState.InMill(to, color))
                                {
                                    //Debug.WriteLine("Create: In Mill detected");
                                    if (newState.TakingIsPossible(State.OppositeColor(color)))
                                    {
                                        //Debug.WriteLine("Create: Taking is possible");
                                        foreach (byte takingPosition in State.TRANSPOSED)
                                        {
                                            if (newState.IsValidTake(takingPosition, State.OppositeColor(color)))
                                            {
                                                //Debug.WriteLine("Create: Valid taking move detected");
                                                var takingNode = root.Add(new Taking(nextAction, takingPosition));
                                                var takeState = rootState.Clone();
                                                takingNode.Data().Update(takeState);
                                                numberOfCreatedNodes += Create(curHeight + 1, height, State.OppositeColor(color), takingNode, takeState, alpha, beta);
                                                if (curHeight + 1 == height)
                                                {
                                                    takingNode.m_score = takeState.Score();
                                                }
                                                else
                                                {
                                                    if (takingNode.Data().Color() == IController.BLACK)
                                                    {
                                                        int maxScore = int.MinValue;

                                                        foreach (GameNode child in takingNode.m_children)
                                                        {
                                                            if (maxScore < child.Score())
                                                            {
                                                                maxScore = child.Score();
                                                            }
                                                        }
                                                        takingNode.m_score = maxScore;
                                                    }
                                                    else
                                                    {
                                                        int minScore = int.MaxValue;
                                                        foreach (GameNode child in takingNode.m_children)
                                                        {
                                                            if (minScore > child.Score())
                                                            {
                                                                minScore = child.Score();
                                                            }
                                                        }
                                                        takingNode.m_score = minScore;
                                                    }
                                                }
                                                // Alpha Beta Pruning
                                                if (color == IController.BLACK)
                                                {
                                                    // Black is minimizer
                                                    if (takeState.Score() < takingNode.m_alpha)
                                                    {
                                                        // Alpha cut
                                                        return numberOfCreatedNodes;
                                                    }
                                                }
                                                else
                                                {
                                                    // White is maximizer
                                                    if (takeState.Score() > takingNode.m_beta)
                                                    {
                                                        // Beta Cut
                                                        return numberOfCreatedNodes;
                                                    }
                                                }
                                            }
                                        }
                                        childNode.Remove();
                                    }
                                    else
                                    {
                                        numberOfCreatedNodes += Create(curHeight + 1, height, State.OppositeColor(color), childNode, newState, alpha, beta);
                                        if (curHeight + 1 == height)
                                        {
                                            childNode.m_score = newState.Score();
                                        }
                                        else
                                        {
                                            if (childNode.Data().Color() == IController.BLACK)
                                            {
                                                int maxScore = int.MinValue;

                                                foreach (GameNode child in childNode.m_children)
                                                {
                                                    if (maxScore < child.Score())
                                                    {
                                                        maxScore = child.Score();
                                                    }
                                                }
                                                childNode.m_score = maxScore;
                                            }
                                            else
                                            {
                                                int minScore = int.MaxValue;
                                                foreach (GameNode child in childNode.m_children)
                                                {
                                                    if (minScore > child.Score())
                                                    {
                                                        minScore = child.Score();
                                                    }
                                                }
                                                childNode.m_score = minScore;
                                            }
                                        }
                                        // Alpha Beta Pruning
                                        if (color == IController.BLACK)
                                        {
                                            // Black is minimizer
                                            if (childNode.Score() < childNode.m_alpha)
                                            {
                                                // Alpha cut
                                                return numberOfCreatedNodes;
                                            }
                                        }
                                        else
                                        {
                                            // White is maximizer
                                            if (childNode.Score() > childNode.m_beta)
                                            {
                                                // Beta Cut
                                                return numberOfCreatedNodes;
                                            }
                                        }
                                    }
                                    // Set siblings alpha or beta value
                                    if (color == IController.BLACK)
                                    {
                                        // Black ist minimizer
                                        alpha = childNode.Score();
                                    }
                                    else
                                    {
                                        // White is maximizer
                                        beta = childNode.Score();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return numberOfCreatedNodes;
        }

        /**
        * Create new nodes recursively.
        * Approximate O(24^(height-curHeight))
        * @param curHeight current subtree height
        * @param height Subtree height
        * @param color Color of next actions
        * @param root Subtree root
        * @param rootState Game state at root
        * @return Number of created nodes
        */
        /*
        public int Create(int curHeight, int height, sbyte color, GameNode root, State rootState)
        {
            //Debug.WriteLine("Create method of GameNode entered");
            int numberOfCreatedNodes = 0;
            if (curHeight != height && !rootState.Finished())
            {
                if (rootState.PlacingPhase(color))
                {
                    foreach (byte position in State.TRANSPOSED)
                    {
                        if (rootState.IsValidPlace(position, color))
                        {
                            ActionPM nextAction = new Placing(color, position);
                            var childNode = root.Add(nextAction, color);
                            var newState = rootState.Clone();
                            //var oldState = rootState.Clone();
                            numberOfCreatedNodes++;
                            childNode.Data().Update(newState);
                            if (newState.InMill(position, color))
                            {
                                //newState = oldState;
                                //Debug.WriteLine("Create: In Mill detected");
                                if (newState.TakingIsPossible(State.OppositeColor(color)))
                                {
                                    //Debug.WriteLine("Create: Taking is possible");
                                    foreach (byte takingPosition in State.TRANSPOSED)
                                    {
                                        if (newState.IsValidTake(takingPosition, State.OppositeColor(color)))
                                        {
                                            //Debug.WriteLine("Create: Valid taking move detected");
                                            var takingNode = root.Add(new Taking(nextAction, takingPosition));
                                            var takeState = rootState.Clone();
                                            takingNode.Data().Update(takeState);
                                            takingNode.m_score = takeState.Score();
                                            numberOfCreatedNodes += Create(curHeight + 1, height, State.OppositeColor(color), takingNode, takeState);
                                        }
                                    }
                                    childNode.Remove();
                                }
                            }
                            else
                            {
                                childNode.m_score = newState.Score();
                                numberOfCreatedNodes += Create(curHeight + 1, height, State.OppositeColor(color), childNode, newState);
                            }
                        }
                    }
                }
                else if (rootState.MovingPhase(color) || rootState.JumpingPhase(color))
                {
                    for (byte i = 0; i < rootState.Board.Length; i++)
                    {
                        foreach (byte to in State.MOVES[i])
                        {
                            if (rootState.IsValidMove(i, to, color))
                            {
                                ActionPM nextAction = new Moving(color, i, to);
                                GameNode childNode = root.Add(nextAction, color);
                                State newState = rootState.Clone();
                                numberOfCreatedNodes++;
                                childNode.Data().Update(newState);
                                if (newState.InMill(to, color))
                                {
                                    //Debug.WriteLine("Create: In Mill detected");
                                    if (newState.TakingIsPossible(State.OppositeColor(color)))
                                    {
                                        //Debug.WriteLine("Create: Taking is possible");
                                        foreach (byte takingPosition in State.TRANSPOSED)
                                        {
                                            if (newState.IsValidTake(takingPosition, State.OppositeColor(color)))
                                            {
                                                //Debug.WriteLine("Create: Valid taking move detected");
                                                var takingNode = root.Add(new Taking(nextAction, takingPosition));
                                                var takeState = rootState.Clone();
                                                takingNode.Data().Update(takeState);
                                                takingNode.m_score = takeState.Score();
                                                numberOfCreatedNodes += Create(curHeight + 1, height, State.OppositeColor(color), takingNode, takeState);
                                            }
                                        }
                                        childNode.Remove();
                                    }
                                }
                                else
                                {
                                    childNode.m_score = newState.Score();
                                    numberOfCreatedNodes += Create(curHeight + 1, height, State.OppositeColor(color), childNode, newState);
                                }
                            }
                        }
                    }
                }
            }

            return numberOfCreatedNodes;
        }
        */

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
            // This method doesn't different between minimizer and maximizer!
            GameNode node = new GameNode(a, score);
            node.m_parent = this;
            m_children.Enqueue(node);
            return node;
        }

        public GameNode Add(Action a)
        {
            // This method doesn't different between minimizer and maximizer!
            var node = new GameNode(a);
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

            //if (a is Taking) a = ((Taking) a).Action;

            foreach (GameNode child in m_children)
            {
                if (child.Data().Equals(a))
                {
                    node = child;
                }
            }

            if (node == null) throw new Exception("Action a is not present in the Priority queue");

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
            State computedState = s.Clone();
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
            GameNode gameNodeOther = (GameNode)other;
            return m_score - gameNodeOther.m_score;
        }
    }
}
