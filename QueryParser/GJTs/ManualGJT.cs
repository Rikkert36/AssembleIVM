using QueryParser;
using QueryParser.GJTComputerFiles;
using QueryParser.GJTComputerFiles.ConstructorStructures;
using QueryParser.NewParser;
using QueryParser.NewParser.TreeNodes;
using QueryParser.NewParser.TreeNodes.Predicates;
using QueryParser.NewParser.TreeNodes.Terminals;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.GJTs {
    abstract class ManualGJT {
        abstract public string GetName();
        abstract public GeneralJoinTree Construct();

        protected TreeNode ParsePredicate(string s) {
            return new PredicateParser().Parse(new Tokenizer(new StreamObject(s)));
        }
    }
}
