using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Tokens {
    class ArithOpToken : Token {
        public ArithOpToken(string value) : base(value) {
        }

        public override string Type => "arithmetic operator";
    }
}
