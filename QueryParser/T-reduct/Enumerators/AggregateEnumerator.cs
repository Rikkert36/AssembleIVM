﻿using AssembleIVM.T_reduct.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AssembleIVM.T_reduct.Enumerators {
    class AggregateEnumerator : Enumerator {
        public override IEnumerable<List<string>> Enumerate(GMRTuple t) {
            AggregateNode AN = (AggregateNode)rho;
            Number total = new Number(0);
            int count = 0;
            foreach (GMRTuple t1 in AN.children[0].index.SemiJoin(AN.variables,t, null)) {
                foreach (List<string> s1 in AN.children[0].Enumerate(t1)) {//Works only if children[0] is in frontier
                    if (AN.aggregateFunction.Equals("sum") || AN.aggregateFunction.Equals("average")) {
                        List<string> header = AN.children[0].RetrieveHeader();
                        int i = header.FindIndex(v => v.Equals(AN.aggregateDimension));
                        if (s1[i].Contains(".")) {
                            total.value += double.Parse(s1[i], CultureInfo.InvariantCulture);
                        } else {
                            total.value += int.Parse(s1[i]);
                        }
                    } 
                    count++;
                }
            }
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