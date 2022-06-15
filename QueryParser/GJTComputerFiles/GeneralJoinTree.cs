using AssembleIVM;
using AssembleIVM.T_reduct;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.GJTComputerFiles {
    class GeneralJoinTree {
        public GJTNode root;
        public HashSet<GJTNode> frontier;
        public HashSet<GJTLeaf> leafs;
        public List<string> outputHeader;
        public List<string> outputVariables;
        public List<string> unionData;

        public bool splitWeekAndYearValues = false;
        public bool uniteWeekAndYearValues = false;

        public GeneralJoinTree(GJTNode root, HashSet<GJTNode> frontier, HashSet<GJTLeaf> leafs,
             List<string> outputHeader, List<string> outputVariables, List<string> unionData) {
            this.root = root;
            this.frontier = frontier;
            this.leafs = leafs;
            this.outputHeader = outputHeader;
            this.outputVariables = outputVariables;
            this.unionData = unionData;
        }

        virtual public ReductTree GenerateReduct(string name) {
            return new ReductTree(this, name);
        }
    }
}
