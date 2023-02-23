using Constants;
using System.IO;
using System.Text.RegularExpressions;

namespace VMTranslator
{
    internal class Parser
    {
        private StreamReader _stream;

        private string CurrentInstruction;

        private readonly Regex RegexLineComment = new Regex(@"^//.*");
        private readonly Regex RegexInlineComment = new Regex(@".+//.*");

        public Parser(StreamReader streamReader)
        {
            _stream = streamReader;
        }

        private void RemoveInlineCommentAndWhiteSpace()
        {
            if (RegexInlineComment.Match(CurrentInstruction).Success)
                CurrentInstruction = Regex.Replace(CurrentInstruction, @"//.*", "").Trim();
        }

        public bool HasMoreLines()
        {
            return !_stream.EndOfStream;
        }

        /// <summary>
        /// Advance method that updates the current instruction by reading the next line from the input stream.
        /// It ensures the new instruction is not an empty line or a line comment, and removes any inline
        /// comments and white spaces from the instruction before setting it as the new current instruction.
        /// </summary>
        public void Advance()
        {
            CurrentInstruction = _stream.ReadLine().Trim();

            if (CurrentInstruction == string.Empty || RegexLineComment.Match(CurrentInstruction).Success)
                Advance();

            RemoveInlineCommentAndWhiteSpace();
        }

        /// <summary>
        /// Retrieves the type of command based on the current instruction.
        /// </summary>
        /// <returns>The type of command as an enumeration value (C_PUSH, C_POP, C_LABEL, C_IF, C_GOTO,
        /// C_FUNCTION, C_CALL, C_RETURN, C_ARITHMETIC, or None).</returns>
        public CommandType GetCommandType()
        {
            string command = CurrentInstruction.Split()[0];

            switch (command)
            {
                case "push": return CommandType.C_PUSH;
                case "pop": return CommandType.C_POP;
                case "label": return CommandType.C_LABEL;
                case "if-goto": return CommandType.C_IF;
                case "goto": return CommandType.C_GOTO;
                case "function": return CommandType.C_FUNCTION;
                case "call": return CommandType.C_CALL;
                case "return": return CommandType.C_RETURN;
                case "add":
                case "sub":
                case "neg":
                case "eq":
                case "gt":
                case "lt":
                case "and":
                case "or":
                case "not": return CommandType.C_ARITHMETIC;
                default: return CommandType.None;
            }
        }

        /// <summary>
        /// This method retrieves the first argument of the current instruction.
        /// If the current instruction is a C_ARITHMETIC command, the first token is returned.
        /// Otherwise, the second token is returned.
        /// </summary>
        /// <returns>A string representation of the first argument of the current instruction.</returns>
        public string Arg1()
        {
            string[] tokens = CurrentInstruction.Split();

            if (GetCommandType() == CommandType.C_ARITHMETIC)
                return tokens[0];
            else
                return tokens[1];
        }

        /// <summary>
        /// This method retrieves the second argument of the current instruction.
        /// </summary>
        /// <returns>A string representation of the second argument of the current instruction.</returns>
        public int Arg2()
        {
            string[] tokens = CurrentInstruction.Split();

            return int.Parse(tokens[2]);
        }
    }
}
