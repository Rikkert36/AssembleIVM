using QueryParser.NewParser;
using QueryParser.NewParser.Tokens;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using QueryParser.NewParser.Parsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser {
    class PredicateParser : Parser {
        TreeNode predicate;
        public override TreeNode Parse(Tokenizer tokenizer) {
            predicate = new ComparisonParser().Parse(tokenizer);
            while (!tokenizer.Eof()) {
                Token current = tokenizer.Peek();
                if (current.Type.Equals("keyword")) {
                    string value = current.value;
                    if (value.Equals("or")) {
                        tokenizer.Next();
                        TreeNode right = new ComparisonParser().Parse(tokenizer);
                        TreeNode newPredicate = new Or(predicate, right);
                        predicate = newPredicate;
                    } else if (value.Equals("and")) {
                        tokenizer.Next();
                        TreeNode right = new ComparisonParser().Parse(tokenizer);
                        TreeNode newPredicate = new And(predicate, right);
                        predicate = newPredicate;
                    } else {
                        return predicate;
                    }
                } else {
                    return predicate;
                }
            }
            return predicate;
        }
    }
}
