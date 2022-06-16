using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class AggregateNode : UnaryNode {
        public string aggregateFunction;
        public string aggregateDimension;

        public AggregateNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates,
            Enumerator enumerator, bool inFrontier, string aggregateFunction, string aggregateDimension) :
            base(name, variables, children, predicates, enumerator, inFrontier) {
            this.aggregateFunction = aggregateFunction;
            this.aggregateDimension = aggregateDimension;
        }

        public override void ComputeDelta(NodeReduct node) {
            foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields }); 
            }
            foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
            }
        }

        public override List<string> RetrieveHeader() {
            return Utils.Union(variables, new List<string> { $"{aggregateFunction}({aggregateDimension})" });
        }
    }
}
