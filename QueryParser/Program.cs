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
            ManualGJT mgjt = new Computenetavail2();
            string name = mgjt.GetName();
            GeneralJoinTree gjt = mgjt.Construct();
            Dictionary<string, Update> datasetUpdates = new Dictionary<string, Update>();
            HashSet<GMRTuple> deletions = new HashSet<GMRTuple> {
                new GMRTuple(4, 1) { fields = new string[]{ "Rik", "12", "ga", "40"} },
                new GMRTuple(4, 1) { fields = new string[]{ "Rik", "12", "hol", "10"} }

            };
            /*datasetUpdates.Add("testinput/A", new Update {
                projectedAddedTuples = new HashSet<GMRTuple>(),
                projectedRemovedTuples = deletions
            });*/
            ReductTree reduct = new ReductTree(gjt, name);
            reduct.RunModel(datasetUpdates, false, false, false);
            Console.WriteLine("done");
        }
    }
}
