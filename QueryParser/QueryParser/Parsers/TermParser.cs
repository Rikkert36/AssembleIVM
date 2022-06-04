using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.Parsers;

using System;
using System.Collections.Generic;
using System.Text;
using QueryParser.NewParser.Tokens;
using QueryParser.NewParser.TreeNodes.Predicates;

namespace QueryParser.NewParser {
    class TermParser : Parser {
        TreeNode term;
        public override TreeNode Parse(Tokenizer tokenizer) {
            term = new FactorParser().Parse(tokenizer);
            while(!tokenizer.Eof()) {
                Token current = tokenizer.Peek();
                string operators = " * / ";
                if (operators.IndexOf(current.value) >= 0) {
                    tokenizer.Next();
                    TreeNode right = new FactorParser().Parse(tokenizer);
                    TreeNode newTerm = new Factor(term, right, current.value);
                    term = newTerm;
                } else {
                    return term;
                }
            }
            return term;
        }
    }
}
