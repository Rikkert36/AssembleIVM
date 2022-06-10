using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class SingleJoinNode : JoinNode {
        public SingleJoinNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates, 
            Enumerator enumerator, bool inFrontier) :
            base(name, variables, children, predicates, enumerator, inFrontier) {
        }

        public override List<string> RetrieveHeader() {
            if (inFrontier) {
                return variables;
            } else {
                return Utils.Union(children[0].RetrieveHeader(), children[1].RetrieveHeader());
            }
        }
    }
}
