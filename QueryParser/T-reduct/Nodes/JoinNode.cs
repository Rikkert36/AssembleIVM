using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using AssembleIVM.T_reduct.Enumerators;

using System.Text;

namespace AssembleIVM.T_reduct {
    abstract class JoinNode : InnerNodeReduct {
        public JoinNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates,
            Enumerator enumerator, bool inFrontier) : base(name, variables, children, predicates, enumerator, inFrontier) {
        }

        public override void ComputeDelta(NodeReduct node) {
            int i = children[0] == node ? 0 : 1;
            int o = i == 0 ? 1 : 0;
            NodeReduct sibling = children[o];
            if ((predicates[i] != null && predicates[i].GetType().Name.Equals("CartesianProduct")) ||
                (predicates[o] != null && predicates[o].GetType().Name.Equals("CartesianProduct"))) {
                if (predicates[i] == null) {
                    foreach (GMRTuple tuple in node.delta.projectedAddedTuples) {
                        delta.unprojectedAddedTuples.Add(tuple);
                    }
                    foreach (GMRTuple tuple in node.delta.projectedRemovedTuples) {
                        delta.unprojectedRemovedTuples.Add(tuple);
                    }
                } 
            } else if (predicates[i] == null) {
                foreach (GMRTuple tuple in node.delta.projectedAddedTuples) {
                    List<GMRTuple> correspondingTuples = sibling
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[o]);
                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count * t.count) { fields = tuple.fields});
                    }
                }
                foreach (GMRTuple tuple in node.delta.projectedRemovedTuples) {
                    List<GMRTuple> correspondingTuples = sibling
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[o]);
                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count * t.count) { fields = tuple.fields });
                    }
                }
            } else {
                foreach (GMRTuple tuple in node.delta.projectedAddedTuples) {
                    List<GMRTuple> correspondingTuples = sibling
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[i]);
                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count * t.count) { fields = t.fields });
                    }
                }
                foreach (GMRTuple tuple in node.delta.projectedRemovedTuples) {
                    List<GMRTuple> correspondingTuples = sibling
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[i]);
                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count * t.count) { fields = t.fields });
                    }
                }
            }
        }

        public override List<string> GetUnprojectHeader() {
            if (predicates[0] == null) {
                return new List<string>(children[0].variables);
            } else {
                return new List<string>(children[1].variables);
            }
        }

        protected override void RemoveTuple(GMRTuple tuple) {
            List<GMRTuple> section = index.Get(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            t.count -= tuple.count;
            if (t.count < 1) {
                section.Remove(t);
                if (section.Count == 0) index.RemoveKey(tuple.fields);
            }
        }
    }
}
