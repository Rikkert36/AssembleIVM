using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Terminals {
    class DimensionName : TreeNode {
        public string value;
        public DimensionName(string value) {
            this.value = value;
        }

        public override TreeNode Clone() {
            return new DimensionName(value);
        }

        public override string GetString() {
            return value;
        }

        
    }
}
