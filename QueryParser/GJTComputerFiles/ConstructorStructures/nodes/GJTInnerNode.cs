using AssembleIVM;
using AssembleIVM.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using AssembleIVM.T_reduct;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.GJTComputerFiles.ConstructorStructures {
    abstract class GJTInnerNode : GJTNode {
        public List<GJTNode> children;
        public List<TreeNode> predicates;
        public GJTInnerNode(string name, List<string> variables, List<GJTNode> children, List<TreeNode> predicates, Enumerator enumerator, string orderDimension = "") :
            base(name, variables, enumerator, orderDimension) {
            this.children = children;
            this.predicates = predicates;
        }  


        
    }
}
