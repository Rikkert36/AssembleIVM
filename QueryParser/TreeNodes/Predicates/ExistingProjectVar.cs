using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Predicates {
    class ExistingProjectVar : TreeNode {
        TreeNode value;
        public ExistingProjectVar(TreeNode value) {
            this.value = value;
        }

        public override TreeNode Clone() {
            return new ExistingProjectVar(value.Clone());
        }

        public override string GetString() {
            return value.GetString();
        }
    }
}
