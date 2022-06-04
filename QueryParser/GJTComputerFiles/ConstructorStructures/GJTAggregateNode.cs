using AssembleIVM.T_reduct;
using AssembleIVM.T_reduct.Nodes;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTComputerFiles.ConstructorStructures {
    class GJTAggregateNode : GJTInnerNode {
        public GJTAggregateNode(string name, string[] variables, List<GJTNode> children, List<TreeNode> predicates) : base(name, variables, children, predicates) {
        }

        public override NodeReduct GenerateReduct(string modelName) {
            List<NodeReduct> reductChildren = new List<NodeReduct>();
            foreach (GJTNode child in children) {
                reductChildren.Add(child.GenerateReduct(modelName));

            }
            AggregateNode result = new AggregateNode(this.name, this.variables, reductChildren, this.predicates);
            foreach (NodeReduct child in result.children) {
                child.SetParent(result);
            }
            result.inFrontier = this.inFrontier;
            return result;
        }

    }
}

