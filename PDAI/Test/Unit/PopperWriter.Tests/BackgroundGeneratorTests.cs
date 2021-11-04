using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Models;
using Test.Utilities;
using PopperWriter;
using System.Collections.Generic;
using System.Linq;

namespace PopperWriter.Tests
{
    [TestClass]
    public class BackgroundGeneratorTests
    {
        [DynamicData("PredicateInput")]
        [DataTestMethod]
        public void GetPredicates_validPredicate_CorrectString(List<string> names, List<List<PredicateOperator>> initialstates, List<List<PredicateOperator>> goalstates, List<string> expected)
        {
            BackgroundGenerator backgroundGenerator = new BackgroundGenerator();

            List<Problem> problems = Models.GetProblemList(names, initialstates, goalstates);

            List<string> result = backgroundGenerator.GetPredicates(problems);

            CollectionAssert.AreEquivalent(expected, result);
        }

        public static IEnumerable<object[]> PredicateInput
        {
            get
            {
                return new[]
                {
                    new object[] {
                        new List<string>() { "p1", "p2", "p3" },
                        new List<List<PredicateOperator>>() {
                            Models.GetPredicateOperatorList(new List<string>() { "(on b1 b1)", "(calibration_target instrument1 star1)", "(power_avail satellite1)" }),
                            Models.GetPredicateOperatorList(new List<string>() { "(on b2 b2)", "(calibration_target instrument2 star2)", "(power_avail satellite2)" }),
                            Models.GetPredicateOperatorList(new List<string>() { "(on b3 b3)", "(calibration_target Instrument3 star3)", "(power_avail satellite3)" }),
                        },
                        new List<List<PredicateOperator>>() {
                            Models.GetPredicateOperatorList(new List<string>() { "(have_image Star1 infrared1)", "(have_image star1 thermograph1)" }),
                            Models.GetPredicateOperatorList(new List<string>() { "(have_image star2 infrared2)", "(have_image star2 thermograph2)" }),
                            Models.GetPredicateOperatorList(new List<string>() { "(have_image star3 infrared3)" }),
                        },
                        new List<string>()
                        {
                            "init_on(b1,b1,p1).", "init_calibration_target(instrument1,star1,p1).", "init_power_avail(satellite1,p1).",
                            "init_on(b2,b2,p2).", "init_calibration_target(instrument2,star2,p2).", "init_power_avail(satellite2,p2).",
                            "init_on(b3,b3,p3).", "init_calibration_target(instrument3,star3,p3).", "init_power_avail(satellite3,p3).",
                            "goal_have_image(star1,infrared1,p1).", "goal_have_image(star1,thermograph1,p1).",
                            "goal_have_image(star2,infrared2,p2).", "goal_have_image(star2,thermograph2,p2).",
                            "goal_have_image(star3,infrared3,p3)."
                        }
                    }
                };
            }
        }
    }
}