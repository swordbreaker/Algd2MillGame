using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;

namespace MillGame.Models
{
    public class GameNode : Node<IAction>
    {
        protected int m_score;

        public GameNode(IAction action) : base(action)
        {
        }

        /**
	     * Create new node and add it to this as a child node.
	     * @param a Action
	     * @param score Score
	     */
        public GameNode Add(IAction a, int score)
        {
            throw new NotImplementedException();
        }

        public GameNode Add(IAction a)
        {
            throw new NotImplementedException();
        }

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
            throw new NotImplementedException();
        }

        public int GetWinnerScore()
        {
            throw new NotImplementedException();
        }

        public int Score()
        {
            throw new NotImplementedException();
        }

        public override int CompareTo(Node<IAction> other)
        {
            throw new NotImplementedException();
        }
    }
}
