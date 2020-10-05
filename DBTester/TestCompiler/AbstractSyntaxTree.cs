using System;
using System.Collections.Generic;
using System.Linq;

namespace DBTester.TestCompiler
{
    public class AbstractSyntaxTree
    {
        IEnumerable<Token> Tokens { get; }

        Dictionary<string, object> Variables { get; }

        public AbstractSyntaxTree(IEnumerable<Token> tokens, Dictionary<string, object> variables)
        {
            Tokens = tokens;
            Variables = variables;
        }

        public object Calc()
        {
            return GetNode(Tokens.ToList()).Calc(Variables);
        }

        private Operator GetNode(List<Token> tokens)
        {
            if (tokens.Any(a => a.Type == TokenType.BooleanArithmetic))
            {
                return GetNode(tokens, TokenType.BooleanArithmetic);
            }

            if (tokens.Any(a => a.Type == TokenType.Comparison))
            {
                return GetNode(tokens, TokenType.Comparison);
            }

            if (tokens.Any(a => a.Type == TokenType.Arithmetic))
            {
                return GetNode(tokens, TokenType.Arithmetic);
            }

            if (tokens.Count == 1)
            {
                return new Operator<Token, Token>(tokens.First(), null, null);
            }

            throw new NotImplementedException();
        }
        private Operator GetNode(List<Token> tokens, TokenType type)
        {
            var index = tokens.FindIndex(a => a.Type == type);

            var left = tokens.Take(index).ToList();
            var right = tokens.Skip(index + 1).ToList();
            var middle = tokens.ElementAt(index);

            if (left.Count == 0 || right.Count == 0)
            {
                throw new NotImplementedException();
            }

            return new Operator<Operator, Operator>(GetNode(left), GetNode(right), middle);
        }
    }
}