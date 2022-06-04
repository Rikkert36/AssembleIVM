using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.QueryParser.TreeNodes.Terminals;
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
            string[] vars1 = new string[] { "a.employee", "a.week", "a.fact", "a.hours" };
            string[] vars2 = new string[] { "b.employee", "b.week", "b.fact", "b.hours" };
            string[] vars3 = new string[] { "c.employee", "c.week", "c.fact", "c.hours" };
            string[] vars5 = new string[] { "a.employee", "a.week", "a.fact" };
            string[] vars6 = new string[] { "b.employee", "b.week", "b.fact" };
            string[] vars7 = new string[] { "c.employee", "c.week", "c.fact" };
            string[] vars4 = new string[] { "a.employee", "a.week" };
            GJTLeaf base1 = new GJTLeaf("base1", vars1, @"copyfactgross1");
            GJTLeaf base2 = new GJTLeaf("base2", vars2, @"copyfactgross1");
            GJTLeaf base3 = new GJTLeaf("base3", vars3, @"copyfactgross1");              
            GJTLeaf base4 = new GJTLeaf("base4", new string[] { "d.fact" }, @"Factnet");
            base4.inFrontier = true;
            TreeNode selPred1 = ParsePredicate("a.fact == \"gross availability\";");
            TreeNode selPred2 = ParsePredicate("b.fact == \"holiday\";");
            TreeNode selPred3 = ParsePredicate("c.fact == \"education\";");
            TreeNode predicate1 = ParsePredicate("a.employee == b.employee and a.week == b.week;");
            TreeNode predicate2 = ParsePredicate("a.employee == c.employee and a.week == c.week;") ;
             
            GJTInnerNode f1 = new GJTAggregateNode("f1", vars5, new List<GJTNode> { base1 }, new List<TreeNode> { selPred1 });
            GJTInnerNode f2 = new GJTAggregateNode("f2", vars6, new List<GJTNode> { base2 }, new List<TreeNode> { selPred2 });
            GJTInnerNode f3 = new GJTAggregateNode("f3", vars7, new List<GJTNode> { base3 }, new List<TreeNode> { selPred3 });
            f1.inFrontier = true;
            f2.inFrontier = true;
            f3.inFrontier = true;
            GJTInnerNode i1 = new GJTJoinNode(
                    "i1",
                    vars4,
                    new List<GJTNode> { f1, f2 },
                    new List<TreeNode> { null, predicate1 });
            GJTInnerNode i2 = new GJTJoinNode(
                "i2",
                vars4,
                new List<GJTNode> { i1, f3 },
                new List<TreeNode> { null, predicate2 });
            GJTInnerNode root = new GJTJoinNode(
                "vars4",
                vars4,
                new List<GJTNode> { i2, base4 },
                new List<TreeNode> { null, new CartesianProduct() });
            
            return new GeneralJoinTree(
                root,
                new HashSet<GJTNode> { f1, f2, f3, base4},
                new HashSet<GJTLeaf> { base1, base2, base3, base4}
                );
        }
    }
}
