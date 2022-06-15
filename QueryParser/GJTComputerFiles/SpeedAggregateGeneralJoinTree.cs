using AssembleIVM.T_reduct;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTComputerFiles {
    class SpeedAggregateGeneralJoinTree : GeneralJoinTree {
        string r1;
        string r2;
        string aggregateDimension;
        public SpeedAggregateGeneralJoinTree(GJTNode root, HashSet<GJTNode> frontier, HashSet<GJTLeaf> leafs, 
            List<string> outputHeader, List<string> outputVariables, List<string> unionData,
            string r1, string r2, string aggregateDimension) : 
            base(root, frontier, leafs, outputHeader, outputVariables, unionData) {
            this.r1 = r1;
            this.r2 = r2;
            this.aggregateDimension = aggregateDimension;
        }

        public override ReductTree GenerateReduct(string name) {
            return new SpeedAggregateReductTree(this, name, r1, r2, aggregateDimension);
        }
    }
}
