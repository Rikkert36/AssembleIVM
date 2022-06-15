using AssembleIVM.QueryParser.TreeNodes.Predicates.AlgebraicExpressions;
using AssembleIVM.T_reduct;
using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.TreeNodes.Predicates.AlgebraicExpressions {
    class DoubleToInt : AlgebraicExpression {
        public DoubleToInt(TreeNode left, TreeNode right) : base(left, right) {
        }

        public override TreeNode Clone() {
            return new DoubleToInt(this.left, this.right);
        }

        public override Number Compute(List<string> header, string[] values) {
            Number left = GetChildValue(this.left, header, values);
            double d = (double)left.value;
            int rounded = (int)Math.Round(d);
            return new Number(rounded);
        }

        public override string GetString() {
            return $"int({this.left.GetString()})";
        }
    }
}
