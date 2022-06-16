using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.GJTComputerFiles.ConstructorStructures.nodes;
using AssembleIVM.QueryParser.TreeNodes.Terminals;
using AssembleIVM.T_reduct.Enumerators;
using AssembleIVM.TreeNodes.Predicates;
using QueryParser;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class Computenetavail2 : ManualGJT {
        public override string GetName() {
            return "computenetavail2";
        }
        public override GeneralJoinTree Construct() {
            List<string> vars1 = new List<string> { "a.employee", "a.fact", "a.week", "a.hours" };
            List<string> vars2 = new List<string> { "b.employee", "b.fact", "b.week", "b.hours" };
            List<string> vars3 = new List<string> { "c.employee", "c.fact", "c.week", "c.hours" };
            List<string> vars5 = new List<string> { "a.employee", "a.fact", "a.week", "a.hours" };
            List<string> vars6 = new List<string> { "b.employee", "b.fact", "b.week", "b.hours" };
            List<string> vars7 = new List<string> { "c.employee", "c.fact", "c.week", "c.hours" };
            List<string> vars4 = new List<string> { "a.employee", "a.week" };
            GJTLeaf base1 = new GJTLeaf("base1", vars1, "copyfactgross1", null);
            GJTLeaf base2 = new GJTLeaf("base2", vars2, "copyfactgross1", null);
            GJTLeaf base3 = new GJTLeaf("base3", vars3, "copyfactgross1", null);              
            GJTLeaf base4 = new GJTLeaf("base4", new List<string> { "d.fact" }, @"Factnet", new FrontierEnumerator());
            base4.inFrontier = true;
            TreeNode selPred1 = ParsePredicate("a.fact == \"gross availability\"");
            TreeNode selPred2 = ParsePredicate("b.fact == \"holiday\"");
            TreeNode selPred3 = ParsePredicate("c.fact == \"education\"");
            TreeNode predicate1 = ParsePredicate("a.employee == b.employee and a.week == b.week");
            TreeNode predicate2 = ParsePredicate("a.employee == c.employee and a.week == c.week") ;
             
            GJTInnerNode f1 = new GJTSelectNode("f1", vars5, new List<GJTNode> { base1 }, new List<TreeNode> { selPred1 }, new FrontierEnumerator());
            GJTInnerNode f2 = new GJTSelectNode("f2", vars6, new List<GJTNode> { base2 }, new List<TreeNode> { selPred2 }, new FrontierEnumerator());
            GJTInnerNode f3 = new GJTSelectNode("f3", vars7, new List<GJTNode> { base3 }, new List<TreeNode> { selPred3 }, new FrontierEnumerator());
            f1.inFrontier = true;
            f2.inFrontier = true;
            f3.inFrontier = true;
            GJTInnerNode i1 = new GJTSingleJoinNode(
                    "i1",
                    vars4,
                    new List<GJTNode> { f1, f2 },
                    new List<TreeNode> { null, predicate1 }, new SingleJoinEnumerator());
            GJTInnerNode i2 = new GJTSingleJoinNode(
                "i2",
                vars4,
                new List<GJTNode> { i1, f3 },
                new List<TreeNode> { null, predicate2 }, new SingleJoinEnumerator());
            GJTInnerNode root = new GJTSingleJoinNode(
                "vars4",
                vars4,
                new List<GJTNode> { i2, base4 },
                new List<TreeNode> { null, new CartesianProduct() },
                new SingleJoinEnumerator());

            return new GeneralJoinTree(
                root,
                new HashSet<GJTNode> { f1, f2, f3, base4 },
                new HashSet<GJTLeaf> { base1, base2, base3, base4 },
                new List<string> { "Employee", "Fact", "Week", "Hours" },
                new List<string> { "a.employee", "d.fact", "a.week", "a.hours - b.hours - c.hours" },
                new List<string> { "copyfactgross1" }
                );
        }

    }
}
