using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Terminals {
    class Function : TreeNode {
        string name;
        TreeNode dimension;
        public Function(string name, TreeNode dimension) {
            this.name = name;
            this.dimension = dimension;
        }

        public override TreeNode Clone() {
            return new Function(name, dimension.Clone());
        }

        public override string GetString() {
            return $"{name}({dimension.GetString()})";
        }
    }
}
