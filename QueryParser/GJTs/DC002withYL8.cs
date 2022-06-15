using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class DC002withYL8 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf l1 = new GJTLeaf(
                "l1",
                new List<string> { "fact", "team", "week.w", "week.y", "hours"},
                "rollupdepartments6",
                new FrontierEnumerator()
                );
            GJTLeaf l2 = new GJTLeaf(
                "l2",
                new List<string> { "year", "length"},
                "Yearlengths",
                new FrontierEnumerator()
                );
            l1.inFrontier = true;
            l2.inFrontier = true;
            TreeNode predicate = ParsePredicate("week.y == year");
            GJTInnerNode root = new GJTSingleJoinNode(
                "root",
                new List<string> { "week.y"},
                new List<GJTNode> { l1, l2},
                new List<TreeNode> { null, predicate},
                new JoinEnumerator()
                );
            return new GeneralJoinTree(
                root,
                new HashSet<GJTNode> {l1, l2 },
                new HashSet<GJTLeaf> { l1, l2},
                new List<string> { "Fact", "Length", "Team", "Week", "Hours" },
                new List<string> { "fact", "length", "team", "week.w", "week.y", "hours"},
                new List<string> { }
                ) {
                splitWeekAndYearValues = true,
                uniteWeekAndYearValues = true
            };
        }

        public override string GetName() {
            return "DC002withYL8";
        }
    }
}
