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
        static void Main(string[] args) {
            RunModel();
            Console.WriteLine("done");
        }

        public static void RunModel() {
            Dictionary<string, Update> datasetUpdates = new Dictionary<string, Update>();
            List<ManualGJT> subModels = new List<ManualGJT> { 
                new Copyfactgross1(),
                new Computenetavail2()
            };
            foreach(ManualGJT sm in subModels) {
                RunSubModel(sm, datasetUpdates);
            }
        }

        public static void RunSubModel(ManualGJT mgjt, Dictionary<string, Update> datasetUpdates) {
            string name = mgjt.GetName();
            GeneralJoinTree gjt = mgjt.Construct();
            ReductTree reduct = new ReductTree(gjt, name);
            reduct.RunModel(datasetUpdates, false, false, true);
        }
    }
}
