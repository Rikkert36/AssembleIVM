using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class SelectNode : UnaryNode {
        public SelectNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates, Enumerator enumerator, bool inFrontier) : base(name, variables, children, predicates, enumerator, inFrontier) {
        }

        public override void ComputeDelta(NodeReduct node) {
            foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                TupleCounter.Increment();
                if (new PredicateTupleEvaluator().Evaluate(new List<string>(node.variables), tuple, predicates[0])) {
                    delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
                }
            }
            foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                TupleCounter.Increment();
                if (new PredicateTupleEvaluator().Evaluate(new List<string>(node.variables), tuple, predicates[0])) {
                    delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
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

        public override List<string> RetrieveHeader() {
            return variables;
        }
    }
}
