using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Predicates {
    class And : TreeNode {

        public TreeNode left;
        public TreeNode right;

        public And(TreeNode left, TreeNode right) {
            this.left = left;
            this.right = right;
        }

        public override TreeNode Clone() {
            return new And(left.Clone(), right.Clone());
        }

        public override string GetString() {
            return $"({left.GetString()} and {right.GetString()})";
        }


    }
}
