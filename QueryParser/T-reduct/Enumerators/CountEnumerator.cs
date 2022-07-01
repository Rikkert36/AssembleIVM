using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class CountEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            CountNode AN = (CountNode)rho;
            Number total = new Number(0);
            int count = 0;
            List<string> header = AN.children[0].RetrieveHeader();
            int i = header.FindIndex(v => v.Equals(AN.aggregateDimension));
            foreach (GMRTuple t1 in AN.children[0].index.SemiJoin(AN.variables, t, null)) {
                foreach (List<string> s1 in AN.children[0].Enumerate(t1)) {
                    count++;
                }
            }

            yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(count) }));

        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            CountNode AN = (CountNode)rho;
            List<GMRTuple> l = AN.Get(t);
            if (l.Count != 0) {
                GMRTuple tuple = l[0];
                yield return new List<string>(Utils.Union(t.fields, new string[] { tuple.count.ToString() }));
            }
        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            CountNode AN = (CountNode)rho;
            int count = 0;
            List<string> header = AN.children[0].RetrieveHeader();

            int i = header.FindIndex(v => v.Equals(AN.aggregateDimension));
            foreach (GMRTuple t1 in AN.children[0].index.SemiJoin(AN.variables, t, null)) {
                foreach (List<string> s1 in AN.children[0].Enumerate(t1)) {//Works only if children[0] is in frontier
                    count++;
                }
            }

            foreach (GMRTuple t1 in AN.children[0].SemiJoinAdded(AN.variables, t, null)) {
                foreach (List<string> s1 in AN.children[0].EnumerateAdded(t1)) {
                    count--;
                }
            }
            foreach (GMRTuple t1 in AN.children[0].SemiJoinRemoved(AN.variables, t, null)) {
                foreach (List<string> s1 in AN.children[0].EnumerateRemoved(t1)) {
                    count++;
                }
            }
            if (count != 0) {
                yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(count) }));

            }
        }
    }
}
