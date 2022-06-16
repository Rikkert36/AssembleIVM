using AssembleIVM.T_reduct;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTComputerFiles.ConstructorStructures {
    abstract class GJTJoinNode : GJTInnerNode {
        public GJTJoinNode(string name, List<string> variables, List<GJTNode> children, List<TreeNode> predicates, Enumerator enumerator, string orderDimension = "")
            : base(name, variables, children, predicates, enumerator, orderDimension) {
        }

        
    }
}
