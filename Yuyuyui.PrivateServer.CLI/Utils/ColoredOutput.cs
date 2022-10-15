using System;

namespace Yuyuyui.PrivateServer.CLI
{
    internal static class ColoredOutput
    {
        public static void Write(object obj, ConsoleColor foregroundColor)
        {
            Console.ForegroundColor = foregroundColor;
            Console.Write(obj);
            Console.ResetColor();
        }
        public static void WriteLine(object obj, ConsoleColor foregroundColor)
        {
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(obj);
            Console.ResetColor();
        }
    }
}
