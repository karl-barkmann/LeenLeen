using CommonServiceLocator;
using Leen.Practices.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Leen.Practices.Tree
{
    /// <summary>
    /// 定义树形结构视图模型。
    /// </summary>
    public abstract partial class TreeViewModel
    {
        private bool _hasResult;
        private bool _showResult;
        private string _searchKeywords;
        private RelayCommand<string> _searchCommand;
        private RelayCommand<object> _locateSearchCommand;
        private IEnumerable<SearchResultViewModel> _searchResults;

        /// <summary>
        /// 
        /// </summary>
        public ICommand LocateSearchCommand
        {
            get
            {
                if (_locateSearchCommand == null)
                    _locateSearchCommand = new RelayCommand<object>(OnLocateSearch, OnCanLocateSearch);
                return _locateSearchCommand;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new RelayCommand<string>(OnSearch, OnCanSearch);

                return _searchCommand;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SearchKeywords
        {
            get => _searchKeywords;
            set
            {
                if (SetProperty(ref _searchKeywords, value, () => SearchKeywords))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        ShowResult = false;
                    }
                    else
                    {
                        IsBusy = true;
                        SearchAsync(value).ContinueWith(x => IsBusy = false);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowResult
        {
            get => _showResult;
            set => SetProperty(ref _showResult, value, () => ShowResult);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasResult
        {
            get => _hasResult;
            set => SetProperty(ref _hasResult, value, () => HasResult);
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<SearchResultViewModel> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value, () => SearchResults);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public Task<IEnumerable<SearchResultViewModel>> SearchAsync(string[] keywords)
        {
            if (keywords == null || keywords.Length < 1 || Nodes == null)
                return null;

            var result = new List<SearchResultViewModel>();
            foreach (var rootNode in Nodes)
            {
                var matchedNodes = SearchNode(rootNode, keywords);
                if (matchedNodes.Count > 0)
                {
                    var vm = new SearchResultViewModel(rootNode.NodeName)
                    {
                        Items = matchedNodes
                    };
                    result.Add(vm);
                }
            }
            return Task.FromResult<IEnumerable<SearchResultViewModel>>(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task LocateAsync(object item)
        {
            await LocateAsyncImpl(item as BaseTreeNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected async virtual Task LocateAsyncImpl(BaseTreeNode item)
        {
            if (item == null)
                return;
            //定位节点之前需要先将树展开，否则父节点不会自动加载子节点
            var nodeDataProvier = ServiceLocator.Current.GetInstance<ITreeNodeDataProvider>();
            var parent = await nodeDataProvier.GetParentNode(item.NodeId);
            if (parent != null)
            {
                var parentNode = await GetNodeAggressivelyAsync(parent.Id);
                if (parentNode != null)
                {
                    await parentNode.ExpandAsync();
                }
            }

            item.IsSelected = true;
        }

        private List<object> SearchNode(BaseTreeNode item, string[] keywords)
        {
            var matchedNodes = new List<object>();
            if (item != BaseTreeNode.Placeholder)
            {
                foreach (var keyword in keywords)
                {
                    if (item.NodeName.Contains(keyword))
                    {
                        matchedNodes.Add(item);
                        break;
                    }
                }

                if (item.Children != null)
                {
                    foreach (var child in item.Children)
                    {
                        var matchedSubNodes = SearchNode(child, keywords);
                        matchedNodes.AddRange(matchedSubNodes);
                    }
                }
            }
            return matchedNodes;
        } 

        private void OnLocateSearch(object arg)
        {
            ShowResult = false;
            _ = LocateAsync(arg);
        }

        private bool OnCanLocateSearch(object arg)
        {
            return true;
        }

        private bool OnCanSearch(string keywords)
        {
            return true;
        }

        private void OnSearch(string keywords)
        {
            if (IsBusy)
                return;

            if (string.IsNullOrEmpty(keywords))
            {
                ShowResult = false;
            }
            else
            {
                IsBusy = true;
                SearchAsync(keywords).ContinueWith(x => IsBusy = false);
            }
        }

        private async Task SearchAsync(string searchKeywords)
        {
            ShowResult = true;
            var keywords = searchKeywords.Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            var result = await SearchAsync(keywords);
            if (result != null && result.Any())
            {
                HasResult = true;
                SearchResults = result;
            }
            else
            {
                HasResult = false;
                SearchResults = null;
            }
        }
    }
}
