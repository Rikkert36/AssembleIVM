using QueryParser.GJTComputerFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class FrontierEnumerator : Enumerator {

        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            TupleCounter.Increment();
            yield return new List<string>(t.fields);
        }

        public override IEnumerable<List<string>> EnumerateAdded(GMRTuple t) {
            TupleCounter.Increment();
            /*            GMRTuple t_n = rho.Get(t)[0];
                        List<GMRTuple> tMinList = rho.delta.projectedRemovedTuples.Get(t.fields);
                        GMRTuple tMin = rho.delta.projectedRemovedTuples.FindTuple(t, tMinList);
                        int tMinCount = tMin == null ? 0 : tMin.count;
                        if (t_n != null && t_n.count - t.count + tMinCount == 0) {*/
            yield return new List<string>(t.fields);
/*            } else {
                Console.WriteLine("huh");
            }*/
        }

        public override IEnumerable<List<string>> EnumerateRemoved(GMRTuple t) {
            TupleCounter.Increment();
            /*            GMRTuple t_n = rho.Get(t)[0];
                        List<GMRTuple> tPlusList = rho.delta.projectedAddedTuples.Get(t.fields);
                        GMRTuple tPlus = rho.delta.projectedAddedTuples.FindTuple(t, tPlusList);
                        int tPlusCount = tPlus == null ? 0 : tPlus.count;
                        if (t_n != null && t.count - tPlusCount > 0) {*/
            yield return new List<string>(t.fields);
/*            } else {
                Console.WriteLine("huh");
            }*/
        }
    }
}
