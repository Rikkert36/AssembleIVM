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
            delta.ProjectTuplesWithAggregateValue(this.variables);
            delta.AddUnionTuples();
        }

        public override void ComputeDelta(NodeReduct node) {
            int i = children[0] == node ? 0 : 1;
            int o = i == 0 ? 1 : 0;
            NodeReduct sibling = children[o];
             if (predicates[i] == null) {
                int aggregateDimensionIndex = sibling.variables.IndexOf(aggregateDimension);
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    List<GMRTuple> correspondingTuples = sibling
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[o]);
                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count * t.count) 
                        { fields = tuple.fields, sum = new Number(t.fields[aggregateDimensionIndex]) });
                    }
                }
                foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                    List<GMRTuple> correspondingTuples = sibling
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[o]);
                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count * t.count)
                        { fields = tuple.fields, sum = new Number(t.fields[aggregateDimensionIndex]) });
                    }
                }
            } else {
                int aggregateDimensionIndex = node.variables.IndexOf(aggregateDimension);
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    List<GMRTuple> correspondingTuples = sibling
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[i]);
                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedAddedTuples.Add(new GMRTuple(t.fields.Length, tuple.count * t.count) 
                        { fields = t.fields, sum = new Number(tuple.fields[aggregateDimensionIndex]) });
                    }
                }
                foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                    List<GMRTuple> correspondingTuples = sibling
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[i]);
                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedRemovedTuples.Add(new GMRTuple(t.fields.Length, tuple.count * t.count)
                        { fields = t.fields, sum = new Number(tuple.fields[aggregateDimensionIndex]) });
                    }
                }
            }
        }

        public List<GMRTuple> SemiJoinLeftChild(GMRTuple tuple) {
            return children[0].index.SemiJoinLeftChild(this.variables, tuple);
        }

        public List<GMRTuple> SemiJoinLeftChildAdded(GMRTuple tuple) {
            return children[0].delta.projectedAddedTuples.SemiJoinLeftChild(this.variables, tuple);
        }
        public List<GMRTuple> SemiJoinLeftChildRemoved(GMRTuple tuple) {
            return children[0].delta.projectedRemovedTuples.SemiJoinLeftChild(this.variables, tuple);
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
