using AssembleIVM.GJTComputerFiles;
using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class QRollupteams6 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf l1 = new GJTLeaf(
                "l1",
                new List<string> { "fact", "a.team", "week", "hours" },
                "sumhoursperteam5",
                new FrontierEnumerator()
                );
            GJTLeaf l2 = new GJTLeaf(
                "l2",
                new List<string> { "b.team", "department" },
                "TeamDepartment",
                new FrontierEnumerator()
                );
            l1.inFrontier = true;
            l2.inFrontier = true;
            TreeNode predicate = ParsePredicate("a.team == b.team");
            GJTInnerNode root = new GJTSingleJoinNode(
                "root",
                new List<string> { "a.team" },
                new List<GJTNode> { l1, l2 },
                new List<TreeNode> { null, predicate},
                new SingleJoinEnumerator()
                );
            return new SpeedAggregateGeneralJoinTree(
                root,
                new HashSet<GJTNode> { l1, l2},
                new HashSet<GJTLeaf> { l1, l2},
                new List<string> { "Fact", "Team", "Week", "Hours"},
                new List<string> { "fact", "department", "week", "sum(hours)"},
                new List<string> { },
                "a.team",
                "department",
                "hours"
                );
        }

        public override string GetName() {
            return "Qrollupteams6";
        }
    }
}
