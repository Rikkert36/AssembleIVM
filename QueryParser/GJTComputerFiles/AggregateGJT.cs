using AssembleIVM.T_reduct;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTComputerFiles {
    class AggregateGJT : GeneralJoinTree {
        public AggregateGJT(GJTNode root, HashSet<GJTNode> frontier, HashSet<GJTLeaf> leafs, List<string> outputHeader, List<string> outputVariables, List<string> unionData) : base(root, frontier, leafs, outputHeader, outputVariables, unionData) {
        }

        public override ReductTree GenerateReduct(string name) {
            return new AggregateReductTree(this, name);
        }
    }
}
