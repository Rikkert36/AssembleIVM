﻿using QueryParser.NewParser.Tokens;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Parsers {
    class FactorParser : Parser {
        public override TreeNode Parse(Tokenizer tokenizer) {
            Token current = tokenizer.Peek();
            if (current.value.Equals("(")) {
                tokenizer.Next();
                TreeNode groupContent = new PredicateParser().Parse(tokenizer);
                if (!tokenizer.Peek().value.Equals(")")) throw new Exception("( not followed by )");
                tokenizer.Next();
                return groupContent;
            } else {
                TreeNode variable = new VariableParser().Parse(tokenizer);
                return variable;
            }
        }
    }
}
