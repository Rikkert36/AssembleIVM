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

        static string location = @$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\data\input\";

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
            RunModel();
            Console.WriteLine("done");
        }

        /*private static void CompareTupleCount() {
            UpdateFixedModel();
            UpdateModelFullForestFixed();
        }*/

        private static void Experiment() {
            Dictionary<string, Update>[] allUpdates = new Dictionary<string, Update>[10];
            //Dictionary<string, Update>[] allUpdatesCopy = new Dictionary<string, Update>[10];
            for (int n = 0; n < 10; n ++) {
                Dictionary<string, Update> updates = GetSmallRandomUpdates(n);
                allUpdates[n] = updates;
                /*Dictionary<string, Update> updatesCopy = new Dictionary<string, Update>();
                foreach (string key in updates.Keys) {
                    updatesCopy.Add(key, updates[key].Clone());
                }
                allUpdatesCopy[(n / 15) - 1] = updatesCopy;*/
            }
            for (int i = 0; i < 10; i++) {
                UpdateModel(allUpdates[i]);
                Console.WriteLine("-----------------------------");
            }
            Console.WriteLine("--------------------------------------------------\n\n\n");
        }

        private static Dictionary<string, Update> GetSmallRandomUpdates(int n) {
            Dictionary<string, Update> result = new Dictionary<string, Update>();
            HashSet<string> alreadyUpdatedTuples = new HashSet<string>();
            for (int i = 0; i < n; i++) {
                if (i % 3 == 0) {
                    if (!result.ContainsKey("DC001")) result.Add("DC001", new Update(new List<string> { "Employee", "Fact", "Week", "Hours" }, new List<string> { }));
                    RandomDC001Update(result["DC001"], alreadyUpdatedTuples);
                } else if (i % 3 == 1) {
                    if (!result.ContainsKey("DC005")) result.Add("DC005", new Update(new List<string> { "Employee", "Week", "Hours" }, new List<string> { }));
                    RandomDC005Update(result["DC005"], alreadyUpdatedTuples);
                } else {
                    if (!result.ContainsKey("R001")) result.Add("R001", new Update(new List<string> { "Employee", "Week", "Team" }, new List<string> { }));
                    RandomR001Update(result["R001"], alreadyUpdatedTuples);
                }
            }
            return result;
        }

        private static Dictionary<string, Update> GetBigRandomUpdates(int n) {
            Dictionary<string, Update> result = new Dictionary<string, Update>();
            HashSet<string> alreadyUpdatedTuples = new HashSet<string>();
            for (int i = 0; i < n / 3; i++) {
                if (!result.ContainsKey("DC001")) result.Add("DC001", new Update(new List<string> { "Employee", "Fact", "Week", "Hours" }, new List<string> { }));
                RandomDC001Update(result["DC001"], alreadyUpdatedTuples);
               
            }
            for (int i = 0; i < n / 3; i++) {
                if (!result.ContainsKey("DC005")) result.Add("DC005", new Update(new List<string> { "Employee", "Week", "Hours" }, new List<string> { }));
                RandomDC005Update(result["DC005"], alreadyUpdatedTuples);

            }
            for (int i = 0; i < n / 3; i++) {
                if (!result.ContainsKey("R001")) result.Add("R001", new Update(new List<string> { "Employee", "Week", "Team" }, new List<string> { }));
                RandomR001Update(result["R001"], alreadyUpdatedTuples);
            }
            return result;
        }

        private static void RandomDC001Update(Update u, HashSet<string> alreadyUpdatedTuples) {
            var rand = new Random();
            List<GMRTuple> tuples = Utils.CSVToTupleSet(@$"{location}\DC001.txt", new List<string> { "Employee", "Fact", "Week", "Hours" }).ToList();
            List<int> hours = new List<int> { 0, 8, 16 };
            while (true) {
                int i = rand.Next(tuples.Count);
                GMRTuple t = tuples[i];
                if (alreadyUpdatedTuples.Contains(t.ToString())) {
                    continue;
                } else {
                    alreadyUpdatedTuples.Add(t.ToString());
                    int newHours = hours[rand.Next(3)];
                    u.AddRemovedTuple(t);
                    GMRTuple tNew = new GMRTuple(4, 1) { fields = new string[] { t.fields[0], t.fields[1], t.fields[2], newHours.ToString() } };
                    u.AddAddedTuple(tNew);
                    break;
                }
            }
        }

        private static void RandomDC005Update(Update u, HashSet<string> alreadyUpdatedTuples) {
            var rand = new Random();
            List<GMRTuple> tuples = Utils.CSVToTupleSet(@$"{location}\DC005.txt", new List<string> { "Employee", "Week", "Hours" }).ToList();
            List<int> hours = new List<int> { 8, 16, 24, 32, 40 };
            while (true) {
                int i = rand.Next(tuples.Count);
                GMRTuple t = tuples[i];
                if (alreadyUpdatedTuples.Contains(t.ToString())) {
                    continue;
                } else {
                    alreadyUpdatedTuples.Add(t.ToString());
                    int newHours = hours[rand.Next(5)];
                    u.AddRemovedTuple(t);
                    GMRTuple tNew = new GMRTuple(3, 1) { fields = new string[] { t.fields[0], t.fields[1], newHours.ToString() } };
                    u.AddAddedTuple(tNew);
                    break;
                }
            }
        }

        private static void RandomR001Update(Update u, HashSet<string> alreadyUpdatedTuples) {
            var rand = new Random();
            List<GMRTuple> tuples = Utils.CSVToTupleSet(@$"{location}\R001.txt", new List<string> { "Employee", "Week", "Team" }).ToList();
            List<string> teams = new List<string> { "Dev1", "Dev2", "Dev3", "Con1", "Con2", "Con3", "Con4", "Mar1", "Mar2" };
            while (true) {
                int i = rand.Next(tuples.Count);
                GMRTuple t = tuples[i];
                if (alreadyUpdatedTuples.Contains(t.ToString())) {
                    continue;
                } else {
                    alreadyUpdatedTuples.Add(t.ToString());
                    string newTeam = teams[rand.Next(teams.Count)];
                    u.AddRemovedTuple(t);
                    GMRTuple tNew = new GMRTuple(3, 1) { fields = new string[] { t.fields[0], t.fields[1], newTeam } };
                    u.AddAddedTuple(tNew);
                    break;
                }
            }
        }

        private static Dictionary<string, Update> UpdatesClone(Dictionary<string, Update> original) {
            Dictionary<string, Update> result = new Dictionary<string, Update>();
            foreach (string key in original.Keys) {
                result[key] = original[key].Clone();
            }
            return result;
        }

        private static void AssertCorrectUpdates() {
            Dictionary<string, Update> updates = GetBigRandomUpdates(10);
            Dictionary<string, Update> updatesClone = UpdatesClone(updates);
            Dictionary<string, Update> updateResults = UpdateFixedModel(updates);
            Dictionary<string, Tuple<HashSet<string>, HashSet<string>>> trueDeltas = GetDeltas(updatesClone);
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
                foreach (string s in checkDelta.Item1) {
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


        public static Dictionary<string, Tuple<HashSet<string>, HashSet<string>>> GetDeltas(Dictionary<string, Update> extraUpdates) {
            Dictionary<string, Update> fullRunDatasetUpdates = new Dictionary<string, Update>();
            Dictionary<string, HashSet<string>> oldDatasetValues = new Dictionary<string, HashSet<string>>();
            Dictionary<string, Tuple<HashSet<string>, HashSet<string>>> result = new Dictionary<string, Tuple<HashSet<string>, HashSet<string>>>();
            TupleCounter.Push("Total run");
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
            TupleCounter.Pop("Total run");
            return result;
        }

        public static void UpdateModel(Dictionary<string, Update> updates) {
            TupleCounter.Push("Total delta update");
            foreach (ManualGJT sm in subModels) {
                UpdateSubModel(sm, updates);
            }
            TupleCounter.Pop("Total delta update");
            Console.WriteLine("");
        }

        public static void UpdateModelFullForest(Dictionary<string, Update> updates) {
            Dictionary<string, Update> fullRunDatasetUpdates = new Dictionary<string, Update>();
            TupleCounter.Push("Total update");
            foreach (ManualGJT sm in subModels) {
                UpdateSubModelNoOutput(sm, updates, fullRunDatasetUpdates);
            }
            TupleCounter.Pop("Total update");
            Console.WriteLine("");
        }

        public static Dictionary<string, Update> UpdateFixedModel(Dictionary<string, Update> datasetUpdates) {
            TupleCounter.Push("Total delta update");
            foreach (ManualGJT sm in subModels) {
                UpdateSubModel(sm, datasetUpdates);
            }
            TupleCounter.Pop("Total delta update");
            /*foreach (string s in datasetUpdates.Keys) {
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
            }*/
            return datasetUpdates;
        }

        public static void UpdateModelFullForestFixed() {
            Dictionary<string, Update> fullRunDatasetUpdates = new Dictionary<string, Update>();
            Dictionary<string, Update> datasetUpdates = DatasetUpdates();
            TupleCounter.Push("Total update");
            foreach (ManualGJT sm in subModels) {
                UpdateSubModelNoOutput(sm, datasetUpdates, fullRunDatasetUpdates);
            }
            TupleCounter.Pop("Total update");
        }

        private static bool IsEqual(Tuple<HashSet<string>, HashSet<string>> check, Tuple<HashSet<string>, HashSet<string>> correct) {
            bool result = true;
            foreach (string s in check.Item1) {
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
                foreach (string s in dif1) {
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
            Update result = new Update(new List<string> { "Employee", "Week", "Team" }, new List<string> { });
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
            TupleCounter.Push($"{name}");
            reduct.UpdateModel(datasetUpdates, false, false);
            TupleCounter.Pop($"{name}");
            Console.WriteLine("");
        }

        public static void UpdateSubModelByRunningFullTree(ManualGJT mgjt, Dictionary<string, Update> extraUpdates,
            Dictionary<string, Update> datasetUpdates,
            Dictionary<string, HashSet<string>> oldDatasetValues) {
            string name = mgjt.GetName();
            GeneralJoinTree gjt = mgjt.Construct();
            ReductTree reduct = gjt.GenerateReduct(name);
            reduct.GetCurrentOutputSet(oldDatasetValues);
            reduct.RunFullModelWithSomeUpdates(datasetUpdates, extraUpdates, false, false);
        }

        public static void UpdateSubModelNoOutput(ManualGJT mgjt, Dictionary<string, Update> extraUpdates,
            Dictionary<string, Update> datasetUpdates) {
            string name = mgjt.GetName();
            GeneralJoinTree gjt = mgjt.Construct();
            ReductTree reduct = gjt.GenerateReduct(name);
            TupleCounter.Push($"{name}");
            reduct.RunFullModelWithSomeUpdates(datasetUpdates, extraUpdates, false, false);
            TupleCounter.Pop($"{name}");
            Console.WriteLine("");
        }



        public static void RunSubModel(ManualGJT mgjt, Dictionary<string, Update> datasetUpdates) {
            string name = mgjt.GetName();
            GeneralJoinTree gjt = mgjt.Construct();
            ReductTree reduct = gjt.GenerateReduct(name);
            reduct.RunModel(datasetUpdates, true, true);
            Console.WriteLine("----------------------");
        }
    }
}
