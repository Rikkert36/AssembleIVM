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
    class Sumhoursperdepartment6 : ManualGJT {
        public override GeneralJoinTree Construct() {
            GJTLeaf base1 = new GJTLeaf("base1",
                new List<string> { "fact", "department", "team", "week", "hours" },
                "fillindepartments6",
                new FrontierEnumerator()
                );
            GJTInnerNode root = new GJTSumNode(
                "root",
                new List<string> { "fact", "department", "week" },
                new List<GJTNode> { base1 },
                new List<TreeNode> { null },
                new SumEnumerator(),
                "hours"
                );
            base1.inFrontier = true;
            return new AggregateGJT(
                root,
                new HashSet<GJTNode> { base1 },
                new HashSet<GJTLeaf> { base1 },
                new List<string> { "Fact", "Department", "Week", "Hours" },
                new List<string> { "fact", "department", "week", "sum(hours)" },
                new List<string> { }
                );
        }

        public override string GetName() {
            return "sumhoursperdepartment6";
        }
    }
}
