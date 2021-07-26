using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 定义支持搜索的视图模型。
    /// </summary>
    public interface ISearchableViewModel
    {
        /// <summary>
        /// 搜索视图模型。
        /// </summary>
        /// <param name="keywords">搜索关键字</param>
        /// <returns></returns>
        Task<IEnumerable<SearchResultViewModel>> SearchAsync(string[] keywords);

        /// <summary>
        /// 定位搜索结果。
        /// </summary>
        /// <param name="item">需要定位的搜索结果。</param>
        /// <returns></returns>
        Task LocateAsync(object item);
    }
}
