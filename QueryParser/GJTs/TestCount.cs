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
            GJTLeaf A = new GJTLeaf("A", new List<string>
                { "A.a", "A.b", "A.c", "A.d"}, "testinput/A", null);
            GJTLeaf B = new GJTLeaf("B", new List<string>
                { "B.a", "B.b", "B.c", "B.d"}, "testinput/B", null);
            GJTLeaf C = new GJTLeaf("C", new List<string>
                { "C.a", "C.b", "C.c"}, "testinput/C", null);
            TreeNode predicate1 = ParsePredicate("A.a == B.a and A.b == B.b;");
            TreeNode predicate2 = ParsePredicate("A.a == C.a;");

            GJTInnerNode n1 = new GJTSingleJoinNode(
                "n1",
                new List<string> { "A.a", "A.b" },
                new List<GJTNode> { A, B },
                new List<TreeNode> { null, predicate1}, null);
            GJTInnerNode n3 = new GJTSumNode(
                 "n1",
                 new List<string> { "C.a", "C.b" },
                 new List<GJTNode> { C },
                 new List<TreeNode> { null},
                 null, 
                 "");
            GJTInnerNode n2 = new GJTSingleJoinNode(
                "n2",
                new List<string> { "A.a"},
                new List<GJTNode> { n1, n3 },
                new List<TreeNode> { null, predicate2 },
                null);
            return new GeneralJoinTree(
                n2,
                new HashSet<GJTNode> { },
                new HashSet<GJTLeaf> { A, B, C },
                null, null, new List<string> { });
        }


    }
}
