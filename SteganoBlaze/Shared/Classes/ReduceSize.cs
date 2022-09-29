namespace SteganoBlaze.Shared.Classes
{
    public static class ReduceSize
    {
        public static string ToString(int size)
        {
            if (size < 0)
                return "";

            if (size < 1024)
                return size.ToString("0.00") + " B";

            if (size < 1024 * 1024)
                return (size / 1024.0).ToString("0.00") + " KB";

            return (size / (1024.0 * 1024.0)).ToString("0.00") + " MB";
        }
        public static string ToString(double size)
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
