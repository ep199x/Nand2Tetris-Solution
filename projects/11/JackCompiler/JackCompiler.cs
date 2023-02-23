using System;
using System.IO;
using System.Linq;

namespace JackCompiler
{
    internal class JackCompiler
    {
        public JackCompiler(string inputPath)
        {
            if (!File.Exists(inputPath) && !Directory.Exists(inputPath))
            {
                throw new ArgumentException("The specified file or directory does not exist.");
            }

            FileAttributes fileAttributes = File.GetAttributes(inputPath);

            if (fileAttributes.HasFlag(FileAttributes.Directory))
                ProcessDirectory(inputPath);
            else if (Path.GetExtension(inputPath) == ".jack")
                ProcessFile(inputPath);
            else
                throw new ArgumentException("Invalid file, compilation failed");
        }

        public void ProcessDirectory(string dirPath)
        {
            var files = Directory.GetFiles(dirPath).Where(f => Path.GetExtension(f) == ".jack");

            if (files == null || !files.Any())
            {
                throw new ArgumentException("The specified directory does not contain any .jack files.");
            }

            foreach (string file in files)
            {
                ProcessFile(file);
            }
        }

        public void ProcessFile(string inputPath)
        {
            var outputPath = Path.ChangeExtension(inputPath, ".vm");

            _ = new CompilationEngine(inputPath, outputPath);
        }
    }
}
