using QueryParser.NewParser.Parsers;
using QueryParser.NewParser.Tokens;
using QueryParser.NewParser.TreeNodes.Predicates;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes {
    class AlgebraicExpressionParser : Parser {
        TreeNode algebraicExpression;
        public override TreeNode Parse(Tokenizer tokenizer) {
            algebraicExpression = new TermParser().Parse(tokenizer);
            while (!tokenizer.Eof()) {
                Token current = tokenizer.Peek();
                string operators = " + - ";
                if(operators.IndexOf(current.value) >= 0 ) {
                    tokenizer.Next();
                    TreeNode right = new TermParser().Parse(tokenizer);
                    TreeNode newAlgebraicExpression = new Term(algebraicExpression, right, current.value);
                    algebraicExpression = newAlgebraicExpression;
                } else {
                    return algebraicExpression;
                }
            }
            return algebraicExpression;
        }
    }
}
