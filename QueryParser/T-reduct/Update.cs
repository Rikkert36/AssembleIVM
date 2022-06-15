using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssembleIVM.T_reduct {
    class Update {
        public List<string> unprojectHeader;
        public List<GMRTuple> unprojectedAddedTuples; //for every join
        public List<GMRTuple> unprojectedRemovedTuples;
        public HashSet<GMRTuple> projectedAddedTuples;
        public HashSet<GMRTuple> projectedRemovedTuples;
        //public HashSet<GMRTuple> appliedAddedTuples;
        //public HashSet<GMRTuple> appliedRemovedTuples;

        public Update() {
            unprojectedAddedTuples = new List<GMRTuple>();
            unprojectedRemovedTuples = new List<GMRTuple>();
        }

        public void ProjectTuples(List<string> projectHeader) {
            List<int> projectIndices = new List<int>();
            foreach (string s in projectHeader) {
                int i = unprojectHeader.IndexOf(s);
                if (i == 3) {
                    Console.WriteLine("heej");
                }
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
