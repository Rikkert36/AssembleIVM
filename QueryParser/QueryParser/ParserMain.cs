using QueryParser.NewParser.TreeNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser {
    class ParserMain {
        string input;

        public TreeNode Parse(string queryString) {
       
            input = ReadQuery(queryString);
            Tokenizer tokenizer = new Tokenizer(new StreamObject(input));
            return new RelationParser().Parse(tokenizer);

        }

        private string ReadQuery(string fileName) {
            return System.IO.File.ReadAllText(@$"C:\Users\rmaas\Documents\Graduation project\code\QueryParser\QueryParser\queries\{fileName}.aql");
        }

    }

    
}
