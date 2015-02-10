using Microsoft.CSharp.RuntimeBinder;
using System;

namespace Smart.Common
{
    public static class DynamicXmlExtensions
    {
        public static T GetValue<T>(Func<string> elementValue, T defaultValue = default(T)) where T : struct
        {
            try
            {
                return elementValue().ToValue<T>();
            }
            catch (RuntimeBinderException)
            {
                return defaultValue;
            }
        }

        public static Nullable<T> GetNullableValue<T>(Func<string> elementValue) where T : struct
        {
            try
            {
                return elementValue().ToNullable<T>();
            }
            catch (RuntimeBinderException)
            {
                return null;
            }
        }

        public static string GetValue(Func<string> elementValue)
        {
            try
            {
                return elementValue();
            }
            catch (RuntimeBinderException)
            {
                return String.Empty;
            }
        }
    }
}
