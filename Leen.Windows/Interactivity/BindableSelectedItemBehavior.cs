/* * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * 作者：李平
 * 日期：2012/7/13 9:52:27
 * 描述：使TreeView的选中项可以双向绑定。
 * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System.Windows.Controls;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// 使TreeView的选中项可以双向绑定。
    /// </summary>
    public class BindableSelectedItemBehavior : Behavior<TreeView>
    {
        /// <summary>
        /// 选定项依赖属性。
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(BindableSelectedItemBehavior), new UIPropertyMetadata(null, OnSelectedItemPropertyChanged));

        /// <summary>
        /// 获取或设置选定项。
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return this.GetValue(SelectedItemProperty);
            }
            set
            {
                this.SetValue(SelectedItemProperty, value);
            }
        }

        #region 私有方法

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
            }
        }

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is TreeViewItem item)
            {
                item.SetValue(TreeViewItem.IsSelectedProperty, true);
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedItem = e.NewValue;
        }

        #endregion
    }
}
