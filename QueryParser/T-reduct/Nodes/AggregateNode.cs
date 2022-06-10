using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class AggregateNode : UnaryNode {
        string aggregateFunction;

        public AggregateNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates, Enumerator enumerator, bool inFrontier, string aggregateFunction) :
            base(name, variables, children, predicates, enumerator, inFrontier) {
            this.aggregateFunction = aggregateFunction;
        }

        public override void ComputeDelta(NodeReduct node) {
            foreach (GMRTuple tuple in node.delta.projectedAddedTuples) {
                delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
            }
            foreach (GMRTuple tuple in node.delta.projectedRemovedTuples) {
                delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
            }
        }

        public override List<string> RetrieveHeader() {
            string aggregateDimension = Utils.SetMinus(children[0].variables, variables)[0];
            return Utils.Union(variables, new List<string> { $"{aggregateFunction}({aggregateDimension})" });
        }
    }
}
