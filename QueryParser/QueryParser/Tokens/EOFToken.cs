using QueryParser.NewParser.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssembleIVM.QueryParser.Tokens {
    class EOFToken : Token {
        public EOFToken(string value) : base(value) {
        }

        public override string Type => "eof";
    }
}
