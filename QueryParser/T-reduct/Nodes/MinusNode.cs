using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using AssembleIVM.T_reduct.Enumerators;
using System.Text;
using System.Linq;
using QueryParser.NewParser.TreeNodes.Predicates;

namespace AssembleIVM.T_reduct.Nodes {
    class MinusNode : InnerNodeReduct {
        public MinusNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates,
            Enumerator enumerator, bool inFrontier) : base(name, variables, children, predicates, enumerator, inFrontier) {
        }

        public override void ComputeDelta(NodeReduct node) {
            if (node == children[0]) {
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) {
                        fields = tuple.fields,
                        isPlusTuple = true
                    });
                }
                foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                    delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) {
                        fields = tuple.fields,
                        isPlusTuple = true
                    });
                }
            } else {
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
                }
                foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                    delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
                }
            }

        }

        public override void ProjectUpdate() {
            delta.ProjectTuples(GetEqualValues());
        }

        private List<string> GetEqualValues() {
            return variables.Select(s => GetEqualValue(s)).ToList();
        }

        private string GetEqualValue(string s) {
            string result = _GetEqualValue(s, predicates[1]);
            if (result != "") {
                return result;
            } else {
                throw new Exception($"cannot find variable {s} in predicate {predicates[1].GetString()}");
            }
        }

        private string _GetEqualValue(string s, TreeNode predicate) {
            if (predicate.GetType().Name.Equals("And")) {
                And and = (And)predicate;
                string s1 = _GetEqualValue(s, and.left);
                string s2 = _GetEqualValue(s, and.right);
                if (s1 != "") {
                    return s1;
                } else if (s2 != "" ){
                    return s2;
                } else {
                    return "";
                }
            } else if (predicate.GetType().Name.Equals("Comparison")) {
                Comparison comparison = (Comparison)predicate;
                string s1 = comparison.left.GetString();
                string s2 = comparison.right.GetString();
                if (s.Equals(s1)) {
                    return s2;
                } else if (s.Equals(s2)) {
                    return s1;
                } else {
                    return "";
                }
            } else {
                throw new Exception($"Missed type {predicate.GetType().Name}");
            }
        }

        public List<GMRTuple> Get(GMRTuple tuple) {
            return index.Get(tuple.fields);
        }

        public bool ExistsAddedTuple(GMRTuple tuple) {
            return delta.projectedAddedTuples.Get(tuple.fields).Count > 0;
        }


        public override List<string> RetrieveHeader() {
            return new List<string>(variables);
        }

        public override List<string> GetUnprojectHeader() {
            return new List<string>(children[1].variables);
        }

        public override void AddTuple(GMRTuple tuple) {
            List<GMRTuple> section = index.GetOrPlace(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            if (t == null) {
                t = new GMRTuple(tuple.count, 0) { fields = tuple.fields };
                if (index.orderDimension.Equals("")) {
                    section.Add(t);
                } else {
                    int loc = index.FindLocation(section, t);
                    section.Insert(loc, t);
                }
            };
            if (tuple.isPlusTuple) t.isPlusTuple = true;
            t.count += tuple.count;
        }

        protected override void RemoveTuple(GMRTuple tuple) {
            List<GMRTuple> section = index.GetOrPlace(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            if (t == null) {
                t = new GMRTuple(tuple.count, 0) { fields = tuple.fields };
                if (index.orderDimension.Equals("")) {
                    section.Add(t);
                } else {
                    int loc = index.FindLocation(section, t);
                    section.Insert(loc, t);
                }
            };
            if (tuple.isPlusTuple) t.isPlusTuple = false;
            t.count -= tuple.count;
            if (t.count == 0 && !t.isPlusTuple) {
                section.Remove(t);
            }
        }
    }
}
