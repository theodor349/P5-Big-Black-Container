using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Models;
using Test.Utilities;
using PopperWriter;
using System.Collections.Generic;
using System.Linq;

namespace PopperWriter.Tests
{
    [TestClass]
    public class BiasGeneratorTests
    {
        [DataTestMethod]
        public void GetUsedPredicates_allPredicatesUsed_CorrectStringList()
        {
            BiasGenerator biasGenerator = new BiasGenerator();
            List<PredicateOperator> predicates = Models.GetPredicateOperatorList(new List<string>() { "(supports instrument0 infrared0)", "(power_avail satellite0)" });
            List<Predicate> possiblePredicates = Models.GetPredicateList(new List<string>() { "supports", "power_avail" });
            List<string> expected = new List<string>() { "supports", "power_avail" };

            List<Predicate> result = biasGenerator.GetUsedPredicates(possiblePredicates, predicates);

            CollectionAssert.AreEquivalent(expected, result.Select(x => x.Name).ToList());
        }

        [DataTestMethod]
        public void GetUsedPredicates_notAllPredicatesUsed_CorrectStringList()
        {
            BiasGenerator biasGenerator = new BiasGenerator();
            List<PredicateOperator> predicates = Models.GetPredicateOperatorList(new List<string>() { "(supports instrument0 infrared0)", "(power_avail satellite0)" });
            List<Predicate> possiblePredicates = Models.GetPredicateList(new List<string>() { "supports", "power_avail", "on_board" });
            List<string> expected = new List<string>() { "supports", "power_avail" };

            List<Predicate> result = biasGenerator.GetUsedPredicates(possiblePredicates, predicates);

            CollectionAssert.AreEquivalent(expected, result.Select(x => x.Name).ToList());
        }

        [DataRow("on_board", new string[] { "instrument", "satellite" }, true, null, "head_pred(on_board,3).")]
        [DataRow("power_avail", new string[] { "satellite" }, false, false, "body_pred(init_power_avail,2).")]
        [DataRow("have_image", new string[] { "direction", "mode" }, false, true, "body_pred(goal_have_image,3).")]
        [DataTestMethod]
        public void GetPredicateDecleration_validPredicate_CorrectString(string predicateName, string[] paramTypes, bool isHeadPred, bool isGoal, string expected)
        {
            BiasGenerator biasGenerator = new BiasGenerator();
            Predicate predicate = Models.GetPredicate(predicateName);
            predicate.Parameters = Models.GetParameterList(paramTypes.ToList());

            string actual = biasGenerator.GetPredicateDecleration(predicate, isHeadPred, isGoal);

            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        public void GetPredicateDeclerations_x_x()
        {
            BiasGenerator biasGenerator = new BiasGenerator();


            var actual = biasGenerator.GetPredicateDeclarations();
        }
    }
}
