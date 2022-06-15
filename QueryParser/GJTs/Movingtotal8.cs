using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class Movingtotal8 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf l1 = new GJTLeaf(
                "l1",
                new List<string> { "a.fact", "a.team", "a.week.w", "a.week.y", "a.hours" },
                "rollupdepartments6",
                new FrontierEnumerator(),
                "a.week.w"
                );
            GJTLeaf l2 = new GJTLeaf(
                "l2",
                new List<string> { "b.fact", "length", "b.team", "b.week.w", "b.week.y", "b.hours" },
                "DC002withYL8",
                new FrontierEnumerator(),
                "b.week.w"
                );
            l1.inFrontier = true;
            l2.inFrontier = true;
            TreeNode predicate = ParsePredicate(@" a.fact == b.fact and a.team == b.team and 
        (
            (a.week.y == b.week.y and a.week.w - b.week.w >= 0 and a.week.w - b.week.w < 5) or
            (a.week.y == b.week.y + 1 and a.week.w + length - b.week.w >= 0
            and a.week.w + length - b.week.w < 5)
        )");
            GJTInnerNode root = new GJTAggregateJoinNode(
                "root",
                new List<string> { "a.fact", "a.team", "a.week.w", "a.week.y" },
                new List<GJTNode> { l1, l2 },
                new List<TreeNode> { null, predicate },
                new AggregateJoinEnumerator(),
                "sum",
                "b.hours"
                );
            return new GeneralJoinTree(
                root,
                new HashSet<GJTNode> { l1, l2 },
                new HashSet<GJTLeaf> { l1, l2 },
                new List<string> { "Fact", "Team", "Week", "Hours" },
                new List<string> { "a.fact", "a.team", "a.week.w", "a.week.y", "sum(b.hours)" },
                new List<string> { }
                ) {
                splitWeekAndYearValues = true,
                uniteWeekAndYearValues = true
            };
        }

        public override string GetName() {
            return "movingtotal8";
        }
    }
}
