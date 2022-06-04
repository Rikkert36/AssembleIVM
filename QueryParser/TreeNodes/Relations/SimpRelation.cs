using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes {
    class SimpRelation : TreeNode {
        string name;
        public SimpRelation(string name) {
            this.name = name;
        }

        public override TreeNode Clone() {
            return new SimpRelation(name);
        }

        public override string GetString() {
            return name;
        }
    }
}
