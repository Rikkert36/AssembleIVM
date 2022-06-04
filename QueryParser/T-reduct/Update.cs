using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssembleIVM.T_reduct {
    class Update {
        public List<string> unprojectHeader;
        public Dictionary<string, Tuple<GMRTuple, int>> unprojectedAddedTuples; //for every join
        public Dictionary<string, Tuple<GMRTuple, int>> unprojectedRemovedTuples;
        public HashSet<GMRTuple> projectedAddedTuples;
        public HashSet<GMRTuple> projectedRemovedTuples;
        //public HashSet<GMRTuple> appliedAddedTuples;
        //public HashSet<GMRTuple> appliedRemovedTuples;

        public Update() {
            unprojectedAddedTuples = new Dictionary<string, Tuple<GMRTuple, int>>();
            unprojectedRemovedTuples = new Dictionary<string, Tuple<GMRTuple, int>>();
        }

        public void ProjectTuples(string[] projectHeader) {
            List<int> projectIndices = new List<int>();
            foreach (string s in new List<string>(projectHeader)) {
                projectIndices.Add(unprojectHeader.IndexOf(s));
            }
            Dictionary<string, GMRTuple> addMap = new Dictionary<string, GMRTuple>();
            foreach (Tuple<GMRTuple, int> tuple in unprojectedAddedTuples.Values) {
                GMRTuple projectTuple = new GMRTuple(projectHeader.Length, tuple.Item2) { 
                    fields = projectIndices.Select(index => tuple.Item1.fields[index]).ToArray()
                };
                string key = projectTuple.ToString();
                if (addMap.ContainsKey(key)) {
                    projectTuple = addMap[key];
                    projectTuple.count += tuple.Item2;
                } else {
                    addMap.Add(key, projectTuple);
                }
            }
            Dictionary<string, GMRTuple> remMap = new Dictionary<string, GMRTuple>();
            foreach (Tuple<GMRTuple, int> tuple in unprojectedRemovedTuples.Values) {
                GMRTuple projectTuple = new GMRTuple(projectHeader.Length, tuple.Item2) {
                    fields = projectIndices.Select(index => tuple.Item1.fields[index]).ToArray()
                };
                string key = projectTuple.ToString();
                if (remMap.ContainsKey(key)) {
                    projectTuple = remMap[key];
                    projectTuple.count += tuple.Item2;
                } else {
                    remMap.Add(key, projectTuple);
                }
            }
            projectedAddedTuples = new HashSet<GMRTuple>();
            foreach (GMRTuple addition in addMap.Values) {
                projectedAddedTuples.Add(addition);
            }
            projectedRemovedTuples = new HashSet<GMRTuple>();
            foreach (GMRTuple deletion in remMap.Values) {
                projectedRemovedTuples.Add(deletion);
            }
        }

        public Update SplitWeekAndYearValues(int weekIndex) {
            return new Update {
                projectedAddedTuples = new HashSet<GMRTuple>(projectedAddedTuples.Select(tuple => tuple.SplitWeekAndYearValue(weekIndex))),
                projectedRemovedTuples = new HashSet<GMRTuple>(projectedRemovedTuples.Select(tuple => tuple.SplitWeekAndYearValue(weekIndex)))
            };

        }

    }
}
