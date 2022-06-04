using QueryParser.NewParser.Tokens;
using QueryParser.NewParser.Parsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.TreeNodes {
    class RelationParser: Parser {
        TreeNode relation;
        public override TreeNode Parse(Tokenizer tokenizer) {
            relation = ParseRelation(tokenizer);
            while (!tokenizer.Eof()) {
                Token current = tokenizer.Peek();
                if (current.Type.Equals("keyword")) {
                    string value = current.value;
                    if (value.Equals("thetajoin")) {
                        tokenizer.Next();
                        if (!tokenizer.Next().value.Equals("(")) throw new Exception("thetajoin should be followed by '('");
                        TreeNode predicate = new PredicateParser().Parse(tokenizer);
                        if (!tokenizer.Peek().value.Equals(")")) throw new Exception("thetajoin should be ended by ')'");
                        tokenizer.Next();
                        TreeNode right = ParseRelation(tokenizer);
                        TreeNode newrelation = new ThetaJoinRelation(relation, right, predicate);
                        relation = newrelation;
                    } else if (tokenizer.relOperators.IndexOf(" " + value + "") >= 0) {
                        tokenizer.Next();
                        TreeNode right = ParseRelation(tokenizer);
                        TreeNode newRelation = new CombinedRelation(relation, right, value);
                        relation = newRelation;
                    } else {
                        return relation;
                    }
                } else {
                    return relation;
                }
            }
            return relation;
        }

        private TreeNode ParseRelation(Tokenizer tokenizer) {
            Token current = tokenizer.Peek();
            if (current.value.Equals("(")) {
                tokenizer.Next();
                TreeNode groupRelation = new RelationParser().Parse(tokenizer);
                TreeNode node = new GroupRelation(groupRelation);
                if (!tokenizer.Peek().value.Equals(")")) throw new Exception("( not followed by )");
                tokenizer.Next();
                if (tokenizer.Peek().value.Equals("as")) {
                    tokenizer.Next();
                    Token CName = tokenizer.Next();
                    if (!CName.Type.Equals("identifier")) return null;
                    return new RenameRelation(node, CName.value);
                } else {
                    return node;
                }
            } else if (current.Type.Equals("keyword")) {
                return new DerivedRelationParser().Parse(tokenizer);
            } else if (current.Type.Equals("identifier")) {
                tokenizer.Next();
                return new SimpRelation(current.value);
            } else {
                throw new Exception("something wrong with" + current);
            }
        }
    }
}
