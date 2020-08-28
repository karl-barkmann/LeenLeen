using System;
using System.Runtime.InteropServices;

namespace Leen.Windows.Controls
{
    public static class DoubleUtil
    {
        internal const double DBL_EPSILON = 2.2204460492503131e-016;
        internal const short DefaultBaseValueSource = 1;

        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal UInt64 UintValue;
        }

        public static bool IsNaN(double value)
        {
            NanUnion t = new NanUnion();
            t.DoubleValue = value;

            UInt64 exp = t.UintValue & 0xfff0000000000000;
            UInt64 man = t.UintValue & 0x000fffffffffffff;

            return (exp == 0x7ff0000000000000 || exp == 0xfff0000000000000) && (man != 0);
        }

        internal static bool IsDoubleFinite(double value)
        {
            return !(double.IsInfinity(value) || IsNaN(value));
        }

        internal static bool LessThanOrClose(double value1, double value2)
        {
            return (value1 < value2) || AreClose(value1, value2);
        }

        internal static bool GreaterThanOrClose(double value1, double value2)
        {
            return (value1 > value2) || AreClose(value1, value2);
        }

        /// <summary>
        /// IsZero - Returns whether or not the double is "close" to 0.  Same as AreClose(double, 0),
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value"> The double to compare to 0. </param>
        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * DBL_EPSILON;
        }

        /// <summary>
        /// IsOne - Returns whether or not the double is "close" to 1.  Same as AreClose(double, 1),
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value"> The double to compare to 1. </param>
        public static bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < 10.0 * DBL_EPSILON;
        }

        public static bool IsBetweenZeroAndOne(double val)
        {
            return (GreaterThanOrClose(val, 0) && LessThanOrClose(val, 1));
        }

        internal static bool AreClose(double value1, double value2)
        {
            //in case they are Infinities (then epsilon check does not work)
            if (value1 == value2) return true;
            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }
    }
}
