using Microsoft.VisualStudio.TestTools.UnitTesting;
using PddlParser.Internal;
using System.Collections.Generic;
using Test.Utilities;
using System.Linq;

namespace Parser.Pddl.Tests
{
    [TestClass]
    public class TypesParserTests
    {
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [TestMethod]
        public void SingleTypeMultiLine(int amount)
        {
            string type = "type";
            var lines = StringGenerator.GetTypesListMiltiLine(amount, type);

            var res =  TypesParser.Parse(lines);

            Assert.AreEqual(amount, res.Count);
            foreach (var t in res)
            {
                Assert.AreEqual(type, t.Type);
            }
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [TestMethod]
        public void SingleTypeSingleLine(int amount)
        {
            string type = "type";
            var lines = StringGenerator.GetTypesListSingleLine(amount, type);

            var res = TypesParser.Parse(lines);

            Assert.AreEqual(amount, res.Count);
            foreach (var t in res)
            {
                Assert.AreEqual(type, t.Type);
            }
        }

        [TestMethod]
        public void Inheritance()
        {
            int expected = 3;
            var lines = new List<string>();
            lines.Add("(:types \t");
            lines.Add("parent");
            lines.Add("type1 type2 - parent");
            lines.Add(")");

            var res = TypesParser.Parse(lines);

            Assert.AreEqual(expected, res.Count);
            // Parent
            var type = res.Where(x => x.Type.Equals("parent")).FirstOrDefault(); ;
            Assert.IsNotNull(type);
            Assert.AreEqual(2, type.Children.Count);
            Assert.IsNotNull(type.Children.Where(x => x.Type.Equals("type1")).FirstOrDefault());
            Assert.IsNotNull(type.Children.Where(x => x.Type.Equals("type2")).FirstOrDefault());
            // Children
            Assert.IsNotNull(res.Where(x => x.Type.Equals("type1")).FirstOrDefault());
            Assert.IsNotNull(res.Where(x => x.Type.Equals("type2")).FirstOrDefault());
        }
    }
}
