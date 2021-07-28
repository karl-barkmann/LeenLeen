using Leen.Common;
using Leen.Practices.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Leen.Practices.Tree
{
    /// <summary>
    /// 定义树形结构视图模型。
    /// </summary>
    public abstract partial class TreeViewModel : TreeAwareViewModel, ITreeViewModel, ITreeAwareViewModel, ISearchableViewModel
    {
        private readonly object _nodesLocker = new object();
        private ObservableCollection<BaseTreeNode> _nodes = new ObservableCollection<BaseTreeNode>();
        private List<ITreeAssociatedViewModel> _associations;
        private BaseTreeNode _selectedNode;

        private readonly AsyncRelayCommand<BaseTreeNode> _expandAllCommand;
        private readonly RelayCommand<BaseTreeNode> _collapseAllCommand;
        private readonly AsyncRelayCommand<BaseTreeNode> _expandCommand;
        private readonly AsyncRelayCommand<BaseTreeNode> _collapseCommand;
        private readonly AsyncRelayCommand<BaseTreeNode> _toggleCommand;

        /// <summary>
        /// 构造 <see cref="TreeViewModel"/> 的实例。
        /// </summary>
        public TreeViewModel():this(new DefaultTreeBehavior())
        {
           
        }

        /// <summary>
        /// 构造 <see cref="TreeViewModel"/> 的实例。
        /// </summary>
        /// <param name="treeBehavior">树行为描述接口。</param>
        public TreeViewModel(ITreeBehaviorDescriptor treeBehavior)
        {
            Behavior = treeBehavior;
            BindingOperations.EnableCollectionSynchronization(_nodes, _nodesLocker);
            _expandAllCommand = new AsyncRelayCommand<BaseTreeNode>(ExpandAllAsync, CanExpand);
            _collapseAllCommand = new RelayCommand<BaseTreeNode>(CollapseAll, CanCollapse);
            _toggleCommand = new AsyncRelayCommand<BaseTreeNode>(Toggle, CanToggle);
            _expandCommand = new AsyncRelayCommand<BaseTreeNode>(Expand, CanExpand);
            _collapseCommand = new AsyncRelayCommand<BaseTreeNode>(Collapse, CanCollapse);
        }

        /// <summary>
        /// 提供展开该节点及其所有子节点的命令。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ICommand ExpandAllCommand
        {
            get { return _expandAllCommand; }
        }

        /// <summary>
        /// 提供收起该节点及其所有子节点的命令。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ICommand CollapseAllCommand
        {
            get { return _collapseAllCommand; }
        }

        /// <summary>
        /// 提供切换该节点的展开/收起的命令。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ICommand ToggleCommand
        {
            get { return _toggleCommand; }
        }

        /// <summary>
        /// 提供展开该节点的命令。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ICommand ExpandCommand
        {
            get { return _expandCommand; }
        }

        /// <summary>
        /// 提供收起该节点的命令。
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ICommand CollapseCommand
        {
            get { return _collapseCommand; }
        }

        /// <summary>
        /// 获取或设置组织结构树根节点集合。
        /// </summary>
        public ObservableCollection<BaseTreeNode> Nodes
        {
            get => _nodes;
            private set => SetProperty(ref _nodes, value, () => Nodes);
        }

        /// <summary>
        /// 获取组织机构的根节点集合。
        /// </summary>
        IList<BaseTreeNode> ITreeViewModel.Nodes
        {
            get { return _nodes; }
        }

        /// <summary>
        /// 获取或设置当前（最后一个选中）选中节点。
        /// </summary>
        public BaseTreeNode SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                if (!OnNodeSelecting(value))
                    return;
                if (SetProperty(ref _selectedNode, value, () => SelectedNode))
                {
                    OnNodeSelected(value);
                }
            }
        }

        /// <summary>
        /// 获取树行为描述。
        /// </summary>
        public ITreeBehaviorDescriptor Behavior { get; }

        /// <summary>
        /// 获取勾选的树节点。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IGrouping<TreeNodeType, BaseTreeNode>> GetCheckedNodes()
        {
            var result = new List<BaseTreeNode>();
            foreach (var node in Nodes)
            {
                if (node.IsChecked == true)
                {
                    var subCheckedNodes = node.GetCheckedNodesRecursive();
                    if (subCheckedNodes != null)
                    {
                        result.AddRange(subCheckedNodes);
                    }
                    result.Add(node);
                }
            }
            return result.GroupBy(x => x.NodeType);
        }

        /// <summary>
        /// 获取选中的树节点。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BaseTreeNode> GetSelectedNodes()
        {
            var result = new List<BaseTreeNode>();
            foreach (var node in Nodes)
            {
                if (node.IsSelected)
                    result.Add(node);
                var selectedNodes = node.GetSelectedNodesRecursive();
                if (selectedNodes != null)
                    result.AddRange(selectedNodes);
            }

            return result;
        }

        /// <summary>
        /// 根据指定标识查找树节点。
        /// </summary>
        /// <param name="nodeId">节点标识。</param>
        /// <returns></returns>
        public async Task<BaseTreeNode> FindNodeAsync(string nodeId)
        {
            foreach (var node in Nodes)
            {
                if (node.NodeId == nodeId)
                    return node;
                var result = await node.GetNodeRecursiveAsync(nodeId);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// 添加树节点关联视图模型接口。
        /// </summary>
        /// <param name="association">树节点关联视图模型接口</param>
        public void AddAssociation(ITreeAssociatedViewModel association)
        {
            if (association == null)
                throw new ArgumentNullException(nameof(association));
            if (_associations == null)
                _associations = new List<ITreeAssociatedViewModel>();
            _associations.Add(association);
        }

        /// <summary>
        /// 移除树节点关联视图模型接口。
        /// </summary>
        /// <param name="association">树节点关联视图模型接口</param>
        public void RemoveAssociation(ITreeAssociatedViewModel association)
        {
            if (association == null)
                throw new ArgumentNullException(nameof(association));
            if (_associations == null)
                return;
            _associations.Remove(association);
        }

        /// <summary>
        /// 选中符合条件的第一个节点。
        /// </summary>
        /// <param name="predicate">条件筛选回调方法。</param>
        /// <returns></returns>
        public async Task<BaseTreeNode> SelectFirstNode(Predicate<BaseTreeNode> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (var root in Nodes)
            {
                var salerOrgNode = await root.FindNodeRecursiveAsync(predicate);
                if (salerOrgNode != null)
                {
                    salerOrgNode.IsSelected = true;
                    return salerOrgNode;
                }
            }

            return null;
        }

        /// <summary>
        /// 展开符合条件的第一个节点。
        /// </summary>
        /// <param name="predicate">条件筛选回调方法。</param>
        /// <returns></returns>
        public async Task<BaseTreeNode> ExpandFirstNode(Predicate<BaseTreeNode> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var node = await SelectFirstNode(predicate);
            if (node != null)
            {
                await node.ExpandAsync();
            }

            return node;
        }

        /// <summary>
        /// 当初始化时调用。
        /// </summary>
        /// <returns></returns>
        protected async override Task OnInitializeAsync()
        {
            IsBusy = true;

            var treeDataProvider = GetInstance<ITreeNodeDataProvider>();
            var rootNodeData = await treeDataProvider.GetRootNodes();
            if (rootNodeData != null)
            {
                foreach (var nodeData in rootNodeData)
                {
                    var node = CreateNode(nodeData);
                    node.PropertyChanged += OnNodePropertyChanged;
                    Nodes.Add(node);
                }

                var rootNode = Nodes.FirstOrDefault();
                if (rootNode != null)
                {
                    rootNode.IsSelected = Behavior.CanNodeSelectable(rootNode);
                }

                IsBusy = false;

                if (Behavior.IsExpanded)
                {
                    await ExpandAllAsync(Nodes.FirstOrDefault());
                }
                else if (Nodes.Count > 0)
                {
                    await Nodes.First().ExpandAsync();
                }
            }
            else
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// 当选中节点时调用。
        /// </summary>
        /// <param name="node">树节点。</param>
        protected virtual bool OnNodeSelecting(BaseTreeNode node)
        {
            bool result = false;
            if (_associations != null)
            {
                foreach (var association in _associations)
                {
                    result &= association.NotifySelecting(node);
                }
            }
            return result;
        }

        /// <summary>
        /// 当节点被选中时调用。
        /// </summary>
        /// <param name="node">树节点。</param>
        protected virtual void OnNodeSelected(BaseTreeNode node)
        {
            if (_associations != null)
            {
                foreach (var association in _associations)
                {
                    association.NotifySelected(node);
                }
            }
        }
        
        /// <summary>
        /// 创建树节点。
        /// </summary>
        /// <param name="payload">节点实体。</param>
        /// <returns></returns>
        protected abstract BaseTreeNode CreateNode(INamedDataEntity payload);

        /// <summary>
        /// 当创建树节点实体时调用。
        /// </summary>
        /// <param name="payload">节点实体。</param>
        protected override async void OnNodeCreate(INamedCascadeDataEntity payload)
        {
            if (payload.ParentId == null)
            {
                if (Nodes == null)
                    Nodes = new ObservableCollection<BaseTreeNode>();

                Nodes.Add(CreateNode(payload));
            }
            else
            {
                var parent = await FindNodeAsync(payload.ParentId);
                if (parent == null)
                    return;
                var node = CreateNode(payload);
                parent.AddNode(node);
            }
        }

        /// <summary>
        /// 当删除树节点实体时调用。
        /// </summary>
        /// <param name="payload">节点实体。</param>
        protected override async void OnNodeDelete(INamedCascadeDataEntity payload)
        {
            var parent = await FindNodeAsync(payload.ParentId);
            if (parent == null || !parent.IsExpanded)
                return;
            var node = await FindNodeAsync(payload.Id);
            if (node == null)
                return;

            parent.RemoveNode(node);
        }

        /// <summary>
        /// 当更新树节点实体时调用。
        /// </summary>
        /// <param name="payload">节点实体。</param>
        protected override async void OnNodeUpdate(INamedCascadeDataEntity payload)
        {
            var parent = await FindNodeAsync(payload.ParentId);
            if (parent == null || !parent.IsExpanded)
                return;

            var node = await FindNodeAsync(payload.Id);
            if (node == null)
                return;

            node.NodeName = payload.Name;
        }

        private bool CanToggle(BaseTreeNode target)
        {
            return Nodes != null;
        }

        private bool CanCollapse(BaseTreeNode target)
        {
            return Nodes != null && Nodes.Any(node => node.CanCollapse());
        }

        private bool CanExpand(BaseTreeNode target)
        {
            return Nodes != null && Nodes.Any(node => node.CanExpand());
        }

        private async Task Expand(BaseTreeNode target)
        {
            await target.ExpandAsync();
        }

        private async Task Toggle(BaseTreeNode target)
        {
            await target.ToggleAsync();
        }

        private Task Collapse(BaseTreeNode target)
        {
            throw new NotImplementedException();
        }

        private void CollapseAll(BaseTreeNode target)
        {
            if (target == null)
            {
                if (Nodes != null)
                {
                    foreach (var node in Nodes)
                    {
                        if (node.CanCollapse())
                        {
                            node.CollapseAll();
                        }
                    }
                }
            }
            else
            {
                target.CollapseAll();
            }
        }

        private async Task ExpandAllAsync(BaseTreeNode target)
        {
            if (target == null)
            {
                await ExpandAllAsync();
            }
            else
            {
                await target.ExpandAllAsync();
            }
        }

        private async Task ExpandAllAsync()
        {
            if (Nodes != null)
            {
                foreach (var node in Nodes)
                {
                    if (node.CanExpand())
                    {
                        await node.ExpandAllAsync();
                    }
                }
            }
        }

        private void OnNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            BaseTreeNode target = sender as BaseTreeNode;
            if (target == null)
                return;
            if (e.PropertyName == ExtractPropertyName(() => target.IsLoadingChildren))
            {
                IsBusy = Nodes.Any(node => node.IsLoadingChildren);
            }
        }
    }
}
