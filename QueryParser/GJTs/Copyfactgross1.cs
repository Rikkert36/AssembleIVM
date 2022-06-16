using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using AssembleIVM.TreeNodes.Predicates;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System.Collections.Generic;


namespace AssembleIVM.GJTs {
    class Copyfactgross1 : ManualGJT {
        public override string GetName() {
            return "copyfactgross1";
        }
        public override GeneralJoinTree Construct() { 
            GJTLeaf l1 = new GJTLeaf("l1", new List<string>
            { "DC005.employee", "DC005.week", "DC005.hours" }, "DC005", new FrontierEnumerator());
            GJTLeaf l2 = new GJTLeaf("l2", new List<string> { "Factgross.fact" }, "Factgross", new FrontierEnumerator());
            l1.inFrontier = true;
            l2.inFrontier = true;


            GJTSingleJoinNode n1 = new GJTSingleJoinNode( "n1",
                new List<string> { "DC005.employee", "DC005.week" },
                new List<GJTNode> { l1, l2 },
                new List<TreeNode> { null, new CartesianProduct()},
                new SingleJoinEnumerator()
                );

            return new GeneralJoinTree(n1, new HashSet<GJTNode> { l1, l2 }, new HashSet<GJTLeaf> { l1, l2},
                new List<string> { "Employee", "Fact", "Week", "Hours"},
                new List<string> {"DC005.employee;", "Factgross.fact", "DC005.week", "DC005.hours" },
                new List<string> { "DC001" });
        }

    }
}
