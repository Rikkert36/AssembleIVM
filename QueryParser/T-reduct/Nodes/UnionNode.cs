using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class UnionNode : InnerNodeReduct {
        public UnionNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates, 
            Enumerator enumerator, bool inFrontier, string orderDimension = "") : 
            base(name, variables, children, predicates, enumerator, inFrontier, orderDimension) {
        }

        public override void ComputeDelta(NodeReduct node) {
            foreach(GMRTuple tuple in node.delta.GetAddedTuples()) {
                TupleCounter.Increment();
                delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
            }
            foreach(GMRTuple tuple in node.delta.GetRemovedTuples()) {
                TupleCounter.Increment();
                delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields});
            }
        }

        public override List<string> GetUnprojectHeader() {
            return new List<string>(children[0].variables);
        }

        public override List<string> RetrieveHeader() {
            return new List<string>(children[0].RetrieveHeader());
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
