using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using AssembleIVM.T_reduct.Enumerators;

using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    abstract class UnaryNode : InnerNodeReduct {
        public UnaryNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates,
            Enumerator enumerator, bool inFrontier) : base(name, variables, children, predicates, enumerator, inFrontier) {

        }

        public override List<string> GetUnprojectHeader() {
            return new List<string>(children[0].variables);
        }

        
    }
}
