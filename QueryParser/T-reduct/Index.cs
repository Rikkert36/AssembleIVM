using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct {
    [Serializable]
    class Index {
        public bool minusIndex = false;
        public List<string> header { get; set; }
        public List<string> eqJoinHeader { get; set; }
        public List<string> orderHeader = new List<string>();
        public Dictionary<string, List<GMRTuple>> tupleMap { get; set; }
 

        public void RemoveKey(string[] tuple) {
            List<string> eqJoinVars = new List<string>();
            foreach (string var in eqJoinHeader) {
                eqJoinVars.Add(tuple[header.IndexOf(var)]);
            }
            string tupleString = TupleToString(eqJoinVars);
            tupleMap.Remove(tupleString);
        }

        public List<GMRTuple> Get(string[] tuple) {
            List<string> eqJoinVars = new List<string>();
            foreach(string var in eqJoinHeader) {
                eqJoinVars.Add(tuple[header.IndexOf(var)]);
            }
            string tupleString = TupleToString(eqJoinVars);
            if (!tupleMap.ContainsKey(tupleString)) tupleMap[tupleString] = new List<GMRTuple>();
            return tupleMap[tupleString];
        }


        public List<GMRTuple> SemiJoin(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            if (predicate == null) {
                return Get(rightTuple.fields);
            } else {
                return new RangeTupleList(eqJoinHeader, orderHeader,rightHeader, rightTuple, predicate)
                    .RetrieveCorrespondingTuples(this);
            }
        }

        public bool AnyJoin(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            return new RangeTupleList(eqJoinHeader, orderHeader, rightHeader, rightTuple, predicate).DoesJoin(this);
        }

        /*private Tuple<GMRTuple, bool> Contains(GMRTuple tuple) {
            List<GMRTuple> section = Get(tuple.fields);
            foreach (GMRTuple t in section) {
                if (tuple.Equals(t)) return new Tuple<GMRTuple, bool>(t, true);
            }
            return new Tuple<GMRTuple, bool>(null, false);
        }*/

        public GMRTuple FindTuple(GMRTuple tuple, List<GMRTuple> section) {
            foreach (GMRTuple otherTuple in section) {
                if (otherTuple.Equals(tuple)) return otherTuple;
            }
            return null;
        }     

        public int FindLocation(List<GMRTuple> section, GMRTuple tuple) {
            int orderDimensionIndex = 0;
            string orderDimension = orderHeader[orderDimensionIndex];
            int orderIndex = header.IndexOf(orderDimension);
            string orderValue = tuple.fields[orderIndex];
            for (int i = 0; i < section.Count; i++) {
                if (Int32.Parse(orderValue) > Int32.Parse(section[i].fields[orderIndex])) {
                    return - 1;
                } else if (Int32.Parse(orderValue) == Int32.Parse(section[i].fields[orderIndex])) {
                    orderDimensionIndex++;
                    orderDimension = orderHeader[orderDimensionIndex];
                    orderIndex = header.IndexOf(orderDimension);
                    orderValue = tuple.fields[orderIndex];
                }
            }
            return section.Count;
        }

        private string TupleToString(List<string> tuple) {
            string result = "";
            foreach(string s in tuple) {
                if (int.TryParse(s, out _) || double.TryParse(s, out _)) {
                    result += $"number({s})";
                } else {
                    result += s;
                }
            }
            return result;
        }
    }
}
