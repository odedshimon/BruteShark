using System;
using System.Collections.Generic;
using System.Text;

namespace BruteSharkCli
{
    public static class CliPrinter
    {
        public static void WriteLine(ConsoleColor consoleColor, string text)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Info(string text) => CliPrinter.WriteLine(ConsoleColor.Green, $"[+] {text}");
        public static void Error(string text) => CliPrinter.WriteLine(ConsoleColor.Red, $"ERROR: {text}");
        public static void Error(Exception exception) => CliPrinter.Error(exception.Message);

    }
}
