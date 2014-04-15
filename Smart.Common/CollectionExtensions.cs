using System.Collections.Generic;
using System.Linq;

namespace Smart.Common
{
    /// <summary>
    /// ICollection接口扩展。
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 添加一组对象到集合中。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="target">目标集合。</param>
        /// <param name="source">源集合。</param>
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (EnumerableExtensions.CanEnumerable<T>(source))
            {
                foreach (var item in source.ToArray())
                {
                    target.Add(item);
                }
            }
        }

        /// <summary>
        /// 添加目标集合指定数量的对象到集合中。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="target">目标集合。</param>
        /// <param name="source">源集合。</param>
        /// <param name="count">需要提取元素的数量。</param>
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source, int count)
        {
            if (EnumerableExtensions.CanEnumerable<T>(source))
            {
                if (count >= source.Count())
                    target.AddRange(source);
                else
                {
                    var array = source.ToArray();
                    for (int i = 0; i < count; i++)
                    {
                        target.Add(array[i]);
                    }
                }
            }
        }
    }
}
