using QueryParser.GJTComputerFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class SingleJoinEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            TupleCounter.Increment();
            JoinNode r = (JoinNode)rho;
            foreach (GMRTuple t1 in r.children[0].SemiJoin(new List<string>(rho.variables), t, r.predicates[0])) {
                foreach (GMRTuple t2 in r.children[1].SemiJoin(new List<string>(rho.variables), t, r.predicates[1])) {
                    foreach (List<string> s1 in r.children[0].Enumerate(t1)) {
                        foreach (List<string> s2 in r.children[1].Enumerate(t2)) {
                            yield return new List<string>(Utils.Union(s1, s2));
                        }
                    }
                }
            }
        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            TupleCounter.Increment();
            JoinNode r = (JoinNode)rho;
            HashSet<string> alreadySeen = new HashSet<string>();
            if (r.children[0].delta != null) {
                foreach (GMRTuple t1 in r.children[0].SemiJoinAdded(new List<string>(rho.variables), t, r.predicates[0])) {
                    foreach (List<string> s1 in r.children[0].EnumerateAdded(t1)) {
                        foreach (GMRTuple t2 in r.children[1].SemiJoin(new List<string>(rho.variables), t, r.predicates[1])) {
                            foreach (List<string> s2 in r.children[1].Enumerate(t2)) {
                                List<string> union = Utils.Union(s1, s2);
                                alreadySeen.Add(Utils.ToString(union));
                                yield return union;

                            }
                        }
                    }
                }
            }
            if (r.children[1].delta != null) {
                foreach (GMRTuple t2 in r.children[1].SemiJoinAdded(new List<string>(rho.variables), t, r.predicates[1])) {
                    foreach (List<string> s2 in r.children[1].EnumerateAdded(t2)) {
                        foreach (GMRTuple t1 in r.children[0].SemiJoin(new List<string>(rho.variables), t, r.predicates[0])) {
                            foreach (List<string> s1 in r.children[0].Enumerate(t1)) {
                                List<string> union = Utils.Union(s1, s2);
                                if (!alreadySeen.Contains(Utils.ToString(union))) {
                                    yield return new List<string>(Utils.Union(s1, s2));
                                }
                            }
                        }
                    }
                }
            }

        }

        public IEnumerable<string> MinusTimesPlus(GMRTuple t) {
            JoinNode r = (JoinNode)rho;
            if (r.children[0].delta != null && r.children[1].delta != null) {
                List<List<string>> S1 = new List<List<string>>();
                List<List<string>> S2 = new List<List<string>>();
                foreach (GMRTuple t1 in r.children[0].SemiJoinRemoved(new List<string>(rho.variables), t, r.predicates[0])) {
                    foreach (List<string> s1 in r.children[0].EnumerateRemoved(t1)) {
                        S1.Add(s1);
                    }
                }
                foreach (GMRTuple t2 in r.children[1].SemiJoinAdded(new List<string>(rho.variables), t, r.predicates[1])) {
                    foreach (List<string> s2 in r.children[1].EnumerateAdded(t2)) {
                        S2.Add(s2);
                    }
                }
                foreach (List<string> s1 in S1) {
                    foreach (List<string> s2 in S2) {
                        TupleCounter.Increment();
                        List<string> union = Utils.Union(s1, s2);
                        yield return String.Join(",", union);
                    }
                }
            }
        }

        public IEnumerable<string> PlusTimesMinus(GMRTuple t) {
            JoinNode r = (JoinNode)rho;
            if (r.children[0].delta != null && r.children[1].delta != null) {
                List<List<string>> S1 = new List<List<string>>();
                List<List<string>> S2 = new List<List<string>>();
                foreach (GMRTuple t1 in r.children[0].SemiJoinAdded(new List<string>(rho.variables), t, r.predicates[0])) {
                    foreach (List<string> s1 in r.children[0].EnumerateAdded(t1)) {
                        S1.Add(s1);
                    }
                }
                foreach (GMRTuple t2 in r.children[1].SemiJoinRemoved(new List<string>(rho.variables), t, r.predicates[1])) {
                    foreach (List<string> s2 in r.children[1].EnumerateRemoved(t2)) {
                        S2.Add(s2);
                    }
                }
                foreach (List<string> s1 in S1) {
                    foreach (List<string> s2 in S2) {
                        TupleCounter.Increment();
                        List<string> union = Utils.Union(s1, s2);
                        yield return String.Join(",", union);
                    }
                }
            }
        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            TupleCounter.Increment();
            JoinNode r = (JoinNode)rho;
            HashSet<string> alreadySeen = new HashSet<string>();
            HashSet<string> minusTimesPlus = MinusTimesPlus(t).ToHashSet();
            HashSet<string> plusTimesMinus = PlusTimesMinus(t).ToHashSet();

            List<List<string>> S1 = new List<List<string>>();
            List<List<string>> S2 = new List<List<string>>();
            if (r.children[0].delta != null && r.children[1].delta != null) {
                foreach (GMRTuple t1 in r.children[0].SemiJoinRemoved(new List<string>(rho.variables), t, r.predicates[0])) {
                    foreach (List<string> s1 in r.children[0].EnumerateRemoved(t1)) {
                        S1.Add(s1);
                    }
                }
                foreach (GMRTuple t2 in r.children[1].SemiJoinRemoved(new List<string>(rho.variables), t, r.predicates[1])) {
                    foreach (List<string> s2 in r.children[1].EnumerateRemoved(t2)) {
                        S2.Add(s2);
                    }
                }
                foreach (List<string> s1 in S1) {
                    foreach (List<string> s2 in S2) {
                        TupleCounter.Increment();
                        List<string> union = Utils.Union(s1, s2);
                        yield return union;
                    }
                }
            }

            if (r.children[0].delta != null) {
                foreach (GMRTuple t1 in r.children[0].SemiJoinRemoved(new List<string>(rho.variables), t, r.predicates[0])) {
                    foreach (List<string> s1 in r.children[0].EnumerateRemoved(t1)) {
                        foreach (GMRTuple t2 in r.children[1].SemiJoin(new List<string>(rho.variables), t, r.predicates[1])) {
                            foreach (List<string> s2 in r.children[1].Enumerate(t2)) {
                                List<string> union = Utils.Union(s1, s2);
                                if (!minusTimesPlus.Contains(String.Join(",", union))) {
                                    alreadySeen.Add(Utils.ToString(union));
                                    yield return union;
                                }
                            }
                        }
                    }
                }
            }
            if (r.children[1].delta != null) {
                foreach (GMRTuple t2 in r.children[1].SemiJoinRemoved(new List<string>(rho.variables), t, r.predicates[1])) {
                    foreach (List<string> s2 in r.children[1].EnumerateRemoved(t2)) {
                        foreach (GMRTuple t1 in r.children[0].SemiJoin(new List<string>(rho.variables), t, r.predicates[0])) {
                            foreach (List<string> s1 in r.children[0].Enumerate(t1)) {
                                List<string> union = Utils.Union(s1, s2);
                                if (!plusTimesMinus.Contains(String.Join(",", union))) {
                                    yield return new List<string>(Utils.Union(s1, s2));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
