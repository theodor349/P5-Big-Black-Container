using Microsoft.VisualStudio.TestTools.UnitTesting;
using PddlParser.Internal;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Pddl.Tests
{
    [TestClass]
    public class ActionParserTests
    {
        [DataRow(1)]
        [TestMethod]
        public void SingleAction_OneType_nArguments_Junk(int amount)
        {
            var name = "action1";
            var entities = new List<Entity>();
            entities.Add(new Entity() { Type = "type1" });

            var lines = new List<string>();
            lines.Add(":precondition (and (clear ?bm) (on ?bm ?bf))");
            lines.Add(":precondition (and (clear ?bm) (on ?bm ?bf))");
            lines.Add("(:action " + name);
            lines.Add(":parameters(");
            for (int i = 0; i < amount; i++)
                lines.Add("?var" + i);
            lines.Add(")");
            lines.Add(":precondition (and (clear ?bm) (on ?bm ?bf))");
            lines.Add(":effect (and (not (clear ?bt)) (not (ontable ?bm))(on ?bm ?bt)))");
            lines.Add(")");
            lines.Add(":effect (and (not (clear ?bt)) (not (ontable ?bm))(on ?bm ?bt)))");
            lines.Add(":effect (and (not (clear ?bt)) (not (ontable ?bm))(on ?bm ?bt)))");


            var res = ActionsParser.Parse(lines, entities);

            Assert.AreEqual(1, res.Count);
            Assert.AreEqual(name, res[0].Name);
            Assert.AreEqual(amount, res[0].Parameters.Count);
            for (int i = 0; i < amount; i++)
            {
                Assert.AreEqual("var" + i, res[0].Parameters[i].Name);
                Assert.AreEqual(entities[0].Type, res[0].Parameters[i].Entity.Type);
            }
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [TestMethod]
        public void SingleAction_OneType_nArguments(int amount)
        {
            var name = "action1";
            var entities = new List<Entity>();
            entities.Add(new Entity() { Type = "type1" });

            var lines = new List<string>();
            lines.Add("(:action " + name);
            lines.Add(":parameters(");
            for (int i = 0; i < amount; i++)
                lines.Add("?var" + i);
            lines.Add(":precondition (and (clear ?bm) (on ?bm ?bf))");
            lines.Add(":effect (and (not (clear ?bt)) (not (ontable ?bm))(on ?bm ?bt)))");
            lines.Add(")");


            var res = ActionsParser.Parse(lines, entities);

            Assert.AreEqual(1, res.Count);
            Assert.AreEqual(name, res[0].Name);
            Assert.AreEqual(amount, res[0].Parameters.Count);
            for (int i = 0; i < amount; i++)
            {
                Assert.AreEqual("var" + i, res[0].Parameters[i].Name);
                Assert.AreEqual(entities[0].Type, res[0].Parameters[i].Entity.Type);
            }
        }
    }
}
