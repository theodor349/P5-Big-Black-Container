using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Models;
using Test.Utilities;
using PopperWriter;

namespace PopperWriter.Tests
{
    [TestClass]
    public class BackgroundGeneratorTests
    {
        [DataRow("(on b1 b8)", "p1", true, "goal_on(b1,b8,p1).")]
        [DataRow("(calibration_target instrument0 star3)", "p5", false, "init_calibration_target(instrument0,star3,p5).")]
        [DataRow("(power_avail satellite0)", "p23", true, "goal_power_avail(satellite0,p23).")]
        [DataTestMethod]
        public void PredicateToString_validPredicate_CorrectString(string predicateString, string problemName, bool isGoal, string expected)
        {
            BackgroundGenerator backgroundGenerator = new BackgroundGenerator();
            PredicateOperator predicate = Models.GetPredicateOperator(predicateString);
            string result = backgroundGenerator.PredicateToString(predicate, problemName, isGoal);
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        public void GetPredicates_validPredicate_CorrectString(string predicateString, string problemName, bool isGoal, string expected)
        {
            
        }
    }
}
