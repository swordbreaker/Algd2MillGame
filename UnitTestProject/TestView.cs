using System;
using System.Diagnostics;
using System.Security.AccessControl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MillGame.Models.Core;

using Action = MillGame.Models.Core.Actions.Action;

namespace UnitTestProject
{
    public class TestView : IView
    {
        public struct UpdateData
        {
            public State State { get; set; }
            public Action Action { get; set; }
            public bool IsComputerAction { get; set; }
        }

        public string HumanName { get; set; }
        public string PlayerName { get; set; }
        public UpdateData LastUpdateData { get; set; }

        public void UpdateBoard(State s, Action a, bool isComputerAction)
        {
            LastUpdateData = new UpdateData
            {
                State = s,
                IsComputerAction = isComputerAction,
                Action = a
            };
            Debug.WriteLine("Update board:");
            Debug.WriteLine($"{s} {a} {isComputerAction}");
        }

        public void PrepareBoard()
        {
            Debug.WriteLine("Prepare Board");
        }

        public void SetComputerName(string name)
        {
            Debug.WriteLine($"Computer name is {name}");
        }

        public void SetHumanName(string name)
        {
            Debug.WriteLine($"Player name is {name}");
        }
    }
}
