using AssembleIVM.T_reduct;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.QueryParser.TreeNodes.Predicates.AlgebraicExpressions {
    abstract class AlgebraicExpression : TreeNode {

        public TreeNode left;
        public TreeNode right;
        public AlgebraicExpression(TreeNode left, TreeNode right) {
            this.left = left;
            this.right = right;
        }

        abstract public Number Compute(List<string> header, string[] values);
        public void FillInChildren(List<string> header, string[] values) {
            if (left.GetType().Name.Equals("DimensionName") || left.GetType().Name.Equals("RelationAttribute")) {
                NumberNode numberNode = left.TryGetNumberNode(header, values);
                if (numberNode != null) left = numberNode;
            } else if (left.GetType().Name.Equals("Term") || left.GetType().Name.Equals("Factor")) {
                AlgebraicExpression algebraicExpression = (AlgebraicExpression)left;
                algebraicExpression.FillInChildren(header, values);
            }
            if (right.GetType().Name.Equals("DimensionName") || right.GetType().Name.Equals("RelationAttribute")) {
                NumberNode numberNode = right.TryGetNumberNode(header, values);
                if (numberNode != null) right = numberNode;
            } else if (right.GetType().Name.Equals("Term") || right.GetType().Name.Equals("Factor")) {
                AlgebraicExpression algebraicExpression = (AlgebraicExpression)right;
                algebraicExpression.FillInChildren(header, values);
            }
        }

        protected Number GetChildValue(TreeNode child, List<string> header, string[] values) {
            if (child.GetType().Name.Equals("DimensionName") ||
                child.GetType().Name.Equals("RelationAttribute")) {
                return new Number(child.FindValue(header, values));
            } else if (child.GetType().Name.Equals("Number")) {
                return new Number(child.GetString());
            } else {
                AlgebraicExpression algebraicExpession = (AlgebraicExpression)child;
                return algebraicExpession.Compute(header, values);
            }
        }
       
    }
}
