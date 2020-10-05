using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DBTester.TestCompiler
{
    public class Lexer
    {
        public string Code { get; }

        public Lexer(string code)
        {
            Code = code;
        }

        public IEnumerable<Token> Lex()
        {
            var tokens = new List<Token>();
            var code = Code.Trim();

            while (!string.IsNullOrEmpty(code))
            {
                var beforeLength = code.Length;
                tokens.Add(Next(ref code));

                var afterLength = code.Length;
                if (beforeLength == afterLength)
                {
                    throw new Exception("Lexing failed.");
                }

                code = code.Trim();
            }

            return tokens;
        }

        private Token Next(ref string code)
        {
            var letter = code[0];

            switch (letter)
            {
                case '\'':
                    return LexString(ref code);

                case '"':
                    return LexString(ref code);

                case '<':
                case '>':
                    return LexComparison(ref code);

                default:
                    var regex = new Regex($"\\S+");
                    var match = regex.Match(code);
                    var value = match?.Groups[0]?.Value;

                    switch (value)
                    {
                        case ">=":
                        case "<=":
                        case "==":
                        case "!=":
                            return LexComparison(ref code);

                        case "-":
                            if (value.Length == 1)
                            {
                                return LexArithmetic(ref code);
                            }
                            break;

                        case "+":
                            return LexArithmetic(ref code);

                        case "and":
                        case "or":
                            return LexBooleanArithmetic(ref code);

                        case "null":
                            return LexNull(ref code);

                        default:
                            break;
                    }
                    break;
            }

            if (letter >= '0' && letter <= '9')
            {
                var token = LexNumber(ref code);
                if (token != null)
                {
                    return token;
                }
            }

            return LexVariable(ref code);
        }

        private Token LexString(ref string code)
        {
            var letter = code[0];
            var first = letter;
            var regex = new Regex($"^{first}([^{first}]+){first}");
            var match = regex.Match(code);
            var value = match?.Groups[1]?.Value;

            code = code.Substring(match.Index + match.Length);

            return new Token<string>(TokenType.String, value);
        }

        private Token LexVariable(ref string code)
        {
            var regex = new Regex($"[\\w.]+");
            var match = regex.Match(code);
            var value = match?.Groups[0]?.Value;

            code = code.Substring(match.Index + match.Length);

            switch (value)
            {
                case "true":
                    return new Token<bool>(TokenType.Boolean, true);

                case "false":
                    return new Token<bool>(TokenType.Boolean, false);
            }

            return new Token<string>(TokenType.Variable, value);
        }

        private Token LexArithmetic(ref string code)
        {
            var regex = new Regex($"\\S+");
            var match = regex.Match(code);
            var value = match?.Groups[0]?.Value;

            code = code.Substring(match.Index + match.Length);

            return new Token<string>(TokenType.Arithmetic, value);
        }

        private Token LexBooleanArithmetic(ref string code)
        {
            var regex = new Regex($"\\S+");
            var match = regex.Match(code);
            var value = match?.Groups[0]?.Value;

            code = code.Substring(match.Index + match.Length);

            return new Token<string>(TokenType.BooleanArithmetic, value);
        }

        private Token LexComparison(ref string code)
        {
            var regex = new Regex($"\\S+");
            var match = regex.Match(code);
            var value = match?.Groups[0]?.Value;

            code = code.Substring(match.Index + match.Length);

            return new Token<string>(TokenType.Comparison, value);
        }

        private Token LexNull(ref string code)
        {
            var regex = new Regex($"\\w+");
            var match = regex.Match(code);
            var value = match?.Groups[0]?.Value;

            code = code.Substring(match.Index + match.Length);

            return new Token<string>(TokenType.Null, null);
        }

        private Token LexNumber(ref string code)
        {
            var wordRegex = new Regex($"[\\w.]+");
            var wordMatch = wordRegex.Match(code);
            var wordValue = wordMatch?.Groups[0]?.Value;

            var numberRegex = new Regex($"[\\d]+(\\.?[\\d]+)?");
            var numberMatch = numberRegex.Match(code);
            var numberValue = numberMatch?.Groups[0]?.Value;

            if (wordValue != numberValue)
            {
                return LexVariable(ref code);
            }

            Decimal number;
            if (Decimal.TryParse(numberValue, out number))
            {
                code = code.Substring(numberMatch.Index + numberMatch.Length);

                return new Token<Decimal>(TokenType.Number, number);
            }

            return null;
        }
    }
}