using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Tokens {
    class ThetaJoinToken : Token {
        public ThetaJoinToken(string value) : base(value) {
        }

        public override string Type => "thetajoin";
    }
}
