using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class SingleJoinNode : JoinNode {
        public SingleJoinNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates, 
            Enumerator enumerator, bool inFrontier, string orderDimension = "") :
            base(name, variables, children, predicates, enumerator, inFrontier, orderDimension) {
        }

        public override void ComputeDelta(NodeReduct node) {
            int i = children[0] == node ? 0 : 1;
            int o = i == 0 ? 1 : 0;
            NodeReduct sibling = children[o];
            if ((predicates[i] != null && predicates[i].GetType().Name.Equals("CartesianProduct")) ||
                (predicates[o] != null && predicates[o].GetType().Name.Equals("CartesianProduct"))) {
                if (predicates[i] == null) {
                    foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                        delta.unprojectedAddedTuples.Add(tuple);
                    }
                    foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                        delta.unprojectedRemovedTuples.Add(tuple);
                    }
                }
            } else if (predicates[i] == null) {
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    List<GMRTuple> correspondingTuples = sibling
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[o]);
                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count * t.count) { fields = tuple.fields });
                    }
                }
                foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                    List<GMRTuple> correspondingTuples = sibling
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[o]);
                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count * t.count) { fields = tuple.fields });
                    }
                }
            } else {
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    List<GMRTuple> correspondingTuples = sibling
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[i]);
                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedAddedTuples.Add(new GMRTuple(t.fields.Length, tuple.count * t.count) { fields = t.fields });
                    }
                }
                foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                    List<GMRTuple> correspondingTuples = sibling
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[i]);
                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedRemovedTuples.Add(new GMRTuple(t.fields.Length, tuple.count * t.count) { fields = t.fields });
                    }
                }
            }
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
