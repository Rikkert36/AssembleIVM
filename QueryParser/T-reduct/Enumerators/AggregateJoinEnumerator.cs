using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class AggregateJoinEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            AggregateJoinNode AN = (AggregateJoinNode)rho;
            Number total = new Number(0);
            int count = 0;
            foreach (GMRTuple t2 in AN.children[1].index.SemiJoin(AN.variables, t, AN.predicates[1])) {//Works only if children[0] is in frontier
                foreach (List<string> s2 in AN.children[1].Enumerate(t2)) {
                    if (AN.aggregateFunction.Equals("sum") || AN.aggregateFunction.Equals("average")) {
                        List<string> header = AN.children[1].RetrieveHeader();
                        int i = header.FindIndex(v => v.Equals(AN.aggregateDimension));
                        if (s2[i].Contains(".")) {
                            total.value += decimal.Parse(s2[i], CultureInfo.InvariantCulture);
                        } else {
                            total.value += int.Parse(s2[i]);
                        }
                    }
                    count++;
                }
            }
            //For now only return correct value for function == sum()
            yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(total.value) }));
            if (AN.aggregateFunction.Equals("sum")) {
                yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(total.value) }));
            } else if (AN.aggregateFunction.Equals("count")) {
                yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(count) }));
            } else if (AN.aggregateFunction.Equals("average")) {
                yield return new List<string>(Utils.Union(t.fields, new string[] { Convert.ToString(Math.Round(total.value / (double)count), 2) }));
            } else {
                throw new Exception($"Not implemented function {AN.aggregateFunction}");
            }
        }
    }
}
