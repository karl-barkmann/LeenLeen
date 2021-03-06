﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Leen.Common.Utils
{
    /// <summary>
    /// 枚举辅助类
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 获取枚举值的的描述信息。
        /// </summary>
        /// <param name="type">枚举类型。</param>
        /// <param name="fieldName">枚举项。</param>
        /// <returns>返回枚举项的Description信息，如果该项没有包含Description则返回枚举项的名称。</returns>
        public static string GetDescription(Type type, string fieldName)
        {
            string desc = String.Empty;

            FieldInfo[] fields = type.GetFields();

            foreach (FieldInfo field in fields)
            {
                if (field.IsSpecialName)
                    continue;

                if (field.Name != fieldName)
                    continue;

                object[] attrs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length > 0)
                    desc = ((DescriptionAttribute)attrs[0]).Description;

            }
            if (String.IsNullOrEmpty(desc))
                desc = fieldName;
            return desc;
        }

        /// <summary>
        /// 获取枚举值的的描述信息。
        /// </summary>
        /// <param name="obj">枚举对象。</param>
        /// <returns>返回枚举项的Description信息，如果该项没有包含Description则返回枚举项的ToString()内容。</returns>
        public static string GetDescription(object obj)
        {
            return GetDescription(obj.GetType(), obj.ToString());
        }

        /// <summary>
        /// 根据枚举值获取枚举对象
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="value">枚举值</param>
        /// <returns>枚举对象</returns>
        public static T Parse<T>(int value) where T : Enum
        {
            T type = (T)Enum.Parse(typeof(T), value.ToString());
            if (Enum.IsDefined(typeof(T), type))
            {
                return type;
            }
            else
            {
                return (T)Enum.ToObject(typeof(T), 0);
            }
        }

        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="type">枚举类型</param>
        /// <returns>枚举</returns>
        public static int GetValue<T>(T type) where T : Enum
        {
            int value = type.GetHashCode();
            return value;
        }

        /// <summary>
        /// 获取枚举值集合。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>() where T : Enum
        {
            foreach (var val in Enum.GetValues(typeof(T)))
            {
                yield return (T)val;
            }
        }

        /// <summary>
        /// 按位获取枚举值集合。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>(T input) where T : Enum
        {
            foreach (T value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }

        /// <summary>
        /// 获取字符串对应的枚举值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse<T>(string value) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}
