using AssembleIVM;
using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.GJTComputerFiles.ConstructorStructures {
    abstract class GJTInnerNode : GJTNode {
        public List<GJTNode> children;
        public List<TreeNode> predicates;
        public GJTInnerNode(string name, string[] variables, List<GJTNode> children, List<TreeNode> predicates):
            base(name, variables) {
            this.children = children;
            this.predicates = predicates;
        }  


        
    }
}
