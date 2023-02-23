using JackCompiler.Constants;

namespace JackCompiler.Models
{
    internal readonly struct Symbol
    {
        public readonly string Type;
        public readonly VariableKind Kind;
        public readonly int Index;

        public Symbol(string type, VariableKind kind, int index)
        {
            Type = type;
            Kind = kind;
            Index = index;
        }
    }
}
