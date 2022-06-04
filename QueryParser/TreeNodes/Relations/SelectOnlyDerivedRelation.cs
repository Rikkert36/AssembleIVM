using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Relations {
    class SelectOnlyDerivedRelation : TreeNode {
        TreeNode selectPredicate;
        TreeNode fromRelation;
        public SelectOnlyDerivedRelation(TreeNode selectPredicate, TreeNode fromRelation) {
            this.selectPredicate = selectPredicate;
            this.fromRelation = fromRelation;
        }

        public override TreeNode Clone() {
            return new SelectOnlyDerivedRelation(selectPredicate.Clone(), fromRelation.Clone());
        }

        public override string GetString() {
            return $"(select {selectPredicate.GetString()} from({fromRelation.GetString()}))";
        }
    }
}
