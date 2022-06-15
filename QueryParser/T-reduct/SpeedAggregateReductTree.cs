using QueryParser.GJTComputerFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct {
    class SpeedAggregateReductTree : ReductTree {
        string r1;
        string r2;
        string aggregateDimension;
        List<string> cHeader;
        public SpeedAggregateReductTree(GeneralJoinTree GJT, string modelName,
            string r1, string r2,  string aggregateDimension) : base(GJT, modelName) {
            this.r1 = r1;
            this.r2 = r2;
            this.aggregateDimension = aggregateDimension;
        }
       

        protected override IEnumerable<GMRTuple> Enumerate(InnerNodeReduct root) {
            List<string> combinedHeader = root.RetrieveHeader();
            int r1Index = combinedHeader.IndexOf(r1);
            int r2Index = combinedHeader.IndexOf(r2);
            int aggregateDimensionIndex = combinedHeader.IndexOf(aggregateDimension);
            /*cHeader = Utils.Union(Utils.Union(
                            new List<string> { r2 },
                            Utils.SetMinus(Utils.SetMinus(combinedHeader, new List<string> { aggregateDimension}),
                            new List<string> { r1 })
                            ), new List<string> { $"sum({aggregateDimension})" });*/
            List<string> keyVariables = Utils.SetMinus(outputVariables, new List<string> { $"sum({aggregateDimension})" });
            Dictionary<string, Tuple<List<string>, Number>> valuePerReplaceDimension 
                = new Dictionary<string, Tuple<List<string>, Number>>();
            foreach (List<GMRTuple> tupleList in root.index.tupleMap.Values) {
                foreach (GMRTuple t in tupleList) {
                    foreach (List<string> s in root.Enumerate(t)) {
                        /*List<string> outputTuple = Utils.Union(//Make sure order is right!
                            new List<string> { s[r2Index] },
                            Utils.SetMinus(Utils.SetMinus(new List<string>(s),
                            new List<string> { s[r1Index] }), new List<string> { s[aggregateDimensionIndex] })
                            );
                        string key = Utils.ToString(outputTuple);*/
                        GMRTuple keyTuple = CreateTuple(keyVariables, combinedHeader, s);
                        string key = keyTuple.ToString();
                        if (!valuePerReplaceDimension.ContainsKey(key)) {
                            valuePerReplaceDimension.Add(key
                                ,new Tuple<List<string>, Number>
                                (new List<string>(keyTuple.fields), new Number(s[aggregateDimensionIndex])));
                        } else {
                            Number oldValue = valuePerReplaceDimension[key].Item2;
                            oldValue.value += new Number(s[aggregateDimensionIndex]).value;
                        }
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
