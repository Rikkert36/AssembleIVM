using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Tokens {
    class StringToken : Token {
        public StringToken(string value) : base(value) {
        }

        public override string Type => "string";
    }
}
