using System;

namespace Yuyuyui.AccountTransfer.CLI
{
    internal static class ColoredOutput
    {
        public static void Write(object obj, ConsoleColor foregroundColor, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(obj);
            Console.ResetColor();
        }
        public static void WriteLine(object obj, ConsoleColor foregroundColor, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine(obj);
            Console.ResetColor();
        }
    }
}