using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Models;
using Test.Utilities;
using PopperWriter;
using System.Linq;
using System.Collections.Generic;

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
        [DynamicData("ActionInput")]
        [DataTestMethod]
        public void GetActions_validAction_CorrectString(List<string> names, List<List<ActionOperator>> goodoperators, List<List<ActionOperator>> badoperators, List<string> expected)
        {
            ExampleGenerator exampleGenerator = new ExampleGenerator();
            List<Problem> problems = Models.GetProblemList(names, goodoperators, badoperators);

            List<string> result = exampleGenerator.GetActions(null, problems);

            CollectionAssert.AreEquivalent(expected, result);
        }

        public static IEnumerable<object[]> ActionInput
        {
            get
            {
                return new[]
                {
                    new object[] {
                        new List<string>() { "p1", "p2", "p3" },
                        new List<List<ActionOperator>>() {
                            Models.GetActionOperatorList(new List<string>() { "drive truck0 depot0 distributor0", "drop hoist0 crate0 pallet0 depot0", "lift hoist0 crate2 pallet0 depot0" }),
                            Models.GetActionOperatorList(new List<string>() { "drive truck1 distributor0 depot0", "lift hoist1 crate0 pallet1 distributor0", "unload hoist0 crate0 truck0 depot0" }),
                            Models.GetActionOperatorList(new List<string>() { "drive truck0 depot0 distributor0", "drop hoist1 crate0 pallet1 distributor0", "load hoist0 crate0 truck0 depot0" })
                        },
                        new List<List<ActionOperator>>() {
                            Models.GetActionOperatorList(new List<string>() { "drive truck0 distributor0 distributor0", "drive truck1 distributor0 distributor0", "drive truck0 depot0 depot0"}),
                            Models.GetActionOperatorList(new List<string>() { "drive truck0 depot0 depot0", "drive truck1 depot0 depot0", "drive truck0 distributor0 distributor0" }),
                            Models.GetActionOperatorList(new List<string>() { "drive truck0 depot0 depot0", "drive truck1 distributor0 depot0", "drive truck1 distributor0 distributor0"})
                        },
                        new List<string>()
                        {
                            "pos(drive(truck0,depot0,distributor0,p1)).", "pos(drop(hoist0,crate0,pallet0,depot0,p1)).", "pos(lift(hoist0,crate2,pallet0,depot0,p1)).",
                            "pos(drive(truck1,distributor0,depot0,p2)).", "pos(lift(hoist1,crate0,pallet1,distributor0,p2)).", "pos(unload(hoist0,crate0,truck0,depot0,p2)).",
                            "pos(drive(truck0,depot0,distributor0,p3)).", "pos(drop(hoist1,crate0,pallet1,distributor0,p3)).", "pos(load(hoist0,crate0,truck0,depot0,p3)).",
                            "neg(drive(truck0,distributor0,distributor0,p1)).", "neg(drive(truck1,distributor0,distributor0,p1)).", "neg(drive(truck0,depot0,depot0,p1)).",
                            "neg(drive(truck0,depot0,depot0,p2)).", "neg(drive(truck1,depot0,depot0,p2)).", "neg(drive(truck0,distributor0,distributor0,p2)).",
                            "neg(drive(truck0,depot0,depot0,p3)).", "neg(drive(truck1,distributor0,depot0,p3)).", "neg(drive(truck1,distributor0,distributor0,p3))."
                        }
                    }
                };
            }
        }*/
    }
}