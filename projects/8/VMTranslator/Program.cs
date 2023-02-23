using System;
using System.IO;

internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Please provide the path to a .vm file or a directory");
            return;
        }

        if (!File.Exists(args[0]) && !Directory.Exists(args[0]))
        {
            Console.WriteLine("The provided path does not exist");
            return;
        }

        try
        {
            Console.WriteLine("The path was successfully opened!");
            Console.WriteLine("Translating...");
            DateTime startTime = DateTime.UtcNow;

            _ = new VMTranslator.VMTranslator(args[0]);

            double duration = DateTime.UtcNow.Subtract(startTime).TotalMilliseconds / 1000;

            Console.WriteLine($"Translation finished successfully in {duration} seconds.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(e.HResult);
        }
    }
}
