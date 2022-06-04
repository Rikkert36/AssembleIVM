using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Parsers {
    class FunctionParser : Parser {
        public override TreeNode Parse(Tokenizer tokenizer) {
            string functionName = tokenizer.Peek().value;
            tokenizer.Next();
            if (!tokenizer.Peek().value.Equals("(")) throw new Exception($"Function {functionName} not opened with '('");
            tokenizer.Next();
            //string dimension = tokenizer.Next().value;
            TreeNode dimension = new VariableParser().Parse(tokenizer);
            if (!tokenizer.Peek().value.Equals(")")) throw new Exception($"Function {functionName} not closed by ')'");
            tokenizer.Next();
            return new Function(functionName, dimension);
        }
    }
}
