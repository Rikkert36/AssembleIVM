using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using AssembleIVM.T_reduct.Enumerators;
using System.Text;
using System.Linq;
using QueryParser.NewParser.TreeNodes.Predicates;


namespace AssembleIVM.T_reduct.Nodes {
    class AntiJoinNode : InnerNodeReduct {
        public AntiJoinNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates,
            Enumerator enumerator, bool inFrontier) : base(name, variables, children, predicates, enumerator, inFrontier) {
        }

        public override void ComputeDeltaInitial(NodeReduct node) {
            if (node == children[0]) {
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    TupleCounter.Increment();
                    if (!children[1].AnyJoin(node.variables, tuple, predicates[1])) {
                        delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
                    }
                }
            } else {
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    TupleCounter.Increment();
                    GMRTuple t2 = children[0].SemiJoin(node.variables, tuple, predicates[1])[0];
                    if (t2 != null) {
                        delta.unprojectedRemovedTuples.Add(new GMRTuple(t2.fields.Length, t2.count) { fields = t2.fields });
                    }
                }
            }
        }

        public override void ComputeDelta(NodeReduct node) {
            if (node == children[0]) {
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    TupleCounter.Increment();
                    if (!children[1].AnyJoin(node.variables, tuple, predicates[1]) &&
                        node.delta.projectedRemovedTuples.GetTuple(tuple) == null) {
                        delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
                    }
                }
                foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                    TupleCounter.Increment();
                    if (node.delta.projectedAddedTuples.GetTuple(tuple) == null &&
                           !children[1].AnyJoin(node.variables, tuple, predicates[1])) {
                        delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
                    }
                }
            } else {
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    TupleCounter.Increment();
                    GMRTuple t1 = children[0].SemiJoin(node.variables, tuple, predicates[1])[0];
                    if (node.delta.projectedRemovedTuples.GetTuple(tuple) == null &&
                        t1 != null) {
                        int count = 0;
                        foreach (GMRTuple t2 in node.SemiJoin(children[0].variables, t1, predicates[1])) {
                            count += t2.count;
                        }
                        foreach (GMRTuple t2 in node.SemiJoinAdded(children[0].variables, t1, predicates[1])) {
                            count -= t2.count;
                        }
                        foreach (GMRTuple t2 in node.SemiJoinRemoved(children[0].variables, t1, predicates[1])) {
                            count += t2.count;
                        }
                        if (count == 0) {
                                delta.unprojectedRemovedTuples.Add(new GMRTuple(t1.fields.Length, t1.count)
                                { fields = t1.fields });
                        }
                    }
                }
                foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                    TupleCounter.Increment();
                    GMRTuple t2 = children[0].SemiJoin(node.variables, tuple, predicates[1])[0];
                    if (node.delta.projectedAddedTuples.GetTuple(tuple) == null &&
                        t2 != null && !node.AnyJoin(children[0].variables, t2, predicates[1])) {
                        delta.unprojectedAddedTuples.Add(new GMRTuple(t2.fields.Length, t2.count) { fields = t2.fields });
                    }
                }
            }

        }

        public override void ApplyUpdate() {
            foreach (GMRTuple tuple in delta.GetAddedTuples()) {
                AddTuple(tuple);
            }
            foreach (GMRTuple tuple in delta.GetRemovedTuples()) {
                RemoveTuple(tuple);
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
                } else if (s2 != "") {
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
        }

        protected override void RemoveTuple(GMRTuple tuple) {
            List<GMRTuple> section = index.Get(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            t.count -= tuple.count;
            section.Remove(t);
            if (section.Count == 0) index.RemoveKey(tuple.fields);
        }
    }
}
