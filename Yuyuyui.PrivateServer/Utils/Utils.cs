using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

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

        #region HttpClientDownload

        public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
        {
            // Get the http headers first to examine the content length
            using var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var contentLength = response.Content.Headers.ContentLength;

            using var download = await response.Content.ReadAsStreamAsync();
            // Ignore progress reporting when no progress reporter was 
            // passed or when the content length is unknown
            if (progress == null || !contentLength.HasValue)
            {
                await download.CopyToAsync(destination);
                progress?.Report(1);
                return;
            }

            // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
            var relativeProgress = new Progress<long>(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
            // Use extension method to report progress while downloading
            await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
            progress.Report(1);
        }

        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long>? progress = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }

        #endregion

        #region Random

        private static readonly Random random = new();

        public static string RandomStrFromChar(string chars, int length)
        {
            return new string(Enumerable.Repeat((byte)0, length)
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

        public static T? Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default : list[random.Next(0, list.Count)];
        }

        public static T? Random<T>(this IEnumerable<T> enumerable, Func<T, float> weightExpression)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            var list = enumerable as IList<T> ?? enumerable.ToList();

            if (list.Count == 0) return default;

            float weightTotal = list.Sum(weightExpression);
            float randomNumber = (random.Next(short.MaxValue) / (float) short.MaxValue) * weightTotal;
            float cumulatedWeight = 0;

            int i;
            for (i = 0; i < list.Count; ++i)
            {
                float weight = weightExpression(list[i]);
                if (randomNumber < cumulatedWeight + weight)
                    return list[i];

                cumulatedWeight += weight;
            }

            return list.Last();
        }

        public static bool ProbabilityCheck(float percentageNormalized)
        {
            return random.NextDouble() < percentageNormalized;
        }

        #endregion

        #region DateTime

        public static long CurrentUnixTime()
        {
            return FromDateTime(DateTime.UtcNow);
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            return UNIX_EPOCH.AddSeconds(unixTime).ToLocalTime();
        }

        private static long FromDateTime(DateTime dateTime)
        {
            double totalSeconds = (dateTime.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
            return (long)totalSeconds;
        }

        public static DateTime ToDateTime(this string str)
        {
            return DateTime.ParseExact(str.Substring(0, 19), "yyyy/MM/dd HH:mm:ss",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        }

        public static long ToUnixTime(this string str)
        {
            return FromDateTime(str.ToDateTime());
        }


        private static readonly DateTime UNIX_EPOCH = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Logging
        public enum LogType
        {
            Trace,
            Info,
            Warning,
            Error
        }

        public delegate void LogCallback(object obj, LogType logType);
        
        private static LogCallback? LogFunction;

        public static void LogTrace(object obj)
        {
            LogFunction?.Invoke(obj, LogType.Trace);
        }

        public static void Log(object obj)
        {
            LogFunction?.Invoke(obj, LogType.Info);
        }

        public static void LogWarning(object obj)
        {
            LogFunction?.Invoke(obj, LogType.Warning);
        }

        public static void LogError(object obj)
        {
            LogFunction?.Invoke(obj, LogType.Error);
        }

        public static void SetLogCallback(LogCallback logFunc)
        {
            LogFunction = logFunc;
        }

        #endregion

        #region Miscs

        // ForEach for all IEnumerables
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        #endregion
    }
}