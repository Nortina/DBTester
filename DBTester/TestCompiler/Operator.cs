using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace DBTester.TestCompiler
{
    public abstract class Operator
    {
        [JsonPropertyName("a")]
        public object Left { get; }

        [JsonPropertyName("b")]
        public object Right { get; }

        [JsonPropertyName("m")]
        public Token Middle { get; }

        public Operator(object left, object right, Token middle)
        {
            Left = left;
            Right = right;
            Middle = middle;
        }

        public abstract object Calc(Dictionary<string, object> variables);
    }

    public class Operator<T, T2> : Operator
    {
        public new T Left { get => (T)base.Left; }

        public new T2 Right { get => (T2)base.Right; }

        public Operator(T left, T2 right, Token middle) : base(left, right, middle)
        {
        }

        public override object Calc(Dictionary<string, object> variables)
        {
            if (Middle == null)
            {
                return GetValue(Left, variables);
            }

            switch (Middle.Type)
            {
                case TokenType.BooleanArithmetic:
                    return CalcBooleanArithmetic(variables);

                case TokenType.Comparison:
                    return CalcComparison(variables);

                case TokenType.Arithmetic:
                    return CalcArithmetic(variables);
            }

            throw new NotImplementedException();
        }

        public bool CalcComparison(Dictionary<string, object> variables)
        {
            var leftValue = GetValue(Left, variables);
            var rightValue = GetValue(Right, variables);

            switch (Middle.Value)
            {
                case "==":
                    return leftValue.Equals(rightValue);

                case "!=":
                    return !leftValue.Equals(rightValue);

                case ">":
                    return leftValue.GetHashCode() > rightValue.GetHashCode();

                case "<":
                    return leftValue.GetHashCode() < rightValue.GetHashCode();

                case ">=":
                    return leftValue.GetHashCode() >= rightValue.GetHashCode();

                case "<=":
                    return leftValue.GetHashCode() <= rightValue.GetHashCode();
            }

            throw new NotImplementedException();
        }

        public object CalcBooleanArithmetic(Dictionary<string, object> variables)
        {
            switch (Middle.Value)
            {
                case "and":
                case "or":
                    {
                        var leftValue = GetValue(Left, variables);
                        if (leftValue is Decimal)
                        {
                            leftValue = (Decimal)leftValue != 0;
                        }

                        switch (Middle.Value)
                        {
                            case "and":
                                if ((bool)leftValue == false)
                                {
                                    return false;
                                }
                                break;

                            case "or":
                                if ((bool)leftValue == true)
                                {
                                    return true;
                                }
                                break;
                        }

                        var rightValue = GetValue(Right, variables);
                        if (rightValue is Decimal)
                        {
                            rightValue = (Decimal)rightValue != 0;
                        }

                        switch (Middle.Value)
                        {
                            case "and":
                                return (bool)leftValue && (bool)rightValue;

                            case "or":
                                return (bool)leftValue || (bool)rightValue;
                        }
                    }
                    break;
            }

            throw new NotImplementedException();
        }

        public object CalcArithmetic(Dictionary<string, object> variables)
        {
            switch (Middle.Value)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                case "%":
                    {
                        var leftValue = GetValue(Left, variables);
                        var rightValue = GetValue(Right, variables);

                        if (leftValue is Decimal leftNumber && rightValue is Decimal rightNumber)
                        {
                            switch (Middle.Value)
                            {
                                case "+":
                                    return leftNumber + rightNumber;

                                case "-":
                                    return leftNumber + rightNumber;

                                case "*":
                                    return leftNumber * rightNumber;

                                case "/":
                                    return leftNumber / rightNumber;

                                case "%":
                                    return leftNumber % rightNumber;
                            }
                        }
                    }
                    break;
            }

            throw new NotImplementedException();
        }

        public object GetValue(object target, Dictionary<string, object> variables)
        {
            if (target is Token token)
            {
                return GetTokenValue(token, variables);
            }

            if (target is Operator op)
            {
                return op.Calc(variables);
            }

            throw new NotImplementedException();
        }

        public object GetTokenValue(Token token, Dictionary<string, object> variables)
        {
            switch (token.Type)
            {
                case TokenType.Null:
                case TokenType.Boolean:
                case TokenType.Number:
                case TokenType.String:
                    return token.Value;

                case TokenType.Variable:
                    var names = (token.Value as string).Split(".");
                    var name = names[0];

                    if (variables.ContainsKey(name))
                    {
                        var value = variables[name];

                        while (names.Length > 1)
                        {
                            names = names.Skip(1).ToArray();
                            var currentName = names[0];

                            switch (currentName)
                            {
                                case "length":
                                    if (value is string stringValue)
                                    {
                                        value = (Decimal)stringValue.Length;
                                        break;
                                    }
                                    throw new NotImplementedException();
                            }
                        }

                        return value;
                    }

                    throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
    }
}