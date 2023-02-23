using System.Collections.Generic;

namespace Constants
{
    /// <summary>
    /// This class "Segments" defines constants for different memory
    /// areas (Argument, Local, Pointer, Static, Temp, This, That) and
    /// provides a dictionariy that associate these constants
    /// with symbols and pointer mappings.
    /// </summary>
    public static class Segments
    {
        public const string Argument = "argument";
        public const string Constant = "constant";
        public const string Local = "local";
        public const string Pointer = "pointer";
        public const string Static = "static";
        public const string Temp = "temp";
        public const string This = "this";
        public const string That = "that";

        public readonly static Dictionary<string, string> Symbols = new Dictionary<string, string>
        {
            [Argument] = "ARG",
            [Local] = "LCL",
            [Static] = "16",
            [Temp] = "5",
            [This] = "THIS",
            [That] = "THAT"
        };

        public readonly static string[] PointerMap = { "THIS", "THAT" };
    }
}
