using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class MinusEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            MinusNode minusNode = (MinusNode)rho;
            foreach (GMRTuple t1 in minusNode.children[0].SemiJoin(minusNode.variables, t, minusNode.predicates[0])) {
                foreach (List<string> s1 in minusNode.children[0].Enumerate(t1)) {
                    if (minusNode.children[1].SemiJoin(minusNode.variables, t, minusNode.predicates[1]).Count == 0) {
                        yield return new List<string>(s1);
                    }
                }
            }
        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            MinusNode minusNode = (MinusNode)rho;
            HashSet<string> alreadySeen = new HashSet<string>();
            if (minusNode.children[1].delta != null) {
                foreach (GMRTuple t1 in minusNode.children[0].SemiJoin(minusNode.variables, t, minusNode.predicates[0])) {
                    foreach (List<string> s1 in minusNode.children[0].Enumerate(t1)) {
                        if (!minusNode.children[1].AnyJoin(minusNode.variables, t, minusNode.predicates[1]) &&
                            minusNode.children[1].AnyJoinRemoved(minusNode.variables, t, minusNode.predicates[1])) {
                            alreadySeen.Add(Utils.ToString(s1));
                            yield return new List<string>(s1);
                        }
                    }
                }
            }
            if (minusNode.children[0].delta != null) {
                foreach (GMRTuple t1 in minusNode.children[0].SemiJoinAdded(minusNode.variables, t, minusNode.predicates[0])) {
                    foreach (List<string> s1 in minusNode.children[0].EnumerateAdded(t1)) {
                        if (minusNode.children[0].index.Get(s1.ToArray()).Count > 0 &&
                            !minusNode.children[1].AnyJoin(minusNode.variables, t, minusNode.predicates[1]) &&
                            !alreadySeen.Contains(Utils.ToString(s1))) {
                            yield return new List<string>(s1);
                        }
                    }
                }
            }

        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            MinusNode minusNode = (MinusNode)rho;
            if (minusNode.children[1].delta != null) {
                foreach (GMRTuple t1 in minusNode.children[0].SemiJoin(minusNode.variables, t, minusNode.predicates[0])) {
                    foreach (List<string> s1 in minusNode.children[0].Enumerate(t1)) {
                        int count = 0;
                        bool minusPresent = false;
                        foreach (GMRTuple t2 in minusNode.children[1].SemiJoin(minusNode.variables, t, minusNode.predicates[1])) {
                            foreach (List<string> s2 in minusNode.children[1].Enumerate(t2)) {
                                count++;
                                minusPresent = true;
                            }
                        }
                        foreach (GMRTuple t2 in minusNode.children[1].SemiJoinAdded(minusNode.variables, t, minusNode.predicates[1])) {
                            foreach (List<string> s2 in minusNode.children[1].EnumerateAdded(t2)) {
                                count--;
                            }
                        }
                        foreach (GMRTuple t2 in minusNode.children[1].SemiJoinRemoved(minusNode.variables, t, minusNode.predicates[1])) {
                            foreach (List<string> s2 in minusNode.children[1].EnumerateRemoved(t2)) {
                                count++;
                            }
                        }
                        if (count == 0 && minusPresent) {
                            yield return s1;
                        }
                    }
                }
            }
            if (minusNode.children[0].delta != null) {
                foreach (GMRTuple t1 in minusNode.children[0].SemiJoinRemoved(minusNode.variables, t, minusNode.predicates[0])) {
                    foreach (List<string> s1 in minusNode.children[0].EnumerateRemoved(t1)) {
                        if (!minusNode.children[1].AnyJoin(minusNode.variables, t, minusNode.predicates[1])) {
                            yield return s1;
                        } else if (minusNode.children[1].delta != null) {
                            int count = 0;
                            foreach (GMRTuple t2 in minusNode.children[1].SemiJoin(minusNode.variables, t, minusNode.predicates[1])) {
                                foreach (List<string> s2 in minusNode.children[1].Enumerate(t2)) {
                                    count++;
                                }
                            }
                            foreach (GMRTuple t2 in minusNode.children[1].SemiJoinAdded(minusNode.variables, t, minusNode.predicates[1])) {
                                foreach (List<string> s2 in minusNode.children[1].EnumerateAdded(t2)) {
                                    count--;
                                }
                            }
                            foreach (GMRTuple t2 in minusNode.children[1].SemiJoinRemoved(minusNode.variables, t, minusNode.predicates[1])) {
                                foreach (List<string> s2 in minusNode.children[1].EnumerateRemoved(t2)) {
                                    count++;
                                }
                            }
                            if (count == 0) {
                                yield return s1;
                            }
                        }
                    }
                }
            }
        }

        /*public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            if (t.count == 1) yield return new List<string> (t.fields);
        }
         * 
         * public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            MinusNode minusNode = (MinusNode)rho;
            if (minusNode.Get(t)[0].count == 1) yield return new List<string>(t.fields);
        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            MinusNode minusNode = (MinusNode)rho;
            if (t.count + minusNode.Get(t)[0].count == 1 && !minusNode.ExistsAddedTuple(t)) yield return new List<string>(t.fields);
        }*/
    }
}
