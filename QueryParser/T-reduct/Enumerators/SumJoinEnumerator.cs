using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class SumJoinEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {

            yield return new List<string>(Utils.Union(t.fields, new string[] { t.sum.value.ToString() }));

        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            AggregateJoinNode AN = (AggregateJoinNode)rho;
            List<GMRTuple> section = AN.Get(t);
            GMRTuple tuple = AN.index.FindTuple(t, section);
            if (tuple != null) {
                yield return new List<string>(Utils.Union(t.fields, new string[] { tuple.sum.value.ToString() }));

            }
        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            AggregateJoinNode AN = (AggregateJoinNode)rho;
            List<GMRTuple> section = AN.Get(t);
            GMRTuple tuple = AN.index.FindTuple(t, section);
            if (tuple != null && tuple.count - t.count == 0) yield break;
            Number sum = tuple == null ? new Number(-t.sum.value) : new Number(tuple.sum.value - t.sum.value);
            yield return new List<string>(Utils.Union(t.fields, new string[] { sum.value.ToString() }));
        }
    }
}
