using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class SumEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            yield return new List<string>(Utils.Union(t.fields, new string[] {t.sum.value.ToString() }));
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
            List<GMRTuple> l = AN.Get(t);
            int c = l.Count == 0 ? -t.count : l[0].count - t.count;
            Number sum = l.Count == 0 ? new Number(-t.sum.value) : new Number(l[0].sum.value - t.sum.value);
            if (c != 0) {
                yield return new List<string>(Utils.Union(t.fields, new string[] { sum.value.ToString() }));

            }
        }
    }
}
