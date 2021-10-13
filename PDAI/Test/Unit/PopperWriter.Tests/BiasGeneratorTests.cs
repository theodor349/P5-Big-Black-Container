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

            string actual = biasGenerator.GetClauseDecleration(predicate, isHeadPred, isGoal);

            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        public void GetPredicateDeclerations_NotAllPredicatesUsed_OnlyUsedPredicatesInResult()
        {
            BiasGenerator biasGenerator = new BiasGenerator();
            Action action = Models.GetAction("switch_on", new List<string> { "instrument", "satellite" });
            List<List<PredicateOperator>> initialstates = new() {
                Models.GetPredicateOperatorList(new List<string>() { "(on_board b1 b1)", "(calibration_target instrument1 star1)", "(power_avail satellite1)" }),
                Models.GetPredicateOperatorList(new List<string>() { "(on_board b2 b2)", "(power_avail satellite0)", "(power_avail satellite2)" }),
                Models.GetPredicateOperatorList(new List<string>() { "(pointing b3 b3)", "(calibration_target instrument2 GroundStation3)", "(power_avail satellite3)" })
            };
            List<List<PredicateOperator>> goalstates = new() {
                Models.GetPredicateOperatorList(new List<string>() { "(on_board b1 b1)", "(power_avail satellite1)" }),
                Models.GetPredicateOperatorList(new List<string>() { "(on_board b2 b2)", "(power_avail satellite0)" }),
                Models.GetPredicateOperatorList(new List<string>() { "(pointing b3 b3)" })
            };
            List<Problem> problems = Models.GetProblemList(initialstates, goalstates);
            List<Predicate> actions = Models.GetSatellitePredicates();
            Domain domain = Models.GetDomain(problems, actions);
            List<string> expected = new() { "head_pred(switch_on,3).", "body_pred(init_on_board,3).", "body_pred(init_calibration_target,3).", "body_pred(init_power_avail,2).", "body_pred(init_pointing,3).", "body_pred(goal_on_board,3).", "body_pred(goal_power_avail,2).", "body_pred(goal_pointing,3)." };

            List<string> actual = biasGenerator.GetPredicateDeclarations(action, domain);

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
