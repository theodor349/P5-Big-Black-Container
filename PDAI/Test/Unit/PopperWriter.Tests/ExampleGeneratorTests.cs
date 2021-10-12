using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Models;
using Test.Utilities;
using PopperWriter;

namespace PopperWriter.Tests
{
    [TestClass]
    public class ExampleGeneratorTests
    {
        [DataRow("drive truck0 depot0 distributor0", "p1", true, "pos(drive(truck0,depot0,distributor0,p1)).")]
        [DataRow("drive truck0 depot0 distributor0", "p1", false, "neg(drive(truck0,depot0,distributor0,p1)).")]
        [DataRow("movebtob b2 b4 b10", "p5", false, "neg(movebtob(b2,b4,b10,p5)).")]
        [DataTestMethod]
        public void ExampleToString_validAction_CorrectString(string actionString, string problemName, bool isPositive, string expected)
        {
            ExampleGenerator exampleGenerator = new ExampleGenerator();
            ActionOperator action = Models.GetActionOperator(actionString);

            string result = exampleGenerator.ActionToString(action, problemName, isPositive);

            Assert.AreEqual(expected, result);
        }

        /*
        [DataRow(new string[] { "" })]
        [DataTestMethod]
        public void GetActions_validAction_CorrectString(string actionString, string problemName, bool isPositive, string expected)
        {

        }*/
    }
}
