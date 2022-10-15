namespace SteganoBlaze.Shared
{
    public static class ByteSize
    {
        public static string Reduce(int size)
        {
            if (size < 0)
                return "";

            if (size < 1024)
                return size.ToString("0.00") + " B";

            if (size < 1024 * 1024)
                return (size / 1024.0).ToString("0.00") + " KB";

            return (size / (1024.0 * 1024.0)).ToString("0.00") + " MB";
        }
        public static string Reduce(double size)
        {
            if (size < 0)
                return "";

            if (size < 1024)
                return size.ToString("0.00") + " B";

            if (size < 1024 * 1024)
                return (size / 1024.0).ToString("0.00") + " KB";

            return (size / (1024.0 * 1024.0)).ToString("0.00") + " MB";
        }
    }
}
