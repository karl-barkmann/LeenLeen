using Leen.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 定义一个绑定基类用于生成枚举集合。
    /// </summary>
    public class BindableEnumValue<T> : BindableBase where T : struct, IConvertible
    {
        private int _value;
        private string _name;
        private bool _isChecked;

        /// <summary>
        /// 构造 <see cref="BindableEnumValue{T}"/> 类的实例。
        /// </summary>
        public BindableEnumValue()
        {

        }

        /// <summary>
        /// 获取其枚举值。
        /// </summary>
        public T EnumValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取或设置枚举标识的值。
        /// </summary>
        public int DisplayValue
        {
            get { return _value; }
            set
            {
                SetProperty(ref _value, value, () => DisplayValue);
            }
        }

        /// <summary>
        /// 获取或设置枚举标识的名称。
        /// </summary>
        public string DisplayName
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value, () => DisplayName);
            }
        }

        /// <summary>
        /// 获取或设置一个值指示当前项是否被选中。
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                SetProperty(ref _isChecked, value, () => IsChecked);
            }
        }


        /// <summary>
        /// 获取用于绑定枚举值集合。
        /// </summary>
        /// <typeparam name="TEnum">枚举的类型。</typeparam>
        /// <returns>生成用于绑定枚举值集合。</returns>
        public static IEnumerable<BindableEnumValue<TEnum>> BindableEnumValues<TEnum>(params TEnum[] skippValues) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated(Enum) type");
            }

            foreach (var enumValue in EnumHelper.GetValues<TEnum>())
            {
                if (skippValues.Contains(enumValue))
                    continue;
                BindableEnumValue<TEnum> value = new BindableEnumValue<TEnum>();
                value.EnumValue = enumValue;
                value.DisplayName = EnumHelper.GetDescription(enumValue);
                value.DisplayValue = value.GetHashCode();
                yield return value;
            }
        }
    }
}
