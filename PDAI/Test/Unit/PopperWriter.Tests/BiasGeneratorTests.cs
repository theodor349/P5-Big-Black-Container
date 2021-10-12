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
            List<PredicateOperator> predicates = Models.GetPredicateOperatorList(new List<string>() { "" })
        }
    }
}
