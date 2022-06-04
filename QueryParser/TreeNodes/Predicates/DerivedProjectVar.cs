using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Predicates {
    class DerivedProjectVar : TreeNode {
        TreeNode name;
        TreeNode predicate;
        public DerivedProjectVar(TreeNode name, TreeNode predicate) {
            this.name = name;
            this.predicate = predicate;
        }

        public override TreeNode Clone() {
            return new DerivedProjectVar(name.Clone(), predicate.Clone());
        }

        public override string GetString() {
            return $"{predicate.GetString()} as {name.GetString()}";
        }
    }
}
