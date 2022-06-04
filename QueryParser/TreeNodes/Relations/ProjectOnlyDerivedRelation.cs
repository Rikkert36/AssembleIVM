using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Relations {
    class ProjectOnlyDerivedRelation : TreeNode {
        TreeNode projectVars;
        TreeNode fromRelation;
        public ProjectOnlyDerivedRelation(TreeNode projectVars, TreeNode fromRelation) {
            this.projectVars = projectVars;
            this.fromRelation = fromRelation;
        }

        public override TreeNode Clone() {
            return new ProjectOnlyDerivedRelation(projectVars.Clone(), fromRelation.Clone());
        }

        public override string GetString() {
            return $"(project {projectVars.GetString()} from({fromRelation.GetString()}))";
        }
    }
}
