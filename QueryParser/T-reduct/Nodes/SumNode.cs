using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class SumNode : UnaryNode {
        public string aggregateDimension;

        public SumNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates,
            Enumerator enumerator, bool inFrontier, string aggregateDimension) :
            base(name, variables, children, predicates, enumerator, inFrontier) {
            this.aggregateDimension = aggregateDimension;
        }

        public  override void ComputeDelta(NodeReduct node) {
            int aggregateDimensionIndex = node.variables.IndexOf(aggregateDimension);
            foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                TupleCounter.Increment();
                delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields, sum = new Number(tuple.fields[aggregateDimensionIndex])}); 
            }
            foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                TupleCounter.Increment();
                delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields, sum = new Number(tuple.fields[aggregateDimensionIndex]) });
            }
        }

        public override void ProjectUpdate() {
            delta.ProjectTuplesWithAggregateValue(this.variables);
            delta.AddUnionTuples();
        }

        public override void ApplyUpdate() {
            foreach (GMRTuple tuple in delta.GetUnionTuples()) {
                AddUnionTuple(tuple);
            }
        }

        public void AddUnionTuple(GMRTuple tuple) {
            List<GMRTuple> section = index.GetOrPlace(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            if (t != null && t.Equals(tuple)) {
                t.count += tuple.count;
                t.sum.value += tuple.sum.value;
            } else {
                if (index.orderDimension.Equals("")) {
                    section.Add(tuple);
                } else {
                    int loc = index.FindLocation(section, tuple);
                    section.Insert(loc, tuple);
                }
            }
            if (t != null && t.count < 1) {
                section.Remove(t);
                if (section.Count == 0) index.RemoveKey(tuple.fields);
            }
        }

        //Never used, but should be overwritten
        protected override void RemoveTuple(GMRTuple tuple) {
            List<GMRTuple> section = index.Get(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            t.count -= tuple.count;
            t.sum.value -= tuple.sum.value;
            if (t.count < 1) {
                section.Remove(t);
                if (section.Count == 0) index.RemoveKey(tuple.fields);
            }
        }

        override public List<GMRTuple> SemiJoinAdded(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            return delta.SemiJoinUnion(rightHeader, rightTuple, predicate);
        }

        override public List<GMRTuple> SemiJoinRemoved(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            return delta.SemiJoinUnion(rightHeader, rightTuple, predicate);
        }

        public override List<string> RetrieveHeader() {
            return Utils.Union(variables, new List<string> { $"sum({aggregateDimension})" });
        }
    }
}
