using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;

namespace MillGame.Models
{
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
	     * Create new node and add it to this as a child node.
	     * @param a Action
	     * @param score Score
	     */
        public GameNode Add(Core.Actions.Action a, int score)
        {
            GameNode node = new GameNode(a, score);
            node.m_parent = this;
            m_children.Enqueue(node);
            return node;
        }

        public GameNode Add(Core.Actions.Action a)
        {
            GameNode node = new GameNode(a);
            node.m_parent = this;
            m_children.Enqueue(node);
            return node;
        }

        /*
         * Removes now unused subtrees
         * O(n)
         */
        public GameNode RemoveUnusedChilds(Core.Actions.Action a)
        {
            GameNode node = null;
            foreach(GameNode child in m_children)
            {
                if (child.Data() != a)
                {
                    m_children.Remove(child);
                } else
                {
                    node = child;
                }
            }
            return node;
        }

        /**
        * Create new nodes recursively.
        * @param curHeight current subtree height
        * @param height Subtree height
        * @param color Color of next actions
        * @param root Subtree root
        * @param rootState Game state at root
        * @return Number of created nodes
        */
        public int Create(int curHeight, int height, byte color, GameNode root, State rootState)
        {
            // TODO Find out how this method is going to be used..
            int numberOfCreatedNodes = 0;

            return numberOfCreatedNodes;
            throw new NotImplementedException();
        }

        /**
	     * Compute game state at this node
	     * @param s Game state at given node v
	     * @param v Game node v must be ancestor of this
	     * @return Game state at this node
	     */
        public State ComputeState(State s, GameNode v)
        {
            State computedState = s.clone();
            GameNode nextNode = null;
            while(nextNode != this)
            {
                nextNode = findNextNode(v);
                nextNode.m_data.Update(computedState);
            }
            return computedState;
        }

        /**
         * Computes the next node in the path from the given node to the this node
         */
        private GameNode findNextNode(GameNode currentNode)
        {
            // TODO Find next node in the path to the this node
            throw new NotImplementedException();
        }

        public int GetWinnerScore()
        {
            // Yanick: Why is this method here? I don't see the use of it. The winner store is stored in the state which is only available in the GameTree!
            // Can I delete it? It's not in the class diagram but in the interface.. WTF
            // Or maybe just compute the winner score (none, black or white)? But then I have to some how get a initial state an ancestor node of this to calculate this value..
            throw new NotImplementedException();
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
        public override int CompareTo(Node<Core.Actions.Action> other)
        {
            GameNode gameNodeOther = (GameNode) other;
            return m_score - gameNodeOther.m_score;
        }
    }
}
