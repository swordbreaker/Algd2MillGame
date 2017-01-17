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
            state.Update(new Placing(IController.WHITE, 10));
            state.Update(new Placing(IController.WHITE, 11));
            state.Update(new Placing(IController.WHITE, 3));

            state.Update(new Placing(IController.BLACK, 13));
            state.Update(new Placing(IController.BLACK, 12));
            state.Update(new Placing(IController.BLACK, 14));

            Debug.WriteLine(state);
            Debug.WriteLine("BLACK");
            Debug.WriteLine(state.Infomations.ToString(0));
            Debug.WriteLine("WHITE");
            Debug.WriteLine(state.Infomations.ToString(1));

            Debug.Write(state.Score());
        }
    }
}
