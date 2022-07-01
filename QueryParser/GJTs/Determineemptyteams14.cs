using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class Determineemptyteams14 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf l1 = new GJTLeaf(
                "l1",
                new List<string> {"a.team", "a.week"},
                "TeamWeekCombos",
                new FrontierEnumerator()
                );
            GJTLeaf l2 = new GJTLeaf(
                "l2",
                new List<string> { "employee", "b.team", "b.week", "value" },
                "teamrelationtoDC12",
                new FrontierEnumerator()
                );
            GJTInnerNode root = new GJTMinusNode(
                "root",
                new List<string> { "a.team", "a.week"},
                new List<GJTNode> { l1, l2},
                new List<TreeNode> { null, ParsePredicate("a.team == b.team and a.week == b.week")},
                new MinusEnumerator()
                );
            l1.inFrontier = true;
            l2.inFrontier = true;
            return new GeneralJoinTree(
                root,
                new HashSet<GJTNode> { l1, l2},
                new HashSet<GJTLeaf> { l1, l2},
                new List<string> { "Team", "Week"},
                new List<string> { "a.team", "a.week"},
                new List<string> { }
                );
        }

        public override string GetName() {
            return "determineemptyteams14";
        }
    }
}
