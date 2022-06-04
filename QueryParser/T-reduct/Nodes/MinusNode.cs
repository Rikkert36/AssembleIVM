using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct.Nodes {
    class MinusNode : InnerNodeReduct {
        public MinusNode(string name, string[] variables, List<NodeReduct> children, List<TreeNode> predicates) : base(name, variables, children, predicates) {
        }

        public override void ComputeDelta(NodeReduct node) {
            
        }

        protected override void RemoveTuple(GMRTuple tuple) {
            List<GMRTuple> section = index.Get(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            t.count -= tuple.count;
/*            if (t.count < 1) {
                section.Remove(t);
                if (section.Count == 0) index.RemoveKey(tuple.fields);
            }*/
        }
    }
}
