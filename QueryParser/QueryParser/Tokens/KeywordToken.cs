using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Tokens {
    class KeywordToken : Token {
        public KeywordToken(string value) : base(value) {
        }

        public override string Type => "keyword";
    }
}
