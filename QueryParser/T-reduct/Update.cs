using QueryParser.NewParser.TreeNodes;
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
            projectedAddedTuples = new Index("") {
                tupleMap = new Dictionary<string, List<GMRTuple>>(),
                header = header,
                eqJoinHeader = eqJoinHeader
            };
            projectedRemovedTuples = new Index("") {
                tupleMap = new Dictionary<string, List<GMRTuple>>(),
                header = header,
                eqJoinHeader = eqJoinHeader
            };
        }

        public Update NewUpdate(NodeReduct node, bool splitWeekAndYearValues) {
            Update result = new Update(node);
            if (!splitWeekAndYearValues) {
                result.SetAddedTuples(GetAddedTuples());
                result.SetRemovedTuples(GetRemovedTuples());
            } else {
                int weekIndex = Utils.GetWeekIndex(result.projectedAddedTuples.header);
                result.SetAddedTuples(GetAddedTuples().Select(tuple => tuple.SplitWeekAndYearValue(weekIndex)));
                result.SetRemovedTuples(GetRemovedTuples().Select(tuple => tuple.SplitWeekAndYearValue(weekIndex)));

            }
            return result;
        }

        public void ProjectTuples(List<string> projectHeader) {
            List<int> projectIndices = new List<int>();
            foreach (string s in projectHeader) {
                int i = unprojectHeader.IndexOf(s);
                projectIndices.Add(i);
            }
            Dictionary<string, GMRTuple> addMap = new Dictionary<string, GMRTuple>();
            foreach (GMRTuple tuple in unprojectedAddedTuples) {
                GMRTuple projectTuple;
                if (tuple.fields.Length == unprojectHeader.Count) {
                    projectTuple = new GMRTuple(projectHeader.Count, tuple.count) {
                        fields = projectIndices.Select(i => tuple.fields[i]).ToArray()
                    };
                } else {
                    projectTuple = tuple;
                }
                string key = projectTuple.ToString();
                if (addMap.ContainsKey(key)) {
                    projectTuple = addMap[key];
                    projectTuple.count += tuple.count;                   
                } else {
                    addMap.Add(key, projectTuple);
                }
                if (tuple.isPlusTuple) projectTuple.isPlusTuple = true;
            }
            Dictionary<string, GMRTuple> remMap = new Dictionary<string, GMRTuple>();
            foreach (GMRTuple tuple in unprojectedRemovedTuples) {
                GMRTuple projectTuple;
                if (tuple.fields.Length == unprojectHeader.Count) {
                    projectTuple = new GMRTuple(projectHeader.Count, tuple.count) {
                        fields = projectIndices.Select(i => tuple.fields[i]).ToArray()
                    };
                } else {
                    projectTuple = tuple;
                }
                string key = projectTuple.ToString();
                if (remMap.ContainsKey(key)) {
                    projectTuple = remMap[key];
                    projectTuple.count += tuple.count;
                } else {
                    remMap.Add(key, projectTuple);
                }
                if (tuple.isPlusTuple) projectTuple.isPlusTuple = true;
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

        public List<GMRTuple> SemiJoinAdded(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            return projectedAddedTuples.SemiJoin(rightHeader, rightTuple, predicate);
        }

        public List<GMRTuple> SemiJoinRemoved(List<string> rightHeader, GMRTuple rightTuple, TreeNode predicate) {
            return projectedRemovedTuples.SemiJoin(rightHeader, rightTuple, predicate);
        }

        public GMRTuple AddAddedTuple(GMRTuple tuple) {
            List<GMRTuple> section = projectedAddedTuples.GetOrPlace(tuple.fields);

            if (projectedAddedTuples.orderDimension.Equals("")) {
                section.Add(tuple);
            } else {
                int loc = projectedAddedTuples.FindLocation(section, tuple);

                section.Insert(loc, tuple);
            }
            return tuple;

        }

        public GMRTuple AddRemovedTuple(GMRTuple tuple) {
            List<GMRTuple> section = projectedRemovedTuples.GetOrPlace(tuple.fields);

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
