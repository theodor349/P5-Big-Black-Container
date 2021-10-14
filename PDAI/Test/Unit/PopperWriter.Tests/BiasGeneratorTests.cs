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
            List<Predicate> usedInitPreds = Models.GetPredicateListFromNumOfParams(new() { "on_board", "calibration_target", "power_avail", "pointing" }, new() { 2, 2, 1, 2 });
            List<Predicate> goalInitPreds = Models.GetPredicateListFromNumOfParams(new() { "on_board", "power_avail", "pointing" }, new() { 2, 1, 2 });
            List<string> expected = new() { "head_pred(switch_on,3).", "body_pred(init_on_board,3).", "body_pred(init_calibration_target,3).", "body_pred(init_power_avail,2).", "body_pred(init_pointing,3).", "body_pred(goal_on_board,3).", "body_pred(goal_power_avail,2).", "body_pred(goal_pointing,3)." };

            List<string> actual = biasGenerator.GetClauseDeclarations(action, usedInitPreds, goalInitPreds);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [DynamicData("GetTypeDeclerationInput")]
        [DataTestMethod]
        public void GetTypeDeclerations_MultipleParametersWithRecursion_CorrectDeclerationsAndOrder(string clauseName, List<Entity> parameters, List<string> expected)
        {
            BiasGenerator biasGenerator = new();

            List<string> actual = biasGenerator.GetTypeDecleration(clauseName, parameters, true);

            CollectionAssert.AreEqual(expected, actual);
        }

        public static IEnumerable<object[]> GetTypeDeclerationInput
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        "take_image",
                        Models.GetEntityList(new List<string>() { "satellite", "direction", "instrument", "mode" }),
                        new List<string>() { "type(take_image,(satellite,direction,instrument,mode,problem))." }
                    },
                    new object[]
                    {
                        "init_clear",
                        Models.GetEntityList(
                            new List<string>() { "surface" },
                            new List<List<Entity>>()
                            {
                                Models.GetEntityList(new List<string>() { "pallet", "crate" })
                            }
                        ),
                        new List<string>()
                        {
                            "type(init_clear,(pallet,problem));",
                            "type(init_clear,(crate,problem));",
                            "type(init_clear,(surface,problem))."
                        }
                    }
                };
            }
        }

        [DataRow("take_image", 4, "direction(take_image,(in,in,in,in,out)).")]
        [DataRow("init_on_board", 3, "direction(init_on_board,(in,in,in,out)).")]
        [DataRow("has_car", 1, "direction(has_car,(in,out)).")]
        [DataTestMethod]
        public void getDirectionDecleration_VariableNumOfParams_CorrectDecleration(string clauseName, int numOfParams, string expected)
        {
            BiasGenerator biasGenerator = new();
            Clause clause = Models.GetClause(clauseName, numOfParams);

            string actual = biasGenerator.GetDirectionDecleration(clause);

            Assert.AreEqual(expected, actual);
        }

        [DynamicData("GetDirectionDeclerationsInput")]
        [DataTestMethod]
        public void GetDirectionDeclerations_VariableNumOfParams_CorrectDecleration(List<Clause> clauses, List<string> expected)
        {
            BiasGenerator biasGenerator = new();

            List<string> actual = biasGenerator.GetDirectionDeclerations(clauses);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        public static IEnumerable<object[]> GetDirectionDeclerationsInput
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        Models.GetClauseList(new() { "two_wheels" }, new() { 1 }),
                        new List<string>() { "direction(two_wheels,(in,out))." }
                    },
                    new object[]
                    {
                        Models.GetClauseList(new() { "take_image", "init_on_board", "has_car" }, new() { 4, 3, 1 }),
                        new List<string>() {
                            "direction(take_image,(in,in,in,in,out)).",
                            "direction(init_on_board,(in,in,in,out)).",
                            "direction(has_car,(in,out))."
                        }
                    }
                };
            }
        }

        [DataRow(new string[] { "max_vars(3).", "max_clauses(5).", "max_body(5)." }, 3)]
        [DataRow(new string[] { "max_vars(4).", "max_clauses(5).", "max_body(5)." }, 4)]
        [DataRow(new string[] { "max_vars(6).", "max_clauses(5).", "max_body(5)." }, 6)]
        [DataTestMethod]
        public void GetConstraints_VariableMaxVars_CorrectDecleration(string[] expectedArray, int maxVars)
        {
            BiasGenerator biasGenerator = new();
            List<string> expected = expectedArray.ToList();

            List<string> actual = biasGenerator.GetConstraints(maxVars);

            CollectionAssert.AreEquivalent(expected, actual);

        }

        [DynamicData("GetUsedPredsInput")]
        [DataTestMethod]
        public void GetUsedPreds_x_x(List<Problem> problems, List<Predicate> predicates, bool initPreds, List<Predicate> expected)
        {
            BiasGenerator biasGenerator = new();

            List<Predicate> actual = biasGenerator.GetUsedPreds(problems, predicates, initPreds);

            CollectionAssert.AreEquivalent(expected.Select(x => x.Name).ToList(), actual.Select(x => x.Name).ToList());
        }

        public static IEnumerable<object[]> GetUsedPredsInput
        {
            get
            {
                return new[]
                {
                    new object[]
                    {
                        Models.GetProblemList(
                            new() {
                                Models.GetPredicateOperatorList(new List<string>() { "(on_board b1 b1)", "(calibration_target instrument1 star1)", "(power_avail satellite1)" }),
                                Models.GetPredicateOperatorList(new List<string>() { "(on_board b2 b2)", "(power_avail satellite0)", "(power_avail satellite2)" }),
                                Models.GetPredicateOperatorList(new List<string>() { "(pointing b3 b3)", "(calibration_target instrument2 GroundStation3)", "(power_avail satellite3)" })
                            },
                            new() {
                                Models.GetPredicateOperatorList(new List<string>() { "(on_board b1 b1)", "(power_avail satellite1)" }),
                                Models.GetPredicateOperatorList(new List<string>() { "(on_board b2 b2)", "(power_avail satellite0)" }),
                                Models.GetPredicateOperatorList(new List<string>() { "(pointing b3 b3)" })
                            }
                        ),
                        Models.GetSatellitePredicates(),
                        true,
                        Models.GetPredicateList(new() { "on_board", "calibration_target", "power_avail", "pointing" })
                    },
                    new object[]
                    {
                        Models.GetProblemList(
                            new() {
                                Models.GetPredicateOperatorList(new List<string>() { "(on_board b1 b1)", "(calibration_target instrument1 star1)", "(power_avail satellite1)" }),
                                Models.GetPredicateOperatorList(new List<string>() { "(on_board b2 b2)", "(power_avail satellite0)", "(power_avail satellite2)" }),
                                Models.GetPredicateOperatorList(new List<string>() { "(pointing b3 b3)", "(calibration_target instrument2 GroundStation3)", "(power_avail satellite3)" })
                            },
                            new() {
                                Models.GetPredicateOperatorList(new List<string>() { "(on_board b1 b1)", "(power_avail satellite1)" }),
                                Models.GetPredicateOperatorList(new List<string>() { "(on_board b2 b2)", "(power_avail satellite0)" }),
                                Models.GetPredicateOperatorList(new List<string>() { "(pointing b3 b3)" })
                            }
                        ),
                        Models.GetSatellitePredicates(),
                        false,
                        Models.GetPredicateList(new() { "on_board", "power_avail", "pointing" })
                    }
                };
            }
        }
    }
}


