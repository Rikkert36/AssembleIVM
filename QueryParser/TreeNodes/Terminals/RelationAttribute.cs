using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Terminals {
    class RelationAttribute : TreeNode {

        public string relationName;
        public string dimensionName;

        public RelationAttribute(string relationName, string dimensionName) {
            this.relationName = relationName;
            this.dimensionName = dimensionName;
        }

        public override TreeNode Clone() {
            return new RelationAttribute(relationName, dimensionName);
        }

        public override string GetString() {
            return $"{relationName}.{dimensionName}";
        }
    }
}
