using Constants;
using System.IO;

namespace VMTranslator
{
    internal class CodeWriter
    {
        private StreamWriter StreamWriter;
        private int CompareLabelCount;
        private int CallCount;
        private string CurrentFileName;

        // Initialize the stream-writer and generates the assembly instructions that
        // effect the bootstrap code that starts the programs's execution.
        public CodeWriter(StreamWriter sw)
        {
            StreamWriter = sw;

            sw.WriteLine(ASMCode.SetSPTo256);
            WriteCall("Sys.init", 0);
        }

        // Informas that the translation of a new VM file hast started.
        public void SetCurrentFileName(string fileName)
        {
            CurrentFileName = fileName;
        }

        /// <summary>
        /// Writes to the output file the assembly code that implements the given arithmetic-logical command.
        /// </summary>
        /// <param name="command">String representation of the given VM command.</param>
        public void WriteArithmetic(string command)
        {
            switch (command)
            {
                case VMArithmetic.Neg:
                case VMArithmetic.Not:
                {
                    StreamWriter.WriteLine(string.Format(ASMCode.UnaryOperation, VMArithmetic.Symbols[command]));
                    break;
                }
                case VMArithmetic.Add:
                case VMArithmetic.Sub:
                case VMArithmetic.And:
                case VMArithmetic.Or:
                {
                    StreamWriter.WriteLine(string.Format(ASMCode.BinaryOperation, VMArithmetic.Symbols[command]));
                    break;
                }
                case VMArithmetic.Eq:
                case VMArithmetic.Gt:
                case VMArithmetic.Lt:
                {
                    StreamWriter.WriteLine(string.Format(ASMCode.Compare, VMArithmetic.Symbols[command], CompareLabelCount++));
                    break;
                }
            }
        }

        /// <summary>
        /// Writes the Assembly code that implements the given C_PUSH or C_POP command for the given segment and index.
        /// </summary>
        /// <param name="command">The type of command (C_PUSH or C_POP).</param>
        /// <param name="segment">The segment to push/pop the value from/to (e.g. argument, local, this, that, temp, static, pointer).</param>
        /// <param name="index">The index within the segment.</param>
        public void WritePushPop(CommandType command, string segment, int index)
        {
            switch (segment)
            {
                case Segments.Constant:
                {
                    if (command == CommandType.C_PUSH)
                    {
                        StreamWriter.WriteLine(string.Format(ASMCode.PushConstant, index));
                    }
                    break;
                }
                case Segments.Argument:
                case Segments.Local:
                case Segments.That:
                case Segments.This:
                {
                    StreamWriter.WriteLine(string.Format(command == CommandType.C_PUSH ? ASMCode.PushLocalArgThisThat : ASMCode.PopLocalArgThisThat, Segments.Symbols[segment], index));
                    break;
                }
                case Segments.Static:
                {
                    StreamWriter.WriteLine(string.Format(command == CommandType.C_PUSH ? ASMCode.PushStatic : ASMCode.PopStatic, $"{CurrentFileName}.{index}"));
                    break;
                }
                case Segments.Temp:
                {
                    StreamWriter.WriteLine(string.Format(command == CommandType.C_PUSH ? ASMCode.PushTemp : ASMCode.PopTemp, Segments.Symbols[segment], index));
                    break;
                }
                case Segments.Pointer:
                {
                    StreamWriter.WriteLine(string.Format(command == CommandType.C_PUSH ? ASMCode.PushPointer : ASMCode.PopPointer, Segments.PointerMap[index]));
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Writes the assembly code for a label command.
        /// </summary>
        /// <param name="label">The label to be written to the .asm file.</param>
        public void WriteLabel(string label)
        {
            StreamWriter.Write(string.Format(ASMCode.Label, label));
        }

        /// <summary>
        /// Writes a goto command to the .asm file.
        /// </summary>
        /// <param name="label">The label to jump to.</param>
        public void WriteGoto(string label)
        {
            StreamWriter.WriteLine(string.Format(ASMCode.Goto, label));
        }

        /// <summary>
        /// Writes an If-Goto command to the output file.
        /// </summary>
        /// <param name="label">The label to jump to if the topmost stack item is not zero.</param>
        public void WriteIf(string label)
        {
            StreamWriter.WriteLine(string.Format(ASMCode.If, label));
        }

        /// <summary>
        /// Writes the ASM code for a function declaration.
        /// </summary>
        /// <param name="functionName">The name of the function being declared.</param>
        /// <param name="nVars">The number of local variables the function will have.</param>
        public void WriteFunction(string functionName, int nVars)
        {
            // Push n local variables onto the stack with initial value 0
            string initLocalVarsCode = "";

            for (int i = 0; i < nVars; i++)
            {
                initLocalVarsCode += ASMCode.InitializeLocalVarOfFunction;
            }

            StreamWriter.WriteLine(string.Format(ASMCode.Function, functionName, initLocalVarsCode));
        }

        /// <summary>
        /// Writes the assembly code to call a function.
        /// </summary>
        /// <param name="functionName">The name of the function to call.</param>
        /// <param name="nArgs">The number of arguments that the function takes.</param>
        public void WriteCall(string functionName, int nArgs)
        {
            var returnLabel = $"{functionName}$ret.{CallCount++}";

            StreamWriter.WriteLine(string.Format(ASMCode.Call, returnLabel, nArgs, functionName));
        }

        public void WriteReturn()
        {
            StreamWriter.WriteLine(ASMCode.Return);
        }

        public void Close()
        {
            StreamWriter.Close();
        }
    }
}
