using System.Text.Json.Serialization;

namespace DBTester.TestCompiler
{
    public class Token
    {
        [JsonPropertyName("v")]
        public object Value { get; }

        [JsonPropertyName("t")]
        public TokenType Type { get; }

        protected Token(TokenType type, object value)
        {
            Type = type;
            Value = value;
        }
    }

    public class Token<T> : Token
    {
        public new T Value { get => (T)base.Value; }

        public Token(TokenType type, T value) : base(type, value)
        {
        }
    }
}