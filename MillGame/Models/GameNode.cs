using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;

namespace MillGame.Models
{
    public class GameNode : Node<int>
    {
        protected int m_score;

        public GameNode(int data) : base(data)
        {
        }

        public GameNode(IAction a) : base(0)
        {
            
        }

        public GameNode(IAction a, int score) : base(score)
        {
            
        }

        public int Add(Node<IAction> v)
        {
            throw new NotImplementedException();
        }

        public void Remove()
        {
            throw new NotImplementedException();
        }

        public int Size()
        {
            throw new NotImplementedException();
        }

        public IAction Data()
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Node<IAction>> Iterator()
        {
            throw new NotImplementedException();
        }

        public override int CompareTo(Node<int> other)
        {
            throw new NotImplementedException();
        }

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
    }
}
