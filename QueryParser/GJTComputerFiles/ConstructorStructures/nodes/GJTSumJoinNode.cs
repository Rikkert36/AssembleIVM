using AssembleIVM.T_reduct;
using AssembleIVM.T_reduct.Enumerators;
using AssembleIVM.T_reduct.Nodes;
using QueryParser.GJTComputerFiles;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTComputerFiles.ConstructorStructures {
    class GJTSumJoinNode : GJTJoinNode {
        public string aggregateFunction;
        public string aggregateDimension;
        public GJTSumJoinNode(string name, List<string> variables, List<GJTNode> children, List<TreeNode> predicates, 
            Enumerator enumerator, string aggregateFunction, string aggregateDimension, string orderDimension = "") : 
            base(name, variables, children, predicates, enumerator, orderDimension) {
            this.aggregateFunction = aggregateFunction;
            this.aggregateDimension = aggregateDimension;
        }            

        public override NodeReduct GenerateReduct(string modelName) {
            List<NodeReduct> reductChildren = new List<NodeReduct>();
            foreach (GJTNode child in children) {
                reductChildren.Add(child.GenerateReduct(modelName));

            }
            AggregateJoinNode result = new AggregateJoinNode(this.name, this.variables, reductChildren, this.predicates, 
                this.enumerator, this.inFrontier, this.aggregateFunction, this.aggregateDimension, this.orderDimension);
            enumerator.rho = result;
            foreach (NodeReduct child in result.children) {
                child.SetParent(result);
            }
            result.inFrontier = this.inFrontier;
            return result;
        }
    }
}
