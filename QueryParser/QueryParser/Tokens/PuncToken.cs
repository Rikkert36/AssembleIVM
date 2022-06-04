using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Tokens {
    class PuncToken : Token {
        public PuncToken(string value) : base(value) {
        }

        public override string Type => "punctuation";
    }
}
