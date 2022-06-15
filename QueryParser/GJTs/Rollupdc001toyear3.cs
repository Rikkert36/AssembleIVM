using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class Rollupdc001toyear3 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf base1 = new GJTLeaf("base1",
                new List<string> { "employee", "fact", "week.w", "week.y", "hours" },
                "computenetavail2",
                new FrontierEnumerator()
                );
            GJTInnerNode root = new GJTAggregateNode(
                "root",
                new List<string> { "employee", "fact", "week.y" },
                new List<GJTNode> { base1 },
                new List<TreeNode> { null },
                new AggregateEnumerator(),
                "sum",
                "hours"
                );
            base1.inFrontier = true;
            return new GeneralJoinTree(
                root,
                new HashSet<GJTNode> { base1 },
                new HashSet<GJTLeaf> { base1 },
                new List<string> { "Employee", "Fact", "Week", "Hours" },
                new List<string> { "employee", "fact", "week.y", "sum(hours)" },
                new List<string> {"computenetavail2"}
                ) { 
                splitWeekAndYearValues = true
            };
        }

        public override string GetName() {
            return "rollupdc001toyear3";
        }
    }
}
