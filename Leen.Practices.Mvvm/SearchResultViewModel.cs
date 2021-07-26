using Leen.Practices.Mvvm;
using System.Collections.Generic;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 搜索结果视图模型。
    /// </summary>
    public class SearchResultViewModel : BindableBase
    {
        private string _group;
        private IEnumerable<object> _items;

        /// <summary>
        /// 构造<see cref="SearchResultViewModel"/>的实例。
        /// </summary>
        /// <param name="group">搜索结果分组。</param>
        public SearchResultViewModel(string group)
        {
            Group = group;
        }

        /// <summary>
        /// 获取或设置搜索结果分组。
        /// </summary>
        public string Group
        {
            get => _group;
            private set => SetProperty(ref _group, value, () => Group);
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<object> Items
        { 
            get => _items;
            set => SetProperty(ref _items, value, () => Items);
        }
    }
}
