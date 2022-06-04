using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes {
    class ThetaJoinRelation : TreeNode {
        TreeNode left;
        TreeNode right;
        TreeNode predicate;

        public ThetaJoinRelation(TreeNode left, TreeNode right, TreeNode predicate) {
            this.left = left;
            this.right = right;
            this.predicate = predicate;
        }

        public override TreeNode Clone() {
            return new ThetaJoinRelation(left.Clone(), right.Clone(), predicate.Clone());
        }

        public override string GetString() {
            return $"({left.GetString()} thetajoin({predicate.GetString()}) {right.GetString()})";
        }
    }
}
