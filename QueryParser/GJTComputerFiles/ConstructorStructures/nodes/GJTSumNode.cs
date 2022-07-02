using AssembleIVM.T_reduct;
using AssembleIVM.T_reduct.Nodes;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTComputerFiles.ConstructorStructures {
    class GJTSumNode : GJTInnerNode {
        string aggregateDimension;
        public GJTSumNode(string name, List<string> variables, List<GJTNode> children, 
            List<TreeNode> predicates, Enumerator enumerator, string aggregateDimension) 
            : base(name, variables, children, predicates, enumerator) {
            this.aggregateDimension = aggregateDimension;
        }
    

        public override NodeReduct GenerateReduct(string modelName) {
            List<NodeReduct> reductChildren = new List<NodeReduct>();
            foreach (GJTNode child in children) {
                reductChildren.Add(child.GenerateReduct(modelName));

            }
            SumNode result = new SumNode(this.name, this.variables, reductChildren, this.predicates, 
                this.enumerator, this.inFrontier, this.aggregateDimension);
            enumerator.rho = result;
            foreach (NodeReduct child in result.children) {
                child.SetParent(result);
            }
            result.inFrontier = this.inFrontier;
            return result;
        }

    }
}

