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

        /// <summary>
        /// 一个占位节点，用于在父节点折叠时使其仍然显示展开符号。
        /// </summary>
        internal static readonly DomainTreeNode Placeholder = new DomainTreeNode() { NodeName = "PlaceHolder" };
        internal static readonly ObservableCollection<BaseTreeNode> PlaceHolderChildren = new ObservableCollection<BaseTreeNode>(new List<BaseTreeNode>(1) { Placeholder });
        private readonly static TaskCompletionSource<IEnumerable<BaseTreeNode>> s_TaskCompletionSource = 
            new TaskCompletionSource<IEnumerable<BaseTreeNode>>();
        private readonly bool _initializeWithPlaceholder;

        internal const byte IsCheckedMask = 0x01;
        internal const byte CheckableMask = 0x02;
        internal const byte IsSelectedMask = 0x04;
        internal const byte SelectableMask = 0x08;
        internal const byte IsExpandedMask = 0x10;
        internal const byte IsLoadingChilrenMask = 0x20;

        private byte _internalFlags = 0x00;
        private string _nodeName;
        private ObservableCollection<BaseTreeNode> _children;
        private bool _hasChildren;
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
        /// </summary>
        protected BaseTreeNode(string nodeId, bool initializeWithPlaceholder)
        {
            NodeId = nodeId;
            _initializeWithPlaceholder = initializeWithPlaceholder;
            if (initializeWithPlaceholder)
            {
                Children = PlaceHolderChildren;
            }
            //_toggleCommand = new RelayCommand(Toggle, CanToggle);
            //_expandCommand = new RelayCommand(InternalExpand, CanExpand);
            //_expandAllCommand = new RelayCommand(async () => await InternalExpandAll(), CanExpand);
            //_collapseCommand = new RelayCommand(InternalCollapse, CanCollapse);
            //_collapseAllCommand = new RelayCommand(CollapseAll, CanCollapse);
            _initializeWithPlaceholder = initializeWithPlaceholder;
        }

        #endregion

        #region properties

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
                SetPropertyWith(ref _childrenCount, value, nameof(ChildrenCount));
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
                SetPropertyWith(ref _level, value, nameof(Level));
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
        /// 获取或设置一个值，指示该节点是否已展开。
        /// </summary>
        public bool IsExpanded
        {
            get { return GetInternalFlag(IsExpandedMask); }
            set
            {
                if (SetInternalFlag(value, IsExpandedMask))
                {
                    if (value)
                    {
#pragma warning disable 4014
                        PopulateChildren();
#pragma warning restore 4014
                    }
                    else
                    {
                        ClearChildren();
                        //If there are no reserved children
                        //we should hold a placeholder to enable node toggling
                        if ((Children == null || Children.Count < 1) && _initializeWithPlaceholder)
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
            get { return GetInternalFlag(SelectableMask); }
            set
            {
                SetInternalFlag(value, SelectableMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该节点是否支持勾选。
        /// </summary>
        public bool Checkable
        {
            get { return GetInternalFlag(CheckableMask); }
            set
            {
                SetInternalFlag(value, CheckableMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该节点是否被选中。
        /// </summary>
        public bool IsSelected
        {
            get { return GetInternalFlag(IsSelectedMask); }
            set
            {
                SetInternalFlag(value, IsSelectedMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该节点是否被勾选。
        /// </summary>
        public bool? IsChecked
        {
            get { return GetInternalFlag(IsCheckedMask); }
            set
            {
                if (SetInternalFlag(value, IsCheckedMask))
                {
                    if (Children != null)
                    {
                        for (int i = 0; i < Children.Count; i++)
                        {
                            _children[i].IsChecked = value;
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
            protected set
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
            get { return GetInternalFlag(IsLoadingChilrenMask); }
            private set
            {
                SetInternalFlag(value, IsLoadingChilrenMask);
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
                    value.CollectionChanged -= Value_CollectionChanged;
                }
                if (SetPropertyWith(ref _children, value, nameof(Children)))
                {
                    if (value != null)
                    {
                        value.CollectionChanged += Value_CollectionChanged;
                    }
                    HasChildren = value != null && value.Count > 0;
                }
            }
        }

        private void Value_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasChildren = Children != null && Children.Count > 0;
        }

        /// <summary>
        /// 获取一个值指示是否包含子节点。
        /// </summary>
        public bool HasChildren
        {
            get { return _hasChildren; }
            private set
            {
                SetProperty(ref _hasChildren, value, () => HasChildren);
            }
        }

        #endregion

        #region public methods

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
        public virtual BaseTreeNode GetNodeRecursive(string nodeId)
        {
            var node = GetNode(nodeId);
            if (node != null)
                return node;

            foreach (var child in Children)
            {
                node = GetNodeRecursive(nodeId);
                if (node != null)
                    return node;
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
            return NodeType.GetHashCode() * NodeName.GetHashCode();
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
            if (object.ReferenceEquals(x, y))
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
            if (x is null)
            {
                return y is null;
            }

            return !x.Equals(y);
        }

        /// <summary>
        /// 获取一个值指示是否可以切换节点展开/收拢。
        /// </summary>
        /// <returns></returns>
        public bool CanToggle()
        {
            return !IsLoadingChildren;
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
            return !IsExpanded && !IsLoadingChildren;
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
        public void Expand()
        {
            if (CanExpand())
            {
                InternalExpand();
            }
        }

        /// <summary>
        /// 切换节点展开或收拢。
        /// </summary>
        public void Toggle()
        {
            if (CanToggle())
            {
                IsExpanded = !IsExpanded;
            }
        }

        /// <summary>
        /// 收拢当前及全部递归子节点。
        /// </summary>
        public void CollapseAll()
        {
            if (CanCollapse())
            {
                IsExpanded = false;
            }
        }

        /// <summary>
        /// 展开当前及全部递归子节点。
        /// </summary>
        /// <returns></returns>
        public async Task ExpandAll()
        {
            if (CanExpand())
            {
                await PopulateChildren();
                SetInternalFlag(true, IsExpandedMask, "IsExpanded");

                IsLoadingChildren = true;

                if (Children != null)
                {
                    for (int i = 0; i < Children.Count; i++)
                    {
                        await _children[i].ExpandAll();
                    }
                }

                IsLoadingChildren = false;
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

        private void InternalExpand()
        {
            IsExpanded = true;
        }

        private async Task InternalExpandAll()
        {
            await ExpandAll();
        }

        private async Task PopulateChildren()
        {
            IsLoadingChildren = true;
            await LoadChildrenAsync().ContinueWith((t) =>
            {
                IEnumerable<BaseTreeNode> children = t.Result;

                if (children != null && children.Any())
                {
                    ChildrenCount = children.Count();
                    if (Children == PlaceHolderChildren)
                    {
                        Application.Current.Dispatcher.Invoke(() => { 
                            //initialize an asynchronous observable collection which allow cross-thread accessing
                            Children = new AsyncObservableCollection<BaseTreeNode>();
                        });
                    }
                    foreach (var child in children)
                    {
                        if (!Children.Contains(child))
                        {
                            //child.PropertyChanged += child_PropertyChanged;
                            child.Level = Level + 1;
                            _children.Add(child);
                            if (child.Checkable && IsChecked.HasValue && IsChecked.Value)
                            {
                                child.IsChecked = true;
                            }
                        }
                        else
                        {
                            //force to select previrous selected child node
                            //TreeViewItem will lost selection when parent TreeViewItem collapsed
                            var reserverdNode = Children.First(c => c == child);
                            reserverdNode.IsSelected = true;
                            if (reserverdNode.Checkable && IsChecked.HasValue && IsChecked.Value)
                            {
                                reserverdNode.IsChecked = true;
                            }
                        }
                    }
                }
                else
                {
                    Children = null;
                }
                IsLoadingChildren = false;
            }).ConfigureAwait(false);
        }

        private void ClearChildren()
        {
            if (Children != null)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    if (_children[i] == Placeholder)
                        continue;
                    _children[i].IsExpanded = false;

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

        private bool GetInternalFlag(byte mask)
        {
            return (_internalFlags & mask) == mask;
        }

        private bool SetInternalFlag(bool? value, byte mask, [CallerMemberName]string propertyName = null)
        {
            if (GetInternalFlag(mask) == value)
            {
                return false;
            }

            if (value.HasValue && value.Value)
            {
                _internalFlags |= mask;
            }
            else
            {
                _internalFlags ^= mask;
            }

            RaisePropertyChangedWith(propertyName);

            return true;
        }

        #endregion
    }
}
