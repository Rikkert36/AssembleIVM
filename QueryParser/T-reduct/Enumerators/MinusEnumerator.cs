using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class MinusEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            if (t.count == 1) yield return new List<string> (t.fields);
        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            MinusNode minusNode = (MinusNode)rho;
            if (minusNode.Get(t)[0].count == 1) yield return new List<string> (t.fields);
        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            MinusNode minusNode = (MinusNode)rho;
            if (t.count + minusNode.Get(t)[0].count == 1 && !minusNode.ExistsAddedTuple(t)) yield return new List<string>(t.fields);
        }
    }
}
