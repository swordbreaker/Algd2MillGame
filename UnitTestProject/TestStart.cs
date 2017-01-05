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
        [TestMethod]
        public void Start()
        {
            var testView = new TestView();
            var controller = new Controller(testView);

            controller.SetPlayerName("Hambbe");
            controller.SetStarter(false);
            controller.StartHumanGame();

            var status = controller.Play(new Placing(IController.WHITE, 1));
            var action = controller.Compute();
            var status2 = controller.Play(new Placing(IController.WHITE, 5));
            controller.Compute();

        }
    }
}
