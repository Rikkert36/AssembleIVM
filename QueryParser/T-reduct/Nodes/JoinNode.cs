using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using AssembleIVM.T_reduct.Enumerators;

using System.Text;

namespace AssembleIVM.T_reduct {
    abstract class JoinNode : InnerNodeReduct {
        public JoinNode(string name, List<string> variables, List<NodeReduct> children, List<TreeNode> predicates,
            Enumerator enumerator, bool inFrontier, string orderDimension = "") : base(name, variables, children, predicates, enumerator, inFrontier, orderDimension) {
        }

        

        public override List<string> GetUnprojectHeader() {
            if (predicates[0] == null) {
                return new List<string>(children[0].variables);
            } else {
                return new List<string>(children[1].variables);
            }
        }

        protected override void RemoveTuple(GMRTuple tuple) {
            List<GMRTuple> section = index.Get(tuple.fields);
            GMRTuple t = index.FindTuple(tuple, section);
            t.count -= tuple.count;
            if (t.count < 1) {
                section.Remove(t);
                if (section.Count == 0) index.RemoveKey(tuple.fields);
            }
        }
    }
}
