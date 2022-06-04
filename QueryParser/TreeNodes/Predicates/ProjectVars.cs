using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Predicates {
    class ProjectVars : TreeNode {
        List<TreeNode> projectVars;
        public ProjectVars(List<TreeNode> projectVars) {
            this.projectVars = projectVars;
        }

        public override TreeNode Clone() {
            return new ProjectVars(projectVars.Select(var => var.Clone()).ToList());
        }

        public override string GetString() {
            string result = "";
            result = projectVars[0].GetString();
            for (int i = 1; i < projectVars.Count; i++) {
                result += $", {projectVars[i].GetString()}";
            }
            return result;
        }
    }
}
