using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using AssembleIVM.T_reduct.Enumerators;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;
using AssembleIVM.T_reduct;
using AssembleIVM.T_reduct.Nodes;

namespace AssembleIVM.GJTComputerFiles.ConstructorStructures.nodes {
    class GJTCountNode : GJTInnerNode {

        string aggregateFunction;
        string aggregateDimension;
        public GJTCountNode(string name, List<string> variables, List<GJTNode> children,
            List<TreeNode> predicates, Enumerator enumerator, string aggregateFunction, string aggregateDimension)
            : base(name, variables, children, predicates, enumerator) {
            this.aggregateFunction = aggregateFunction;
            this.aggregateDimension = aggregateDimension;
        }


        public override NodeReduct GenerateReduct(string modelName) {
            List<NodeReduct> reductChildren = new List<NodeReduct>();
            foreach (GJTNode child in children) {
                reductChildren.Add(child.GenerateReduct(modelName));

            }
            CountNode result = new CountNode(this.name, this.variables, reductChildren, this.predicates,
                this.enumerator, this.inFrontier,this.aggregateDimension);
            enumerator.rho = result;
            foreach (NodeReduct child in result.children) {
                child.SetParent(result);
            }
            result.inFrontier = this.inFrontier;
            return result;
        }
    }
}
