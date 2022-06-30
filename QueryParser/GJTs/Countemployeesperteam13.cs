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
    class Countemployeesperteam13 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf leaf = new GJTLeaf(
                "leaf",
                new List<string> { "employee", "team", "week", "value"},
                "teamrelationtoDC12",
                new FrontierEnumerator()
                );
            leaf.inFrontier = true;
            GJTInnerNode root = new GJTAggregateNode(
                "root",
                new List<string> { "team", "week"},
                new List<GJTNode> { leaf },
                new List<TreeNode> { null},
                new AggregateEnumerator(),
                "count",
                "employee"
                );
            return new AggregateGJT(
                root,
                new HashSet<GJTNode> { leaf},
                new HashSet<GJTLeaf> { leaf},
                new List<string> { "Team", "Week", "Count"},
                new List<string> { "team", "week", "count(employee)"},
                new List<string> { }
                );
        }

        public override string GetName() {
            return "countemployeesperteam13";
        }
    }
}
