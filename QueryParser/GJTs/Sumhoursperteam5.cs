using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class Sumhoursperteam5 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf base1 = new GJTLeaf("base1",
                new List<string> { "employee", "fact", "team", "week", "hours"},
                "fillinteams4",
                new FrontierEnumerator()
                );
            GJTInnerNode root = new GJTAggregateNode(
                "root",
                new List<string> { "fact", "team", "week"},
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
                new List<string> { "Fact", "Team", "Week", "Hours"},
                new List<string> { "fact", "team", "week", "sum(hours)"},
                new List<string> { }
                );
        }

        public override string GetName() {
            return "sumhoursperteam5";
        }
    }
}
