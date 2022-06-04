using QueryParser.NewParser.Tokens;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Parsers {
    class ProjectParser : Parser {
        public override TreeNode Parse(Tokenizer tokenizer) {
            List<TreeNode> projectVars = new List<TreeNode>();
            projectVars.Add(ParseProjectVar(tokenizer));
            while(!(tokenizer.Eof())) {
                Token current = tokenizer.Peek();
                if(current.value.Equals(",")) {
                    tokenizer.Next();
                    projectVars.Add(ParseProjectVar(tokenizer));
                } else {
                    return new ProjectVars(projectVars);
                }
            }
            return new ProjectVars(projectVars);
        }

        private TreeNode ParseProjectVar(Tokenizer tokenizer) {
            TreeNode nameOrExpr = new PredicateParser().Parse(tokenizer);
            if (tokenizer.Peek().value.Equals("as")) {
                tokenizer.Next();
                TreeNode newName = new VariableParser().Parse(tokenizer);
                return new DerivedProjectVar(newName, nameOrExpr);
            } else {
                return new ExistingProjectVar(nameOrExpr);
            }

        }
    }
}
