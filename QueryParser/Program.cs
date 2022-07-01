using AssembleIVM;
using AssembleIVM.GJTs;
using AssembleIVM.QueryParser.TreeNodes.Predicates.AlgebraicExpressions;
using AssembleIVM.T_reduct;
using QueryParser.GJTComputerFiles;
using QueryParser.NewParser;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QueryParser {
    class Program {

        private static List<ManualGJT> subModels = new List<ManualGJT> {
                new Copyfactgross1(),
                new Computenetavail2(),
                new Rollupdc001toyear3(),
                new Fillinteams4(),
                new Sumhoursperteam5(),
                new Fillindepartments6(),
                new Sumhoursperdepartment6(),
                new Rollupdepartments6(),
                new Computeadjustedhours7(),
                new DC002withYL8(),
                new Movingtotal8(),
                new Rollupdc002toyear9(),
                new Rollupdc007toyear10(),
                new Computepercentagefacts11(),
                new TeamrelationtoDC12(),
                new Countemployeesperteam13(),
                new Determineemptyteams14()
            };

        static void Main(string[] args) {
            AssertCorrectUpdates();
            Console.WriteLine("done");
        }

        private static void AssertCorrectUpdates() {
            Dictionary<string, Update> updateResults = UpdateModel();
            Dictionary<string, Tuple<HashSet<string>, HashSet<string>>> trueDeltas = GetDeltas();
            bool everythingEqual = true;
            foreach (string dataset in trueDeltas.Keys) {
                Update updateResult = updateResults[dataset];
                HashSet<GMRTuple> added = updateResult.GetAddedTuples().ToHashSet();
                HashSet<GMRTuple> removed = updateResult.GetRemovedTuples().ToHashSet();

                Tuple<HashSet<string>, HashSet<string>> checkDelta = UpdateSetToStrings(added, removed);
                Tuple<HashSet<string>, HashSet<string>> trueDelta = trueDeltas[dataset];

                Console.WriteLine(dataset);
                Console.WriteLine("Added:");
                Console.WriteLine("Result:");
                foreach(string s in checkDelta.Item1 ) {
                    Console.WriteLine(s);
                }
                Console.WriteLine("");
                Console.WriteLine("True:");
                foreach (string s in trueDelta.Item1) {
                    Console.WriteLine(s);
                }
                Console.WriteLine("");
                Console.WriteLine("Removed:");
                Console.WriteLine("Result:");
                foreach (string s in checkDelta.Item2) {
                    Console.WriteLine(s);
                }
                Console.WriteLine("");
                Console.WriteLine("True:");
                foreach (string s in trueDelta.Item2) {
                    Console.WriteLine(s);
                }
                bool equal = IsEqual(checkDelta, trueDelta);
                Console.WriteLine($"correct update:{equal}");
                if (!equal) {
                    everythingEqual = false;
                    PrintDifference(checkDelta, trueDelta);
                }
                Console.WriteLine("---------------------------");

            }
            Console.WriteLine($"Update works for this update: {everythingEqual}");
        }

        private static Tuple<HashSet<string>, HashSet<string>> UpdateSetToStrings(HashSet<GMRTuple> added, HashSet<GMRTuple> removed) {
            HashSet<string> addedWithDups = added.Select(tuple => tuple.GetString()).ToHashSet();
            HashSet<string> removedWithDups = removed.Select(tuple => tuple.GetString()).ToHashSet();
            return new Tuple<HashSet<string>, HashSet<string>>(
                new HashSet<string>(addedWithDups.Except(removedWithDups)),
                new HashSet<string>(removedWithDups.Except(addedWithDups))
                );
        }

        public static void RunModel() {
            Dictionary<string, Update> datasetUpdates = new Dictionary<string, Update>();
            foreach (ManualGJT sm in subModels) {
                RunSubModel(sm, datasetUpdates);
            }
        }


        public static Dictionary<string, Tuple<HashSet<string>, HashSet<string>>> GetDeltas() {
            Dictionary<string, Update> fullRunDatasetUpdates = new Dictionary<string, Update>();
            Dictionary<string, Update> extraUpdates = DatasetUpdates();
            Dictionary<string, HashSet<string>> oldDatasetValues = new Dictionary<string, HashSet<string>>();
            Dictionary<string, Tuple<HashSet<string>, HashSet<string>>> result = new Dictionary<string, Tuple<HashSet<string>, HashSet<string>>>();
            foreach (ManualGJT sm in subModels) {
                UpdateSubModelByRunningFullTree(sm, extraUpdates, fullRunDatasetUpdates, oldDatasetValues);
                if (fullRunDatasetUpdates.ContainsKey(sm.GetName())) {
                    Console.WriteLine($"{sm.GetName()}:\n");
                    HashSet<string> newSet = fullRunDatasetUpdates[sm.GetName()].GetAddedTuples().Select(tuple => tuple.GetString()).ToHashSet();
                    HashSet<string> oldSet = oldDatasetValues[sm.GetName()];
                    HashSet<string> added = new HashSet<string>(newSet.Except(oldSet));
                    HashSet<string> removed = new HashSet<string>(oldSet.Except(newSet));
                    if (added.Count != 0 || removed.Count != 0) {
                        result.Add(sm.GetName(), new Tuple<HashSet<string>, HashSet<string>>(added, removed));
                    }
                    Console.WriteLine($"added:");
                    foreach (string s in added) {
                        Console.WriteLine(s);
                    }
                    Console.WriteLine($"\nremoved:");
                    foreach (string s in removed) {
                        Console.WriteLine(s);
                    }
                    Console.Write("----------------------\n");
                }
            }
            return result;
        }

        public static Dictionary<string, Update> UpdateModel() {

            Dictionary<string, Update> datasetUpdates = DatasetUpdates();

            foreach (ManualGJT sm in subModels) {
                Timer.Start(sm.GetName());
                UpdateSubModel(sm, datasetUpdates);
                Timer.Stop(sm.GetName());
                Console.WriteLine("---------------------");
            }
            foreach (string s in datasetUpdates.Keys) {
                Console.WriteLine($"{s}:\n");
                Console.WriteLine($"added:");
                foreach (GMRTuple t in datasetUpdates[s].GetAddedTuples()) {
                    Console.WriteLine(t.GetString());
                }
                Console.WriteLine($"\nremoved:");
                foreach (GMRTuple t in datasetUpdates[s].GetRemovedTuples()) {
                    Console.WriteLine(t.GetString());
                }
                Console.Write("----------------------\n");
            }
            return datasetUpdates;
        }

        private static bool IsEqual(Tuple<HashSet<string>, HashSet<string>> check, Tuple<HashSet<string>, HashSet<string>> correct) {
            bool result = true;
            foreach(string s in check.Item1) {
                if (!correct.Item1.Contains(s)) result = false;
            }
            foreach (string s in check.Item2) {
                if (!correct.Item2.Contains(s)) result = false;
            }
            foreach (string s in correct.Item1) {
                if (!check.Item1.Contains(s)) result = false;
            }
            foreach (string s in correct.Item2) {
                if (!check.Item2.Contains(s)) result = false;
            }
            return result;
        }

        private static void PrintDifference(Tuple<HashSet<string>, HashSet<string>> check, Tuple<HashSet<string>, HashSet<string>> correct) {
            HashSet<string> dif1 = new HashSet<string>(check.Item1.Except(correct.Item1));
            if (dif1.Count > 0) {
                foreach(string s in dif1) {
                    Console.WriteLine($"Should not be in update+ but is: {s}");
                }
            }
            HashSet<string> dif2 = new HashSet<string>(check.Item2.Except(correct.Item2));
            if (dif2.Count > 0) {
                foreach (string s in dif2) {
                    Console.WriteLine($"Should not be in update- but is: {s}");
                }
            }
            HashSet<string> dif3 = new HashSet<string>(correct.Item1.Except(check.Item1));
            if (dif3.Count > 0) {
                foreach (string s in dif3) {
                    Console.WriteLine($"Should be in update+ but is not: {s}");
                }
            }
            HashSet<string> dif4 = new HashSet<string>(correct.Item2.Except(check.Item2));
            if (dif4.Count > 0) {
                foreach (string s in dif4) {
                    Console.WriteLine($"Should be in update- but is not: {s}");
                }
            }

        }

        private static Dictionary<string, Update> DatasetUpdates() {
            Dictionary<string, Update> result = new Dictionary<string, Update>();
            result["R001"] = R001Update();
            result["DC001"] = DC001Update();
            return result;
        }

        private static Update DC001Update() {
            Update result = new Update(new List<string> { "Employee", "Fact", "Week", "Hours" }, new List<string> { });
            /*result.SetAddedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Rik", "education", "W01.2022", "8" } } });
            result.SetRemovedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Rik", "education", "W01.2022", "0" } } });*/
            result.SetAddedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Anne", "holiday", "W52.2022", "8" } } });
            result.SetRemovedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Anne", "holiday", "W52.2022", "0" } } });
            return result;
        }

        private static Update R001Update() {
            Update result = new Update(new List<string> { "Employee", "Fact", "Week", "Hours" }, new List<string> { });
            result.SetAddedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Jochem", "W47.2023", "Dev1" } } });
            result.SetRemovedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Jochem", "W47.2023", "Dev3" } } });
            result.SetAddedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Joris", "W14.2023", "Dev2" } } });
            result.SetRemovedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Joris", "W14.2023", "Dev1" } } });
            result.SetAddedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Ymke", "W14.2023", "Dev2" } } });
            result.SetRemovedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Ymke", "W14.2023", "Dev1" } } });
            result.SetAddedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Bowie", "W14.2023", "Dev2" } } });
            result.SetRemovedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Bowie", "W14.2023", "Dev1" } } });
            result.SetAddedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Bowie", "W18.2023", "Dev2" } } });
            result.SetRemovedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Bowie", "W18.2023", "Dev1" } } });
            return result;
        }

        public static void UpdateSubModel(ManualGJT mgjt, Dictionary<string, Update> datasetUpdates) {
            string name = mgjt.GetName();
            GeneralJoinTree gjt = mgjt.Construct();
            ReductTree reduct = gjt.GenerateReduct(name);
            reduct.UpdateModel(datasetUpdates, false, false);
        }

        public static void UpdateSubModelByRunningFullTree(ManualGJT mgjt, Dictionary<string, Update> extraUpdates,
            Dictionary<string, Update> datasetUpdates,
            Dictionary<string, HashSet<string>> oldDatasetValues) {
            string name = mgjt.GetName();
            Timer.Start(name);
            GeneralJoinTree gjt = mgjt.Construct();
            ReductTree reduct = gjt.GenerateReduct(name);
            reduct.GetCurrentOutputSet(oldDatasetValues);
            reduct.RunFullModelWithSomeUpdates(datasetUpdates, extraUpdates, false, false);
            Timer.Stop(name);
        }



        public static void RunSubModel(ManualGJT mgjt, Dictionary<string, Update> datasetUpdates) {
            string name = mgjt.GetName();
            GeneralJoinTree gjt = mgjt.Construct();
            ReductTree reduct = gjt.GenerateReduct(name);
            Timer.Start(name);
            reduct.RunModel(datasetUpdates, true, true);
            Timer.Stop(name);
            Console.WriteLine("----------------------");
        }
    }
}
