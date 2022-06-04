using QueryParser.NewParser.Parsers;
using QueryParser.NewParser.Tokens;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser {
    class ComparisonParser : Parser {
        TreeNode comparison;
        public override TreeNode Parse(Tokenizer tokenizer) {
            comparison = new AlgebraicExpressionParser().Parse(tokenizer);
            if (!tokenizer.Eof()) {
                Token current = tokenizer.Peek();
                string comparisonOperators = " < <= >= > == != ";
                if (comparisonOperators.IndexOf(" " + current.value + " ") >= 0) {
                    tokenizer.Next();
                    TreeNode right = new AlgebraicExpressionParser().Parse(tokenizer);
                    TreeNode newComparison = new Comparison(comparison, right, current.value);
                    return newComparison;
                } else {
                    return comparison;
                }
            }
            return comparison;
        }

    }
}
