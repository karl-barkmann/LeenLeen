using Leen.Common;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Leen.Windows.Controls
{
    [TemplatePart(Name = "PART_TreeView", Type = typeof(TreeView))]
    public class TreeSelect : ComboBox
    {
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(nameof(SelectionMode), typeof(SelectionMode), typeof(TreeSelect), new PropertyMetadata(SelectionMode.Single));

        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(TreeSelect), new PropertyMetadata(OnSelectedItemsChanged));

        private static void OnSelectedItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TreeSelect)sender).UpdateSelectedItems(e.NewValue as IList, e.OldValue as IList);
        }

        public new string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public static new readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string), typeof(TreeSelect));

        public new string SelectedValuePath
        {
            get => (string)GetValue(SelectedValuePathProperty);
            set => SetValue(SelectedValuePathProperty, value);
        }

        public static new readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(nameof(SelectedValuePath), typeof(string), typeof(TreeSelect));

        public new object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public static new readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(TreeSelect), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TreeSelect)sender).UpdateSelectedItem();
        }

        public new object SelectedValue
        {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public static new readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(object), typeof(TreeSelect), new PropertyMetadata(null));


        public new string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static new readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TreeSelect));

        public string TextSeparator
        {
            get => (string)GetValue(TextSeparatorProperty);
            set => SetValue(TextSeparatorProperty, value);
        }

        public static readonly DependencyProperty TextSeparatorProperty =
            DependencyProperty.Register(nameof(TextSeparator), typeof(string), typeof(TreeSelect), new PropertyMetadata(","));

        public int? MaxTextLength
        {
            get => (int?)GetValue(MaxTextLengthProperty);
            set => SetValue(MaxTextLengthProperty, value);
        }

        public static readonly DependencyProperty MaxTextLengthProperty =
            DependencyProperty.Register(nameof(MaxTextLength), typeof(int?), typeof(TreeSelect));

        public string ExceededTextFiller
        {
            get => (string)GetValue(ExceededTextFillerProperty);
            set => SetValue(ExceededTextFillerProperty, value);
        }

        public static readonly DependencyProperty ExceededTextFillerProperty =
            DependencyProperty.Register(nameof(ExceededTextFiller), typeof(string), typeof(TreeSelect), new PropertyMetadata("..."));

        static TreeSelect()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeSelect), new FrameworkPropertyMetadata(typeof(TreeSelect)));
        }

        public TreeSelect()
        {
            Loaded -= TreeSelect_Loaded;
            Loaded += TreeSelect_Loaded;
            SizeChanged -= TreeSelect_SizeChanged;
            SizeChanged += TreeSelect_SizeChanged;
        }

        private TreeView _treeView;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _treeView = GetTemplateChild("PART_TreeView") as TreeView;
            if (_treeView != null)
            {
                _treeView.MouseLeftButtonUp += OnTreeViewMouseUp;
                _treeView.AddHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(Treeview_Selected));
            }
        }

        private bool _interChanged = false;
        private void OnTreeViewMouseUp(object sender, RoutedEventArgs e)
        {
            if (SelectionMode == SelectionMode.Multiple)
            {
                _interChanged = true;
                SelectedItems.Clear();
                foreach (var item in GenerateMultiObject(Items))
                {
                    SelectedItems.Add(item);
                }
                _interChanged = false;
            }
            else
            {
                SelectedItem = _treeView.SelectedItem;
                IsDropDownOpen = false;
            }
            UpdateText();
        }

        private void Treeview_Selected(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TreeViewItem item)
            {
                item.BringIntoView();
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            if (item is ComboBoxItem)
                return true;
            else
                return false;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var uie = element as FrameworkElement;

            if (!(item is ComboBoxItem))
            {
                var textBinding = new Binding(DisplayMemberPath)
                {
                    Source = item
                };
                uie.SetBinding(ContentPresenter.ContentProperty, textBinding);
            }

            base.PrepareContainerForItemOverride(element, item);
        }

        private void TreeSelect_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateText();
        }

        private void TreeSelect_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateText();
        }

        private void UpdateSelectedItem()
        {
            if (SelectedItem == null || string.IsNullOrEmpty(SelectedValuePath))
            {
                SelectedValue = null;
            }
            else
            {
                SelectedValue = SelectedItem.GetType().GetProperty(SelectedValuePath).GetValue(SelectedItem, null);
            }

            base.SelectedItem = SelectedItem;
            SetCurrentValue(Selector.SelectedItemProperty, SelectedItem);
            UpdateText();
        }

        private void UpdateSelectedItems(IList newitem, IList olditem)
        {
            if (olditem != null)
            {
                foreach (object item in olditem)
                {
                    if (item.GetType().GetProperty("IsChecked") != null)
                    {
                        item.GetType().GetProperty("IsChecked").SetValue(item, false);
                    }
                }

                ((INotifyCollectionChanged)olditem).CollectionChanged -= TreeSelect_CollectionChanged;
            }

            if (newitem != null)
            {
                foreach (object item in newitem)
                {
                    if (item.GetType().GetProperty("IsChecked") != null)
                    {
                        item.GetType().GetProperty("IsChecked").SetValue(item, true);
                    }
                }

                ((INotifyCollectionChanged)newitem).CollectionChanged += TreeSelect_CollectionChanged;
            }

            UpdateText();
        }

        private void TreeSelect_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_interChanged) return;

            UpdateText();
        }

        private void UpdateText()
        {
            if (!IsLoaded)
                return;

            if (SelectionMode == SelectionMode.Single)
            {
                Text = GenerateText(SelectedItem);
            }
            else
            {
                Text = GenerateMultiText();
            }
        }

        public string GenerateText(object selectedItem)
        {
            var text = "";
            if (selectedItem == null)
            {
                text = "";
            }
            else if (selectedItem is ComboBoxItem)
            {
                var msi = selectedItem as ComboBoxItem;
                text += msi.Content.ToString();
            }
            else
            {
                if (!string.IsNullOrEmpty(DisplayMemberPath) && selectedItem.GetType().GetProperty(DisplayMemberPath) != null)
                    text += selectedItem.GetType().GetProperty(DisplayMemberPath).GetValue(selectedItem, null).ToString();
                else
                    text += selectedItem.ToString();

                if (selectedItem.GetType().GetProperty("IsSelected") != null)
                {
                    selectedItem.GetType().GetProperty("IsSelected").SetValue(selectedItem, true);
                }
            }

            return text;
        }

        private string GenerateMultiText()
        {
            var text = "";
            var isFirst = true;

            if (SelectedItems != null)
            {
                foreach (var item in SelectedItems)
                {
                    string txt = null;
                    if (item is ComboBoxItem)
                    {
                        var msi = item as ComboBoxItem;
                        txt = msi.Content.ToString();
                    }
                    else
                    {
                        if (item.GetType().GetProperty("IsChecked") != null && item.GetType().GetProperty("IsChecked").GetValue(item, null).ToString() == "True")
                        {
                            if (!string.IsNullOrEmpty(DisplayMemberPath) && item.GetType().GetProperty(DisplayMemberPath) != null)
                                txt = item.GetType().GetProperty(DisplayMemberPath).GetValue(item, null).ToString();
                            else
                                txt = item.ToString();
                        }
                    }

                    if (!isFirst)
                        text += TextSeparator;
                    else
                        isFirst = false;

                    text += txt;

                    if (MaxTextLength == null)
                    {
                        if (!ValidateStringWidth(text + ExceededTextFiller))
                        {
                            if (text.Length == 0)
                                return null;
                            text = text.Remove(text.Length - 1);
                            while (!ValidateStringWidth(text + ExceededTextFiller))
                            {
                                if (text.Length == 0)
                                    return null;
                                text = text.Remove(text.Length - 1);
                            }
                            return text + ExceededTextFiller;
                        }
                    }
                    else if (text.Length >= MaxTextLength)
                    {
                        return text.Omit((int)MaxTextLength, ExceededTextFiller);
                    }
                }
            }
            return text;
        }

        private List<object> GenerateMultiObject(System.Collections.IEnumerable items)
        {
            List<object> objs = new List<object>();
            foreach (var item in items)
            {
                object obj = null;
                if (item is ComboBoxItem)//这个还未支持多选，按单选处理
                {
                    var msi = item as ComboBoxItem;
                    if (msi.IsSelected)
                    {
                        obj = item;
                    }
                }
                else
                {
                    if (item.GetType().GetProperty("IsChecked") != null && item.GetType().GetProperty("IsChecked").GetValue(item, null).ToString() == "True")
                    {
                        obj = item;
                    }
                }

                if (obj != null)
                    objs.Add(obj);

                if (item.GetType().GetProperty("Children") != null)
                {
                    objs.AddRange(GenerateMultiObject(item.GetType().GetProperty("Children").GetValue(item, null) as System.Collections.IEnumerable));
                }
            }

            return objs;
        }

        private bool ValidateStringWidth(string text)
        {
            var size = MeasureString(text);
            if (size.Width > (ActualWidth - Padding.Left - Padding.Right - 30))
                return false;
            else
                return true;

        }

        private Size MeasureString(string candidate)
        {
            Typeface typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Display);

            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}
