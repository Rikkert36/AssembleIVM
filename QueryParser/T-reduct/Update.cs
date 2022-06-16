using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssembleIVM.T_reduct {
    class Update {
        public List<string> unprojectHeader;
        public List<GMRTuple> unprojectedAddedTuples; //for every join
        public List<GMRTuple> unprojectedRemovedTuples;
        public Index projectedAddedTuples;
        public Index projectedRemovedTuples;
        //public HashSet<GMRTuple> appliedAddedTuples;
        //public HashSet<GMRTuple> appliedRemovedTuples;

        public Update(NodeReduct node) {
            unprojectedAddedTuples = new List<GMRTuple>();
            unprojectedRemovedTuples = new List<GMRTuple>();
            projectedAddedTuples = node.index.CopyWithoutData();
            projectedRemovedTuples = node.index.CopyWithoutData();
        }

        public Update(List<string> header, List<string> eqJoinHeader) {
            unprojectedAddedTuples = new List<GMRTuple>();
            unprojectedRemovedTuples = new List<GMRTuple>();
            projectedAddedTuples = new Index("") { tupleMap = new Dictionary<string, List<GMRTuple>>(),
                header = header, 
                eqJoinHeader = eqJoinHeader };
            projectedRemovedTuples = new Index("") {
                tupleMap = new Dictionary<string, List<GMRTuple>>(),
                header = header,
                eqJoinHeader = eqJoinHeader
            };
        }

        public void ProjectTuples(List<string> projectHeader) {
            List<int> projectIndices = new List<int>();
            foreach (string s in projectHeader) {
                int i = unprojectHeader.IndexOf(s);
                projectIndices.Add(i);
            }
            Dictionary<string, GMRTuple> addMap = new Dictionary<string, GMRTuple>();
            foreach (GMRTuple tuple in unprojectedAddedTuples) {
                GMRTuple projectTuple = new GMRTuple(projectHeader.Count, tuple.count) {
                    fields = projectIndices.Select(i => tuple.fields[i]).ToArray()
                };
                string key = projectTuple.ToString();
                if (addMap.ContainsKey(key)) {
                    projectTuple = addMap[key];
                    projectTuple.count += tuple.count;
                } else {
                    addMap.Add(key, projectTuple);
                }
            }
            Dictionary<string, GMRTuple> remMap = new Dictionary<string, GMRTuple>();
            foreach (GMRTuple tuple in unprojectedRemovedTuples) {
                GMRTuple projectTuple = new GMRTuple(projectHeader.Count, tuple.count) {
                    fields = tuple.fields
                };
                string key = projectTuple.ToString();
                if (remMap.ContainsKey(key)) {
                    projectTuple = remMap[key];
                    projectTuple.count += tuple.count;
                } else {
                    remMap.Add(key, projectTuple);
                }
            }
            
            foreach (GMRTuple tuple in addMap.Values) {
                AddAddedTuple(tuple);
            }

            foreach (GMRTuple tuple in remMap.Values) {
                AddRemovedTuple(tuple);
            }
        }

        public void SetAddedTuples(IEnumerable<GMRTuple> tupleList) {
            foreach (GMRTuple tuple in tupleList) {
                AddAddedTuple(tuple);
            }
        }

        public void SetRemovedTuples(IEnumerable<GMRTuple> tupleList) {
            foreach (GMRTuple tuple in tupleList) {
                AddRemovedTuple(tuple);
            }
        }

        public IEnumerable<GMRTuple> GetAddedTuples() {
            foreach (List<GMRTuple> tupleList in projectedAddedTuples.tupleMap.Values) {
                foreach (GMRTuple tuple in tupleList) {
                    yield return tuple;
                }
            }
        }

        public IEnumerable<GMRTuple> GetRemovedTuples() {
            foreach (List<GMRTuple> tupleList in projectedRemovedTuples.tupleMap.Values) {
                foreach (GMRTuple tuple in tupleList) {
                    yield return tuple;
                }
            }
        }

        public GMRTuple AddAddedTuple(GMRTuple tuple) {
            List<GMRTuple> section = projectedAddedTuples.GetOrPlace(tuple.fields);
            GMRTuple t = projectedAddedTuples.FindTuple(tuple, section);
            if (t != null) {
                t.count += tuple.count;
                return t;
            } else {
                if (projectedAddedTuples.orderDimension.Equals("")) {
                    section.Add(tuple);
                } else {
                    int loc = projectedAddedTuples.FindLocation(section, tuple);
                    section.Insert(loc, tuple);
                }
                return tuple;
            }
        }

        public GMRTuple AddRemovedTuple(GMRTuple tuple) {
            List<GMRTuple> section = projectedRemovedTuples.GetOrPlace(tuple.fields);
            GMRTuple t = projectedRemovedTuples.FindTuple(tuple, section);
            if (t != null) {
                t.count += tuple.count;
                return t;
            } else {
                if (projectedRemovedTuples.orderDimension.Equals("")) {
                    section.Add(tuple);
                } else {
                    int loc = projectedRemovedTuples.FindLocation(section, tuple);
                    section.Insert(loc, tuple);
                }
                return tuple;
            }
        }

    }
}
