using System.Collections.Generic;

namespace Constants
{
    /// <summary>
    /// Contains string constants for the various arithmetic operations and a
    /// dictionary mapping those operations to their symbolic representations.
    /// </summary>
    internal class VMArithmetic
    {
        public const string Add = "add";
        public const string And = "and";
        public const string Eq = "eq";
        public const string Gt = "gt";
        public const string Lt = "lt";
        public const string Neg = "neg";
        public const string Not = "not";
        public const string Or = "or";
        public const string Sub = "sub";

        public static Dictionary<string, string> Symbols = new Dictionary<string, string>
        {
            [Add] = "+",
            [And] = "&",
            [Eq] = "JNE",
            [Gt] = "JGE",
            [Lt] = "JLE",
            [Neg] = "-",
            [Not] = "!",
            [Or] = "|",
            [Sub] = "-"
        };
    }
}
