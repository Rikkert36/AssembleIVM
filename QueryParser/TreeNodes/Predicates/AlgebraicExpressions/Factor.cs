using AssembleIVM.QueryParser.TreeNodes.Predicates.AlgebraicExpressions;
using AssembleIVM.T_reduct;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Predicates {
    class Factor : AlgebraicExpression {
        public string factorOperator;
        public Factor(TreeNode left, TreeNode right, string factorOperator): base(left, right) {
            this.factorOperator = factorOperator;
        }

        public override TreeNode Clone() {
            return new Factor(left.Clone(), right.Clone(), factorOperator);
        }

        public override Number Compute(List<string> header, string[] values) {
            Number left = GetChildValue(this.left, header, values);
            Number right = GetChildValue(this.right, header, values);
            if (factorOperator.Equals("*")) {
                return new Number(left.value * right.value);
            } else {
                return new Number(Math.Round((double)left.value / right.value, 2));
            }
        }

        public override string GetString() {
            return $"({left.GetString()} {factorOperator} {right.GetString()})";
        }
    }
}
