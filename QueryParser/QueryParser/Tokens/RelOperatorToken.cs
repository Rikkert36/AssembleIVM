using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Tokens {
    class RelOperatorToken : Token {
        public RelOperatorToken(string value) : base(value) {
        }

        public override string Type => "relOperator";
    }
}
