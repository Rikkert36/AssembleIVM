using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using AssembleIVM.TreeNodes.Predicates;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class TeamrelationtoDC12 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf l1 = new GJTLeaf(
                "l1",
                new List<string> { "employee", "week", "team"},
                "R001",
                new FrontierEnumerator()
                );
            GJTLeaf l2 = new GJTLeaf(
                "l2",
                new List<string> { "value"},
                "Valuetrue",
                new FrontierEnumerator()
                );
            l1.inFrontier = true;
            l2.inFrontier = true;
            GJTInnerNode root = new GJTSingleJoinNode(
                "root",
                new List<string> { "employee", "week"},
                new List<GJTNode> { l1, l2},
                new List<TreeNode> { null, new CartesianProduct()},
                new SingleJoinEnumerator()
                );
            return new GeneralJoinTree(
                root,
                new HashSet<GJTNode> { l1, l2},
                new HashSet<GJTLeaf> { l1, l2},
                new List<string> { "Employee", "Team", "Week", "Value"},
                new List<string> { "employee", "team", "week", "value"},
                new List<string> { }
                );

        }

        public override string GetName() {
            return "teamrelationtoDC12";
        }
    }
}
