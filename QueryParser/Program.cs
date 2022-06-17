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
            UpdateModel();
            Console.WriteLine("done");
        }

        public static void RunModel() {
            Dictionary<string, Update> datasetUpdates = new Dictionary<string, Update>();
            foreach (ManualGJT sm in subModels) {
                RunSubModel(sm, datasetUpdates);
            }

        }

        public static void UpdateModel() {

            Dictionary<string, Update> datasetUpdates = DatasetUpdates();

            foreach (ManualGJT sm in subModels) {
                UpdateSubModel(sm, datasetUpdates);
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
        }

        private static Dictionary<string, Update> DatasetUpdates() {
            Dictionary<string, Update> result = new Dictionary<string, Update>();
            result["DC001"] = DC001Update();
            return result;
        }

        private static Update DC001Update() {
            Update result = new Update(new List<string> { "Employee", "Fact", "Week", "Hours" }, new List<string> { });
            result.SetAddedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Rik", "education", "W01.2022", "8" } } });
            result.SetRemovedTuples(new HashSet<GMRTuple> { new GMRTuple(4, 1) { fields = new string[] { "Rik", "education", "W01.2022", "0" } } });
            return result;
        }

        public static void UpdateSubModel(ManualGJT mgjt, Dictionary<string, Update> datasetUpdates) {
            string name = mgjt.GetName();
            Timer.Start(name);
            GeneralJoinTree gjt = mgjt.Construct();
            ReductTree reduct = gjt.GenerateReduct(name);
            reduct.RunModel(datasetUpdates, true, false, false);
            Timer.Stop(name);
        }



        public static void RunSubModel(ManualGJT mgjt, Dictionary<string, Update> datasetUpdates) {
            string name = mgjt.GetName();
            GeneralJoinTree gjt = mgjt.Construct();
            ReductTree reduct = gjt.GenerateReduct(name);
            Timer.Start(name);
            reduct.RunModel(datasetUpdates, false, true, true);
            Timer.Stop(name);
            Console.WriteLine("----------------------");
        }
    }
}
