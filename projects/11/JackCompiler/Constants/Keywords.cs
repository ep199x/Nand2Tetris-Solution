using System.Collections.Generic;

namespace JackCompiler.Constants
{
    public class Keywords
    {
        public const string Class = "class";
        public const string Method = "method";
        public const string Function = "function";
        public const string Constructor = "constructor";
        public const string Int = "int";
        public const string Boolean = "boolean";
        public const string Char = "char";
        public const string Void = "void";
        public const string Var = "var";
        public const string Static = "static";
        public const string Field = "field";
        public const string Let = "let";
        public const string Do = "do";
        public const string If = "if";
        public const string Else = "else";
        public const string While = "while";
        public const string Return = "return";
        public const string True = "true";
        public const string False = "false";
        public const string Null = "null";
        public const string This = "this";

        public readonly static IEnumerable<string> Datatypes = new List<string>()
        {
            Void,
            Boolean,
            Int,
            Char
        };
    }
}
