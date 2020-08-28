using Leen.Practices.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 定义组织结构和设备树节点。
    /// </summary>
    public abstract class BaseTreeNode : BindableBase, IEquatable<BaseTreeNode>, IEqualityComparer<BaseTreeNode>, IComparer<BaseTreeNode>, IComparable<BaseTreeNode>
    {
        #region fields

        private static readonly AllEnabledNodeBehavior DefaultBehavior = new AllEnabledNodeBehavior();
        /// <summary>
        /// 一个占位节点，用于在父节点折叠时使其仍然显示展开符号。
        /// </summary>
        internal static readonly PlaceholderNode Placeholder = new PlaceholderNode();
        internal static readonly ObservableCollection<BaseTreeNode> PlaceHolderChildren = new ObservableCollection<BaseTreeNode>(new List<BaseTreeNode>(1) { Placeholder });
        private readonly static TaskCompletionSource<IEnumerable<BaseTreeNode>> s_TaskCompletionSource = 
            new TaskCompletionSource<IEnumerable<BaseTreeNode>>();

        internal const byte IsCheckedMask = 0x01;
        internal const byte IsSelectedMask = 0x02;
        internal const byte IsExpandedMask = 0x04;
        internal const byte IsLoadingChildrenMask = 0x08;
        internal const byte HasChildrenMask = 0x10;

        internal const byte SelectableMask = 0x01;
        internal const byte CheckableMask = 0x02;
        internal const byte ExpandableMask = 0x04;
        internal const byte EnabledMask = 0x08;

        private byte _internalStateFlags = 0x00;
        private byte _internalBehaviorFlags = 0x00;
        private string _nodeName;
        private ObservableCollection<BaseTreeNode> _children;
        private TreeNodeType _nodeType;
        private int _level;
        private int _childrenCount;

        //private bool? _isChecked;
        //private bool _isSelected;
        //private bool _checkable = false;
        //private bool _selectable = true;
        //private bool _isExpanded;
        //private bool _isLoadingChildren;

        private readonly RelayCommand _expandCommand;
        private readonly RelayCommand _collapseCommand;
        private readonly RelayCommand _expandAllCommand;
        private readonly RelayCommand _collapseAllCommand;
        private readonly RelayCommand _toggleCommand;

        #endregion

        #region consturctor

        static BaseTreeNode()
        {
            s_TaskCompletionSource.SetResult(null);
        }

        /// <summary>
        /// 构造<see cref="BaseTreeNode"/>的实例。
        /// <paramref name="nodeId">节点唯一标识</paramref>。
        /// <paramref name="nodeType">节点类型。</paramref>
        /// </summary>
        protected BaseTreeNode(string nodeId, TreeNodeType nodeType)
        {
            if (string.IsNullOrEmpty(nodeId))
                throw new ArgumentException("节点标识不应为空", nameof(nodeId));
            NodeId = nodeId;
            NodeType = nodeType;
            Behavior = DefaultBehavior;
            Behave();
            //_toggleCommand = new RelayCommand(Toggle, CanToggle);
            //_expandCommand = new RelayCommand(InternalExpand, CanExpand);
            //_expandAllCommand = new RelayCommand(async () => await InternalExpandAll(), CanExpand);
            //_collapseCommand = new RelayCommand(InternalCollapse, CanCollapse);
            //_collapseAllCommand = new RelayCommand(CollapseAll, CanCollapse);
        }

        #endregion

        #region properties

        /// <summary>
        /// 获取树节点行为描述接口。
        /// </summary>
        public ITreeNodeBehavior Behavior { get; private set; }

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
        /// 获取该组织机构节点上包含的直属子组织节点数目和直属设备数量。
        /// <remarks>
        /// 通过该值来决定是否提供树节点的折叠选项。
        /// </remarks>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int ChildrenCount
        {
            get { return _childrenCount; }
            private set
            {
                SetProperty(ref _childrenCount, value, nameof(ChildrenCount));
            }
        }

        /// <summary>
        /// 获取或设置该节点的树节点深度。
        /// </summary>
        public int Level
        {
            get { return _level; }
            internal set
            {
                SetProperty(ref _level, value, nameof(Level));
            }
        }

        /// <summary>
        /// 获取一个值指示该节点是一个占位节点。
        /// </summary>
        public bool IsPlaceHolder
        {
            get { return this == Placeholder; }
        }

        /// <summary>
        /// 获取或设置一个值，指示该节点是否启用。
        /// </summary>
        public bool IsEnabled
        {
            get { return GetInternalBehaviorFlag(EnabledMask); }
            set
            {
                SetInternalBehaviorFlag(value, EnabledMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该节点是否支持展开。
        /// </summary>
        public bool Expandable
        {
            get { return GetInternalBehaviorFlag(ExpandableMask); }
            set
            {
                if (SetInternalBehaviorFlag(value, ExpandableMask))
                {
                    if (value)
                    {
                        //If there are no reserved children
                        //we should hold a placeholder to enable node toggling
                        if (Children == null || Children.Count < 1)
                            Children = PlaceHolderChildren;
                    }
                    else
                    {
                        if (Children == PlaceHolderChildren)
                            Children = null;
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该节点是否已展开。
        /// <para>
        /// 此属性仅用于界面绑定，如需从代码中展开节点应调用<see cref="ExpandAsync"/>。
        /// </para>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsExpanded
        {
            get { return GetInternalStateFlag(IsExpandedMask); }
            set
            {
                if (!IsEnabled || !Expandable)
                    return;
                if (SetInternalStateFlag(value, IsExpandedMask))
                {
                    if (value)
                    {
                        _ = PopulateChildren();
                    }
                    else
                    {
                        ClearChildren();
                        //If there are no reserved children
                        //we should hold a placeholder to enable node toggling
                        if ((Children == null || Children.Count < 1) && GetInternalBehaviorFlag(ExpandableMask))
                        {
                            Children = PlaceHolderChildren;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该节点是否支持选中。
        /// </summary>
        public bool Selectable
        {
            get { return GetInternalBehaviorFlag(SelectableMask); }
            set
            {
                SetInternalBehaviorFlag(value, SelectableMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该节点是否支持勾选。
        /// </summary>
        public bool Checkable
        {
            get { return GetInternalBehaviorFlag(CheckableMask); }
            set
            {
                SetInternalBehaviorFlag(value, CheckableMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该节点是否被选中。
        /// </summary>
        public bool IsSelected
        {
            get { return GetInternalStateFlag(IsSelectedMask); }
            set
            {
                if (!IsEnabled || !Selectable)
                    return;
                SetInternalStateFlag(value, IsSelectedMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该节点是否被勾选。
        /// </summary>
        public bool? IsChecked
        {
            get { return GetInternalStateFlag(IsCheckedMask); }
            set
            {
                if (!IsEnabled || !Checkable)
                    return;
                if (SetInternalStateFlag(value, IsCheckedMask))
                {
                    if (Children != null && Behavior.CanCheckedBeInherited)
                    {
                        for (int i = 0; i < Children.Count; i++)
                        {
                            Children[i].IsChecked = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取此节点的标识。
        /// </summary>
        public string NodeId { get; }

        /// <summary>
        /// 获取或设置节点类型。
        /// </summary>
        public TreeNodeType NodeType
        {
            get { return _nodeType; }
            private set
            {
                SetProperty(ref _nodeType, value, nameof(NodeType));
            }
        }

        /// <summary>
        /// 获取或设置节点名称。
        /// </summary>
        public string NodeName
        {
            get { return _nodeName; }
            set
            {
                SetProperty(ref _nodeName, value, nameof(NodeName));
            }
        }

        /// <summary>
        /// 获取一个值指示该节点是否正在加载子节点。
        /// </summary>
        public bool IsLoadingChildren
        {
            get { return GetInternalStateFlag(IsLoadingChildrenMask); }
            private set
            {
                SetInternalStateFlag(value, IsLoadingChildrenMask);
            }
        }

        /// <summary>
        /// 获取或设置该节点的直接子节点集合。
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ObservableCollection<BaseTreeNode> Children
        {
            get { return _children; }
            private set
            {
                if (value != null)
                {
                    value.CollectionChanged -= OnChildrenCollectionChanged;
                }
                if (SetProperty(ref _children, value, nameof(Children)))
                {
                    if (value != null)
                    {
                        value.CollectionChanged += OnChildrenCollectionChanged;
                    }
                    HasChildren = value != null && value.Count > 0;
                }
            }
        }

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasChildren = Children != null && Children.Count > 0;
        }

        /// <summary>
        /// 获取一个值指示是否包含子节点。
        /// </summary>
        public bool HasChildren
        {
            get { return GetInternalStateFlag(HasChildrenMask); }
            private set
            {
                SetInternalStateFlag(value, HasChildrenMask);
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// 设置树节点行为。
        /// </summary>
        /// <remarks>设置新行为时，将自动应用到当前节点。</remarks>
        /// <param name="loadingBehavior">行为接口。</param>
        public void SetBehavior(ITreeNodeBehavior loadingBehavior)
        {
            Behavior = loadingBehavior ?? throw new ArgumentNullException(nameof(loadingBehavior));
            Behave();
        }

        /// <summary>
        /// 使用已配置的 <see cref="ITreeNodeBehavior"/> 设置当前节点的行为。
        /// </summary>
        public void Behave()
        {
            Behave(this);
        }

        /// <summary>
        /// 使用已配置的 <see cref="ITreeNodeBehavior"/> 设置节点的行为。
        /// </summary>
        /// <param name="node">指定的节点。</param>
        public void Behave(BaseTreeNode node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            node.Selectable = Behavior.CanNodeSelectable(node);
            node.Checkable = Behavior.CanNodeCheckable(node);
            node.IsEnabled = Behavior.IsNodeEnabled(node);
            node.Expandable = Behavior.CanNodeExpandable(node);
        }

        /// <summary>
        /// 获取已选中的子节点。
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<BaseTreeNode> GetSelectedNodes()
        {
            return Children?.Where(x => x.IsSelected);
        }

        /// <summary>
        /// 获取已选中的子节点，包括子节点的子节点。
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<BaseTreeNode> GetSelectedNodesRecursive()
        {
            var nodes = new List<BaseTreeNode>();
            var selectedNodes = GetSelectedNodes();
            if (selectedNodes != null)
            {
                nodes.AddRange(selectedNodes);

                foreach (var node in selectedNodes)
                {
                    var subSelectedNodes = node.GetSelectedNodesRecursive();
                    if (subSelectedNodes != null)
                        nodes.AddRange(subSelectedNodes);
                }
            }
            return selectedNodes;
        }

        /// <summary>
        /// 添加一个子节点到这个节点下。
        /// </summary>
        /// <param name="childNode">需要添加的子节点。</param>
        public void AddNode(BaseTreeNode childNode)
        {
            if (childNode == null)
            {
                throw new ArgumentNullException(nameof(childNode));
            }

            if (childNode.Behavior == DefaultBehavior && Behavior != DefaultBehavior)
                childNode.SetBehavior(Behavior);
            if (IsExpanded)
            {
                if (Children == null)
                    Children = new ObservableCollection<BaseTreeNode>(new BaseTreeNode[] { childNode });
                else
                    Children.Add(childNode);
            }
        }

        /// <summary>
        /// 从该节点删除指定的子节点。
        /// </summary>
        /// <param name="childNode">需要删除的子节点。</param>
        public void RemoveNode(BaseTreeNode childNode)
        {
            if (childNode == null)
            {
                throw new ArgumentNullException(nameof(childNode));
            }

            if (IsExpanded && Children != null)
            {
                if (!Children.Remove(childNode))
                {
                    var existsNode = Children.FirstOrDefault(x => x.NodeId == childNode.NodeId);
                    if (existsNode != null)
                        Children.Remove(existsNode);
                }
            }
        }

        /// <summary>
        /// 替换该节点指定的子节点。
        /// </summary>
        /// <param name="oldChildNode">要替换的节点标识。</param>
        /// <param name="newChildNode">要替换为的节点标识。</param>
        public void ReplaceNode(BaseTreeNode oldChildNode, BaseTreeNode newChildNode)
        {
            if (oldChildNode == null)
            {
                throw new ArgumentNullException(nameof(oldChildNode));
            }

            if (newChildNode == null)
            {
                throw new ArgumentNullException(nameof(newChildNode));
            }

            if (string.IsNullOrEmpty(oldChildNode.NodeId) || string.IsNullOrEmpty(newChildNode.NodeId))
            {
                throw new ArgumentException("任意节点标识都不应为空", nameof(newChildNode));
            }

            if (oldChildNode.NodeId != newChildNode.NodeId)
            {
                throw new ArgumentException("节点标识应相同", nameof(newChildNode));
            }

            var index = Children.IndexOf(oldChildNode);
            if (index < 0)
            {
                oldChildNode = GetNode(oldChildNode.NodeId);
                if (oldChildNode != null)
                    index = Children.IndexOf(oldChildNode);
            }

            if (index < 0)
                return;

            newChildNode.SetBehavior(oldChildNode.Behavior);
            Children[index] = newChildNode;
        }

        /// <summary>
        /// 获取指定节点标识的子节点。
        /// </summary>
        /// <param name="nodeId">节点标识。</param>
        /// <returns></returns>
        public virtual BaseTreeNode GetNode(string nodeId)
        {
            return Children?.Where(x => x.NodeId == nodeId).FirstOrDefault();
        }

        /// <summary>
        /// 获取指定节点标识的子节点，查找包括子节点的子节点。
        /// </summary>
        /// <param name="nodeId">节点标识</param>
        /// <returns></returns>
        public async virtual Task<BaseTreeNode> GetNodeRecursiveAsync(string nodeId)
        {
            if (!IsExpanded && Expandable)
            {
                await InternalExpandAsync();
            }

            var node = GetNode(nodeId);
            if (node != null)
                return node;

            if (Children != null)
            {
                foreach (var child in Children)
                {
                    node = await child.GetNodeRecursiveAsync(nodeId);
                    if (node != null)
                        return node;
                }
            }

            return null;
        }

        /// <summary>
        /// 查找符合条件的第一个子节点。
        /// </summary>
        /// <param name="predicate">判断节点是否符合条件的方法。</param>
        /// <returns></returns>
        public virtual BaseTreeNode FindNode(Predicate<BaseTreeNode> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return Children?.FirstOrDefault(x => predicate(x));
        }

        /// <summary>
        /// 递归的查找符合条件的第一个子节点。
        /// </summary>
        /// <param name="predicate">判断节点是否符合条件的方法。</param>
        /// <returns></returns>
        public async virtual Task<BaseTreeNode> FindNodeRecursiveAsync(Predicate<BaseTreeNode> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (!IsExpanded && Expandable)
            {
                await InternalExpandAsync();
            }

            var node = FindNode(predicate);
            if (node != null)
                return node;

            if (Children != null && Children.Any())
            {
                foreach (var child in Children)
                {
                    if (predicate(child))
                        return child;
                    var result = await child.FindNodeRecursiveAsync(predicate);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        /// <summary>
        /// 清理资源，退订事件。
        /// </summary>
        public virtual void CleanUp()
        {
            ClearChildren();
        }

        /// <summary>
        /// 获取对象Hash值。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return NodeId.GetHashCode();
        }


        /// <summary>
        /// 获取自定义格式化输出。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", Level, NodeType, NodeName);
        }

        /// <summary>
        /// 比较目标对象是否与该节点相等。
        /// </summary>
        /// <param name="obj">待比较的目标对象。</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as BaseTreeNode);
        }

        /// <summary>
        /// 比较目标节点是否与该节点相等。
        /// </summary>
        /// <param name="other">待比较的目标节点。</param>
        /// <returns></returns>
        public bool Equals(BaseTreeNode other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is null)
                return false;

            return GetHashCode() == other.GetHashCode();
        }

        /// <summary>
        /// 比较两个节点是否是否相等。
        /// </summary>
        /// <param name="x">待比较的节点<paramref name="x"/>。</param>
        /// <param name="y">待比较的节点<paramref name="y"/>。</param>
        /// <returns></returns>
        public bool Equals(BaseTreeNode x, BaseTreeNode y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if ((x == null && y != null) || (x != null && y == null))
                return false;

            return x.GetHashCode() == y.GetHashCode();
        }

        /// <summary>
        /// 获取目标节点的Hash值。
        /// </summary>
        /// <param name="obj">目标节点。</param>
        /// <returns></returns>
        public int GetHashCode(BaseTreeNode obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(BaseTreeNode x, BaseTreeNode y)
        {
            if (Equals(x, y))
            {
                return 0;
            }

            if (x != null)
            {
                return x.CompareTo(y);
            }

            if (y != null)
            {
                return y.CompareTo(x);
            }

            return 0;
        }

        /// <summary>
        /// 将此节点与指定节点进行比较。
        /// </summary>
        /// <param name="other">要比较的节点。</param>
        /// <returns>返回比较值，等于0时表示两节点相等；返回-1表示此节点小于指定节点；返回1表示此节点大于指定节点。</returns>
        public int CompareTo(BaseTreeNode other)
        {
            if (other == null)
                return 1;

            if(Equals(other))
            {
                return 0;
            }

            return string.Compare(ToString(), other.ToString(), StringComparison.CurrentCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >(BaseTreeNode x,BaseTreeNode y)
        {
            if (x is null)
            {
                return y.CompareTo(x) == 1;
            }
            return x.CompareTo(y) == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >=(BaseTreeNode x, BaseTreeNode y)
        {
            int compare;
            if (x is null)
            {
                compare = y.CompareTo(x);
                return compare != -1;
            }

            compare = x.CompareTo(y);

            return x.CompareTo(y) == 1 || compare == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <(BaseTreeNode x, BaseTreeNode y)
        {
            if (x is null)
            {
                return y.CompareTo(x) == 1;
            }
            return x.CompareTo(y) == -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <=(BaseTreeNode x, BaseTreeNode y)
        {
            int compare;
            if (x is null)
            {
                compare = y.CompareTo(x);
                return compare == 1 || compare == 0;
            }

            compare = x.CompareTo(y);

            return compare != -1; 
        }

        /// <summary>
        /// 比较两个对象是否相等。
        /// </summary>
        /// <param name="x">待比较的节点<paramref name="x"/>。</param>
        /// <param name="y">待比较的节点<paramref name="y"/>。</param>
        /// <returns></returns>
        public static bool operator ==(BaseTreeNode x, BaseTreeNode y)
        {
            if (x is null)
            {
                return y is null;
            }

            return x.Equals(y);
        }

        /// <summary>
        ///  比较两个对象是否不相等。
        /// </summary>
        /// <param name="x">待比较的节点<paramref name="x"/>。</param>
        /// <param name="y">待比较的节点<paramref name="y"/>。</param>
        /// <returns></returns>
        public static bool operator !=(BaseTreeNode x, BaseTreeNode y)
        {
            if (ReferenceEquals(x, y))
                return false;

            if (x == null)
                return true;

            return !x.Equals(y);
        }

        /// <summary>
        /// 获取一个值指示是否可以切换节点展开/收拢。
        /// </summary>
        /// <returns></returns>
        public bool CanToggle()
        {
            if (IsExpanded)
                return CanCollapse();
            else
                return CanExpand();
        }

        /// <summary>
        /// 获取一个值指示是否可以收起该节点。
        /// </summary>
        /// <returns></returns>
        public bool CanCollapse()
        {
            return !IsLoadingChildren && IsExpanded;
        }

        /// <summary>
        /// 获取一个值指示是否可以展开该节点。
        /// </summary>
        /// <returns></returns>
        public bool CanExpand()
        {
            return !IsExpanded && !IsLoadingChildren && Expandable;
        }

        /// <summary>
        /// 收起该节点。
        /// </summary>
        public void Collapse()
        {
            if (CanCollapse())
            {
                InternalCollapse();
            }
        }

        /// <summary>
        /// 展开该节点。
        /// </summary>
        public async Task ExpandAsync()
        {
            if (CanExpand())
            {
                await InternalExpandAsync();
            }
        }

        /// <summary>
        /// 切换节点展开或收拢。
        /// </summary>
        public async Task ToggleAsync()
        {
            if (!CanToggle())
            {
                return;
            }

            if (IsExpanded)
                Collapse();
            else
                await ExpandAsync();
        }

        /// <summary>
        /// 收拢当前及全部递归子节点。
        /// </summary>
        public void CollapseAll()
        {
            if (CanCollapse())
            {
                InternalCollapse();
            }
        }

        /// <summary>
        /// 展开当前及全部递归子节点。
        /// </summary>
        /// <returns></returns>
        public async Task ExpandAllAsync()
        {
            if (CanExpand())
            {
                await InternalExpandAllAsync();
            }
        }

        /// <summary>
        /// 加载子节点。
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected async virtual Task<IEnumerable<BaseTreeNode>> LoadChildrenAsync()
        {
            return await s_TaskCompletionSource.Task;
        }

        #endregion

        #region private helpers

        private void InternalCollapse()
        {
            IsExpanded = false;
        }

        private async Task InternalExpandAsync()
        {
            await PopulateChildren();
            //仅更改属性并通知，因为上一步我们已经将子节点展开。
            SetInternalStateFlag(true, IsExpandedMask, nameof(IsExpanded));
        }

        private async Task InternalExpandAllAsync()
        {
            await InternalExpandAsync();

            IsLoadingChildren = true;

            if (Children != null)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    await _children[i].ExpandAllAsync();
                }
            }

            IsLoadingChildren = false;
        }

        private async Task PopulateChildren()
        {
            IsLoadingChildren = true;
            IEnumerable<BaseTreeNode> children = await LoadChildrenAsync().ConfigureAwait(false);
            if (children != null && children.Any())
            {
                ChildrenCount = children.Count();
                if (Children == PlaceHolderChildren)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //initialize an asynchronous observable collection which allow cross-thread accessing
                        Children = new AsyncObservableCollection<BaseTreeNode>();
                    });
                }
                foreach (var child in children)
                {
                    if (Behavior.CanBehaviorBeInherited)
                        child.SetBehavior(Behavior);
                    if (!Children.Contains(child))
                    {
                        child.Level = Level + 1;
                        _children.Add(child);
                        if (child.Checkable && Behavior.CanCheckedBeInherited && IsChecked.HasValue && IsChecked.Value)
                        {
                            child.IsChecked = true;
                        }
                    }
                }

                if (Behavior.SelectFirstChildOnExpanded && Children.Any(x => x.IsSelected))
                {
                    var first = Children.First();
                    if (first.Selectable && first.IsEnabled)
                        first.IsSelected = true;
                }
            }
            else
            {
                Children = null;
            }
            IsLoadingChildren = false;
        }

        private void ClearChildren()
        {
            if (Children != null)
            {
                Children.CollectionChanged -= OnChildrenCollectionChanged;
                for (int i = 0; i < Children.Count; i++)
                {
                    if (_children[i] == Placeholder)
                        continue;
                    _children[i].IsExpanded = false;

                    //已选中或勾选的设备将会被保留
                    if (!_children[i].IsSelected && _children[i].IsChecked.HasValue && !_children[i].IsChecked.Value)
                    {
                        ClearChild(_children[i]);
                        i--;
                    }
                }
            }
        }

        private void ClearChild(BaseTreeNode child)
        {
            child.CleanUp();
            _children.Remove(child);
        }

        private bool GetInternalBehaviorFlag(byte mask)
        {
            return (_internalBehaviorFlags & mask) == mask;
        }

        private bool SetInternalBehaviorFlag(bool? value, byte mask, [CallerMemberName]string propertyName = null)
        {
            if (GetInternalBehaviorFlag(mask) == value)
            {
                return false;
            }

            if (value.HasValue && value.Value)
            {
                _internalBehaviorFlags |= mask;
            }
            else
            {
                _internalBehaviorFlags ^= mask;
            }

            if (propertyName != null)
                RaisePropertyChanged(propertyName);

            return true;
        }

        private bool GetInternalStateFlag(byte mask)
        {
            return (_internalStateFlags & mask) == mask;
        }

        private bool SetInternalStateFlag(bool? value, byte mask, [CallerMemberName]string propertyName = null)
        {
            if (GetInternalStateFlag(mask) == value)
            {
                return false;
            }

            if (value.HasValue && value.Value)
            {
                _internalStateFlags |= mask;
            }
            else
            {
                _internalStateFlags ^= mask;
            }

            if (propertyName != null)
                RaisePropertyChanged(propertyName);

            return true;
        }

        #endregion
    }
}
