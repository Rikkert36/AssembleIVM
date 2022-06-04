using AssembleIVM.QueryParser.TreeNodes.Predicates.AlgebraicExpressions;
using AssembleIVM.T_reduct;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes.Predicates {
    class Term : AlgebraicExpression {
        public string termOperator;
        public Term(TreeNode left, TreeNode right, string termOperator): base(left, right) {
            
            this.termOperator = termOperator;
        }

        public override TreeNode Clone() {
            return new Term(left.Clone(), right.Clone(), termOperator); 
        }

        public override Number Compute(List<string> header, string[] values) {
            Number left = GetChildValue(this.left, header, values);
            Number right = GetChildValue(this.right, header, values);
            if (termOperator.Equals("+")) {
                return new Number(left.value + right.value);
            } else {
                return new Number(left.value - right.value);
            }
        }

        public override string GetString() {
            return $"({left.GetString()} {termOperator} {right.GetString()})";
        }
    }
}
