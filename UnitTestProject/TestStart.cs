using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MillGame.Models;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;
using Action = MillGame.Models.Core.Actions.Action;

namespace UnitTestProject
{
    [TestClass]
    public class TestStart
    {
        private Controller _controller;
        private readonly sbyte[] _millPositions = new sbyte[24];

        [TestMethod]
        public void Start()
        {
            for (var j = 0; j < _millPositions.Length; j++)
            {
                _millPositions[j] = -1;
            }

            var testView = new TestView();
            _controller = new Controller(testView);

            _controller.SetPlayerName("Hambbe");
            _controller.SetStarter(false);
            _controller.StartHumanGame();


            int i = 0;
            do
            {
                PlayerPlace(IController.WHITE, i);
                var act = ComputerPlace(IController.BLACK);
                i++;
            } while (_controller.GameTree.CurrentState().PlacingPhase(IController.BLACK));

            //var status = _controller.Play(new Placing(IController.WHITE, 1));
            //var action = _controller.Compute();
            //var status2 = _controller.Play(new Placing(IController.WHITE, 1));
            //controller.Compute();

        }

        public void PlayerPlace(sbyte color, int i)
        {
            var status = IController.Status.INVALIDACTION;
            while (status != IController.Status.OK)
            {
                if (_millPositions[i] == -1)
                {
                    var placing = new Placing(color, i);
                    status = _controller.Play(placing);
                    Debug.WriteLine(status);

                    if (status == IController.Status.CLOSEDMILL)
                    {
                        for (int k = 0; k < _millPositions.Length; k++)
                        {
                            if (_millPositions[k] == State.OppositeColor(color))
                            {
                                Debug.WriteLine($"Take {k} from Computer");
                                _controller.Play(new Taking(placing, k));
                                break;
                            }
                        }
                    }
                }
                i++;
            }
            _millPositions[i-1] = color;
        }

        public Action ComputerPlace(sbyte color)
        {
            var action = _controller.Compute();
            _millPositions[((Placing) action).EndPosition] = color;
            return action;
        }
    }
}
