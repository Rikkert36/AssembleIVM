using QueryParser.GJTComputerFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssembleIVM.T_reduct {
    class AggregateReductTree : ReductTree {
        public AggregateReductTree(GeneralJoinTree GJT, string modelName) : base(GJT, modelName) {
        }

        protected override Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>> EnumerateDelta() {
            List<string> combinedHeader = root.RetrieveHeader();
            HashSet<GMRTuple> delta = root.delta.GetUnionTuples().ToHashSet();
            if (uniteWeekAndYearValues) {
                int firstWeekIndex = Utils.GetWeekIndex(outputVariables);
                return new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(
                    EnumerateAdded(delta, combinedHeader)
                    .Select(tuple => tuple.UniteWeekAndYearValues(firstWeekIndex)).ToHashSet(),
                    EnumerateRemoved(delta, combinedHeader)
                    .Select(tuple => tuple.UniteWeekAndYearValues(firstWeekIndex)).ToHashSet());
            } else {
                return new Tuple<HashSet<GMRTuple>, HashSet<GMRTuple>>(
                    EnumerateAdded(delta, combinedHeader).ToHashSet(),
                    EnumerateRemoved(delta, combinedHeader).ToHashSet());
            }
        }


    }
}
