using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using AssembleIVM.T_reduct.Enumerators;
using System.Text;
using System.Linq;
using QueryParser.NewParser.TreeNodes.Predicates;


namespace AssembleIVM.T_reduct.Nodes {
    class QuickMinusNode : InnerNodeReduct {
        public QuickMinusNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates,
            Enumerator enumerator, bool inFrontier) : base(name, variables, children, predicates, enumerator, inFrontier) {
        }

        public override void ComputeDeltaInitial(NodeReduct node) {
            if (node == children[0]) {
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    if (!children[1].AnyJoin(node.variables, tuple, predicates[1])) {
                        delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
                    }
                }
            } else {
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    List<GMRTuple> correspondingTuples = children[0].SemiJoin(node.variables, tuple, predicates[1]);

                    foreach (GMRTuple t in correspondingTuples) {
                        delta.unprojectedRemovedTuples.Add(new GMRTuple(t.fields.Length, t.count) { fields = t.fields });
                    }
                }
            }
        }

        public override void ComputeDelta(NodeReduct node) {
            if (node == children[0]) {
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    if (!children[1].AnyJoin(node.variables, tuple, predicates[1]) &&
                        children[1].AnyJoinRemoved(node.variables, tuple, predicates[1])) {
                        delta.unprojectedAddedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
                    }
                }
                foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                    if (children[0].delta.projectedAddedTuples.GetTuple(tuple) == null) {
                        if (children[1].delta == null && !children[1].AnyJoin(node.variables, tuple, predicates[1])) {
                            delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
                        } else {
                            int count = 0;
                            foreach (GMRTuple t2 in children[1].SemiJoin(node.variables, tuple, predicates[1])) {
                                count += t2.count;
                            }
                            foreach (GMRTuple t2 in children[1].SemiJoinAdded(variables, tuple, predicates[1])) {
                                count -= t2.count;
                            }
                            foreach (GMRTuple t2 in children[1].SemiJoinRemoved(variables, tuple, predicates[1])) {
                                count += t2.count;

                            }
                            if (count == 0) {
                                delta.unprojectedRemovedTuples.Add(new GMRTuple(tuple.fields.Length, tuple.count) { fields = tuple.fields });
                            }
                        }
                    }
                }
            } else {
                foreach (GMRTuple tuple in node.delta.GetAddedTuples()) {
                    List<GMRTuple> correspondingTuples = children[0].SemiJoin(node.variables, tuple, predicates[1]);
                    if (correspondingTuples.Count > 0) {
                        GMRTuple t1 = correspondingTuples[0];
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
                            foreach (GMRTuple t in correspondingTuples) {
                                delta.unprojectedRemovedTuples.Add(new GMRTuple(t.fields.Length, t.count) { fields = t.fields });
                            }

                        }
                    }
                }
                foreach (GMRTuple tuple in node.delta.GetRemovedTuples()) {
                    List<GMRTuple> correspondingTuples = children[0].SemiJoin(node.variables, tuple, predicates[1]);
                    if (correspondingTuples.Count > 0) {
                        GMRTuple t1 = correspondingTuples[0];
                        if (!node.AnyJoin(children[0].variables, t1, predicates[1])) {
                            foreach (GMRTuple t in correspondingTuples) {
                                delta.unprojectedAddedTuples.Add(new GMRTuple(t.fields.Length, t.count) { fields = t.fields });
                            }
                        }
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
            section.Remove(t);
            if (section.Count == 0) index.RemoveKey(tuple.fields);
        }
    }
}
