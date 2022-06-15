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
        public string orderDimension;
        public Dictionary<string, List<GMRTuple>> tupleMap { get; set; }
    
        public Index(string orderDimension) {
            this.orderDimension = orderDimension;
        }

        public void RemoveKey(string[] tuple) {
            List<string> eqJoinVars = new List<string>();
            foreach (string var in eqJoinHeader) {
                eqJoinVars.Add(tuple[header.IndexOf(var)]);
            }
            string tupleString = TupleToString(eqJoinVars);
            tupleMap.Remove(tupleString);
        }

        private string GetKey(string[] tuple) {
            List<string> eqJoinVars = new List<string>();
            foreach (string var in eqJoinHeader) {
                if (tuple.Length == header.Count) {
                    eqJoinVars.Add(tuple[header.IndexOf(var)]);
                } else {
                    eqJoinVars.Add(tuple[eqJoinHeader.IndexOf(var)]);
                }
            }
            return TupleToString(eqJoinVars);
        }

        public bool ContainsKey(string[] tuple) {
            return tupleMap.ContainsKey(GetKey(tuple));
        }

        public List<GMRTuple> Get(string[] tuple) {
            string key = GetKey(tuple);
            if (!tupleMap.ContainsKey(key)) return new List<GMRTuple>();
            return tupleMap[key];
        }

        public List<GMRTuple> GetOrPlace(string[] tuple) {
            string key = GetKey(tuple);
            if (!tupleMap.ContainsKey(key)) tupleMap[key] = new List<GMRTuple>();
            return tupleMap[key];
        }


        public List<GMRTuple> SemiJoin(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            if (predicate == null) {
                return Get(rightTuple.fields);
            } else if (predicate.GetType().Name.Equals("CartesianProduct")) {
                return Get(rightTuple.fields);
            } else {
                return new JoinTupleGenerator(eqJoinHeader, orderDimension, rightHeader, rightTuple, predicate)
                    .RetrieveCorrespondingTuples(this);
            }
        }

        public bool AnyJoin(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            return new JoinTupleGenerator(eqJoinHeader, orderDimension, rightHeader, rightTuple, predicate).DoesJoin(this);
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
            /*int orderDimensionIndex = 0;
            string orderDimension = orderHeader[orderDimensionIndex];*/
            int orderIndex = header.IndexOf(orderDimension);
            string orderValue = tuple.fields[orderIndex];
            for (int i = 0; i < section.Count; i++) {
                if (Int32.Parse(orderValue) > Int32.Parse(section[i].fields[orderIndex])) {
                    return i;
                } else if (Int32.Parse(orderValue) == Int32.Parse(section[i].fields[orderIndex])) {
                    /*orderDimensionIndex++;
                    orderDimension = orderHeader[orderDimensionIndex];
                    orderIndex = header.IndexOf(orderDimension);
                    orderValue = tuple.fields[orderIndex];*/
                    return i;
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
