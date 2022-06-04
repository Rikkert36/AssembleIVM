using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Tokens {
    class CompToken : Token {
        public CompToken(string value) : base(value) {
        }

        public override string Type => "comperaror";
    }
}
