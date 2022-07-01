using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class SumEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            SumNode AN = (SumNode)rho;
            Number total = new Number(0);
            int count = 0;
            List<string> header = AN.children[0].RetrieveHeader();
            int i = header.FindIndex(v => v.Equals(AN.aggregateDimension));
            foreach (GMRTuple t1 in AN.children[0].index.SemiJoin(AN.variables, t, null)) {
                foreach (List<string> s1 in AN.children[0].Enumerate(t1)) {

                    if (s1[i].Contains(".")) {
                        total.value += double.Parse(s1[i], CultureInfo.InvariantCulture);
                    } else {
                        total.value += int.Parse(s1[i]);
                    }

                    count++;
                }
            }

            yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(total.value) }));

        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            SumNode AN = (SumNode)rho;
            List<GMRTuple> l = AN.Get(t);
            if (l.Count != 0) {
                GMRTuple tuple = l[0];
                yield return new List<string>(Utils.Union(t.fields, new string[] { tuple.sum.value.ToString() }));

            }
        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            SumNode AN = (SumNode)rho;
            Number total = new Number(0);
            int count = 0;
            List<string> header = AN.children[0].RetrieveHeader();

            int i = header.FindIndex(v => v.Equals(AN.aggregateDimension));
            foreach (GMRTuple t1 in AN.children[0].index.SemiJoin(AN.variables, t, null)) {
                foreach (List<string> s1 in AN.children[0].Enumerate(t1)) {//Works only if children[0] is in frontier

                    if (s1[i].Contains(".")) {
                        total.value += double.Parse(s1[i], CultureInfo.InvariantCulture);
                    } else {
                        total.value += int.Parse(s1[i]);
                    }
                    count++;
                }
            }

            foreach (GMRTuple t1 in AN.children[0].SemiJoinAdded(AN.variables, t, null)) {
                foreach (List<string> s1 in AN.children[0].EnumerateAdded(t1)) {

                    if (s1[i].Contains(".")) {
                        total.value -= double.Parse(s1[i], CultureInfo.InvariantCulture);
                    } else {
                        total.value -= int.Parse(s1[i]);
                    }

                    count--;
                }
            }
            foreach (GMRTuple t1 in AN.children[0].SemiJoinRemoved(AN.variables, t, null)) {
                foreach (List<string> s1 in AN.children[0].EnumerateRemoved(t1)) {

                    if (s1[i].Contains(".")) {
                        total.value += double.Parse(s1[i], CultureInfo.InvariantCulture);
                    } else {
                        total.value += int.Parse(s1[i]);
                    }

                    count++;
                }
            }
            if (count != 0) {

                yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(total.value) }));

            }
        }
    }
}
