using Leen.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// 使 <see cref="TreeView"/> 在接口 <see cref="ISelectable"/> 的帮助下实现支持Shift 及 Ctrl 的按键多项选中。
    /// <para>
    /// 实现者应明确了解到在 WPF 中 <see cref="TreeView"/> 不支持设置多个选中项，该 <see cref="Behavior{TreeView}"/> 仅通过 <see cref="ISelectable"/> 接口使 ViewModel 获得选中值。
    /// 因此XAML开发者不应设置 <see cref="TreeViewItem"/> 的 IsSelected 属性为通知到目标（OneWay TwoWay等）的绑定。
    /// 同时实现者应根据 <see cref="ISelectable.IsSelected"/> 的值来实现触发器以在 UI 中表现多个选中项。
    /// </para>
    /// </summary>
    public class SupportMultSelectionBehavior : Behavior<TreeView>
    {
        private TreeViewItem _lastItemSelected; // Used in shift selections
        private TreeViewItem _itemToCheck; // Used when clicking on a selected item to check if we want to deselect it or to drag the current selection
        private TreeViewItem _multiSelectionStartItem; //Used when enable multi-selection

        /// <summary>
        /// 选中单个节点执行委托
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public delegate bool SelectSingleItem(object item);

        /// <summary>
        /// 选中多个节点执行委托
        /// </summary>
        /// <param name="startItem">开始节点</param>
        /// <param name="endItem">结束节点</param>
        /// <returns></returns>
        public delegate bool SelectMultiItem(object startItem, object endItem);

        /// <summary>
        /// 选中单个树节点时发生。为了支持虚拟化，需要由外部来执行选中操作
        /// </summary>
        public event SelectSingleItem SelectSingleTreeViewItem;

        /// <summary>
        /// 选中某个范围内的多个树节点时发生。为了支持虚拟化，需要由外部来执行选中操作
        /// </summary>
        public event SelectMultiItem SelectMultiTreeViewItem;

        /// <summary>
        /// 在行为附加到 AssociatedObject 后调用。
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseDown -= AssociatedObject_PreviewMouseDown;
            AssociatedObject.PreviewMouseUp -= AssociatedObject_PreviewMouseUp;
            AssociatedObject.SelectedItemChanged -= AssociatedObject_SelectedItemChanged;
        }

        /// <summary>
        /// 在行为与其 AssociatedObject 分离时（但在它实际发生之前）调用。
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseDown += AssociatedObject_PreviewMouseDown;
            AssociatedObject.PreviewMouseUp += AssociatedObject_PreviewMouseUp;
            AssociatedObject.SelectedItemChanged += AssociatedObject_SelectedItemChanged;
        }

        /// <summary>
        /// 触发选中单个节点事件
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool OnSelectSingleTreeViewItem(object item)
        {
            bool? retValue = SelectSingleTreeViewItem?.Invoke(item);

            return retValue ?? false;
        }

        /// <summary>
        /// 触发选中某个范围内的节点事件
        /// </summary>
        /// <param name="startItem">开始节点</param>
        /// <param name="endItem">结束节点</param>
        /// <returns></returns>
        protected bool OnSelectMultiTreeViewItem(object startItem, object endItem)
        {
            bool? retValue = SelectMultiTreeViewItem?.Invoke(startItem, endItem);

            return retValue ?? false;
        }

        private static bool IsCtrlPressed
        {
            get { return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl); }
        }

        private static bool IsShiftPressed
        {
            get { return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift); }
        }

        private void AssociatedObject_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_itemToCheck == null)
                return;

            if (_itemToCheck.Header is ISelectable selectableItemToCheck)
            {
                if (!(e.OriginalSource is FrameworkElement fe))
                {
                    return;
                }

                TreeViewItem item = this.GetTreeViewItemClicked(fe);

                if (item != null && item.Header != null)
                {
                    if (!(item.Header is ISelectable))
                        return;

                    if (!IsCtrlPressed)
                    {
                        OnSelectSingleTreeViewItem(_itemToCheck);
                        _lastItemSelected = _itemToCheck;
                    }
                    else
                    {
                        selectableItemToCheck.IsSelected = false;
                        _lastItemSelected = null;
                    }
                }
            }
        }

        private void AssociatedObject_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_multiSelectionStartItem == null || e.OldValue == null)
                return;

            var startIndex = AssociatedObject.Items.IndexOf(_multiSelectionStartItem);
            var endIndex = AssociatedObject.Items.IndexOf(_lastItemSelected);
            var unSelectedIndex = AssociatedObject.Items.IndexOf(e.OldValue);

            if (unSelectedIndex >= startIndex && unSelectedIndex <= endIndex)
            {
                (e.OldValue as ISelectable).IsSelected = true;
            }
        }

        private void AssociatedObject_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.OriginalSource is FrameworkElement fe))
            {
                return;
            }

            TreeViewItem item = GetTreeViewItemClicked(fe);

            if (item != null && item.Header != null)
            {
                SelectedItemChangedHandler(item);
            }
        }

        private void SelectedItemChangedHandler(TreeViewItem item)
        {
            ISelectable content = (ISelectable)item.Header;

            _itemToCheck = null;

            if (content.IsSelected)
            {
                // Check it at the mouse up event to avoid deselecting everything when clicking to drag
                _itemToCheck = item;
            }
            else
            {
                if (IsCtrlPressed)
                {
                    content.IsSelected = true;
                    _multiSelectionStartItem = _lastItemSelected;
                    _lastItemSelected = item;
                }
                else if (IsShiftPressed && _lastItemSelected != null)
                {
                    if (OnSelectMultiTreeViewItem(_lastItemSelected, item))
                    {
                        _multiSelectionStartItem = _lastItemSelected;
                        _lastItemSelected = item;
                    }
                }
                else
                {
                    OnSelectSingleTreeViewItem(item);
                    _multiSelectionStartItem = null;
                    _lastItemSelected = item;
                }
            }
        }

        private TreeViewItem GetTreeViewItemClicked(UIElement sender)
        {
            Point point = sender.TranslatePoint(new Point(0, 0), this.AssociatedObject);
            DependencyObject visualItem = this.AssociatedObject.InputHitTest(point) as DependencyObject;
            while (visualItem != null && !(visualItem is TreeViewItem))
            {
                visualItem = VisualTreeHelper.GetParent(visualItem);
            }

            return visualItem as TreeViewItem;
        }

        private static IEnumerable<TreeViewItem> GetTreeViewItemItems(TreeViewItem treeViewItem, bool includeCollapsedItems)
        {
            if (treeViewItem == null)
            {
                return null;
            }
            List<TreeViewItem> returnItems = new List<TreeViewItem>();

            for (int index = 0; index < treeViewItem.Items.Count; index++)
            {
                TreeViewItem item = (TreeViewItem)treeViewItem.ItemContainerGenerator.ContainerFromIndex(index);

                if (item != null)
                {
                    returnItems.Add(item);

                    if (includeCollapsedItems || item.IsExpanded)
                    {
                        var treeViewItems = GetTreeViewItemItems(item, includeCollapsedItems);
                        if (treeViewItems != null)
                        {
                            returnItems.AddRange(treeViewItems);
                        }
                    }
                }
            }

            return returnItems;
        }
    }
}
