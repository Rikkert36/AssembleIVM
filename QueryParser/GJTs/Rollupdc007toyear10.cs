using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class Rollupdc007toyear10 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf l1 = new GJTLeaf(
                "l1",
                new List<string> { "fact", "team", "week.w", "week.y", "adjusted_hours" },
                "computeadjustedhours7",
                new FrontierEnumerator()
                );
            l1.inFrontier = true;
            GJTInnerNode root = new GJTAggregateNode(
                "root",
                new List<string> { "fact", "team", "week.y" },
                new List<GJTNode> { l1 },
                new List<TreeNode> { null },
                new AggregateEnumerator(),
                "sum",
                "adjusted_hours"
                );
            return new GeneralJoinTree(
                root,
                new HashSet<GJTNode> { l1 },
                new HashSet<GJTLeaf> { l1 },
                new List<string> { "Fact", "Team", "Week", "Adjusted_hours" },
                new List<string> { "fact", "team", "week.y", "sum(adjusted_hours)" },
                new List<string> { "computeadjustedhours7" }
                ) {
                splitWeekAndYearValues = true
            };
        }

        public override string GetName() {
            return "rollupdc007toyear10";
        }
    }
}
