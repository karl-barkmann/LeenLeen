using System;
using System.Collections;
using System.Collections.Generic;

namespace Leen.Common
{
    /// <summary>
    /// 表示可通过索引访问的对象的强类型列表。 提供用于并发对列表进行搜索、排序和操作的方法。
    /// </summary>
    /// <typeparam name="T">列表中元素的类型。</typeparam>
    public class ConcurrentList<T> : IList<T>, IList
    {
        private readonly object locker = new object();
        private readonly List<T> source;

        /// <summary>
        /// 构造并发强类型列表的实例。
        /// </summary>
        public ConcurrentList()
        {
            source = new List<T>();
        }

        /// <summary>
        /// 构造并发强类型列表的实例。
        /// </summary>
        /// <param name="capcity">初始容量</param>
        public ConcurrentList(int capcity)
        {
            source = new List<T>(capcity);
        }

        /// <summary>
        /// 构造并发强类型列表的实例。
        /// </summary>
        /// <param name="source">初始元素集合。</param>
        public ConcurrentList(IEnumerable<T> source)
        {
            this.source = new List<T>(source);
        }

        /// <summary>
        /// 获取元素项在列表中的索引。
        /// </summary>
        /// <param name="item">元素项。</param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            lock (locker)
            {
                return source.IndexOf(item);
            }
        }

        /// <summary>
        /// 在列表指定索引位置插入元素。
        /// </summary>
        /// <param name="index">插入的索引。</param>
        /// <param name="item">插入的元素。</param>
        public void Insert(int index, T item)
        {
            lock (locker)
            {
                source.Insert(index, item);
            }
        }

        /// <summary>
        /// 移除位于指定索引的元素。
        /// </summary>
        /// <param name="index">要移除的元素的索引。</param>
        public void RemoveAt(int index)
        {
            lock (locker)
            {
                source.RemoveAt(index);
            }
        }

        /// <summary>
        /// 获取或设置位于指定索引的元素。
        /// </summary>
        /// <param name="index">要设置或获取的索引。</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                lock (locker)
                {
                    return source[index];
                }
            }
            set
            {
                lock (locker)
                {
                    source[index] = value;
                }
            }
        }

        /// <summary>
        /// 添加元素的列表末尾,如果指定元素已经存在则更新。
        /// </summary>
        /// <param name="value">要添加的元素。</param>
        public void Add(T value)
        {
            lock (locker)
            {
                source.Add(value);
            }
        }

        /// <summary>
        /// 清除列表。
        /// </summary>
        public void Clear()
        {
            lock (locker)
            {
                source.Clear();
            }
        }

        /// <summary>
        /// 确定元素是否包含于列表中。
        /// </summary>
        /// <param name="item">要查找的元素。</param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            lock (locker)
            {
                return source.Contains(item);
            }
        }

        /// <summary>
        /// 从数组指定位置开始复制列表中元素到数组。
        /// </summary>
        /// <param name="array">要复制到的数组。</param>
        /// <param name="arrayIndex">开始放置元素的索引。</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (locker)
            {
                source.CopyTo(array, arrayIndex);
            }
        }

        /// <summary>
        /// 获取列表中元素个数。
        /// </summary>
        public int Count
        {
            get
            {
                lock (locker)
                {
                    return source.Count;
                }
            }
        }

        /// <summary>
        /// 获取一个值，指示列表是否只读。
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 从列表中移除指定元素。
        /// </summary>
        /// <param name="item">要移除的元素。</param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            lock (locker)
            {
                return source.Remove(item);
            }
        }

        /// <summary>
        /// 返回循环访问的枚举器。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return source.GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问的枚举器。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 添加元素到列表结尾，并返回添加项的索引。
        /// </summary>
        /// <param name="value">要添加的元素。</param>
        /// <returns></returns>
        public int Add(object value)
        {
            lock (locker)
            {
                source.Add(ParseValue(value));
                return source.Count - 1;
            }
        }

        /// <summary>
        /// 确定元素是否包含于列表中。
        /// </summary>
        /// <param name="value">要查找的元素。</param>
        /// <returns></returns>
        public bool Contains(object value)
        {
            lock (locker)
            {
                return source.Contains(ParseValue(value));
            }
        }

        /// <summary>
        /// 确定元素在列表中的索引。
        /// </summary>
        /// <param name="value">要查找的元素。</param>
        /// <returns></returns>
        public int IndexOf(object value)
        {
            lock (locker)
            {
                return source.IndexOf(ParseValue(value));
            }
        }

        /// <summary>
        /// 在指定索引处插入元素。
        /// </summary>
        /// <param name="index">要插入的列表中德索引。</param>
        /// <param name="value">要插入的元素。</param>
        public void Insert(int index, object value)
        {
            lock (locker)
            {
                source.Insert(index, ParseValue(value));
            }
        }

        /// <summary>
        /// 获取一个值，指示列表容量是否为固定大小。
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// 从列表中移除元素。
        /// </summary>
        /// <param name="value">要移除的元素。</param>
        public void Remove(object value)
        {
            lock (locker)
            {
                source.Remove(ParseValue(value));
            }
        }

        /// <summary>
        /// 获取或设置位于指定索引的元素。
        /// </summary>
        /// <param name="index">要获取或设置的元素的索引。</param>
        /// <returns></returns>
        object IList.this[int index]
        {
            get
            {
                lock (locker)
                {
                    return source[index];
                }
            }
            set
            {
                lock (locker)
                {
                    source[index] = ParseValue(value);
                }
            }
        }

        /// <summary>
        /// 从数组的指定位置开始复制元素到数组中。
        /// </summary>
        /// <param name="array">要负责到的数组。</param>
        /// <param name="index">开始复制的索引。</param>
        public void CopyTo(Array array, int index)
        {
            lock (locker)
            {
                int count = 0;
                foreach (T item in source)
                {
                    if (index > count++)
                        continue;
                    array.SetValue(item, count);
                }
            }
        }

        /// <summary>
        /// 获取一个值，指示列表是否线程安全。
        /// </summary>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// 获取集合同步锁定的对象。
        /// </summary>
        public object SyncRoot
        {
            get { return locker; }
        }

        /// <summary>
        /// 查找从第一个元素到最后一个元素的范围内第一个匹配元素的从零开始的索引。
        /// </summary>
        /// <param name="match">定义搜索元素的条件。</param>
        /// <returns></returns>
        public int FindIndex(Predicate<T> match)
        {
            return FindIndex(0, match);
        }

        /// <summary>
        /// 查找从指定索引到最后一个元素的范围内第一个匹配元素的从零开始的索引。
        /// </summary>
        /// <param name="startIndex">从零开始的搜索的起始索引。</param>
        /// <param name="match">定义搜索元素的条件。</param>
        /// <returns></returns>
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            lock (locker)
            {
                return source.FindIndex(startIndex, match);
            }
        }

        /// <summary>
        /// 将元素列表复制到新的数组中。
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            lock (locker)
            {
                return source.ToArray();
            }
        }

        /// <summary>
        /// 查找从指定索引并包含指定元素数量的范围内第一个匹配元素的从零开始的索引。
        /// </summary>
        /// <param name="startIndex">从零开始的搜索的起始索引。</param>
        /// <param name="count">要匹配的元素数量。</param>
        /// <param name="match">定义搜索元素的条件。</param>
        /// <returns></returns>
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            lock (locker)
            {
                return source.FindIndex(startIndex, count, match);
            }
        }

        /// <summary>
        /// 对每个元素执行指定操作。
        /// </summary>
        /// <param name="action">要对每个元素执行的委托。</param>
        public void Foreach(Action<T> action)
        {
            lock (locker)
            {
                source.ForEach(action);
            }
        }

        /// <summary>
        /// 对列表内的元素进行排序。
        /// </summary>
        public void Sort()
        {
            lock (locker)
            {
                source.Sort();
            }
        }

        /// <summary>
        /// 对列表内的元素进行排序。
        /// </summary>
        /// <param name="comparsion">比较元素时要使用的对比委托。</param>
        public void Sort(Comparison<T> comparsion)
        {
            lock (locker)
            {
                source.Sort(comparsion);
            }
        }

        private static T ParseValue(object value)
        {
            if (value == null)
            {
                if (typeof(T).IsValueType)
                {
                    throw new ArgumentException("当泛型列表初始化为值类型列表时不能插入null值。");
                }
                return default;
            }
            else
            {
                T result = (T)value;
                if (result == null)
                {
                    throw new ArgumentException(
                        String.Format("要插入的元素类型'{0}'与列表初始化的泛型类型不匹配。", typeof(T)));
                }
                else
                {
                    return result;
                }
            }
        }
    }
}
