using AssembleIVM.T_reduct;
using AssembleIVM.T_reduct.Enumerators;
using AssembleIVM.T_reduct.Nodes;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTComputerFiles.ConstructorStructures.nodes {
    class GJTAntiJoinNode : GJTInnerNode {
        public GJTAntiJoinNode(string name, List<string> variables, List<GJTNode> children, List<TreeNode> predicates, Enumerator enumerator, string orderDimension = "") : base(name, variables, children, predicates, enumerator, orderDimension) {
        }

        public override NodeReduct GenerateReduct(string modelName) {
            List<NodeReduct> reductChildren = new List<NodeReduct>();
            foreach (GJTNode child in children) {
                reductChildren.Add(child.GenerateReduct(modelName));

            }
            AntiJoinNode result = new AntiJoinNode(this.name, this.variables, reductChildren, this.predicates, this.enumerator, this.inFrontier);
            enumerator.rho = result;
            foreach (NodeReduct child in result.children) {
                child.SetParent(result);
            }
            result.inFrontier = this.inFrontier;
            return result;
        }
    }
}
