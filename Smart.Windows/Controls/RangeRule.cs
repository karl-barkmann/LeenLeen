using System;
using System.Globalization;
using System.Windows.Controls;

namespace Smart.Windows.Controls
{
    /// <summary>
    /// 范围验证
    /// </summary>
    internal class RangeRule : ValidationRule
    {
        private int minLength;
        private int maxLength;
        private string propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeRule"/> class.
        /// </summary>
        public RangeRule()
        {

        }

        /// <summary>
        /// 获取或设置 最小值
        /// </summary>
        /// <value>The min.</value>
        public int MinLength
        {
            get { return minLength; }
            set { minLength = value; }
        }

        /// <summary>
        /// 获取或设置 最大值
        /// </summary>
        /// <value>The max.</value>
        public int MaxLength
        {
            get { return maxLength; }
            set { maxLength = value; }
        }

        /// <summary>
        /// 获取或设置 验证的属性名称
        /// </summary>
        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }

        /// <summary>
        /// 当在派生类中重写时，对值执行验证检查。
        /// </summary>
        /// <param name="value">要检查的来自绑定目标的值。</param>
        /// <param name="cultureInfo">要在此规则中使用的区域性。</param>
        /// <returns>
        /// 一个 <see cref="T:System.Windows.Controls.ValidationResult"/> 对象。
        /// </returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (value is string)
                {
                    string str = value.ToString();
                    if (str.Length < MinLength)
                        return new ValidationResult(false, String.Format("{0} 不能小于 {1} 位数", propertyName, minLength));
                    else if (str.Length > MaxLength)
                        return new ValidationResult(false, String.Format("{0} 不能大于 {1} 位数", propertyName, maxLength));
                }
                else
                    return new ValidationResult(false, "不合法字符串 ");
            }
            catch (Exception e)
            {
                return new ValidationResult(false, e.Message);
            }
            return new ValidationResult(true, null);
        }
    }

}
