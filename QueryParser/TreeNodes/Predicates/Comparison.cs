using QueryParser.NewParser.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Predicates {
    class Comparison : TreeNode {
        public TreeNode left;
        public TreeNode right;
        public string compareOperator;

        public Comparison(TreeNode left, TreeNode right, string compareOperator) {
            this.left = left;
            this.right = right;
            this.compareOperator = compareOperator;
        }

        public override string GetString() {
            return $"({left.GetString()} {compareOperator} {right.GetString()})";
        }

        public override TreeNode Clone() {
            return new Comparison(left.Clone(), right.Clone(), compareOperator);
        }
    }
}
