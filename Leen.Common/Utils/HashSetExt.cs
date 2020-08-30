using System.Collections.Generic;

namespace Leen.Common.Utils
{
    /// <summary>
    /// 表示值的集扩展类。
    /// </summary>
    /// <typeparam name="T">集的值类型。</typeparam>
    public class HashSetExt<T> : HashSet<T>
    {
        /// <summary>
        /// 获取指定值。
        /// </summary>
        /// <param name="key">值的索引。</param>
        /// <returns></returns>
        public bool this[T key]
        {
            get
            {
                return Contains(key);
            }
            set
            {
                if (value)
                {
                    Add(key);
                }
                else
                {
                    Remove(key);
                }
            }
        }
    }
}
