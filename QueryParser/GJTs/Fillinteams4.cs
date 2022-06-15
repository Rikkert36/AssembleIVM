using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class Fillinteams4 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf l1 = new GJTLeaf("l1",
                new List<string> { "a.employee", "fact", "a.week", "hours" },
                "computenetavail2",
                new FrontierEnumerator()
                );
            GJTLeaf l2 = new GJTLeaf("l2",
                new List<string> { "b.employee", "b.week", "team"},
                "r001",
                new FrontierEnumerator()
                );
            TreeNode predicate = ParsePredicate("a.employee == b.employee and a.week == b.week");
            GJTInnerNode root = new GJTSingleJoinNode("root",
                new List<string> { "a.employee", "a.week"},
                new List<GJTNode> { l1, l2 },
                new List<TreeNode> { null, predicate},
                new JoinEnumerator()
                );
            l1.inFrontier = true;
            l2.inFrontier = true;
            return new GeneralJoinTree(
                root,
                new HashSet<GJTNode> { l1, l2 },
                new HashSet<GJTLeaf> { l1, l2 },
                new List<string> { "Employee", "Fact", "Team", "Week", "Hours"},
                new List<string> { "a.employee", "fact", "team", "a.week", "hours" },
                new List<string> { }
                );
        }

        public override string GetName() {
            return "fillinteams4";
        }
    }
}
