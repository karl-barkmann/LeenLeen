using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Leen.Common.Utils
{
    /// <summary>
    /// 支持根据<see cref="CsvColumnAttribute"/>特性将对象写入本地文件。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CsvWriter<T> where T : new()
    {
        private readonly PropertyInfo[] _properties;

        private CsvWriter(PropertyInfo[] properties)
        {
            _properties = properties;
        }

        /// <summary>
        /// 创建指定类型示例的写入器。
        /// </summary>
        /// <returns></returns>
        public static CsvWriter<T> Create()
        {
            var properties = typeof(T).GetProperties();
            return new CsvWriter<T>(properties);
        }

        /// <summary>
        /// 将指定类型对象集合写入文件中。
        /// </summary>
        /// <param name="fileName">csv文件名。</param>
        /// <param name="data">类型对象集合。</param>
        /// <returns></returns>
        public async Task WriteFile(string fileName, IEnumerable<T> data)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name can not to be null or empty.", nameof(fileName));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            FileStream fileStream = null;
            StreamWriter writer = null;
            try
            {
                if (File.Exists(fileName))
                    fileStream = File.Open(fileName, FileMode.Truncate);
                else
                    fileStream = File.Create(fileName);

                fileStream.Seek(0, SeekOrigin.Begin);
                writer = new StreamWriter(fileStream, Encoding.UTF8);

                var columnHeaders =  new List<string>(_properties.Select(x => x.Name));

                await writer.WriteLineAsync(string.Join(",", columnHeaders)).ConfigureAwait(false);

                List<string> values = new List<string>();
                foreach (var item in data)
                {
                    values.Clear();
                    foreach (var property in _properties)
                    {
                        var value = property.GetValue(item);
                        values.Add(value?.ToString());
                    }
                    await writer.WriteLineAsync(string.Join(",", values)).ConfigureAwait(false);
                }

                await writer.FlushAsync().ConfigureAwait(false);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
                if (fileStream != null)
                    fileStream.Dispose();
            }
        }
    }
}
