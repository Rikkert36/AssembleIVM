using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class TestCount : ManualGJT {
        public override string GetName() {
            return "testcount";
        }
        public override GeneralJoinTree Construct() {
            GJTLeaf A = new GJTLeaf("A", new string[]
                { "A.a", "A.b", "A.c", "A.d"}, "testinput/A");
            GJTLeaf B = new GJTLeaf("B", new string[]
                { "B.a", "B.b", "B.c", "B.d"}, "testinput/B");
            GJTLeaf C = new GJTLeaf("C", new string[]
                { "C.a", "C.b", "C.c"}, "testinput/C");
            TreeNode predicate1 = ParsePredicate("A.a == B.a and A.b == B.b;");
            TreeNode predicate2 = ParsePredicate("A.a == C.a;");

            GJTInnerNode n1 = new GJTJoinNode(
                "n1",
                new string[] { "A.a", "A.b" },
                new List<GJTNode> { A, B },
                new List<TreeNode> { null, predicate1});
            GJTInnerNode n3 = new GJTAggregateNode(
                 "n1",
                 new string[] { "C.a", "C.b" },
                 new List<GJTNode> { C },
                 new List<TreeNode> { null});
            GJTInnerNode n2 = new GJTJoinNode(
                "n2",
                new string[] { "A.a"},
                new List<GJTNode> { n1, n3 },
                new List<TreeNode> { null, predicate2 });
            return new GeneralJoinTree(
                n2,
                new HashSet<GJTNode> { },
                new HashSet<GJTLeaf> { A, B, C });
        }
    }
}
