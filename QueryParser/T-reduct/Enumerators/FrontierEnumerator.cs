using QueryParser.GJTComputerFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class FrontierEnumerator : Enumerator {

        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            yield return new List<string>(t.fields);
        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            yield return new List<string>(t.fields);
        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            yield return new List<string>(t.fields);
        }
    }
}
