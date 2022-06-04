using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes {
    class GroupRelation : TreeNode {
        readonly TreeNode relation;

        public GroupRelation(TreeNode relation) {
            this.relation = relation;
        }

        public override TreeNode Clone() {
            return new GroupRelation(relation.Clone());
        }

        public override string GetString() {
            return relation.GetString();
        }
    }
}
