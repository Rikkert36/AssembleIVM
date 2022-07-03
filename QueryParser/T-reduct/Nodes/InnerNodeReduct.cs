using AssembleIVM.T_reduct;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AssembleIVM.T_reduct.Enumerators;
using System.Text;

namespace AssembleIVM {
    abstract class InnerNodeReduct : NodeReduct {
        public List<NodeReduct> children;
        public List<TreeNode> predicates;

        public InnerNodeReduct(string name, List<string> variables,
            List<NodeReduct> children, List<TreeNode> predicates, Enumerator enumerator, bool inFrontier, string orderDimension = "") :
            base(name, variables, enumerator, inFrontier, orderDimension) {
            this.children = children;
            this.predicates = predicates;
        }

        public abstract List<string> GetUnprojectHeader();

        public abstract void ComputeDelta(NodeReduct node);

        public virtual void ComputeDeltaInitial(NodeReduct node) {
            ComputeDelta(node);
        }

    }
}
