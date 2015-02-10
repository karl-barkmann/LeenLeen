using System;

namespace Smart.Common
{
    public static class GenericExtensions
    {
        public static Nullable<T> ToNullable<T>(this string s) where T : struct
        {
            Nullable<T> result = new Nullable<T>();
            try
            {
                if (!string.IsNullOrEmpty(s) && s.Trim().Length > 0)
                {
                    result = ParserCache<T>.Parse(s);
                }
            }
            catch { }
            return result;
        }

        public static T ToValue<T>(this string s) where T : struct
        {
            T result = new T();

            try
            {
                if (!string.IsNullOrEmpty(s) && s.Trim().Length > 0)
                {
                    result = ParserCache<T>.Parse(s);
                }
            }
            catch { }

            return result;
        }

        private static class ParserCache<T> where T : struct
        {
            public static T Parse(string s)
            {
                T i;
                TryParseFunc(s, out i);
                return i;
            }

            delegate T ParsePattern(string s);
            delegate bool TryParsePattern(string s, out T i);

            private static readonly ParsePattern ParseFunc;
            private static readonly TryParsePattern TryParseFunc;

            static ParserCache()
            {
                var parseMethod = typeof(T).GetMethod("Parse", new Type[] { typeof(string) });
                if (parseMethod == null)
                {
                    if (typeof(T) == typeof(string))
                        ParseFunc = x => (T)(object)x;
                    else
                        ParseFunc = x => default(T);
                }
                else
                {
                    ParseFunc = (ParsePattern)Delegate.CreateDelegate(typeof(ParsePattern), parseMethod);
                }

                var tryPaseMethod = typeof(T).GetMethod("TryParse", new Type[] { typeof(string), typeof(T).MakeByRefType() });
                if (tryPaseMethod == null)
                {
                    if (typeof(T) == typeof(string))
                    {
                        TryParseFunc = (string s, out T i) =>
                        {
                            i = (T)(object)s;
                            return true;
                        };
                    }
                    else
                    {
                        TryParseFunc = (string s, out T i) =>
                        {
                            i = default(T);
                            return false;
                        };
                    }
                }
                else
                {
                    TryParseFunc = (TryParsePattern)Delegate.CreateDelegate(typeof(TryParsePattern), tryPaseMethod);
                }
            }
        }
    }
}
