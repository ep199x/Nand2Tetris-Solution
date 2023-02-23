using JackCompiler.Constants;

namespace JackCompiler.Models
{
    internal readonly struct Token
    {
        public readonly TokenType Type { get; }
        public readonly string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
