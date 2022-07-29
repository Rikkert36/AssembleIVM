using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct {
    [Serializable]
    class Index {
        public List<string> header { get; set; }
        public List<string> eqJoinHeader { get; set; }
        public string orderDimension;
        public Dictionary<string, List<GMRTuple>> tupleMap { get; set; }

        public Index(string orderDimension) {
            this.orderDimension = orderDimension;
        }

        public Index CopyWithoutData() {
            return new Index(orderDimension) {
                tupleMap = new Dictionary<string, List<GMRTuple>>(),
                header = header,
                eqJoinHeader = eqJoinHeader,
            };
        }


        public void RemoveKey(string[] tuple) {
            List<string> eqJoinVars = new List<string>();
            foreach (string var in eqJoinHeader) {
                eqJoinVars.Add(tuple[header.IndexOf(var)]);
            }
            string tupleString = TupleToString(eqJoinVars);
            tupleMap.Remove(tupleString);
        }

        private string GetKey(List<string> parentHeader, string[] tuple) {
            List<string> eqJoinVars = new List<string>();
            foreach (string var in eqJoinHeader) {
                eqJoinVars.Add(tuple[parentHeader.IndexOf(var)]);
            }
            return TupleToString(eqJoinVars);
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

        public GMRTuple GetTuple(GMRTuple tuple) {
            List<GMRTuple> section = Get(tuple.fields);
            if (section == null) return null;
            return FindTuple(tuple, section);
        }

        public List<GMRTuple> Get(string[] tuple) {
            string key = GetKey(tuple);
            if (!tupleMap.ContainsKey(key)) return new List<GMRTuple>();
            return tupleMap[key];
        }

        /*This method is now made such that it only returns one value, in general it should return a list,
        but, since for the only aggregatejointree in this model this does not have to be, we do it like this*/ 
        public List<GMRTuple> Get(List<string> header, GMRTuple tuple) {
            string key = GetKey(header, tuple.fields);
            if (!tupleMap.ContainsKey(key)) return new List<GMRTuple>();
            List<GMRTuple> result = new List<GMRTuple>();
            GMRTuple found = FindTuple(tuple, tupleMap[key]);
            if (found != null) result.Add(found);
            return result;
        }

        public List<GMRTuple> GetOrPlace(string[] tuple) {
            string key = GetKey(tuple);
            if (!tupleMap.ContainsKey(key)) tupleMap[key] = new List<GMRTuple>();
            return tupleMap[key];
        }

        //Only for aggregatejoinnode
        public List<GMRTuple> SemiJoinLeftChild(List<string> parentHeader, GMRTuple tuple) {
            return Get(parentHeader, tuple);
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

        public GMRTuple FindTuple(GMRTuple tuple, List<GMRTuple> section) {
            if (orderDimension.Equals("")) {
                foreach (GMRTuple otherTuple in section) {
                    TupleCounter.Increment();
                    if (otherTuple.Equals(tuple)) return otherTuple;
                }
            } else {
                int orderDimensionLocation = header.IndexOf(orderDimension);
                int L = 0;
                int R = section.Count - 1;
                if (orderDimension.Equals("Week")) {
                    int x = int.Parse(tuple.fields[orderDimensionLocation].Substring(1, 2));
                    while (L <= R) {
                        TupleCounter.Increment();
                        int m = (L + R) / 2;
                        if (int.Parse(section[m].fields[orderDimensionLocation].Substring(1,2)) < x) {
                            L = m + 1;
                        } else if (int.Parse(section[m].fields[orderDimensionLocation].Substring(1,2)) > x) {
                            R = m - 1;
                        } else {
                            return section[m];
                        }
                    }
                } else {
                    int x = int.Parse(tuple.fields[orderDimensionLocation]);
                    while (L <= R) {
                        TupleCounter.Increment();
                        int m = (L + R) / 2;
                        if (int.Parse(section[m].fields[orderDimensionLocation]) < x) {
                            L = m + 1;
                        } else if (int.Parse(section[m].fields[orderDimensionLocation]) > x) {
                            R = m - 1;
                        } else {
                            return section[m];
                        }
                    }
                }
                
                
            }
            return null;
        }

        public int FindLocation(List<GMRTuple> section, GMRTuple tuple) {
            int orderDimensionLocation = header.IndexOf(orderDimension);
            int L = 0;
            int R = section.Count - 1;
            int result = -1;
            if (orderDimension.Equals("Week")) {
                int x = int.Parse(tuple.fields[orderDimensionLocation].Substring(1, 2));
                while (L <= R) {
                    TupleCounter.Increment();
                    int m = (L + R) / 2;
                    if (int.Parse(section[m].fields[orderDimensionLocation].Substring(1, 2)) < x) {
                        L = m + 1;
                        result = L;
                    } else if (int.Parse(section[m].fields[orderDimensionLocation].Substring(1, 2)) > x) {
                        R = m - 1;
                        result = R;
                    } 
                }
            } else {
                int x = int.Parse(tuple.fields[orderDimensionLocation]);
                while (L <= R) {
                    TupleCounter.Increment();
                    int m = (L + R) / 2;
                    if (int.Parse(section[m].fields[orderDimensionLocation]) < x) {
                        L = m + 1;
                        result = L;
                    } else if (int.Parse(section[m].fields[orderDimensionLocation]) > x) {
                        R = m - 1;
                        result = R;
                    } 
                }
            }
            if (result == R) {
                return result + 1;
            } else {
                return result;
            }
        }

        private string TupleToString(List<string> tuple) {
            string result = "";
            foreach (string s in tuple) {
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
