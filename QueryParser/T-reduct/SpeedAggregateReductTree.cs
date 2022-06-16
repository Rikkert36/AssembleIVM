using QueryParser.GJTComputerFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct {
    class SpeedAggregateReductTree : ReductTree {
        public string r1;
        string r2;
        string aggregateDimension;
        List<string> cHeader;
        public SpeedAggregateReductTree(GeneralJoinTree GJT, string modelName,
            string r1, string r2, string aggregateDimension) : base(GJT, modelName) {
            this.r1 = r1;
            this.r2 = r2;
            this.aggregateDimension = aggregateDimension;
        }


        protected override IEnumerable<GMRTuple> Enumerate(HashSet<GMRTuple> tupleList, List<string> combinedHeader) {
            int aggregateDimensionIndex = combinedHeader.IndexOf(aggregateDimension);
            List<string> keyVariables = Utils.SetMinus(outputVariables, new List<string> { $"sum({aggregateDimension})" });
            Dictionary<string, Tuple<List<string>, Number>> valuePerReplaceDimension
                = new Dictionary<string, Tuple<List<string>, Number>>();
            
            foreach (GMRTuple t in tupleList) {
                foreach (List<string> s in root.Enumerate(t)) {
                    GMRTuple keyTuple = CreateTuple(keyVariables, combinedHeader, s);
                    string key = keyTuple.ToString();
                    if (!valuePerReplaceDimension.ContainsKey(key)) {
                        valuePerReplaceDimension.Add(key
                            , new Tuple<List<string>, Number>
                            (new List<string>(keyTuple.fields), new Number(s[aggregateDimensionIndex])));
                    } else {
                        Number oldValue = valuePerReplaceDimension[key].Item2;
                        oldValue.value += new Number(s[aggregateDimensionIndex]).value;
                    }
                }
            }
            foreach (Tuple<List<string>, Number> outputTuple in valuePerReplaceDimension.Values) {
                string valueString = Convert.ToString(outputTuple.Item2.value);

                yield return CreateTuple(outputVariables, outputVariables,
                    Utils.Union(outputTuple.Item1, new List<string> { valueString })
                    );
            }

        }
    }
}
