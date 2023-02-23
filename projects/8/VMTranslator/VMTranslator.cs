using Constants;
using System;
using System.IO;
using System.Linq;

namespace VMTranslator
{
    internal class VMTranslator
    {
        private CodeWriter CodeWriter;

        /// <summary>
        /// Initializes the VMTranslator with the specified input file or directory and
        /// converts the contents into a .asm file.
        /// </summary>
        /// <param name="inputPath">Path of the .vm file or directory containing .vm files.</param>
        /// <exception cref="ArgumentException">Thrown when the specified file or directory does not exist or is not a valid file type.</exception>
        public VMTranslator(string inputPath)
        {
            bool isDirectory = false, isFile = false;
            string outputPath = "";

            if (!File.Exists(inputPath) && !Directory.Exists(inputPath))
            {
                throw new ArgumentException("The specified file or directory does not exist.");
            }

            FileAttributes fileAttributes = File.GetAttributes(inputPath);

            if (fileAttributes.HasFlag(FileAttributes.Directory))
            {
                isDirectory = true;
                outputPath = Path.Combine(inputPath, Path.ChangeExtension(Path.GetFileName(inputPath), ".asm"));
            }
            else if (Path.GetExtension(inputPath) == ".vm")
            {
                isFile = true;
                outputPath = Path.ChangeExtension(inputPath, ".asm");
            }
            else
            {
                throw new ArgumentException("Invalid file, compilation failed");
            }

            using (StreamWriter sw = new StreamWriter(outputPath))
            {
                CodeWriter = new CodeWriter(sw);

                if (isDirectory)
                    ProcessDirectory(inputPath);
                else if (isFile)
                    ProcessFile(inputPath);

                CodeWriter.Close();
            }
        }

        public void ProcessDirectory(string dirPath)
        {
            var files = Directory.GetFiles(dirPath).Where(f => Path.GetExtension(f) == ".vm");

            if (files == null || !files.Any())
            {
                throw new ArgumentException("The specified directory does not contain any .vm files.");
            }

            foreach (string file in files)
            {
                ProcessFile(file);
            }
        }

        /// <summary>
        /// Processes the input file and converts it into a .asm file.
        /// </summary>
        /// <param name="filePath">Path of the input file to be processed.</param>
        public void ProcessFile(string filePath)
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                Parser parser = new Parser(streamReader);

                CodeWriter.SetCurrentFileName(Path.GetFileNameWithoutExtension(filePath));

                while (parser.HasMoreLines())
                {
                    parser.Advance();

                    CommandType commandType = parser.GetCommandType();

                    switch (commandType)
                    {
                        case CommandType.C_PUSH:
                        case CommandType.C_POP:
                            CodeWriter.WritePushPop(commandType, parser.Arg1(), parser.Arg2());
                            break;
                        case CommandType.C_ARITHMETIC:
                            CodeWriter.WriteArithmetic(parser.Arg1());
                            break;
                        case CommandType.C_LABEL:
                            CodeWriter.WriteLabel(parser.Arg1());
                            break;
                        case CommandType.C_GOTO:
                            CodeWriter.WriteGoto(parser.Arg1());
                            break;
                        case CommandType.C_IF:
                            CodeWriter.WriteIf(parser.Arg1());
                            break;
                        case CommandType.C_FUNCTION:
                            CodeWriter.WriteFunction(parser.Arg1(), parser.Arg2());
                            break;
                        case CommandType.C_CALL:
                            CodeWriter.WriteCall(parser.Arg1(), parser.Arg2());
                            break;
                        case CommandType.C_RETURN:
                            CodeWriter.WriteReturn();
                            break;
                        default:
                            break;
                    }
                }

                streamReader.Close();
            }
        }
    }
}
