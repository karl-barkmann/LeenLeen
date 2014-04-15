
namespace Smart.Common.Utils
{
    public static class NumberHelper
    {
        public static string GetOptimalSize(long size)
        {
            if (size >= 0x10000000000L)
            {
                return string.Format("{0:F}TB", ((float) size) / 1.099512E+12f);
            }
            if (size >= 0x40000000L)
            {
                return string.Format("{0:F}GB", ((float) size) / 1.073742E+09f);
            }
            if (size >= 0x100000L)
            {
                return string.Format("{0:F}MB", ((float) size) / 1048576f);
            }
            if (size >= 0x400L)
            {
                return string.Format("{0:F}KB", ((float) size) / 1024f);
            }
            return (size.ToString() + "B");
        }
    }
}

