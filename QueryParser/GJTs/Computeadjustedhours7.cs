using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class Computeadjustedhours7 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf l1 = new GJTLeaf(
                "l1",
                new List<string> { "a.fact", "team", "a.week", "hours"},
                "rollupdepartments6",
                new FrontierEnumerator()
                );
            GJTLeaf l2 = new GJTLeaf(
                "l2",
                new List<string> { "b.fact", "b.week", "percentage"},
                "DC006",
                new FrontierEnumerator()
                );
            l1.inFrontier = true;
            l2.inFrontier = true;
            TreeNode predicate = ParsePredicate("a.fact == b.fact and a.week == b.week");
            GJTInnerNode root = new GJTSingleJoinNode(
                "root",
                new List<string> { "a.fact", "a.week"},
                new List<GJTNode> { l1, l2},
                new List<TreeNode> { null, predicate},
                new JoinEnumerator()
                );
            return new GeneralJoinTree(
                root,
                new HashSet<GJTNode> { l1, l2},
                new HashSet<GJTLeaf> { l1, l2},
                new List<string> { "Fact", "Team", "Week", "Adjusted hours"},
                new List<string> { "a.fact", "team", "a.week", "int(hours * (1 + percentage))"},
                new List<string> { }
                );
        }

        public override string GetName() {
            return "computeadjustedhours7";
        }
    }
}
