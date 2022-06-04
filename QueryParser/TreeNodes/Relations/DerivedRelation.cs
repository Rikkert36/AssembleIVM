using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes {
    class DerivedRelation : TreeNode {
        TreeNode projectVars;
        TreeNode selectPredicate;
        TreeNode fromRelation;
        public DerivedRelation(TreeNode projectVars, TreeNode selectPredicate, TreeNode fromRelation) {
            this.projectVars = projectVars;
            this.selectPredicate = selectPredicate;
            this.fromRelation = fromRelation;
        }

        public override TreeNode Clone() {
            return new DerivedRelation(projectVars.Clone(), selectPredicate.Clone(), fromRelation.Clone());
        }

        public override string GetString() {
            return $"(project {projectVars.GetString()} select {selectPredicate.GetString()} from({fromRelation.GetString()}))";
        }
    }
}
