using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser.Tokens {
    abstract class Token {
        public readonly dynamic value;
        abstract public string Type { get; }

        public Token(dynamic value) {
            this.value = value;
        }

    }
}
