using AssembleIVM.QueryParser.TreeNodes.Predicates.AlgebraicExpressions;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.T_reduct {
    class Bound {
        public bool isTight;
        public TreeNode node;
        public Bound(TreeNode node, bool isTight) {
            this.isTight = isTight;
            this.node = node;
        }

        public bool ViolatesTuple(GMRTuple tuple, List<string> tupleHeader, string dimension, 
            bool lower) {
            Number bound = node.ComputeValue(tupleHeader, tuple.fields);
            int index = tupleHeader.IndexOf(dimension);
            if (lower && isTight) {
                return double.Parse(tuple.fields[index]) > bound.value;
            } else if (lower && !isTight) {
                return double.Parse(tuple.fields[index]) >= bound.value;
            } else if (!lower && isTight) {
                return double.Parse(tuple.fields[index]) < bound.value;
            } else {
                return double.Parse(tuple.fields[index]) <= bound.value;
            }
        }
        
    }
}
