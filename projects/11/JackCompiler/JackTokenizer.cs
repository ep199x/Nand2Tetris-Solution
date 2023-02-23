using JackCompiler.Constants;
using JackCompiler.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace JackCompiler
{
    internal class JackTokenizer
    {
        private readonly StreamReader _reader;
        private string _currentLine;
        private Token _currentToken { get; set; }
        private string _currentClassName;
        public int CurrentLineCount;

        public TokenType TokenType => _currentToken.Type;
        public string KeywordValue => _currentToken.Value;
        public char SymbolValue => char.Parse(_currentToken.Value);
        public string IdentifierValue => _currentToken.Value;
        public int IntValue => int.Parse(_currentToken.Value);
        public string StringValue => _currentToken.Value.Substring(1, _currentToken.Value.Length - 2);
        public string CurrentTokenValue => _currentToken.Value;

        private IEnumerable<(Regex Expression, TokenType TType)> _typeExpressions = new List<(Regex Expression, TokenType TType)>()
        {
            (new Regex(@"//[\S\s]*"), TokenType.NotDefined),
            (new Regex(@"/\*\*[\s\S]*?\*/"), TokenType.NotDefined),

            (new Regex(@"/\*\*"), TokenType.MultilineCommentStart),

            (new Regex(@"^class(?=\s)"), TokenType.Keyword),
            (new Regex(@"^constructor(?=\s)"), TokenType.Keyword),
            (new Regex(@"^function(?=\s)"), TokenType.Keyword),
            (new Regex(@"^method(?=\s)"), TokenType.Keyword),
            (new Regex(@"^field(?=\s)"), TokenType.Keyword),
            (new Regex(@"^static(?=\s)"), TokenType.Keyword),
            (new Regex(@"^var(?=\s)"), TokenType.Keyword),
            (new Regex(@"^char(?=\s)"), TokenType.Keyword),
            (new Regex(@"^boolean(?=\s)"), TokenType.Keyword),
            (new Regex(@"^int(?=\s)"), TokenType.Keyword),
            (new Regex(@"^void(?=\s)"), TokenType.Keyword),
            (new Regex(@"^true(?=[^\w\d\e])"), TokenType.Keyword),
            (new Regex(@"^false(?=[^\w\d\e])"), TokenType.Keyword),
            (new Regex(@"^null(?=[^\w\d\e])"), TokenType.Keyword),
            (new Regex(@"^this(?=[^\w\d\e])"), TokenType.Keyword),
            (new Regex(@"^let(?=\s)"), TokenType.Keyword),
            (new Regex(@"^do(?=\s)"), TokenType.Keyword),
            (new Regex(@"^if(?=[^\w\d\e])"), TokenType.Keyword),
            (new Regex(@"^else(?=[^\w\d\e])"), TokenType.Keyword),
            (new Regex(@"^while(?=[^\w\d\e])"), TokenType.Keyword),
            (new Regex(@"^return(?=[^\w\d\e])"), TokenType.Keyword),

            (new Regex(@"^" + "\\\"(.*?)\\\""), TokenType.String),
            (new Regex(@"^\d+"), TokenType.Int),
            (new Regex(@"^[A-Za-z][A-Za-z0-9_]*"), TokenType.Identifier),

            (new Regex(@"^\" + Symbols.OpeningBrace), TokenType.Symbol),
            (new Regex(@"^\" + Symbols.ClosingBrace), TokenType.Symbol),
            (new Regex(@"^\" + Symbols.OpeningSquareBracket), TokenType.Symbol),
            (new Regex(@"^\" + Symbols.ClosingSquareBracket), TokenType.Symbol),
            (new Regex(@"^\" + Symbols.OpeningCurlyBrace), TokenType.Symbol),
            (new Regex(@"^\" + Symbols.ClosingCurlyBrace), TokenType.Symbol),
            (new Regex(@"^"  + Symbols.Comma), TokenType.Symbol),
            (new Regex(@"^\" + Symbols.Dot), TokenType.Symbol),
            (new Regex(@"^"  + Symbols.Semicolon), TokenType.Symbol),

            (new Regex(@"^\" + Symbols.Plus), TokenType.Symbol),
            (new Regex(@"^" +  Symbols.Minus), TokenType.Symbol),
            (new Regex(@"^\" + Symbols.Star), TokenType.Symbol),
            (new Regex(@"^" +  Symbols.Slash), TokenType.Symbol),
            (new Regex(@"^<="), TokenType.Symbol),
            (new Regex(@"^" +  Symbols.LessThan), TokenType.Symbol),
            (new Regex(@"^>="), TokenType.Symbol),
            (new Regex(@"^" +  Symbols.GreaterThan), TokenType.Symbol),
            (new Regex(@"^!="), TokenType.Symbol),
            (new Regex(@"^" +  Symbols.Equal), TokenType.Symbol),

            (new Regex(@"^" +  Symbols.Ampersand), TokenType.Symbol),
            (new Regex(@"^\" + Symbols.Pipe), TokenType.Symbol),
            (new Regex(@"^" +  Symbols.Tilde), TokenType.Symbol)
        };

        public JackTokenizer(StreamReader reader, string className)
        {
            _reader = reader;
            _currentLine = string.Empty;
            CurrentLineCount = -1;
        }

        public bool HasMoreTokens()
        {
            return !_reader.EndOfStream;
        }

        public void Advance()
        {
            var error = true;

            while (_currentLine == string.Empty)
                ReadNextLine();

            foreach (var(expression, tokenType) in _typeExpressions) // Key: Expression, Value: TokenType
            {
                Match match = expression.Match(_currentLine);

                if (match.Success)
                {
                    error = false;
                    _currentLine = expression.Replace(_currentLine, "").Trim();

                    if (tokenType != TokenType.MultilineCommentStart && tokenType != TokenType.NotDefined)
                    {
                        _currentToken = new(tokenType, match.Value);
                        return;
                    }

                    // skip over doctype multi-line comments
                    if (tokenType == TokenType.MultilineCommentStart)
                    {
                        error = SkipOverDocTypeComment();
                    }

                    break;
                }
            }

            if (error)
                throw new Exception($"Compilation error in file '{_currentClassName}' at line {CurrentLineCount}.\nToken unknown.");

            Advance();
        }

        // This methods skips over doctype comments. These comments can be multi-line comments.
        private bool SkipOverDocTypeComment()
        {
            ReadNextLine(); // "/**.." -> "*.." | "*..*/" | "*/" 

            // loop throw the lines until the line ends with "*/"
            while (!_currentLine.EndsWith(string.Concat(Symbols.Star, Symbols.Slash))) 
            {
                // check if every line starts with '*'
                if (!_currentLine.StartsWith(Symbols.Star.ToString()))
                {
                    throw new Exception("Doc type comment content do not starts with '*'");
                }

                ReadNextLine();
            }

            ReadNextLine();
            
            return false;
        }

        private void ReadNextLine()
        {
            _currentLine = _reader.ReadLine().Trim();
            CurrentLineCount++;
        }
    }
}
