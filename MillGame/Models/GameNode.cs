using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;

namespace MillGame.Models
{
    public class GameNode : Node<Core.Actions.Action>
    {
        protected int m_score;

        public GameNode(Core.Actions.Action action) : base(action)
        {
        }

        /**
	     * Create new node and add it to this as a child node.
	     * @param a Action
	     * @param score Score
	     */
        public GameNode Add(Core.Actions.Action a, int score)
        {
            this.m_data = a;
            this.m_score = score;
            return this;
        }

        public GameNode Add(Core.Actions.Action a)
        {
            this.m_data = a;
            return this;
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

        public override int CompareTo(Node<Core.Actions.Action> other)
        {
            // TODO: Find good compare mechanism to build the tree and to find a given element efficent in the tree
            throw new NotImplementedException();
        }
    }
}
