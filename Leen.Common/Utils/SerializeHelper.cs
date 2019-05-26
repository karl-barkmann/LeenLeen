using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Leen.Common.Utils
{
    /// <summary>
    /// 序列化帮助类
    /// </summary>
    public class SerializeHelper
    {
        /// <summary>
        /// 把对象序列化为xml
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string ObjectToXml(object obj)
        {
            if (obj == null) return null;

            string xml = null;
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                xml = writer.GetStringBuilder().ToString();
            }

            return xml;
        }

        /// <summary>
        /// 把xml序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static T XmlToObject<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return default;

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            TextReader reader = new StringReader(xml);
            var result = (T)serializer.Deserialize(reader);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="xmlFilePath"></param>
        public static void ObjectToXmlFile(object obj, string xmlFilePath)
        {
            if (string.IsNullOrEmpty(xmlFilePath))
                return;

            var xml = ObjectToXml(obj);
            using (var fileStream = File.Create(xmlFilePath))
            {
                using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    writer.Write(xml);
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="xmlFilePath"></param>
        public static async Task ObjectToXmlFileAsync(object obj, string xmlFilePath)
        {
            if (string.IsNullOrEmpty(xmlFilePath))
                return;

            var xml = ObjectToXml(obj);
            using (var fileStream = File.Create(xmlFilePath))
            {
                using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    await writer.WriteAsync(xml);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlFilePath"></param>
        /// <returns></returns>
        public static T XmlFileToObject<T>(string xmlFilePath)
        {
            if (string.IsNullOrEmpty(xmlFilePath) || !File.Exists(xmlFilePath))
                return default;

            using (var fileStream = File.OpenRead(xmlFilePath))
            {
                using (var reader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    var xml = reader.ReadToEnd();

                    return XmlToObject<T>(xml);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlFilePath"></param>
        /// <returns></returns>
        public static async Task<T> XmlFileToObjectAsync<T>(string xmlFilePath)
        {
            if (string.IsNullOrEmpty(xmlFilePath) || !File.Exists(xmlFilePath))
                return default;

            using (var fileStream = File.OpenRead(xmlFilePath))
            {
                using (var reader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    var xml = await reader.ReadToEndAsync();

                    return XmlToObject<T>(xml);
                }
            }
        }
    }
}
