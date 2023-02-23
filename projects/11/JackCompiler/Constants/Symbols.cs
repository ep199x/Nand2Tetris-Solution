using System.Collections.Generic;

namespace JackCompiler.Constants
{
    public static class Symbols
    {
        public const char OpeningBrace = '(';
        public const char ClosingBrace = ')';
        public const char OpeningCurlyBrace = '{';
        public const char ClosingCurlyBrace = '}';
        public const char OpeningSquareBracket = '[';
        public const char ClosingSquareBracket = ']';
        public const char Dot = '.';
        public const char Comma = ',';
        public const char Semicolon = ';';
        public const char Plus = '+';
        public const char Minus = '-';
        public const char Star = '*';
        public const char Slash = '/';
        public const char Ampersand = '&';
        public const char Pipe = '|';
        public const char LessThan = '<';
        public const char GreaterThan = '>';
        public const char Equal = '=';
        public const char Tilde = '~';

        public readonly static Dictionary<char, string> BinaryOperations = new Dictionary<char, string>()
        {
            [Plus] = "add",
            [Minus] = "sub",
            [Star] = "call Math.multiply 2",
            [Slash] = "call Math.divide 2",
            [Ampersand] = "and",
            [Pipe] = "or",
            [LessThan] = "lt",
            [GreaterThan] = "gt",
            [Equal] = "eq"
        };

        public readonly static Dictionary<char, string> UnaryOperations = new Dictionary<char, string>()
        {
            [Tilde] = "not",
            [Minus] = "neg"
        };
    }
}
