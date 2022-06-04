using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class AggregateNode : InnerNodeReduct {
        public AggregateNode(string name, string[] variables, List<NodeReduct> children, List<TreeNode> predicates) : base(name, variables, children, predicates) {
        }

        public override void ComputeDelta(NodeReduct node) {
            delta.unprojectHeader = new List<string>(node.variables);
            foreach (GMRTuple tuple in node.delta.projectedAddedTuples) {
                if (predicates[0] != null) {
                    if (new PredicateTupleEvaluator().Evaluate(new List<string>(variables), tuple, predicates[0])) {
                        string key = tuple.ToString();
                        delta.unprojectedAddedTuples[key] = new Tuple<GMRTuple, int>(tuple, tuple.count);
                    }
                } else {
                    string key = tuple.ToString();
                    delta.unprojectedAddedTuples[key] = new Tuple<GMRTuple, int>(tuple, tuple.count);
                }
            }
            foreach (GMRTuple tuple in node.delta.projectedRemovedTuples) {
                string key = tuple.ToString();
                delta.unprojectedRemovedTuples[key] = new Tuple<GMRTuple, int>(tuple, tuple.count);
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
