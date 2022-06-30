using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class UnionEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            UnionNode r = (UnionNode)rho;
            foreach (GMRTuple t1 in r.children[0].SemiJoin(new List<string>(rho.variables), t, r.predicates[0])) {
                foreach (List<string> s1 in r.children[0].Enumerate(t1)) {
                    yield return s1;
                }
            }
            foreach (GMRTuple t2 in r.children[1].SemiJoin(new List<string>(rho.variables), t, r.predicates[1])) {
                foreach (List<string> s2 in r.children[1].Enumerate(t2)) {
                    yield return s2;
                }
            }
        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            UnionNode r = (UnionNode)rho;
            HashSet<string> alreadySeen = new HashSet<string>();
            if (r.children[0].delta != null) {
                foreach (GMRTuple t1 in r.children[0].SemiJoinAdded(new List<string>(rho.variables), t, r.predicates[0])) {
                    foreach (List<string> s1 in r.children[0].EnumerateAdded(t1)) {
                        if (!alreadySeen.Contains(Utils.ToString(s1))) {
                            alreadySeen.Add(Utils.ToString(s1));
                            yield return s1;
                        }
                    }
                }
            }
            if (r.children[1].delta != null) {
                foreach (GMRTuple t2 in r.children[1].SemiJoinAdded(new List<string>(rho.variables), t, r.predicates[1])) {
                    foreach (List<string> s2 in r.children[1].EnumerateAdded(t2)) {
                        if (!alreadySeen.Contains(Utils.ToString(s2))) {
                            alreadySeen.Add(Utils.ToString(s2));
                            yield return s2;
                        }
                    }
                }
            }
        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            UnionNode r = (UnionNode)rho;
            HashSet<string> alreadySeen = new HashSet<string>();
            if (r.children[0].delta != null) {
                foreach (GMRTuple t1 in r.children[0].SemiJoinRemoved(new List<string>(rho.variables), t, r.predicates[0])) {
                    foreach (List<string> s1 in r.children[0].EnumerateRemoved(t1)) {
                        if (!alreadySeen.Contains(Utils.ToString(s1))) {
                            alreadySeen.Add(Utils.ToString(s1));
                            yield return s1;
                        }
                    }
                }
            }
            if (r.children[1].delta != null) {
                foreach (GMRTuple t2 in r.children[1].SemiJoinRemoved(new List<string>(rho.variables), t, r.predicates[1])) {
                    foreach (List<string> s2 in r.children[1].EnumerateRemoved(t2)) {
                        if (!alreadySeen.Contains(Utils.ToString(s2))) {
                            alreadySeen.Add(Utils.ToString(s2));
                            yield return s2;
                        }
                    }
                }
            }
        }
    }
}
