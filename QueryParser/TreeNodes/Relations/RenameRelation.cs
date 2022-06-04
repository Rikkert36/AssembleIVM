using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes {
    class RenameRelation : TreeNode {
        public TreeNode relation;
        public string cName;
        public RenameRelation(TreeNode relation, string cName) {
            this.relation = relation;
            this.cName = cName;
        }

        public override TreeNode Clone() {
            return new RenameRelation(relation.Clone(), cName);
        }

        public override string GetString() {
            return "(" + relation.GetString() + " as " + cName + ")";
        }
    }
}
