using JackCompiler.Constants;
using JackCompiler.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JackCompiler
{
    internal class SymbolTable
    {
        private readonly Dictionary<string, Symbol> _classSymbolTable;
        private readonly Dictionary<string, Symbol> _subroutineSymbolTable;

        public SymbolTable()
        {
            _classSymbolTable = new Dictionary<string, Symbol>();
            _subroutineSymbolTable = new Dictionary<string, Symbol>();
        }

        public void ResetSubroutineSymbolTable()
        {
            _subroutineSymbolTable.Clear();
        }

        public bool TryGetValue(string key, out Symbol symbol)
        {
            return (_classSymbolTable.TryGetValue(key, out symbol) ||
                _subroutineSymbolTable.TryGetValue(key, out symbol));
        }

        public void DefineStaticOrField(string name, string type, VariableKind kind)
        {
            if (kind != VariableKind.Static && kind != VariableKind.Field)
                throw new ArgumentException($"Invalid variable type. Static or field variable expected.", nameof(kind));

            _classSymbolTable.Add(name, new Symbol(type, kind, VariablesCount(kind)));
        }

        public void DefineLocal(string name, string type)
        {
            _subroutineSymbolTable.Add(name, new Symbol(type, VariableKind.Local, VariablesCount(VariableKind.Local)));
        }

        public void DefineArgument(string name, string type)
        {
            _subroutineSymbolTable.Add(name, new Symbol(type, VariableKind.Argument, VariablesCount(VariableKind.Argument)));
        }


        public int VariablesCount(VariableKind kind)
        {
            return _classSymbolTable.Values.Count(e => e.Kind == kind) +
                   _subroutineSymbolTable.Values.Count(e => e.Kind == kind);
        }

        public VariableKind KindOf(string name)
        {
            if (_subroutineSymbolTable.TryGetValue(name, out Symbol subroutineSymbol))
                return subroutineSymbol.Kind;
            else if (_classSymbolTable.TryGetValue(name, out Symbol classSymbol))
                return classSymbol.Kind;
            else
                throw new ArgumentException("Access to unknown variable.", nameof(name));
        }

        public string TypeOf(string name)
        {
            if (_subroutineSymbolTable.TryGetValue(name, out Symbol subroutineSymbol))
                return subroutineSymbol.Type;
            else if (_classSymbolTable.TryGetValue(name, out Symbol classSymbol))
                return classSymbol.Type;
            else
                throw new ArgumentException("Access to unknown variable.", nameof(name));
        }

        public int IndexOf(string name)
        {
            if (_subroutineSymbolTable.TryGetValue(name, out Symbol subroutineSymbol))
                return subroutineSymbol.Index;
            else if (_classSymbolTable.TryGetValue(name, out Symbol classSymbol))
                return classSymbol.Index;
            else
                throw new ArgumentException("Access to unknown variable.", nameof(name));
        }
    }
}
