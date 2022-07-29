using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class AntiJoinEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            TupleCounter.Increment();
            yield return new List<string>(t.fields);
        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            TupleCounter.Increment();
            yield return new List<string>(t.fields);

        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            TupleCounter.Increment();
            yield return new List<string>(t.fields);       
        }
    }
}
