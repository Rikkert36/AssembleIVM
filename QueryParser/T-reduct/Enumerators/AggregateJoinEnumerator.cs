using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class AggregateJoinEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            AggregateJoinNode AN = (AggregateJoinNode)rho;
            Number total = new Number(0);
            int count = 0;
            int i = -1;
            foreach (GMRTuple t2 in AN.children[1].index.SemiJoin(AN.variables, t, AN.predicates[1])) {//Works only if children[0] is in frontier
                foreach (List<string> s2 in AN.children[1].Enumerate(t2)) {
                    if (AN.aggregateFunction.Equals("sum") || AN.aggregateFunction.Equals("average")) {
                        List<string> header = AN.children[1].RetrieveHeader();
                        if (i == -1) i = header.FindIndex(v => v.Equals(AN.aggregateDimension));
                        if (s2[i].Contains(".")) {
                            total.value += decimal.Parse(s2[i], CultureInfo.InvariantCulture);
                        } else {
                            total.value += int.Parse(s2[i]);
                        }
                    }
                    count++;
                }
            }


            if (count != 0) {
                if (AN.aggregateFunction.Equals("sum")) {
                    yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(total.value) }));
                } else if (AN.aggregateFunction.Equals("count")) {
                    yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(count) }));
                } else if (AN.aggregateFunction.Equals("average")) {
                    yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(Math.Round(total.value / (double)count), 2) }));
                } else {
                    throw new Exception($"Not implemented function {AN.aggregateFunction}");
                }
            }
        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            return Enumerate(t);
        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            AggregateJoinNode AN = (AggregateJoinNode)rho;
            Number total = new Number(0);
            int count = 0;
            int i = -1;
            if (AN.children[0].delta != null) {
                int leftCounter = 0;
                foreach (GMRTuple t1 in AN.SemiJoinLeftChild(t)) {
                    foreach (List<string> s1 in AN.children[0].Enumerate(t1)) {
                        leftCounter++;
                    }
                }
                foreach (GMRTuple t1 in AN.SemiJoinLeftChildAdded(t)) {
                    foreach (List<string> s1 in AN.children[0].EnumerateAdded(t1)) {
                        leftCounter--;
                    }
                }
                foreach (GMRTuple t1 in AN.SemiJoinLeftChildRemoved(t)) {
                    foreach (List<string> s1 in AN.children[0].EnumerateRemoved(t1)) {
                        leftCounter++;
                    }
                }
                if (leftCounter == 0) yield break;
            }
            foreach (GMRTuple t2 in AN.children[1].index.SemiJoin(AN.variables, t, AN.predicates[1])) {//Works only if children[0] is in frontier
                foreach (List<string> s2 in AN.children[1].Enumerate(t2)) {
                    if (AN.aggregateFunction.Equals("sum") || AN.aggregateFunction.Equals("average")) {
                        List<string> header = AN.children[1].RetrieveHeader();
                        if (i == -1) i = header.FindIndex(v => v.Equals(AN.aggregateDimension));
                        if (s2[i].Contains(".")) {
                            total.value += decimal.Parse(s2[i], CultureInfo.InvariantCulture);
                        } else {
                            total.value += int.Parse(s2[i]);
                        }
                    }
                    count++;
                }
            }
            if (AN.children[1].delta != null) {
                foreach (GMRTuple t2 in AN.children[1].SemiJoinAdded(AN.variables, t, AN.predicates[1])) {
                    foreach (List<string> s2 in AN.children[1].EnumerateAdded(t2)) {
                        if (AN.aggregateFunction.Equals("sum") || AN.aggregateFunction.Equals("average")) {
                            List<string> header = AN.children[0].RetrieveHeader();
                            if (s2[i].Contains(".")) {
                                total.value -= double.Parse(s2[i], CultureInfo.InvariantCulture);
                            } else {
                                total.value -= int.Parse(s2[i]);
                            }
                        }
                        count--;
                    }
                }
                foreach (GMRTuple t2 in AN.children[1].SemiJoinRemoved(AN.variables, t, AN.predicates[1])) {
                    foreach (List<string> s2 in AN.children[1].EnumerateRemoved(t2)) {
                        if (AN.aggregateFunction.Equals("sum") || AN.aggregateFunction.Equals("average")) {
                            List<string> header = AN.children[1].RetrieveHeader();
                            if (s2[i].Contains(".")) {
                                total.value += double.Parse(s2[i], CultureInfo.InvariantCulture);
                            } else {
                                total.value += int.Parse(s2[i]);
                            }
                        }
                        count++;
                    }
                }
            }

            if (AN.aggregateFunction.Equals("sum")) {
                yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(total.value) }));
            } else if (AN.aggregateFunction.Equals("count")) {
                yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(count) }));
            } else if (AN.aggregateFunction.Equals("average")) {
                yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(Math.Round(total.value / (double)count), 2) }));
            } else {
                throw new Exception($"Not implemented function {AN.aggregateFunction}");
            }
        }
    }
}
