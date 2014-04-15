using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Smart.Common
{
    /// <summary>
    /// IEnumerable接口扩展。
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 是否可以在此集合上进行Linq查询。
        /// <para>避免集合为空或集合元素个数为零造成的错误。</para>
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="source">集合。</param>
        /// <returns></returns>
        public static bool CanEnumerable<T>(IEnumerable<T> source)
        {
            return source != null && source.Count() > 0;
        }

        /// <summary>
        /// 对集合中的所有元素执行指定操作。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="source">集合。</param>
        /// <param name="action">指定操作。</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (action == null)
                return;
            if (CanEnumerable(source))
            {
                foreach (var item in source.ToArray())
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// 对集合中的所有元素执行指定操作，且当退出条件满足时退出。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="source">集合。</param>
        /// <param name="action">指定操作。</param>
        /// <param name="brekFunc">条件计算式。</param>
        public static void ForEachWhile<T>(this IEnumerable<T> source, Action<T> action, Func<T, bool> brekFunc)
        {
            if (action == null)
                return;
            if (CanEnumerable(source))
            {
                foreach (var item in source.ToArray())
                {
                    if (brekFunc(item))
                        break;
                    action(item);
                }
            }
        }

        /// <summary>
        /// 对集合中的所有元素执行指定操作，且当继续条件满足时跳过继续。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="source">集合。</param>
        /// <param name="action">指定操作。</param>
        /// <param name="continueFunc">继续条件计算式。</param>
        public static void ForEachContinue<T>(this IEnumerable<T> source, Action<T> action, Func<T, bool> continueFunc)
        {
            if (action == null)
                return;
            if (CanEnumerable(source))
            {
                foreach (var item in source.ToArray())
                {
                    if (continueFunc(item))
                        continue;
                    action(item);
                }
            }
        }

        /// <summary>
        /// 对集合中的所有元素执行指定操作，且当退出条件满足时退出或继续条件满足时跳过继续。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="source">集合。</param>
        /// <param name="action">指定操作。</param>
        /// <param name="brekFunc">退出条件计算式。</param>
        /// <param name="continueFunc">继续条件计算式。</param>
        public static void ForEachWhile<T>(this IEnumerable<T> source, Action<T> action, Func<T, bool> brekFunc, Func<T, bool> continueFunc)
        {
            if (action == null)
                return;
            if (CanEnumerable(source))
            {
                foreach (var item in source.ToArray())
                {
                    if (brekFunc(item))
                        break;
                    if (continueFunc(item))
                        continue;
                    action(item);
                }
            }
        }

        /// <summary>
        /// 对集合中的所有元素执行指定操作。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="source">集合。</param>
        /// <param name="action">指定操作。</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (action == null)
                return;
            int index = 0;
            if (CanEnumerable(source))
            {
                foreach (var element in source)
                {
                    action(element, index);
                    ++index;
                }
            }
        }

        /// <summary>
        /// 对集合中的所有元素执行指定操作，且当退出条件满足时退出。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="source">集合。</param>
        /// <param name="action">指定操作。</param>
        /// <param name="brekFunc">条件计算式。</param>
        public static void ForEachWhile<T>(this IEnumerable<T> source, Action<T, int> action, Func<T, bool> brekFunc)
        {
            if (action == null)
                return;
            int index = 0;
            if (CanEnumerable(source))
            {
                foreach (var element in source)
                {
                    if (brekFunc(element))
                        break;
                    action(element, index);
                    ++index;
                }
            }
        }

        /// <summary>
        /// 对集合中的所有元素执行指定操作，且当继续条件满足时跳过继续。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="source">集合。</param>
        /// <param name="action">指定操作。</param>
        /// <param name="continueFunc">继续条件计算式。</param>
        public static void ForEachContinue<T>(this IEnumerable<T> source, Action<T, int> action, Func<T, bool> continueFunc)
        {
            if (action == null)
                return;
            int index = 0;
            if (CanEnumerable(source))
            {
                foreach (var element in source)
                {
                    if (continueFunc(element))
                    {
                        ++index;
                        continue;
                    }
                    action(element, index);
                    ++index;
                }
            }
        }

        /// <summary>
        /// 对集合中的所有元素执行指定操作，且当退出条件满足时退出或继续条件满足时跳过继续。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="source">集合。</param>
        /// <param name="action">指定操作。</param>
        /// <param name="brekFunc">退出条件计算式。</param>
        /// <param name="continueFunc">继续条件计算式。</param>
        public static void ForEachWhile<T>(this IEnumerable<T> source, Action<T, int> action, Func<T, bool> brekFunc, Func<T, bool> continueFunc)
        {
            if (action == null)
                return;
            int index = 0;
            if (CanEnumerable(source))
            {
                foreach (var element in source)
                {
                    if (brekFunc(element))
                        break;
                    if (continueFunc(element))
                    {
                        ++index;
                        continue;
                    }
                    action(element, index);
                    ++index;
                }
            }
        }

        /// <summary>
        /// 提取集合中从指定索引开始指定数量的元素。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="source">集合。</param>
        /// <param name="index">开始的索引。</param>
        /// <param name="count">提取的数量。</param>
        /// <returns></returns>
        public static IEnumerable<T> Cut<T>(this IEnumerable<T> source, int index, int count)
        {
            if (!CanEnumerable(source))
                return null;
            if (index > source.Count())
                return null;
            if (index < -1)
                throw new ArgumentException("index", "index必须大于等于零");
            if ((index + count) <= source.Count())
                return source;

            lock (source)
            {
                List<T> result = new List<T>();
                var array = source.ToArray();
                for (int i = index; i < count; i++)
                {
                    result.Add(array[i]);
                }
                return result;
            }
        }

        /// <summary>
        /// 提取排序后指定数量的元素。
        /// </summary>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <param name="source">集合。</param>
        /// <param name="keySelector">排序筛选器。</param>
        /// <param name="top">提取的数量。</param>
        /// <returns></returns>
        public static IEnumerable<T> Top<T>(this IEnumerable<T> source, Func<T, int> keySelector, int top)
        {
            if (!CanEnumerable(source))
                return null;

            if (top <= 0)
                return null;

            if (keySelector == null)
                source.Take(top);

            var result = source.OrderByDescending(keySelector);

            return result.Take(top);
        }

        public static IEnumerable<T> Select<T>(this IEnumerable src, Func<Object, T> transform)
        {
            if (src == null)
            {
                return null;
            }
            return SelectImpl(src, transform);
        }

        public static IEnumerable Where(this IEnumerable src, Func<Object, bool> predicate)
        {
            if (src == null)
            {
                return null;
            }
            return WhereImpl(src, predicate);
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> src, Func<T> factory)
        {
            if (src == null)
            {
                return factory();
            }

            using (var itor = src.GetEnumerator())
            {
                if (itor.MoveNext())
                {
                    return itor.Current;
                }
            }
            return factory();
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> src, Func<T, bool> predicate, Func<T> factory)
        {
            if (src == null)
            {
                return factory();
            }
            using (var itor = src.Where(predicate).GetEnumerator())
            {
                if (itor.MoveNext())
                {
                    return itor.Current;
                }
            }
            return factory();
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> src, T head)
        {
            if (src == null)
            {
                return Enumerable.Repeat(head, 1);
            }
            else
            {
                return Enumerable.Repeat(head, 1).Concat(src);
            }
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> src, T tail)
        {
            if (src == null)
            {
                return Enumerable.Repeat(tail, 1);
            }
            else
            {
                return src.Concat(Enumerable.Repeat(tail, 1));
            }
        }

        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> src, int times)
        {
            if (src == null)
            {
                yield break;
            }
            for (int i = 0; i < times; ++i)
            {
                foreach (var x in src)
                {
                    yield return x;
                }
            }
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values. 
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The maximal element, according to the projection.</returns>

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer = null) where TSource : class
        {
            if (source == null)
            {
                return default(TSource);
            }
            if (comparer == null)
            {
                comparer = Comparer<TKey>.Default;
            }
            using (var itor = source.GetEnumerator())
            {
                if (!itor.MoveNext())
                {
                    return default(TSource);
                }
                TSource max = default(TSource);
                TKey maxKey = selector(max);
                while (itor.MoveNext())
                {
                    var v = itor.Current;
                    var p = selector(v);
                    if (comparer.Compare(p, maxKey) > 0)
                    {
                        max = v;
                        maxKey = p;
                    }
                }
                return max;
            }
        }

        public static bool IsSame<T>(this IEnumerable<T> source, IEnumerable<T> comparand)
        {
            if ((object)source == null)
            {
                return (object)comparand == null;
            }
            if ((object)comparand == null)
            {
                return false;
            }
            using (var sit = source.GetEnumerator())
            {
                using (var cit = comparand.GetEnumerator())
                {
                    do
                    {
                        if (!sit.MoveNext())
                        {
                            return !cit.MoveNext();
                        }
                        if (!cit.MoveNext())
                        {
                            return false;
                        }
                    } while (Object.Equals(sit.Current, cit.Current));
                    return false;
                }
            }
        }

        #region 私有方法

        private static IEnumerable WhereImpl(this IEnumerable src, Func<Object, bool> predicate)
        {
            foreach (var x in src)
            {
                if (predicate(x))
                {
                    yield return x;
                }
            }
        }

        private static IEnumerable<T> SelectImpl<T>(IEnumerable src, Func<Object, T> transform)
        {
            foreach (var x in src)
            {
                yield return transform(x);
            }
        }

        #endregion
    }
}
