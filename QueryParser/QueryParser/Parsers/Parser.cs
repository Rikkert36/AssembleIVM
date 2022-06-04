using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Parsers {
    abstract class Parser {
        abstract public TreeNode Parse(Tokenizer tokenizer);
    }
}
