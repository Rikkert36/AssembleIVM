using QueryParser.NewParser.Tokens;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Relations;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Parsers {
    class DerivedRelationParser : Parser {
        TreeNode projectVars;
        TreeNode selectPredicate;
        public override TreeNode Parse(Tokenizer tokenizer) {
            Token current = tokenizer.Peek();
            if(current.value.Equals("project")) {
                tokenizer.Next();
                projectVars = new ProjectParser().Parse(tokenizer);
                current = tokenizer.Peek();
                if(current.value.Equals("select")) {
                    tokenizer.Next();
                    selectPredicate = new PredicateParser().Parse(tokenizer);
                    current = tokenizer.Peek();
                }
            } else {
                if (!current.value.Equals("select")) throw new Exception("A derived relation should either have a project or select");
                tokenizer.Next();
                selectPredicate = new PredicateParser().Parse(tokenizer);
                current = tokenizer.Peek();
            }
            if (current.value != "from") throw new Exception("A derived relation should have a from");
            tokenizer.Next();
            current = tokenizer.Peek();
            if (current.value != "(") throw new Exception("The from should be followed by a '(' ')' body");
            tokenizer.Next();
            TreeNode fromRelation = new RelationParser().Parse(tokenizer);
            current = tokenizer.Peek();
            if (current.value != ")") throw new Exception("The from should be ended by a ')'");
            tokenizer.Next();

            if(projectVars == null) {
                return new SelectOnlyDerivedRelation(selectPredicate, fromRelation);
            } else if (selectPredicate == null) {
                return new ProjectOnlyDerivedRelation(projectVars, fromRelation);
            }
            return new DerivedRelation(projectVars, selectPredicate, fromRelation);

        }

    }
}
