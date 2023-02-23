using JackCompiler.Constants;
using JackCompiler.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VMTranslator;

namespace JackCompiler
{
    internal class CompilationEngine
    {
        private readonly JackTokenizer _tokenizer;
        private VMWriter _vmWriter;
        private SymbolTable _symbolTable;
        private string _className;
        private string _currentSubroutineName;
        private string _currentSubroutineType;
        private int _ifCount;
        private int _whileCount;

        private Dictionary<VariableKind, string> _segmentMap = new Dictionary<VariableKind, string>()
        {
            [VariableKind.Static] = Segments.Static,
            [VariableKind.Field] = Segments.This,
            [VariableKind.Argument] = Segments.Argument,
            [VariableKind.Local] = Segments.Local
        };

        /// <summary>
        /// Gets its input from a JackTokenizer and writes its output using the VMWriter.
        /// </summary>
        public CompilationEngine(string inputPath, string outputPath)
        {
            using StreamReader reader = new(inputPath);
            using StreamWriter writer = new(outputPath);

            _tokenizer = new(reader, inputPath);
            _vmWriter = new(writer);

            _symbolTable = new();

            CompileClass();

            reader.Close();
            writer.Flush();
            writer.Close();
        }

        #region Parsing Helper Methods
        private void TryAdvance()
        {
            if (!_tokenizer.HasMoreTokens())
                throw new Exception("No more tokens left to consume");

            _tokenizer.Advance();
        }

        private void ValidateTokenType(TokenType type)
        {
            if (_tokenizer.TokenType != type)
                throw new InvalidDataException($"Token of type '{type}' expected in class '{_className}' at line {_tokenizer.CurrentLineCount}.");
        }

        private void ValidateSymbol(char symbol = char.MinValue)
        {
            ValidateTokenType(TokenType.Symbol);

            if (symbol != char.MinValue && symbol != _tokenizer.SymbolValue)
                throw new Exception($"Symbol '{symbol}' expected in class '{_className}' at line {_tokenizer.CurrentLineCount}.");
        }

        private void ParseSymbol(char symbol = char.MinValue)
        {
            TryAdvance();
            ValidateSymbol(symbol);
        }

        private void ParseKeyword(string keyword = "")
        {
            TryAdvance();
            ValidateTokenType(TokenType.Keyword);

            if (keyword != string.Empty && keyword != _tokenizer.KeywordValue)
                throw new Exception($"Keyword '{keyword}' expected in class '{_className}' at line {_tokenizer.CurrentLineCount}.");
        }

        private void ParseIdentifier()
        {
            TryAdvance();
            ValidateTokenType(TokenType.Identifier);
        }

        private void ParseString()
        {
            TryAdvance();
            ValidateTokenType(TokenType.String);
        }

        private void ParseInteger()
        {
            TryAdvance();
            ValidateTokenType(TokenType.Int);
        }
        #endregion

        /// <summary>
        /// Compiles a complete Jack class.
        /// </summary>
        private void CompileClass()
        {
            ParseKeyword(Keywords.Class);
            ParseIdentifier();
            _className = _tokenizer.IdentifierValue;

            ParseSymbol(Symbols.OpeningCurlyBrace);

            TryAdvance(); // "{" -> "}" | "classVarDec keyword" | "subroutine keyword"

            while (_tokenizer.TokenType == TokenType.Keyword)
            {
                if (_tokenizer.KeywordValue == Keywords.Static
                    || _tokenizer.KeywordValue == Keywords.Field)
                    CompileClassVarDec();
                else if (_tokenizer.KeywordValue == Keywords.Method
                    || _tokenizer.KeywordValue == Keywords.Function
                    || _tokenizer.KeywordValue == Keywords.Constructor)
                    CompileSubroutine();
            }

            ValidateSymbol(Symbols.ClosingCurlyBrace);
        }

        private void CompileClassVarDec()
        {
            var kind = _tokenizer.KeywordValue == Keywords.Static ? VariableKind.Static : VariableKind.Field;
            TryAdvance(); // "field | static" -> "datatype"

            if (_tokenizer.TokenType != TokenType.Identifier
                    && _tokenizer.TokenType == TokenType.Keyword
                    && !Keywords.Datatypes.Contains(_tokenizer.KeywordValue))
                throw new Exception($"Datatype expected in line {_tokenizer.CurrentLineCount}.");

            var datatype = _tokenizer.CurrentTokenValue;
            ParseIdentifier();

            _symbolTable.DefineStaticOrField(_tokenizer.IdentifierValue, datatype, kind);

            ParseSymbol(); // "varName" -> "," | ";"

            while (_tokenizer.SymbolValue == Symbols.Comma)
            {
                ParseIdentifier(); // Parse next variable name of same datatype
                _symbolTable.DefineStaticOrField(_tokenizer.IdentifierValue, datatype, kind);
                ParseSymbol();
            }

            ValidateSymbol(Symbols.Semicolon);
            TryAdvance();
        }

        private void CompileSubroutine()
        {
            _symbolTable.ResetSubroutineSymbolTable();
            _ifCount = 0;
            _whileCount = 0;

            if (_tokenizer.KeywordValue == Keywords.Method)
                _symbolTable.DefineArgument(Keywords.This, _className);

            _currentSubroutineType = _tokenizer.CurrentTokenValue;

            TryAdvance(); // "subroutine type" -> "return value"

            if (_tokenizer.TokenType != TokenType.Identifier
                && _tokenizer.TokenType != TokenType.Keyword
                && !Keywords.Datatypes.Contains(_tokenizer.KeywordValue))
                throw new InvalidDataException($"Return type expected at line {_tokenizer.CurrentLineCount}");

            ParseIdentifier();
            _currentSubroutineName = _tokenizer.CurrentTokenValue;

            ParseSymbol(Symbols.OpeningBrace);
            TryAdvance(); // "(" -> ")" | "parameter list"
            CompileParameterList();
            ValidateSymbol(Symbols.ClosingBrace);

            ParseSymbol(Symbols.OpeningCurlyBrace);
            TryAdvance(); // "{" -> "}" | "varDec or statements"
            CompileSubroutineBody();
            ValidateSymbol(Symbols.ClosingCurlyBrace);
            TryAdvance();
        }

        private void CompileParameterList()
        {
            if (_tokenizer.TokenType == TokenType.Symbol && _tokenizer.SymbolValue == Symbols.ClosingBrace)
                return;
            if (_tokenizer.TokenType != TokenType.Identifier && _tokenizer.TokenType != TokenType.Keyword && !Keywords.Datatypes.Contains(_tokenizer.KeywordValue))
                throw new Exception($"Datatype expected but '{_tokenizer.CurrentTokenValue}' found.");

            var datatype = _tokenizer.CurrentTokenValue;

            ParseIdentifier();

            _symbolTable.DefineArgument(_tokenizer.IdentifierValue, datatype);

            ParseSymbol(); // "name" -> ")" | ","

            if (_tokenizer.SymbolValue == Symbols.Comma)
            {
                TryAdvance(); // "," -> "datatype"
                CompileParameterList();
            }
        }

        private void CompileSubroutineBody()
        {
            CompileLocalVariableDeclaration();

            _vmWriter.WriteFunction($"{_className}.{_currentSubroutineName}", _symbolTable.VariablesCount(VariableKind.Local));

            if (_currentSubroutineType == Keywords.Constructor)
            {
                _vmWriter.WritePush(Segments.Constant, _symbolTable.VariablesCount(VariableKind.Field));
                _vmWriter.WriteCall("Memory.alloc", 1);
                _vmWriter.WritePop(Segments.Pointer, 0);
            }
            else if (_currentSubroutineType == Keywords.Method)
            {
                _vmWriter.WritePush(Segments.Argument, 0);
                _vmWriter.WritePop(Segments.Pointer, 0);
            }

            CompileStatements();
        }

        private void CompileLocalVariableDeclaration()
        {
            while (_tokenizer.TokenType == TokenType.Keyword && _tokenizer.CurrentTokenValue == Keywords.Var)
            {
                TryAdvance(); // "var" -> "datatype"

                if (_tokenizer.TokenType != TokenType.Identifier
                    && _tokenizer.TokenType == TokenType.Keyword
                    && !Keywords.Datatypes.Contains(_tokenizer.KeywordValue))
                    throw new Exception($"Datatype expected in line {_tokenizer.CurrentLineCount}.");

                var datatype = _tokenizer.CurrentTokenValue;

                ParseIdentifier();
                _symbolTable.DefineLocal(_tokenizer.IdentifierValue, datatype);

                ParseSymbol(); // "name" -> ";" | ","

                while (_tokenizer.TokenType == TokenType.Symbol && _tokenizer.SymbolValue == Symbols.Comma)
                {
                    ParseIdentifier();

                    //_symbolTable.DefineLocalOrArgument(_tokenizer.IdentifierValue, datatype, VariableKind.Local);
                    _symbolTable.DefineLocal(_tokenizer.IdentifierValue, datatype);

                    ParseSymbol(); // "name" -> ";" | ","
                }

                ValidateSymbol(Symbols.Semicolon);
                TryAdvance(); // ";" -> "statements"
            }
        }

        private void CompileStatements()
        {
            bool exitLoop = false;

            while (_tokenizer.TokenType == TokenType.Keyword && !exitLoop)
            {
                switch (_tokenizer.KeywordValue)
                {
                    case Keywords.Let:
                        CompileLet();
                        break;
                    case Keywords.If:
                        CompileIf();
                        break;
                    case Keywords.While:
                        CompileWhile();
                        break;
                    case Keywords.Do:
                        CompileDo();
                        break;
                    case Keywords.Return:
                        CompileReturn();
                        break;
                    default:
                        exitLoop = true;
                        break;
                }
            }
        }

        private void CompileLet()
        {
            ParseIdentifier();

            var name = _tokenizer.IdentifierValue;

            // ParseSymbol(Symbols.Equal);
            ParseSymbol(); // "name" -> "=" | "["

            if (_tokenizer.SymbolValue == Symbols.OpeningSquareBracket)
            {
                TryAdvance(); // "[" -> "identifier" | "int"

                if (_tokenizer.TokenType != TokenType.Int && _tokenizer.TokenType != TokenType.Identifier)
                    throw new Exception("Int or identifier expected");

                CompileExpression();
                ValidateSymbol(Symbols.ClosingSquareBracket);
                TryAdvance();

                _vmWriter.WritePush(_segmentMap[_symbolTable.KindOf(name)], _symbolTable.IndexOf(name));
                _vmWriter.WriteArithmetic("add");

                ValidateSymbol(Symbols.Equal);
                TryAdvance(); // "=" -> "expression"
                CompileExpression();

                var destinationSegment = _segmentMap[_symbolTable.KindOf(name)];

                //_vmWriter.WritePop(destinationSegment, _symbolTable.IndexOf(name));
                _vmWriter.WritePop(Segments.Temp, 0);
                _vmWriter.WritePop(Segments.Pointer, 1);
                _vmWriter.WritePush(Segments.Temp, 0);
                _vmWriter.WritePop(Segments.That, 0);

                ValidateSymbol(Symbols.Semicolon);
                TryAdvance(); // ";" -> "next statement"
            }
            else
            {
                ValidateSymbol(Symbols.Equal);
                TryAdvance(); // "=" -> "expression"
                CompileExpression();

                var destinationSegment = _segmentMap[_symbolTable.KindOf(name)];
                _vmWriter.WritePop(destinationSegment, _symbolTable.IndexOf(name));

                ValidateSymbol(Symbols.Semicolon);
                TryAdvance(); // ";" -> "next statement"
            }
        }

        private void CompileIf()
        {
            var ifTrueLabel = $"{_className}.{_currentSubroutineName}_if_true{_ifCount}";
            var ifFalseLabel = $"{_className}.{_currentSubroutineName}$if_false{_ifCount}";
            var ifEndLabel = $"{_className}.{_currentSubroutineName}$if_end{_ifCount++}";

            ParseSymbol(Symbols.OpeningBrace);
            TryAdvance(); // "(" -> ")" | "expression"
            CompileExpression();
            ValidateSymbol(Symbols.ClosingBrace);

            _vmWriter.WriteIf(ifTrueLabel);
            _vmWriter.WriteGoto(ifFalseLabel);
            _vmWriter.WriteLabel(ifTrueLabel);

            ParseSymbol(Symbols.OpeningCurlyBrace);
            TryAdvance(); // "{" -> "}" | "statements"
            CompileStatements();
            ValidateSymbol(Symbols.ClosingCurlyBrace);
            TryAdvance(); // "}" -> "nextStatement" | "else"

            if (_tokenizer.TokenType == TokenType.Keyword && _tokenizer.KeywordValue == Keywords.Else)
            {
                ParseSymbol(Symbols.OpeningCurlyBrace);
                TryAdvance(); // "{" -> "}" | "statements"

                _vmWriter.WriteGoto(ifEndLabel);
                _vmWriter.WriteLabel(ifFalseLabel);

                CompileStatements();
                ValidateSymbol(Symbols.ClosingCurlyBrace);
                TryAdvance(); // "{" -> "}" | "statements"

                _vmWriter.WriteLabel(ifEndLabel);
            }
            else
            {
                _vmWriter.WriteLabel(ifFalseLabel);
            }
        }

        private void CompileWhile()
        {
            var whileExpressionLabel = $"{_className}.{_currentSubroutineName}$while_exp{_whileCount}";
            var whileEndLabel = $"{_className}.{_currentSubroutineName}$while_end{_whileCount++}";

            _vmWriter.WriteLabel(whileExpressionLabel);

            ParseSymbol(Symbols.OpeningBrace);
            TryAdvance(); // "(" -> ")" | "expression"
            CompileExpression();
            ValidateSymbol(Symbols.ClosingBrace);


            _vmWriter.WriteArithmetic("not");
            _vmWriter.WriteIf(whileEndLabel);

            ParseSymbol(Symbols.OpeningCurlyBrace);
            TryAdvance(); // "{" -> "}" | "statements"
            CompileStatements();
            ValidateSymbol(Symbols.ClosingCurlyBrace);
            TryAdvance(); // "}" -> "next statement"

            _vmWriter.WriteGoto(whileExpressionLabel);
            _vmWriter.WriteLabel(whileEndLabel);
        }

        private void CompileDo()
        {
            ParseIdentifier();

            string className;
            var subName = _tokenizer.IdentifierValue;
            var nArguments = 0;

            TryAdvance(); // "" -> "." | "(" 

            if (_tokenizer.TokenType == TokenType.Symbol && _tokenizer.SymbolValue == Symbols.Dot)
            {
                ParseIdentifier();

                if (_symbolTable.TryGetValue(subName.ToString(), out Symbol symbol))
                {
                    _vmWriter.WritePush(_segmentMap[symbol.Kind], symbol.Index);
                    nArguments++;
                    className = symbol.Type;
                }
                else
                {
                    className = subName;
                }

                subName = _tokenizer.IdentifierValue;
                TryAdvance(); // "" -> "." | "(" 
            }
            else
            {
                className = _className;
                _vmWriter.WritePush(Segments.Pointer, 0);
                nArguments++;
            }

            ValidateSymbol(Symbols.OpeningBrace);
            TryAdvance(); // "(" -> ")" | "expression"

            nArguments += CompileExpressionList();

            ValidateSymbol(Symbols.ClosingBrace);
            ParseSymbol(Symbols.Semicolon);
            TryAdvance(); // ";" -> "next statement"

            _vmWriter.WriteCall($"{className}.{subName}", nArguments);
            _vmWriter.WritePop(Segments.Temp, 0);
        }

        private void CompileReturn()
        {
            TryAdvance(); // "return" -> ";" | "expression"

            if (_tokenizer.TokenType == TokenType.Symbol && _tokenizer.SymbolValue == Symbols.Semicolon)
                _vmWriter.WritePush(Segments.Constant, 0);
            else
                CompileExpression();

            ValidateSymbol(Symbols.Semicolon);
            TryAdvance();
            _vmWriter.WriteReturn();
        }

        private void CompileExpression()
        {
            CompileTerm();

            while (_tokenizer.TokenType == TokenType.Symbol && Symbols.BinaryOperations.Keys.Contains(_tokenizer.SymbolValue))
            {
                var binaryOperation = _tokenizer.SymbolValue;

                TryAdvance();
                CompileTerm();

                if (!Symbols.BinaryOperations.TryGetValue(binaryOperation, out string command))
                    throw new Exception();

                _vmWriter.WriteArithmetic(command);
            }
        }

        private void CompileTerm()
        {
            switch (_tokenizer.TokenType)
            {
                case TokenType.Symbol:
                {
                    if (_tokenizer.SymbolValue == Symbols.OpeningBrace)
                    {
                        TryAdvance(); // "(" -> "expression"
                        CompileExpression();
                        ValidateSymbol(Symbols.ClosingBrace);
                        TryAdvance();
                    }
                    else if (Symbols.UnaryOperations.TryGetValue(_tokenizer.SymbolValue, out string command))
                    {
                        TryAdvance(); // "unary operation" -> "next term"
                        CompileTerm();
                        _vmWriter.WriteArithmetic(command);
                    }
                    else
                        throw new Exception();

                    break;
                }
                case TokenType.String:
                {
                    string stringValue = _tokenizer.StringValue;

                    _vmWriter.WritePush(Segments.Constant, stringValue.Length);
                    _vmWriter.WriteCall("String.new", 1);

                    foreach (char c in stringValue)
                    {
                        _vmWriter.WritePush(Segments.Constant, (int)c);
                        _vmWriter.WriteCall("String.appendChar", 2);
                    }

                    TryAdvance();
                    break;
                }
                case TokenType.Int:
                {
                    _vmWriter.WritePush(Segments.Constant, _tokenizer.IntValue);
                    TryAdvance();
                    break;
                }
                case TokenType.Keyword:
                {
                    switch (_tokenizer.KeywordValue)
                    {
                        case Keywords.True:
                            _vmWriter.WritePush(Segments.Constant, 0);
                            _vmWriter.WriteArithmetic("not");
                            break;
                        case Keywords.False:
                        case Keywords.Null:
                            _vmWriter.WritePush(Segments.Constant, 0);
                            break;
                        case Keywords.This:
                            _vmWriter.WritePush(Segments.Pointer, 0);
                            break;
                        default:
                            throw new Exception($"'{_tokenizer.KeywordValue}' is not as keyword valid.");
                    }

                    TryAdvance();
                    break;
                }
                case TokenType.Identifier:
                {
                    // Routine must distinguish between a variable, an array or a subroutine.
                    // A single look-ahead token, which may be once of "[", "(", "." suffices to
                    // distinguish between the possibilities. Any other token is not part of this term and
                    // should not be advanced over.
                    var identifier = _tokenizer.IdentifierValue;
                    ParseSymbol(); // "name" -> ";" | ")" | "[" | "(" | "."

                    switch (_tokenizer.SymbolValue)
                    {
                        case Symbols.OpeningBrace:
                        {
                            break;
                        }
                        case Symbols.Dot:
                        {
                            string className;
                            var nArguments = 0;

                            if (_symbolTable.TryGetValue(identifier, out var symbol))
                            {
                                className = symbol.Type;
                                _vmWriter.WritePush(_segmentMap[symbol.Kind], symbol.Index);
                                nArguments++;
                            }
                            else
                            {
                                className = identifier;
                            }

                            ParseIdentifier();
                            identifier = _tokenizer.IdentifierValue;
                            ParseSymbol(Symbols.OpeningBrace);
                            TryAdvance(); // "(" -> ")" | "expression list"

                            nArguments += CompileExpressionList();
                            ValidateSymbol(Symbols.ClosingBrace);
                            TryAdvance();

                            _vmWriter.WriteCall($"{className}.{identifier}", nArguments);
                            break;
                        }
                        case Symbols.OpeningSquareBracket:
                        {
                            TryAdvance(); // "[" -> "expression"
                            CompileExpression();
                            ValidateSymbol(Symbols.ClosingSquareBracket);
                            TryAdvance();

                            _vmWriter.WritePush(_segmentMap[_symbolTable.KindOf(identifier)], _symbolTable.IndexOf(identifier));
                            _vmWriter.WriteArithmetic("add");
                            _vmWriter.WritePop(Segments.Pointer, 1);
                            _vmWriter.WritePush(Segments.That, 0);
                            break;
                        }
                        default:
                        {
                            var localVariable = identifier.ToString();
                            var kind = _symbolTable.KindOf(localVariable);
                            _vmWriter.WritePush(_segmentMap[kind], _symbolTable.IndexOf(localVariable));
                            break;
                        }
                    }
                    break;
                }
                default:
                    throw new Exception($"Error while compiling error at line {_tokenizer.CurrentLineCount}.");
            }
        }

        private int CompileExpressionList()
        {
            // Returns 0 if the current token is a closing brace
            if (_tokenizer.TokenType == TokenType.Symbol && _tokenizer.SymbolValue == Symbols.ClosingBrace)
                return 0;

            // There should be at least one argument in this expression list
            int numberOfArguments = 1;

            CompileExpression();

            // Continue looping until a closing brace is encountered
            while (_tokenizer.TokenType == TokenType.Symbol && _tokenizer.SymbolValue != Symbols.ClosingBrace)
            {
                ValidateSymbol(Symbols.Comma);
                TryAdvance(); // "," -> "expression"

                CompileExpression();
                numberOfArguments++;
            }

            // Return the number of arguments count
            return numberOfArguments;
        }
    }
}
