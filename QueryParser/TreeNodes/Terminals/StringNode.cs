using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.QueryParser.TreeNodes.Terminals {
    class StringNode : TreeNode {
        public readonly string value;

        public StringNode(string value) {
            this.value = value;
        }

        public override TreeNode Clone() {
            return new StringNode(value);
        }

        public override string GetString() {
            return value;
        }
    }
}
