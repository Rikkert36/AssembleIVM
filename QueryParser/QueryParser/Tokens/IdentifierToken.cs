using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Tokens {
    class IdentifierToken : Token {
        public IdentifierToken(string value) : base(value) {
        }

        public override string Type => "identifier";
    }
}
