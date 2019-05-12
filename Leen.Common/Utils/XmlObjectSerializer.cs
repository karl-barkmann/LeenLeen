using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Leen.Common.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class XmlObjectSerializer
    {
        private static readonly Type[] WriteTypes =
        {
            typeof(string), typeof(DateTime), typeof(Enum), 
            typeof(decimal), typeof(Guid)
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || WriteTypes.Contains(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static XElement ToXml(this object obj, string root = null)
        {
            if (obj == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(root))
            {
                root = "object";
            }

            var type = obj.GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var xe = new XElement(XmlConvert.EncodeName(root));

            props.ForEach(p =>
            {
                var val = p.GetValue(obj, null);
                if (val == null) return;
                var name = p.Name;
                xe.Add(p.PropertyType.IsSimpleType() ? new XElement(name, val) : val.ToXml(name));
            });

            return xe;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="targeType"></param>
        /// <returns></returns>
        public static object ToObject(this XElement root, Type targeType)
        {
            if (root == null)
                return null;

            if (!root.HasElements)
                return null;

            var props = targeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var instance = Activator.CreateInstance(targeType);

            props.ForEach(p =>
            {
                var element = root.Element(p.Name);
                if (element == null) return;
                if (IsSimpleType(p.PropertyType))
                {
                    p.SetValue(instance,
                        TypeDescriptor.GetConverter(p.PropertyType).ConvertFromString(element.Value), null);
                }
                else
                {
                    p.SetValue(instance, ToObject(element, p.PropertyType), null);
                }
            });

            return instance;
        }
    }
}
