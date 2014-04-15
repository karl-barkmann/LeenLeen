using System;
using System.Text;

namespace Smart.Common
{
    public static class EncodingExtensions
    {
        public static string ToBase64(this byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            return Convert.ToBase64String(bytes);
        }

        public static byte[] FromBase64(this string base64)
        {
            if (base64 == null)
            {
                return null;
            }
            return Convert.FromBase64String(base64);
        }

        public static byte[] ToUtf8(this string str)
        {
            if (str == null)
            {
                return null;
            }
            return Encoding.UTF8.GetBytes(str);
        }

        public static string FromUtf8(this byte[] utf8)
        {
            if (utf8 == null)
            {
                return null;
            }
            return Encoding.UTF8.GetString(utf8);
        }

        public static string FromUtf8(this byte[] utf8, int index, int count)
        {
            return Encoding.UTF8.GetString(utf8, index, count);
        }

        public static string FromUtf8(this byte[] utf8, int count)
        {
            return Encoding.UTF8.GetString(utf8, 0, count);
        }

        public static byte[] ToAscii(this string str)
        {
            if (str == null)
            {
                return null;
            }
            return Encoding.ASCII.GetBytes(str);
        }

        public static string FromAscii(this byte[] ascii)
        {
            if (ascii == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(ascii);
        }
    }
}
