using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class CountNode : UnaryNode {
        public string aggregateDimension;
        public CountNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates,
            Enumerator enumerator, bool inFrontier, string aggregateDimension) :
            base(name, variables, children, predicates, enumerator, inFrontier) {
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

        public override void ProjectUpdate() {
            delta.ProjectTuples(this.variables);
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

        override public List<GMRTuple> SemiJoinAdded(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            return delta.SemiJoinUnion(rightHeader, rightTuple, predicate);
        }

        override public List<GMRTuple> SemiJoinRemoved(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            return delta.SemiJoinUnion(rightHeader, rightTuple, predicate);
        }

        public override List<string> RetrieveHeader() {
            return Utils.Union(variables, new List<string> { $"count({aggregateDimension})" });
        }

        //Is also never used since RemoveTuple is only applied in ApplyUpdate(), but that is overwritten by a method that does not use RemoveTuple
        protected override void RemoveTuple(GMRTuple deletion) {
            throw new NotImplementedException();
        }
    }
}
