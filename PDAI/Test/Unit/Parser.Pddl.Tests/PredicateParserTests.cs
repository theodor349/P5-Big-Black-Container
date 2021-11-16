using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;
using PddlParser.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Parser.Pddl.Tests
{
    [TestClass]
    // In these tests the arguments in the name of the methods refer to the amount of predicates.
    public class PredicateParserTests
    {
        [DataRow(1)]
        [TestMethod]
        public void Predicate_OneType_nArguments_Junk(int amount)
        {
            var entities = new List<Entity>();
            entities.Add(new Entity() { Type = "type1" });

            var lines = new List<string>();
            lines.Add("(ontable ?x)");
            lines.Add("(ontable ?x)");
            lines.Add("(:predicates (clear ?x)");
            lines.Add("(ontable ?x)");
            lines.Add("; ; Family members will be used in descending order");
            lines.Add("; ; the max is the number of member at present");
            lines.Add("(on ");
            for (int i = 0; i < amount; i++)
                lines.Add("?var" + i);
            lines.Add(")");
            lines.Add(")");

            var res = PredicatesParser.Parse(lines, entities);

            Assert.AreEqual(3, res.Count);
            Assert.AreEqual(amount, res[2].Parameters.Count);
            for (int i = 0; i < amount; i++)
            {
                Assert.AreEqual("var" + i, res[2].Parameters[i].Name);
                Assert.AreEqual(entities[0].Type, res[2].Parameters[i].Entity.Type);
            }
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [TestMethod]
        public void Predicate_OneType_nArguments(int amount)
        {
            var entities = new List<Entity>();
            entities.Add(new Entity() { Type = "type1" });

            var lines = new List<string>();
            lines.Add("(:predicates (clear ?x)");
            lines.Add("(ontable ?x)");
            lines.Add("(on ");
            for (int i = 0; i < amount; i++)
                lines.Add("?var" + i);
            lines.Add(")");
            lines.Add(")");

            var res = PredicatesParser.Parse(lines, entities);

            Assert.AreEqual(3, res.Count);
            Assert.AreEqual(amount, res[2].Parameters.Count);
            Assert.AreEqual("clear", res[0].Name);
            Assert.AreEqual("on", res[2].Name);
            for (int i = 0; i < amount; i++)
            {
                Assert.AreEqual("var" + i, res[2].Parameters[i].Name);
                Assert.AreEqual(entities[0].Type, res[2].Parameters[i].Entity.Type);
            }
        }

        [DataRow(2, 1)]
        [DataRow(3, 2)]
        [DataRow(4, 4)]
        [TestMethod]
        public void Predicate_nType_mArguments(int types, int arguments)
        {
            var typeName = "Type";
            var entities = new List<Entity>();
            entities.Add(new Entity() { Type = "object" });
            for (int i = 0; i < types; i++)
                entities.Add(new Entity() { Type = typeName + i });

            var lines = new List<string>();
            lines.Add("(:predicates (arbitraryName ");
            for (int t = 0; t < types; t++)
            {
                for (int a = 0; a < arguments; a++)
                {
                    lines.Add("?var" + (a + t * arguments));
                }
                lines.Add("- " + typeName + t);
            }
            lines.Add(")");
            lines.Add(")");

            var res = PredicatesParser.Parse(lines, entities);
            entities.RemoveAt(0);

            Assert.AreEqual(1, res.Count);
            Assert.AreEqual(types * arguments, res[0].Parameters.Count);
            for (int i = 0; i < types * arguments; i++)
            {
                Assert.AreEqual("var" + i, res[0].Parameters[i].Name);
                Assert.AreEqual(entities[i / arguments].Type, res[0].Parameters[i].Entity.Type);
            }
        }
    }
}
