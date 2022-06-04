using AssembleIVM.T_reduct;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AssembleIVM {
    abstract class InnerNodeReduct : NodeReduct {
        public List<NodeReduct> children;
        public List<TreeNode> predicates;

        public InnerNodeReduct(string name, string[] variables,
            List<NodeReduct> children, List<TreeNode> predicates) : base(name, variables) {
            this.children = children;
            this.predicates = predicates;
        }

        public abstract void ComputeDelta(NodeReduct node);

    }
}
