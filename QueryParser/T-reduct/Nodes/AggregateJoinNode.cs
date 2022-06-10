using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class AggregateJoinNode : JoinNode {
        public AggregateJoinNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates, Enumerator enumerator, bool inFrontier) :
            base(name, variables, children, predicates, enumerator, inFrontier) {
        }

        public override List<string> RetrieveHeader() {
            throw new NotImplementedException();
        }
    }
}
