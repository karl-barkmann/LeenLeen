using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Leen.Common.Utils
{
    /// <summary>
    /// 将Csv文件读取为对象集合，支持使用<see cref="CsvColumnAttribute"/>特性自动解析。
    /// </summary>
    public sealed class CsvReader<T> where T : new()
    {
        private readonly CsvColumn[] _columns;

        private CsvReader(PropertyInfo[] properties)
        {
            _columns = properties.Select(x => new CsvColumn(x)).ToArray();
            IgnoreInvalidRow = true;
        }

        private Func<string[], T> ValueConverter { get; set; }

        private Func<Dictionary<string, string>, T> NameConventionValueConverter { get; set; }

        /// <summary>
        /// 创建指定类型的Csv文件读取器，并指定值转换器。
        /// </summary>
        /// <param name="valueConverter">根据值字段集合进行转换的转换器。</param>
        /// <returns></returns>
        public static CsvReader<T> Create(Func<string[], T> valueConverter)
        {
            var properties = typeof(T).GetProperties();
            var reader = new CsvReader<T>(properties);
            reader.ValueConverter = valueConverter;
            reader.SkipHeader = true;
            return reader;
        }

        /// <summary>
        /// 创建指定类型的Csv文件读取器，并指定值转换器。
        /// </summary>
        /// <param name="valueConverter">根据表头和值字段字典进行转换的转换器。</param>
        /// <returns></returns>
        public static CsvReader<T> Create(Func<Dictionary<string, string>, T> valueConverter)
        {
            var properties = typeof(T).GetProperties();
            var reader = new CsvReader<T>(properties);
            reader.NameConventionValueConverter = valueConverter;
            reader.SkipHeader = false;
            return reader;
        }

        /// <summary>
        /// 创建指定类型的Csv文件读取器。
        /// </summary>
        /// <returns></returns>
        public static CsvReader<T> Create()
        {
            return Create(true);
        }

        /// <summary>
        /// 创建指定类型的Csv文件读取器，并指示是否忽略表头。
        /// </summary>
        /// <param name="skipHeader">是否忽略表头。</param>
        /// <returns></returns>
        public static CsvReader<T> Create(bool skipHeader)
        {
            var properties = typeof(T).GetProperties();
            var reader = new CsvReader<T>(properties);
            reader.SkipHeader = skipHeader;
            return reader;
        }

        /// <summary>
        /// 获取或设置一个值指示是否跳过表头。
        /// </summary>
        public bool SkipHeader { get; set; }

        /// <summary>
        /// 获取或设置一个值指示是否忽略无效行或错误行。
        /// </summary>
        public bool IgnoreInvalidRow { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="invalidRowCallback"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ReadFile(string fileName, Action<string> invalidRowCallback)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("文件名不能为空", nameof(fileName));

            if (!File.Exists(fileName))
                throw new FileNotFoundException("指定的文件名不存在", fileName);

            var startRowIndex = SkipHeader ? 1 : 0;
            List<T> results = new List<T>();
            using (var reader = File.OpenText(fileName))
            {
                if (!reader.BaseStream.CanRead || reader.EndOfStream)
                    return results;

                int currentRowsIndex = 0;
                string[] headers = null;
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync().ConfigureAwait(false);
                    var cells = line.Split(new char[] { ',' });
                    if (currentRowsIndex++ < startRowIndex)
                    {
                        headers = cells;
                        continue;
                    }

                    if (cells == null || cells.Length < 1)
                    {
                        invalidRowCallback?.Invoke(line);
                        continue;
                    }

                    T value = default;
                    if (ValueConverter == null && NameConventionValueConverter == null)
                    {
                        value = ReflectionValueConverter(ZipToDic(headers, cells));
                    }
                    else
                    {
                        if (SkipHeader)
                        {
                            value = ValueConverter(cells);
                        }
                        else
                        {

                            value = NameConventionValueConverter(ZipToDic(headers, cells));
                        }
                    }

                    if (value == null && IgnoreInvalidRow)
                    {
                        invalidRowCallback?.Invoke(line);
                        continue;
                    }
                    results.Add(value);
                }
            }
            return results;
        }

        private Dictionary<string, string> ZipToDic(string[] keys, string[] values)
        {
            var result = new Dictionary<string, string>(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                string key;
                if (keys.Length <= i)
                {
                    key = $"Key@{i}";
                }
                else if (result.ContainsKey(keys[i]))
                {
                    key = $"{keys[i]}@{i}";
                }
                else
                {
                    key = keys[i];
                }

                result.Add(key, values[i]);
            }

            return result;
        }

        private T ReflectionValueConverter(Dictionary<string, string> values)
        {
            var result = new T();
            var nameConvetion = _columns.Any(x => !string.IsNullOrEmpty(x.Header) && !string.IsNullOrWhiteSpace(x.Header));
            if (nameConvetion)
            {
                foreach (var column in _columns)
                {
                    if (values.ContainsKey(column.Header))
                    {
                        var value = values[column.Header];
                        var convertedValue = TypeDescriptor.GetConverter(column.Property.PropertyType).ConvertFromString(value);
                        column.Property.SetValue(result, convertedValue);
                    }
                }
            }
            else
            {
                var cells = values.Values.ToArray();
                foreach (var column in _columns.OrderBy(x => x.Index))
                {
                    if (column.Index.HasValue && column.Index.Value < cells.Length)
                    {
                        var value = cells[column.Index.Value];
                        var convertedValue = TypeDescriptor.GetConverter(column.Property.PropertyType).ConvertFromString(value);
                        column.Property.SetValue(result, convertedValue);
                    }
                }
            }

            return result;
        }

        private class CsvColumn
        {
            private CsvColumnAttribute _attribute;

            public CsvColumn(PropertyInfo propertyInfo)
            {
                Property = propertyInfo;
            }

            public string Header
            {
                get
                {
                    if (_attribute == null)
                    {
                        _attribute = Property.GetCustomAttribute<CsvColumnAttribute>();
                    }

                    return _attribute?.Header;
                }
            }

            public int? Index
            {
                get
                {
                    if (_attribute == null)
                    {
                        _attribute = Property.GetCustomAttribute<CsvColumnAttribute>();
                    }
                    return _attribute?.Index;
                }
            }

            public PropertyInfo Property { get; }
        }
    }

    /// <summary>
    /// 定义类型转换为Csv文件时字段表头。
    /// </summary>
    public class CsvColumnAttribute : Attribute
    {
        /// <summary>
        /// 构造 <see cref="CsvColumnAttribute"/> 实例。
        /// </summary>
        public CsvColumnAttribute()
        {
        }

        /// <summary>
        /// 获取或设置表头名称。
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// 获取或设置表头索引。
        /// </summary>
        public int Index { get; set; }
    }
}
