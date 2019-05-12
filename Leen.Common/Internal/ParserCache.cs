using System;

namespace Leen.Common.Internal
{
    internal static class ParserCache<T> where T : struct
    {
        private const string cStyleTrue = "1";
        private const string cStyleFalse = "0";

        public static T Parse(string valueStr)
        {
            if (typeof(T) == typeof(bool) && (valueStr == cStyleTrue || valueStr == cStyleFalse))
            {
                //We need this boxing or unboxing here. 
                object result = false;
                if (valueStr == cStyleFalse)
                {
                    result = false;
                }
                else if (valueStr == cStyleTrue)
                {
                    result = true;
                }

                return (T)result;
            }
            else if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), valueStr);
            }
            else
            {
                return parseFunc(valueStr);
            }
        }

        public static bool TryParse(string valueStr, out T value)
        {
            if (typeof(T) == typeof(bool) && (valueStr == cStyleTrue || valueStr == cStyleFalse))
            {
                //We need this boxing or unboxing here. 
                object outValue = false;
                bool result = false;
                if (valueStr == cStyleFalse)
                {
                    outValue = false;
                    result = true;
                }
                else if (valueStr == cStyleTrue)
                {
                    outValue = true;
                    result = true;
                }

                value = (T)outValue;
                return result;
            }
            else if (typeof(T).IsEnum)
            {
                return Enum.TryParse(valueStr, out value);
            }
            else
            {
                return tryParseFunc(valueStr, out value);
            }
        }

        delegate T ParsePattern(string s);
        delegate bool TryParsePattern(string s, out T i);

        private static readonly ParsePattern parseFunc;
        private static readonly TryParsePattern tryParseFunc;

        static ParserCache()
        {
            var parseMethod = typeof(T).GetMethod("Parse", new Type[] { typeof(string) });
            if (parseMethod == null)
            {
                if (typeof(T) == typeof(string))
                    parseFunc = x => (T)(object)x;
                else
                    parseFunc = x => default;
            }
            else
            {
                parseFunc = (ParsePattern)Delegate.CreateDelegate(typeof(ParsePattern), parseMethod);
            }

            var tryPaseMethod = typeof(T).GetMethod("TryParse", new Type[] { typeof(string), typeof(T).MakeByRefType() });
            if (tryPaseMethod == null)
            {
                if (typeof(T) == typeof(string))
                {
                    tryParseFunc = (string s, out T i) =>
                    {
                        i = (T)(object)s;
                        return true;
                    };
                }
                else
                {
                    tryParseFunc = (string s, out T i) =>
                    {
                        i = default;
                        return false;
                    };
                }
            }
            else
            {
                tryParseFunc = (TryParsePattern)Delegate.CreateDelegate(typeof(TryParsePattern), tryPaseMethod);
            }
        }
    }
}
