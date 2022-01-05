namespace Yuyuyui.PrivateServer
{
    public static class Utils
    {
        #region FileSystem

        public static string EnsureDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        #endregion

        #region Random

        private static readonly Random random = new();

        private static string RandomStrFromChar(string chars, int length)
        {
            return new string(Enumerable.Repeat((byte) 0, length)
                .Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        public static string GenerateRandomHexString(int length)
        {
            return RandomStrFromChar("0123456789abcdef", length);
        }

        public static string GenerateRandomDigit(int length) // first digit 0 is preserved
        {
            return
                $"{RandomStrFromChar("123456789", 1)}{RandomStrFromChar("0123456789", length - 1)}";
        }

        #endregion

        #region DateTime

        public static long CurrentUnixTime()
        {
            return FromDateTime(DateTime.UtcNow);
        }

        private static long FromDateTime(DateTime dateTime)
        {
            double totalSeconds = (dateTime.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
            return (long) totalSeconds;
        }

        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Logging

        public delegate void LogCallback(object obj);

        public static LogCallback LogTrace;
        public static LogCallback Log;
        public static LogCallback LogWarning;
        public static LogCallback LogError;

        public static void SetLogCallbacks(LogCallback logTrace, LogCallback log, LogCallback logWarning, LogCallback logError)
        {
            LogTrace = logTrace;
            Log = log;
            LogWarning = logWarning;
            LogError = logError;
        }

        #endregion
    }
}