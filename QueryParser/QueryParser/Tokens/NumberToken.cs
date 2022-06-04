using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Tokens {
    class NumberToken : Token {
        public override string Type => "number";

        public NumberToken(string value): base(value) { }
    }
}
