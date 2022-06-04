using AssembleIVM;
using AssembleIVM.QueryParser.TreeNodes.Predicates.AlgebraicExpressions;
using AssembleIVM.T_reduct;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes {
    abstract class TreeNode {
        abstract public string GetString();

        public string FindValue(List<string> header, string[] values) {
            int i = header.IndexOf(GetString());
            if (i != -1) {
                return values[i];
            } else {
                return "valueNotInHeader";
            }
        }

        public NumberNode FillInNumbers(List<string> header, string[] values) {
            if (this.GetType().Name.Equals("DimensionName") || this.GetType().Name.Equals("RelationAttribute")) {
                NumberNode numberNode = this.TryGetNumberNode(header, values);
                if (numberNode != null) return numberNode;
            } else if (this.GetType().Name.Equals("Term") || this.GetType().Name.Equals("Factor")) {
                AlgebraicExpression algebraicExpression = (AlgebraicExpression)this;
                algebraicExpression.FillInChildren(header, values);
            }
            return null;
        }

        public Number ComputeValue(List<string> header, string[] values) {
            if (this.GetType().Name.Equals("DimensionName") ||
                this.GetType().Name.Equals("RelationAttribute")) {
                return new Number(FindValue(header, values));
            } else if (GetType().Name.Equals("Number")) {
                return new Number(GetString());
            } else {
                AlgebraicExpression algebraicExpession = (AlgebraicExpression)this;
                return algebraicExpession.Compute(header, values);
            }
        }

        public NumberNode TryGetNumberNode(List<string> header, string[] values) {
            string val = FindValue(header, values);
            if (!val.Equals("valueNotInHeader")) {
                return new NumberNode(val, !val.Substring(0, 1).Equals("-"));
            } else {
                return null;
            }
        }

        public abstract TreeNode Clone();

    }
}
