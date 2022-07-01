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
    class Rollupdepartments6 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf l1 = new GJTLeaf(
                "l1",
                new List<string>{"fact", "a.team", "week", "hours"},
                "sumhoursperdepartment6",
                new FrontierEnumerator()
                );
            GJTLeaf l2 = new GJTLeaf(
                "l2",
                new List<string> { "b.team"},
                "Teamtotal",
                new FrontierEnumerator()
                );
            GJTInnerNode aggregateNode = new GJTSumNode(
                "aggregate",
                new List<string> { "fact", "week"},
                new List<GJTNode> { l1 },
                new List<TreeNode> { },
                new SumEnumerator(),
                "sum",
                "hours"
                );
            GJTInnerNode root = new GJTSingleJoinNode(
                "root",
                new List<string> { "fact", "week"},
                new List<GJTNode> { aggregateNode, l2 },
                new List<TreeNode> { null, new CartesianProduct()},
                new SingleJoinEnumerator()
                );
            return new GeneralJoinTree(
                root,
                new HashSet<GJTNode> { l1, l2},
                new HashSet<GJTLeaf> { l1, l2 },
                new List<string> { "Fact", "Team", "Week", "Hours"},
                new List<string> { "fact", "b.team", "week", "sum(hours)"},
                new List<string> { "sumhoursperteam5","sumhoursperdepartment6" }
                );
        }

        public override string GetName() {
            return "rollupdepartments6";
        }
    }
}
