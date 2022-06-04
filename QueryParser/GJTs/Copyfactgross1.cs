using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.TreeNodes.Predicates;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    class Copyfactgross1 : ManualGJT {
        public override string GetName() {
            return "copyfactgross1";
        }
        public override GeneralJoinTree Construct() { 
            GJTLeaf l1 = new GJTLeaf("l1", new string[]
            { "DC005.employee", "DC005.week", "DC005.hours" }, @"DC005");
            GJTLeaf l2 = new GJTLeaf("l2", new string[] { "Factgross.fact" }, @"Factgross");
            l1.inFrontier = true;
            l2.inFrontier = true;

            GJTJoinNode n1 = new GJTJoinNode( "n1",
                new string[] { "DC005.employee", "DC005.week" },
                new List<GJTNode> { l1, l2 },
                new List<TreeNode> { null, new CartesianProduct()}
                );
            return new GeneralJoinTree(n1, new HashSet<GJTNode> { l1, l2 }, new HashSet<GJTLeaf> { l1, l2});
        }

        
    }
}
