using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public override void ProjectUpdate() {
            base.ProjectUpdate();
            delta.AddUnionTuples();
        }

        override public List<GMRTuple> SemiJoinAdded(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            return delta.SemiJoinUnion(rightHeader, rightTuple, predicate);
        }

        override public List<GMRTuple> SemiJoinRemoved(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            return delta.SemiJoinUnion(rightHeader, rightTuple, predicate);
        }

        public override List<string> RetrieveHeader() {
            return Utils.Union(variables, new List<string> { $"{aggregateFunction}({aggregateDimension})" });
        }
    }
}
