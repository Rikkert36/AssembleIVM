using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.TreeNodes.Predicates {
    class CartesianProduct : TreeNode {
        public override TreeNode Clone() {
            return new CartesianProduct();
        }

        public override string GetString() {
            throw new NotImplementedException();
        }
    }
}
