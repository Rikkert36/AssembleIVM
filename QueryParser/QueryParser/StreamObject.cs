using System;
using System.Collections.Generic;
using System.Text;

namespace QueryParser.NewParser {
    class StreamObject {
        readonly string input;
        int pos = 0, line = 1, col = 0;
        public StreamObject(string input) {
            this.input = input;
        }
        public char Next() {
            char ch = input[pos++];
            if (ch.Equals("\n") || ch.Equals("\r")) {
                line++;
                col = 0;
            } else {
                col++;
            }
            return ch;
        }
        public char Peek() {
            if (pos >= input.Length) return ';';
            return input[pos];
        }
        public bool Eof() {
            return Peek().Equals(';');
        }
        public void Croak(string msg) {
            throw new Exception(msg + " (" + line + ":" + col + ")");
        }
    }
}
