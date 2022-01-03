namespace Yuyuyui.PrivateServer
{
    public static class Utils
    {
        public static string EnsureDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        private static readonly Random random = new Random();

        public static string GenerateRandomHexString(int length)
        {
            const string chars = "0123456789abcdef";
            return new string(Enumerable.Repeat((byte) 0, length)
                .Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        public static string GenerateRandomPlayerCode() // "1000000000" ~ "9999999999", first digit 0 is preserved
        {
            const string chars = "0123456789";
            string suffix = new string(Enumerable.Repeat((byte) 0, 9)
                .Select(_ => chars[random.Next(chars.Length)]).ToArray());
            char prefix = chars[random.Next(9) + 1];
            return $"{prefix}{suffix}";
        }
        
        public static long CurrentUnixTime()
        {
            return FromDateTime(DateTime.UtcNow);
        }
        private static long FromDateTime(DateTime dateTime)
        {
            double totalSeconds = (dateTime.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
            return (long)totalSeconds;
        }
        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}