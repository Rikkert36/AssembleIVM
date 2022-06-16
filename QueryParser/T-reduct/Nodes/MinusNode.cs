using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using AssembleIVM.T_reduct.Enumerators;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class MinusNode : InnerNodeReduct {
        public MinusNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates,
            Enumerator enumerator, bool inFrontier): base(name, variables, children, predicates, enumerator, inFrontier) { 
        }

        public override void ComputeDelta(NodeReduct node) {
            if (node == children[0]) {
                NodeReduct sibling = children[1];
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    if (!sibling.AnyJoin(new List<string>(node.variables), tuple, predicates[1])) {
                        delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count ) { fields = tuple.fields });
                    }
                }
                foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                    if (!sibling.AnyJoin(new List<string>(node.variables), tuple, predicates[1])) {
                        delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
                    };
                }
            } else {
                NodeReduct sibling = children[0];
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    foreach (GMRTuple correspondingTuple in sibling.SemiJoin(new List<string>(node.variables), tuple, predicates[1])) {
                        delta.unprojectedRemovedTuples.Add(new GMRTuple(correspondingTuple.fields.Length, correspondingTuple.count) { fields = correspondingTuple.fields });

                    }
                }
                foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                    foreach (GMRTuple correspondingTuple in sibling.SemiJoin(new List<string>(node.variables), tuple, predicates[1])) {
                        if (!node.AnyJoin(new List<string>(variables), correspondingTuple, predicates[1])) {
                            delta.unprojectedAddedTuples.Add(new GMRTuple(correspondingTuple.fields.Length, correspondingTuple.count) { fields = correspondingTuple.fields });

                        }
                    }
                }

            }
        }

        public override List<string> RetrieveHeader() {
            return new List<string> ( variables );
        }

        public override List<string> GetUnprojectHeader() {
            return new List<string>(children[0].variables);
        }

        protected override void RemoveTuple(GMRTuple tuple) {
            if (!index.ContainsKey(tuple.fields)) return;
            List<GMRTuple> section = index.Get(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            if (t == null) return;
            section.Remove(t);
        }
    }
}
