using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct {
    class JoinNode : InnerNodeReduct {
        public JoinNode(string name, string[] variables, List<NodeReduct> children, List<TreeNode> predicates) : base(name, variables, children, predicates) {
        }

        public override void ComputeDelta(NodeReduct node) {
            int i = children[0] == node ? 0 : 1;
            int o = i == 0 ? 1 : 0;
            NodeReduct sibling = children[o];
            if ((predicates[i] != null && predicates[i].GetType().Name.Equals("CartesianProduct")) ||
                (predicates[o] != null && predicates[o].GetType().Name.Equals("CartesianProduct"))) {
                if (predicates[i] == null) {
                    delta.unprojectHeader = new List<string>(node.variables);
                    foreach (GMRTuple tuple in node.delta.projectedAddedTuples) {
                        string key = tuple.ToString();
                        delta.unprojectedAddedTuples[key] = new Tuple<GMRTuple, int>(tuple, tuple.count);
                    }
                    foreach (GMRTuple tuple in node.delta.projectedRemovedTuples) {
                        string key = tuple.ToString();
                        delta.unprojectedRemovedTuples[key] = new Tuple<GMRTuple, int>(tuple, tuple.count);
                    }
                }
            } else if (predicates[i] == null) {
                delta.unprojectHeader = new List<string>(node.variables);
                foreach (GMRTuple tuple in node.delta.projectedAddedTuples) {
                    List<GMRTuple> correspondingTuples = sibling.index
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[o]);
                    foreach (GMRTuple t in correspondingTuples) {
                        string key = tuple.ToString() + t.ToString();
                        delta.unprojectedAddedTuples[key] = new Tuple<GMRTuple, int>(tuple, tuple.count * t.count);
                    }
                }
                foreach (GMRTuple tuple in node.delta.projectedRemovedTuples) {
                    List<GMRTuple> correspondingTuples = sibling.index
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[o]);
                    foreach (GMRTuple t in correspondingTuples) {
                        string key = tuple.ToString() + t.ToString();
                        delta.unprojectedRemovedTuples[key] = new Tuple<GMRTuple, int>(tuple, tuple.count * t.count);
                    }
                }
            } else {
                delta.unprojectHeader = new List<string>(sibling.variables);
                foreach (GMRTuple tuple in node.delta.projectedAddedTuples) {
                    List<GMRTuple> correspondingTuples = sibling.index
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[i]);
                    foreach (GMRTuple t in correspondingTuples) {
                        string key = t.ToString() + tuple.ToString();
                        delta.unprojectedAddedTuples[key] = new Tuple<GMRTuple, int>(t, tuple.count * t.count);
                    }
                }
                foreach (GMRTuple tuple in node.delta.projectedRemovedTuples) {
                    List<GMRTuple> correspondingTuples = sibling.index
                        .SemiJoin(new List<string>(node.variables), tuple, predicates[i]);
                    foreach (GMRTuple t in correspondingTuples) {
                        string key = t.ToString() + tuple.ToString();
                        delta.unprojectedRemovedTuples[key] = new Tuple<GMRTuple, int>(t, tuple.count * t.count);
                    }
                }
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
