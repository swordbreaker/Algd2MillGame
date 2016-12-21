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
    public class GameTree : IGameTree
    {
        public void Print()
        {
            throw new NotImplementedException();
        }

        public int Size()
        {
            throw new NotImplementedException();
        }

        public void Create(int height, Placing pa)
        {
            throw new NotImplementedException();
        }

        public State CurrentState()
        {
            throw new NotImplementedException();
        }

        public void HumanPlayer(Action a)
        {
            throw new NotImplementedException();
        }

        public Action ComputerPlayer()
        {
            throw new NotImplementedException();
        }
    }
}
