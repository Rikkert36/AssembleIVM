using QueryParser.GJTComputerFiles.ConstructorStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.GJTComputerFiles {
    class GeneralJoinTree {
        public GJTNode root;
        public HashSet<GJTNode> frontier;
        public HashSet<GJTLeaf> leafs;

        public bool splitWeekAndYearValues = false;
        
        public GeneralJoinTree(GJTNode root, HashSet<GJTNode> frontier, HashSet<GJTLeaf> leafs) {
            this.root = root;
            this.frontier = frontier;
            this.leafs = leafs;
        }
    }
}
