using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var state = new State();
            state.Update(new Placing(IController.WHITE, 0));
            state.Update(new Placing(IController.WHITE, 1));
            state.Update(new Placing(IController.WHITE, 2));
            state.Update(new Placing(IController.WHITE, 9));
            state.Update(new Placing(IController.WHITE, 21));

            state.Update(new Placing(IController.BLACK, 03));
            state.Update(new Placing(IController.BLACK, 04));
            state.Update(new Placing(IController.BLACK, 05));
            state.Update(new Placing(IController.BLACK, 13));
            state.Update(new Placing(IController.BLACK, 20));
            state.Update(new Placing(IController.BLACK, 14));
            state.Update(new Placing(IController.BLACK, 12));

            Debug.WriteLine(state);
            State.ScoreInfomations inf = new State.ScoreInfomations();
            Debug.WriteLine("BLACK");
            Debug.WriteLine(state.Infomations.ToString(0));
            Debug.WriteLine("WHITE");
            Debug.WriteLine(state.Infomations.ToString(1));

            Debug.Write(state.Score());
        }
    }
}
