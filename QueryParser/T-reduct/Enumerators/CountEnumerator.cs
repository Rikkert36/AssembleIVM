using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class CountEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            TupleCounter.Increment();
            yield return new List<string>(Utils.Union(t.fields, new string[] { t.count.ToString() }));
        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            TupleCounter.Increment();
            CountNode AN = (CountNode)rho;
            List<GMRTuple> l = AN.Get(t);
            if (l.Count != 0) {
                GMRTuple tuple = l[0];
                yield return new List<string>(Utils.Union(t.fields, new string[] { tuple.count.ToString() }));
            }
        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            TupleCounter.Increment();
            CountNode AN = (CountNode)rho;
            List<GMRTuple> l = AN.Get(t);
            int c = l.Count == 0 ? -t.count : l[0].count - t.count;
            if (c != 0) {
                yield return new List<string>(Utils.Union(t.fields, new string[] { c.ToString() }));
            }

          
        }
    }
}
