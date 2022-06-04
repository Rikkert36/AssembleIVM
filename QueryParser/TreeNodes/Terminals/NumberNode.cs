using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Terminals {
    class NumberNode : TreeNode {
        public readonly string value;
        public bool positive;

        public NumberNode(string value, bool positive) {
            this.value = value;
            this.positive = positive;
        }

        public override TreeNode Clone() {
            return new NumberNode(value, positive);
        }

        public override string GetString() {
            if (positive) return value;
            return "-" + value;
        }
    }
}
