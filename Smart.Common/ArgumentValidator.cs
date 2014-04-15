using System;

namespace Smart.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class ArgumentValidator
    {
        public static void NotNullValidator(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName, "参数不能为空。");
            }
        }

        public static void ValueRangeValidator(DateTime minValue, DateTime maxValue, DateTime actualValue, string parameterName)
        {
            if ((actualValue < minValue) || (actualValue > maxValue))
            {
                throw new ArgumentOutOfRangeException(parameterName, actualValue, "参数超过许可的范围。");
            }
        }

        public static void ValueRangeValidator(int minValue, int maxValue, int actualValue, string parameterName)
        {
            if ((actualValue < minValue) || (actualValue > maxValue))
            {
                throw new ArgumentOutOfRangeException(parameterName, actualValue, "参数超过许可的范围。");
            }
        }

        public static void ValueRangeValidator(long minValue, long maxValue, long actualValue, string parameterName)
        {
            if ((actualValue < minValue) || (actualValue > maxValue))
            {
                throw new ArgumentOutOfRangeException(parameterName, actualValue, "参数超过许可的范围。");
            }
        }

        public static void ValueRangeValidator(float minValue, float maxValue, float actualValue, string parameterName)
        {
            if ((actualValue < minValue) || (actualValue > maxValue))
            {
                throw new ArgumentOutOfRangeException(parameterName, actualValue, "参数超过许可的范围。");
            }
        }

        public static void ValueRangeValidator(TimeSpan minValue, TimeSpan maxValue, TimeSpan actualValue, string parameterName)
        {
            if ((actualValue < minValue) || (actualValue > maxValue))
            {
                throw new ArgumentOutOfRangeException(parameterName, actualValue, "参数超过许可的范围。");
            }
        }
    }
}

