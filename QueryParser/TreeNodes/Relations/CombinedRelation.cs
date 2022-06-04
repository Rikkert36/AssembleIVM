using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes {
    class CombinedRelation : TreeNode {
        public TreeNode left;
        public TreeNode right;
        string relOperator;

        public CombinedRelation(TreeNode left, TreeNode right, string relOperator) {
            this.left = left;
            this.right = right;
            this.relOperator = relOperator;
        }

        public override TreeNode Clone() {
            return new CombinedRelation(left.Clone(), right.Clone(), relOperator);
        }

        public override string GetString() {
            return $"({left.GetString()} {relOperator} {right.GetString()})"; 
        }
    }
}
