using AssembleIVM.QueryParser.TreeNodes.Terminals;
using QueryParser.NewParser.Tokens;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Parsers {
    class VariableParser : Parser {
        string relName = "";
        public override TreeNode Parse(Tokenizer tokenizer) {
            Token current = tokenizer.Peek();
            if (current.Type.Equals("number")) {
                tokenizer.Next();
                return new NumberNode(current.value, true);
            } else if (current.Type.Equals("keyword")) {
                return new FunctionParser().Parse(tokenizer);
            } else if (current.Type.Equals("string")) {
                tokenizer.Next();
                return new StringNode(current.value);
            } else {
                string dimName = tokenizer.Next().value;
                if(tokenizer.Peek().value.Equals(".")) {
                    bool firstDot = true;
                    while(tokenizer.Peek().value.Equals(".")) {
                        if (!firstDot) {
                            relName += ".";
                        }
                        firstDot = false;
                        relName += dimName;
                        tokenizer.Next();
                        dimName = tokenizer.Next().value;
                    }
                    return new RelationAttribute(relName, dimName);
                } else {
                    return new DimensionName(dimName);
                }
            }
        }
    }
}
