using System.Net;

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

        private static LogCallback? LogTraceFunction;
        private static LogCallback? LogFunction;
        private static LogCallback? LogWarningFunction;
        private static LogCallback? LogErrorFunction;

        public static void LogTrace(object obj)
        {
            LogTraceFunction?.Invoke(obj);
        }
        public static void Log(object obj)
        {
            LogFunction?.Invoke(obj);
        }
        public static void LogWarning(object obj)
        {
            LogWarningFunction?.Invoke(obj);
        }
        public static void LogError(object obj)
        {
            LogErrorFunction?.Invoke(obj);
        }

        public static void SetLogCallbacks(LogCallback logTrace, LogCallback log, LogCallback logWarning, LogCallback logError)
        {
            LogTraceFunction = logTrace;
            LogFunction = log;
            LogWarningFunction = logWarning;
            LogErrorFunction = logError;
        }

        #endregion

        #region Misc

        public static byte[] ReadAllBytes(this Stream inputStream)
        {
            if (inputStream is MemoryStream stream)
                return stream.ToArray();

            using var memoryStream = new MemoryStream();
            inputStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        public static Dictionary<string, string> GetCookieDictFromString(string cookie)
        {
            return cookie.Split(';',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Split('='))
                .ToDictionary(e => e[0], e => e.Length > 1 ? e[1] : "")!;
        }

        public static void ShowRequestProperties2 (this HttpListenerRequest request)
        {
            Console.WriteLine("KeepAlive: {0}", request.KeepAlive);
            Console.WriteLine("Local end point: {0}", request.LocalEndPoint.ToString());
            Console.WriteLine("Remote end point: {0}", request.RemoteEndPoint.ToString());
            Console.WriteLine("Is local? {0}", request.IsLocal);
            Console.WriteLine("HTTP method: {0}", request.HttpMethod);
            Console.WriteLine("Protocol version: {0}", request.ProtocolVersion);
            Console.WriteLine("Is authenticated: {0}", request.IsAuthenticated);
            Console.WriteLine("Is secure: {0}", request.IsSecureConnection);
        }

        #endregion
    }
}