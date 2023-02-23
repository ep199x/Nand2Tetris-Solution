using System.IO;

namespace JackCompiler
{
    internal class VMWriter
    {
        private StreamWriter _writer;

        public VMWriter(StreamWriter writer)
        {
            _writer = writer;
        }

        public void WritePush(string segment, int index)
        {
            _writer.WriteLine($"push {segment} {index}");
        }

        public void WritePop(string segment, int index)
        {
            _writer.WriteLine($"pop {segment} {index}");
        }

        public void WriteArithmetic(string command)
        {
            _writer.WriteLine(command);
        }

        public void WriteArithmeticNot()
        {
            _writer.WriteLine("not");
        }

        public void WriteArithmeticNeg()
        {
            _writer.WriteLine("neg");
        }

        public void WriteLabel(string label)
        {
            _writer.WriteLine($"label {label}");
        }

        public void WriteGoto(string label)
        {
            _writer.WriteLine($"goto {label}");
        }

        public void WriteIf(string label)
        {
            _writer.WriteLine($"if-goto {label}");
        }

        public void WriteCall(string name, int nArguments)
        {
            _writer.WriteLine($"call {name} {nArguments}");
        }

        public void WriteFunction(string name, int nArguments)
        {
            _writer.WriteLine($"function {name} {nArguments}");
        }

        public void WriteReturn()
        {
            _writer.WriteLine($"return");
        }
    }
}
