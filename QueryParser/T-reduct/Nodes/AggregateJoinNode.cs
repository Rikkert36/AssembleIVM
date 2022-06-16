using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class AggregateJoinNode : JoinNode {
        public string aggregateFunction;
        public string aggregateDimension;
        public AggregateJoinNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates,
            Enumerator enumerator, bool inFrontier, string aggregateFunction, string aggregateDimension, string orderDimension = "") :
            base(name, variables, children, predicates, enumerator, inFrontier, orderDimension) {
            this.aggregateFunction = aggregateFunction;
            this.aggregateDimension = aggregateDimension;
        }

        public override List<string> RetrieveHeader() {
            return Utils.Union(variables, new List<string> { $"{aggregateFunction}({aggregateDimension})" });
        }
    }
}
