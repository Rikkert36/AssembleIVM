using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Predicates {
    class Or : TreeNode {

        public TreeNode left;
        public TreeNode right;

        public Or(TreeNode left, TreeNode right) {
            this.left = left;
            this.right = right;
        }

        public override TreeNode Clone() {
            return new Or(left.Clone(), right.Clone());
        }

        public override string GetString() {
            return $"({left.GetString()} or {right.GetString()})";
        }
    }
}
