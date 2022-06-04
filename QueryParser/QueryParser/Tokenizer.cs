using QueryParser.NewParser.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace QueryParser.NewParser {
    class Tokenizer {
        readonly StreamObject input;
        Token current;
        public readonly string relOperators = " natjoin thetajoin carpro intersect union minus ";
        readonly string keywords = " project select from natjoin thetajoin carpro intersect union minus and or as sum count min max average ";
        
        public Tokenizer(StreamObject input) {
            this.input = input;
        }
        private bool Check(string regex, char c) {
            return new Regex(regex).IsMatch(c.ToString());
        }
        private bool IsKeyword(string x) {
            return keywords.IndexOf(" " + x + " ") >= 0;
        }
        private bool IsDigit(char c) {
            return Char.IsDigit(c);
        }
        private bool IsIdStart(char c) {
            return Char.IsLetter(c);
        }
        private bool IsID(char c) {
            return IsIdStart(c) || "-_'".IndexOf(c) >= 0 || IsDigit(c);
        }
        private bool IsArOp(char c) {
            return "+-/*".IndexOf(c) >= 0;
        }
        private bool IsPunc(char c) {
            return ",;().".IndexOf(c) >= 0;
        }
        private bool IsComp(char c) {
            return "<>=!".IndexOf(c) >= 0;
        }
        private bool IsWS(char c) {
            return " \t\n\r".IndexOf(c) >= 0;
        }
        private string ReadWhile(Func<char, bool> predicate) {
            string result = "";
            while(!input.Eof() && predicate(input.Peek())) {
                result += input.Next();
            }
            return result;
        }
        public NumberToken ReadNumber() {
            bool HasDot = false;
            string number = ReadWhile((char c) => {
                if (c.Equals(".")) {
                    if (HasDot) {
                        return false;
                    } else {
                        HasDot = true;
                        return true;
                    }
                }
                return IsDigit(c);
            });
            return new NumberToken(number);
        }
        public Token ReadIDToken() {
            string ID = ReadWhile(IsID);
            if (IsKeyword(ID)) {
                return new KeywordToken(ID);
            }
            return new IdentifierToken(ID);
        }
        public string ReadEscaped(char end) {
            string result = "";
            while (!input.Eof()) {
                char c = input.Next();
                if (c.Equals(end)) {
                    break;
                } else {
                    result += c;
                }
            }
            return result;
        }
        public StringToken ReadString() {
            input.Next();
            return new StringToken(ReadEscaped('"'));
        }

        public Token ReadNext() {
            ReadWhile(IsWS);
            char c = input.Peek();
            if (input.Eof()) return null;
            if (c.Equals('"')) return ReadString();
            if (IsDigit(c)) return ReadNumber();
            if (IsIdStart(c)) return ReadIDToken();
            if (IsPunc(c)) return new PuncToken(input.Next().ToString());
            if (IsArOp(c)) return new ArithOpToken(ReadWhile(IsArOp));
            if (IsComp(c)) return new CompToken(ReadWhile(IsComp));
            input.Croak("Can't handle character: " + c);
            return null;
        }
        public Token Peek() {
            if (current == null) {
                current = ReadNext();
            }
            return current;
        }
        public Token Next() {
            Token token = current;
            current = null;
            if (token == null) {
                return ReadNext();
            }
            return token;
        }
        public bool Eof() {
            return Peek() == null;
        }
    }
}
