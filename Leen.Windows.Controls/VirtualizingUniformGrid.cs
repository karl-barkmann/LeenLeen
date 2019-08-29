using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 提供一种在所有单元格大小相同的网格中安排内容的方法并支持启用UI虚拟化。
    /// </summary>
    public class VirtualizingUniformGrid : VirtualizingPanel, IScrollInfo
    {
        // Scrolling physical "line" metrics.
        internal const double _scrollLineDelta = 16.0;   // Default physical amount to scroll with one Up/Down/Left/Right key
        internal const double _mouseWheelDelta = 48.0;   // Default physical amount to scroll with one MouseWheel.

        /// <summary>
        /// 获取或设置此面板的滚动行为在滚动时是否依滚动方向对齐。
        /// <para>
        /// 不启用该行为则滚动时始终向顶部对齐。
        /// </para>
        /// </summary>
        public bool AlignByScrollingDirection
        {
            get { return (bool)GetValue(ScrollByDirectionProperty); }
            set { SetValue(ScrollByDirectionProperty, value); }
        }

        /// <summary>
        /// 提供对 <see cref="AlignByScrollingDirection"/> 依赖属性支持。
        /// </summary>
        public static readonly DependencyProperty ScrollByDirectionProperty =
            DependencyProperty.Register(nameof(AlignByScrollingDirection), typeof(bool), typeof(VirtualizingUniformGrid), new PropertyMetadata(false));

        /// <summary>
        /// Specifies the number of columns in the grid
        /// A value of 0 indicates that the column count should be dynamically 
        /// computed based on the number of rows (if specified) and the 
        /// number of non-collapsed children in the grid
        /// </summary>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="Columns" /> property.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
                DependencyProperty.Register(
                        "Columns",
                        typeof(int),
                        typeof(VirtualizingUniformGrid),
                        new FrameworkPropertyMetadata(
                                (int)0,
                                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange,
                                ColumnsPropertyChanged),
                        new ValidateValueCallback(ValidateColumns));

        private static void ColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as VirtualizingUniformGrid;
            panel.OnColumnsChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnColumnsChanged(object oldValue, object newValue)
        {

        }

        private static bool ValidateColumns(object o)
        {
            return (int)o >= 0;
        }

        /// <summary>
        /// Specifies the number of rows in the grid
        /// A value of 0 indicates that the row count should be dynamically 
        /// computed based on the number of columns (if specified) and the 
        /// number of non-collapsed children in the grid
        /// </summary>
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="Rows" /> property.
        /// </summary>
        public static readonly DependencyProperty RowsProperty =
                DependencyProperty.Register(
                        "Rows",
                        typeof(int),
                        typeof(VirtualizingUniformGrid),
                        new FrameworkPropertyMetadata(
                                (int)0,
                                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
                        new ValidateValueCallback(ValidateRows));

        private static bool ValidateRows(object o)
        {
            return (int)o >= 0;
        }

        #region Private Members

        #region Scrolling Vars

        private ScrollViewer _scrollOwner;
        private TranslateTransform _translateTransform;
        private Size _extent = new Size(0, 0);
        private Size _viewport = new Size(0, 0);
        private Point _offset = new Point(0, 0);
        private bool _vertScrollable = false;
        private Size _previousMeasureSize = new Size(0, 0);
        private Size _measuredChildDesiredSize = Size.Empty;
        double _scrollTarget = 0;
        ScrollDirection _scrollDirection = ScrollDirection.None;
        private Storyboard _scrollStoryboard;
        private Storyboard _smoothScrollStoryboard;
        private DoubleAnimation _scrollAnimation;
        private DoubleAnimation _smoothScrollAnimation;

        #endregion Scrolling Vars

        #region Virtualization Vars

        private int _hiddenRowsAtTop = 0;
        private List<UIElement> _realizedChildren;

        #endregion Virtualization Vars

        #endregion Private Members

        public VirtualizingUniformGrid() : base()
        {
            _extent = new Size(0, 0);
            _vertScrollable = false;
            _offset = new Point(0, 0);
            CreateScrollStoryboard();
            CreateSmoothScrollStoryboard();
            RenderTransform = _translateTransform = new TranslateTransform();
        }

        #region Overrides

        protected override Size MeasureOverride(Size availableSize)
        {
            Size sizeSoFar = new Size(0, 0);
            double maxWidth = 0.0;
            double maxHeight = 0.0;

            // We need to access InternalChildren before the generator to work around a bug
            IList<UIElement> children = RealizedChildren;
            IItemContainerGenerator generator = ItemContainerGenerator;

            UpdateScrollInfo(availableSize);

            CalculateControlIndices(out int firstVisibleItemIndex, out int lastVisibleItemIndex);
            //RecycleContainers();

            if ((firstVisibleItemIndex < 0 && lastVisibleItemIndex < 0) || lastVisibleItemIndex - firstVisibleItemIndex < 0)
            {
                return _previousMeasureSize;
            }

            // Get the generator position of the first visible data item
            GeneratorPosition start = generator.GeneratorPositionFromIndex(firstVisibleItemIndex);

            // Get index where we'd insert the child for this position. If the item is realized
            // (position.Offset == 0), it's just position.Index, otherwise we have to add one to
            // insert after the corresponding child
            int childIndex = (start.Offset == 0) ? start.Index : start.Index + 1;

            var childConstraintSize = CalculateChildSize(availableSize);
            using (generator.StartAt(start, GeneratorDirection.Forward, true))
            {
                for (int itemIndex = firstVisibleItemIndex; itemIndex <= lastVisibleItemIndex; ++itemIndex, ++childIndex)
                {
                    UIElement child = generator.GenerateNext(out bool newlyRealized) as UIElement;
                    if (child == null) continue;

                    if (newlyRealized)
                    {
                        InsertContainer(childIndex, child, false);
                    }
                    else
                    {
                        if (childIndex >= children.Count || !(children[childIndex] == child))
                        {
                            // we have a recycled container (if it was realized container it would have been returned in the
                            // propert location). Note also that recycled containers are NOT in the RealizedChildren list.
                            InsertContainer(childIndex, child, true);
                        }
                        else
                        {
                            // previously realized child, so do nothing
                        }
                    }

                    #region Measure Logic

                    bool skipChildMeasure = _measuredChildDesiredSize != Size.Empty;
                    if (!skipChildMeasure)
                    {
                        child.Measure(childConstraintSize);
                        _measuredChildDesiredSize = child.DesiredSize;
                    }
                    else
                    {
                        child.Measure(childConstraintSize);
                    }

                    double hiddenPanelHeight = (_hiddenRowsAtTop * childConstraintSize.Height);

                    if (sizeSoFar.Width + _measuredChildDesiredSize.Width > availableSize.Width)
                    {
                        //Switching row
                        sizeSoFar.Width = 0.0;
                        sizeSoFar.Height += _measuredChildDesiredSize.Height;
                        SetNextPosition(child, sizeSoFar.Width, sizeSoFar.Height + hiddenPanelHeight);
                        sizeSoFar.Width += _measuredChildDesiredSize.Width;
                    }
                    else
                    {
                        SetNextPosition(child, sizeSoFar.Width, sizeSoFar.Height + hiddenPanelHeight);
                        sizeSoFar.Width += _measuredChildDesiredSize.Width;
                        maxWidth = Math.Max(maxWidth, sizeSoFar.Width);
                    }
                    #endregion Measure Logic
                }
            }

            //DisconnectRecycledContainers();
            CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);
            if (DoubleUtil.IsNaN(availableSize.Height) || !DoubleUtil.IsDoubleFinite(availableSize.Height))
            {
                maxHeight = Math.Min(availableSize.Height, Math.Max(sizeSoFar.Height, childConstraintSize.Height));
            }
            else
            {
                maxHeight = availableSize.Height;
            }
            _previousMeasureSize = new Size(maxWidth, maxHeight);
            _measuredChildDesiredSize = Size.Empty;
            return _previousMeasureSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            UpdateScrollInfo(finalSize);

            for (int i = 0; i < RealizedChildren.Count; i++)
            {
                UIElement child = RealizedChildren[i];
                var childDesiredSize = child.DesiredSize;
                if (childDesiredSize.Height == 0.0 || childDesiredSize.Width == 0.0)
                    continue;

                var nextPostion = (Point)child.GetValue(NextPositionProperty);
                var childRect = new Rect(nextPostion.X, nextPostion.Y, childDesiredSize.Width, childDesiredSize.Height);
                child.Arrange(childRect);
            }

            return finalSize;
        }

        protected override void BringIndexIntoView(int itemIndex)
        {
            var slot = GetSlotForFirstVisibleIndex(itemIndex);

            SetVerticalOffset(slot.Height);
        }

        private Size GetSlotForFirstVisibleIndex(int index)
        {
            int childrenPerRow = CalculateChildrenPerRow(_viewport);
            var childSize = CalculateChildSize(_viewport);
            double extentHeight = index * (childSize.Height / childrenPerRow);
            var targetY = Math.Max(0, (index % childrenPerRow == 0) ? extentHeight : extentHeight - childSize.Height);
            Size slot = new Size(_offset.X, targetY);
            return slot;
        }

        private Size CalculateChildSize(Size availSize)
        {
            int columns = Math.Max(1, Columns);
            return new Size(availSize.Width / columns, availSize.Width / columns / 1.15);
        }

        #endregion Overrides

        private static readonly DependencyProperty NextPositionProperty =
            DependencyProperty.RegisterAttached(
                "NextPosition",
                typeof(Point),
                typeof(VirtualizingUniformGrid),
                new PropertyMetadata(new Point()));

        private void SetNextPosition(UIElement child, double x, double y)
        {
            if (child != null)
            {
                child.SetValue(NextPositionProperty, new Point(x, y));
            }
        }

        #region Virtualization

        private void InsertContainer(int childIndex, UIElement container, bool isRecycled)
        {
            // index in Children collection, whereas childIndex is the index into the RealizedChildren collection
            int visualTreeIndex = 0;
            UIElementCollection children = InternalChildren;

            if (childIndex > 0)
            {
                // find the item before where we want to insert the new item
                visualTreeIndex = ChildIndexFromRealizedIndex(childIndex - 1);
                visualTreeIndex++;
            }

            if (isRecycled && visualTreeIndex < children.Count && children[visualTreeIndex] == container)
            {
                // don't insert if a recycled container is in the proper place already
            }
            else
            {
                if (visualTreeIndex < children.Count)
                {
                    int insertIndex = visualTreeIndex;
                    if (isRecycled && VisualTreeHelper.GetParent(container) != null)
                    {
                        // If the container is recycled we have to remove it from its place in the visual tree and 
                        // insert it in the proper location.   We cant use an internal Move api, so we are removing
                        // and inserting the container
                        int containerIndex = children.IndexOf(container);
                        RemoveInternalChildRange(containerIndex, 1);
                        if (containerIndex < insertIndex)
                        {
                            insertIndex--;
                        }

                        InsertInternalChild(insertIndex, container);
                    }
                    else
                    {
                        InsertInternalChild(insertIndex, container);
                    }
                }
                else
                {
                    if (isRecycled && VisualTreeHelper.GetParent(container) != null)
                    {
                        // Recycled container is still in the tree; move it to the end
                        int originalIndex = children.IndexOf(container);
                        RemoveInternalChildRange(originalIndex, 1);
                        AddInternalChild(container);
                    }
                    else
                    {
                        AddInternalChild(container);
                    }
                }
            }

            // Keep realizedChildren in sync with the visual tree.
            RealizedChildren.Insert(childIndex, container);
            ItemContainerGenerator.PrepareItemContainer(container);
        }

        /// <summary>
        ///     Takes an index from the realized list and returns the corresponding index in the Children collection
        /// </summary>
        private int ChildIndexFromRealizedIndex(int realizedChildIndex)
        {
            UIElementCollection children = InternalChildren;
            // If we're not recycling containers then we're not using a realizedChild index and no translation is necessary
            if (realizedChildIndex < RealizedChildren.Count)
            {
                UIElement child = RealizedChildren[realizedChildIndex];

                for (int i = realizedChildIndex; i < children.Count; i++)
                {
                    if (children[i] == child)
                    {
                        return i;
                    }
                }
            }

            return realizedChildIndex;
        }

        private void RecycleContainers(int firstVisibleItemIndex, int lastVisibleItemIndex)
        {
            UIElementCollection children = InternalChildren;
            IItemContainerGenerator generator = ItemContainerGenerator;
            if (children.Count == 0) return;

            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            int recycleRangeStart = -1;
            int recycleRangeCount = 0;
            int childCount = RealizedChildren.Count;
            for (int i = 0; i < childCount; i++)
            {
                bool recycleContainer = false;

                int itemIndex = itemsControl.ItemContainerGenerator.IndexFromContainer(RealizedChildren[i]);
                int itemIndex1 = itemsControl.Items.IndexOf((RealizedChildren[i] as ContentControl).Content);

                Debug.Assert(itemIndex == itemIndex1);
                if (itemIndex >= 0 && (itemIndex < firstVisibleItemIndex || itemIndex > lastVisibleItemIndex))
                {
                    recycleContainer = true;
                }

                if (!children.Contains(RealizedChildren[i]))
                {
                    recycleContainer = false;
                    RealizedChildren.RemoveAt(i);
                    i--;
                    childCount--;
                }

                if (recycleContainer)
                {
                    if (recycleRangeStart == -1)
                    {
                        recycleRangeStart = i;
                        recycleRangeCount = 1;
                    }
                    else
                    {
                        recycleRangeCount++;
                    }
                }
                else
                {
                    if (recycleRangeCount > 0)
                    {
                        GeneratorPosition position = new GeneratorPosition(recycleRangeStart, 0);
                        ((IRecyclingItemContainerGenerator)generator).Recycle(position, recycleRangeCount);
                        _realizedChildren.RemoveRange(recycleRangeStart, recycleRangeCount);

                        childCount -= recycleRangeCount;
                        i -= recycleRangeCount;
                        recycleRangeCount = 0;
                        recycleRangeStart = -1;
                    }
                }
            }

            if (recycleRangeCount > 0)
            {
                GeneratorPosition position = new GeneratorPosition(recycleRangeStart, 0);
                ((IRecyclingItemContainerGenerator)generator).Recycle(position, recycleRangeCount);
                _realizedChildren.RemoveRange(recycleRangeStart, recycleRangeCount);
            }
        }

        private void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated)
        {
            UIElementCollection children = InternalChildren;
            IItemContainerGenerator generator = ItemContainerGenerator;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);
                if (itemIndex < minDesiredGenerated || itemIndex > maxDesiredGenerated)
                {
                    generator.Remove(childGeneratorPos, 1);
                    RealizedChildren.Remove(children[i]);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        /// <summary>
        ///     Recycled containers still in the InternalChildren collection at the end of Measure should be disconnected
        ///     from the visual tree.  Otherwise they're still visible to things like Arrange, keyboard navigation, etc.
        /// </summary>
        private void DisconnectRecycledContainers()
        {
            int realizedIndex = 0;
            UIElement realizedChild = RealizedChildren.Count > 0 ? RealizedChildren[0] : null;
            UIElementCollection children = InternalChildren;

            int removeStartRange = -1;
            int removalCount = 0;
            for (int i = 0; i < children.Count; i++)
            {
                UIElement visualChild = children[i];

                if (visualChild == realizedChild)
                {
                    if (removalCount > 0)
                    {
                        RemoveInternalChildRange(removeStartRange, removalCount);
                        i -= removalCount;
                        removalCount = 0;
                        removeStartRange = -1;
                    }

                    realizedIndex++;

                    if (realizedIndex < RealizedChildren.Count)
                    {
                        realizedChild = RealizedChildren[realizedIndex];
                    }
                    else
                    {
                        realizedChild = null;
                    }
                }
                else
                {
                    if (removeStartRange == -1)
                    {
                        removeStartRange = i;
                    }

                    removalCount++;
                }
            }

            if (removalCount > 0)
            {
                RemoveInternalChildRange(removeStartRange, removalCount);
            }
        }

        /// <summary>
        /// When items are removed, remove the corresponding UI if necessary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    _realizedChildren.RemoveRange(args.Position.Index, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    RealizedChildren.Clear();
                    break;
            }
        }

        private IList<UIElement> RealizedChildren
        {
            get
            {
                EnsureRealizedChildren();
                return _realizedChildren;
            }
        }


        private void EnsureRealizedChildren()
        {
            if (_realizedChildren == null)
            {
                UIElementCollection children = InternalChildren;

                _realizedChildren = new List<UIElement>(children.Count);

                for (int i = 0; i < children.Count; i++)
                {
                    _realizedChildren.Add(children[i]);
                }
            }
        }

        private void CalculateControlIndices(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
        {
            int itemsCount = GetItemsCount();
            int childrenPerRow = CalculateChildrenPerRow(_viewport);

            var childSize = CalculateChildSize(_viewport);
            firstVisibleItemIndex = (int)Math.Floor(_offset.Y / childSize.Height) * childrenPerRow;
            lastVisibleItemIndex = (int)Math.Ceiling((_offset.Y + _viewport.Height) / childSize.Height) * childrenPerRow - 1;
            lastVisibleItemIndex = Math.Max(firstVisibleItemIndex, Math.Min(itemsCount - 1, lastVisibleItemIndex));
            
            if (childrenPerRow == 0)
            {
                _hiddenRowsAtTop = 0;
            }
            else
            {
                _hiddenRowsAtTop = (int)Math.Floor((decimal)(firstVisibleItemIndex / childrenPerRow));
            }
        }

        private int GetItemsCount()
        {
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
            return itemsControl.Items.Count;
        }

        private int CalculateChildrenPerRow(Size availableSize)
        {
            // Figure out how many children fit on each row
            int childrenPerRow;
            var childSize = CalculateChildSize(availableSize);
            if (availableSize.Width == double.PositiveInfinity)
            {
                childrenPerRow = InternalChildren.Count;
            }
            else
            {
                childrenPerRow = Math.Max(1, (int)Math.Floor(availableSize.Width / childSize.Width));
            }

            return childrenPerRow;
        }

        #endregion Virtualization

        private void UpdateScrollInfo(Size availableSize)
        {
            Size extent = MeasureExtent(availableSize);
            Size oldExtent = _extent;
            if (_extent != extent)
            {
                _extent = extent;
            }

            Size oldViewport = _viewport;
            if (availableSize != _viewport)
            {
                _viewport = availableSize;
            }

            UpdateOffset(oldExtent, extent, oldViewport, _viewport);

            if (_scrollOwner != null)
                _scrollOwner.InvalidateScrollInfo();
        }

        private void UpdateOffset(Size oldExtent, Size newExtent, Size oldViewPort, Size newViewPort)
        {
            if (oldExtent == newExtent) return;

            if (newExtent.Height == 0)
            {
                AnimateVerticalOffset(-_offset.Y);
            }

            if (newExtent.Height == 0 || oldExtent.Height == 0) return;

            if (oldViewPort != newViewPort)
            {
                double yFactor = newExtent.Height / oldExtent.Height;

                var offset = (_offset.Y * yFactor) - _offset.Y;
                AnimateVerticalOffset(offset);
            }
        }

        private Size MeasureExtent(Size availableSize)
        {
            int itemCount = GetItemsCount();
            var childSize = CalculateChildSize(availableSize);
            double colCount = Math.Max(1d, Math.Floor(availableSize.Width / childSize.Width));
            colCount = Math.Min(colCount, itemCount);
            colCount = colCount >= 1 ? colCount : 1;

            double rowCount = Math.Ceiling(itemCount / colCount);

            Size result = new Size(colCount * childSize.Width, (rowCount * childSize.Height));
            return result;
        }

        #region Scrolling Animations

        internal double VerticalScrollOffset
        {
            get { return (double)GetValue(VerticalScrollOffsetProperty); }
            set { SetValue(VerticalScrollOffsetProperty, value); }
        }

        private void AnimateVerticalOffsetSmoothly(double offset)
        {
            if (Math.Sign(offset) < 0)
            {
                if (_scrollDirection == ScrollDirection.Down) //scroll direction reversed while animating. Flip around immediately
                {
                    _scrollTarget = ScrollOwner.VerticalOffset;
                }
                _scrollDirection = ScrollDirection.Up;
            }
            else if (Math.Sign(offset) > 0)
            {
                if (_scrollDirection == ScrollDirection.Up) //scroll direction reversed while animating. Flip around immediately
                {
                    _scrollTarget = ScrollOwner.VerticalOffset;
                }
                _scrollDirection = ScrollDirection.Down;
            }

            _scrollTarget += offset;
            _scrollTarget = Math.Max(Math.Min(_scrollTarget, ScrollOwner.ScrollableHeight), 0);

            _smoothScrollAnimation.To = _scrollTarget;
            _smoothScrollAnimation.From = ScrollOwner.VerticalOffset;

            if (_smoothScrollAnimation.From != _smoothScrollAnimation.To)
            {
                _smoothScrollStoryboard.Begin();
            }
        }

        private void AnimateVerticalOffset(double offset)
        {
            if (Math.Sign(offset) < 0)
            {
                if (_scrollDirection == ScrollDirection.Down) //scroll direction reversed while animating. Flip around immediately
                {
                    _scrollTarget = ScrollOwner.VerticalOffset;
                }
                _scrollDirection = ScrollDirection.Up;
            }
            else if (Math.Sign(offset) > 0)
            {
                if (_scrollDirection == ScrollDirection.Up) //scroll direction reversed while animating. Flip around immediately
                {
                    _scrollTarget = ScrollOwner.VerticalOffset;
                }
                _scrollDirection = ScrollDirection.Down;
            }

            _scrollTarget += offset;
            _scrollTarget = Math.Max(Math.Min(_scrollTarget, ScrollOwner.ScrollableHeight), 0);

            _scrollStoryboard.Pause();
            _scrollAnimation.To = _scrollTarget;
            _scrollAnimation.From = ScrollOwner.VerticalOffset;

            if (_scrollAnimation.From != _scrollAnimation.To)
            {
                _scrollStoryboard.Begin();
            }
        }

        public void SetVerticalOffsetViaMediator(double offset)
        {
            if (offset < 0 || _viewport.Height >= _extent.Height)
            {
                offset = 0;
            }
            else if (offset + _viewport.Height >= _extent.Height)
            {
                offset = _extent.Height - _viewport.Height;
            }

            _scrollTarget = offset;
            _offset.Y = offset;

            if (_scrollOwner != null)
                _scrollOwner.InvalidateScrollInfo();

            _translateTransform.Y = -offset;

            InvalidateMeasure();
        }

        internal static readonly DependencyProperty VerticalScrollOffsetProperty =
           DependencyProperty.Register(
               nameof(VerticalScrollOffset),
               typeof(double),
               typeof(VirtualizingUniformGrid),
               new PropertyMetadata(0.0, OnVerticalScrollOffsetPropertyChanged));

        private static void OnVerticalScrollOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VirtualizingUniformGrid panel && panel.ScrollOwner != null)
            {
                panel.SetVerticalOffsetViaMediator((double)e.NewValue);
            }
        }

        private void CreateScrollStoryboard()
        {
            _scrollStoryboard = new Storyboard();
            _scrollAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(.3),
                EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut }
            };
            _scrollAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("VerticalScrollOffset"));
            Storyboard.SetTarget(_scrollAnimation, this);
            _scrollStoryboard.Children.Add(_scrollAnimation);
            _scrollStoryboard.Completed += (s, e) =>
            {
                _scrollDirection = ScrollDirection.None;
            };
        }

        private void CreateSmoothScrollStoryboard()
        {
            _smoothScrollStoryboard = new Storyboard();
            _smoothScrollAnimation = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(.3),
            };
            _smoothScrollAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("VerticalScrollOffset"));
            Storyboard.SetTarget(_smoothScrollAnimation, this);
            _smoothScrollStoryboard.Children.Add(_smoothScrollAnimation);
            _smoothScrollStoryboard.Completed += (s, e) =>
            {
                _scrollDirection = ScrollDirection.None;
            };
        }

        #endregion Scrolling Animations

        #region IScrollInfo

        public ScrollViewer ScrollOwner
        {
            get
            {
                return _scrollOwner;
            }
            set
            {
                _scrollOwner = value;
            }
        }

        public bool CanVerticallyScroll
        {
            get
            {
                return _vertScrollable;
            }
            set
            {
                _vertScrollable = value;
            }
        }

        public double ExtentHeight
        {
            get
            {
                return _extent.Height;
            }
        }

        public double ExtentWidth
        {
            get
            {
                return _extent.Width;
            }
        }

        public double HorizontalOffset
        {
            get
            {
                return _offset.X;
            }
        }

        public double VerticalOffset
        {
            get
            {
                return _offset.Y;
            }
        }

        public double ViewportHeight
        {
            get
            {
                return _viewport.Height;
            }
        }

        public double ViewportWidth
        {
            get
            {
                return _viewport.Width;
            }
        }

        public void LineUp()
        {
            var childSize = CalculateChildSize(_viewport);
            var offset = childSize.Height;
            if (AlignByScrollingDirection)
            {
                var offScrollingItemCol = (ScrollOwner.ScrollableHeight - VerticalOffset) / childSize.Height;
                if (Math.Floor(offScrollingItemCol) < offScrollingItemCol &&
                    !DoubleUtil.AreClose(offScrollingItemCol, Math.Ceiling(offScrollingItemCol)))
                {
                    var offScrollingOffset = (offScrollingItemCol - Math.Floor(offScrollingItemCol)) * childSize.Height;
                    offset = childSize.Height - offScrollingOffset;
                }
            }
            else
            {
                var offScrollingItemCol = VerticalOffset / childSize.Height;
                if (Math.Floor(offScrollingItemCol) < offScrollingItemCol &&
                    !DoubleUtil.AreClose(offScrollingItemCol, Math.Ceiling(offScrollingItemCol)))
                {
                    var offScrollingOffset = (offScrollingItemCol - Math.Floor(offScrollingItemCol)) * childSize.Height;
                    offset = offScrollingOffset;
                }

                if (DoubleUtil.IsBetweenZeroAndOne(offset))
                {
                    offset = childSize.Height;
                }
            }

            AnimateVerticalOffsetSmoothly(-offset);
        }

        public void LineDown()
        {
            var childSize = CalculateChildSize(_viewport);
            var offset = childSize.Height;
            var offScrollingItemCol = VerticalOffset / childSize.Height;

            if (Math.Floor(offScrollingItemCol) < offScrollingItemCol &&
                !DoubleUtil.AreClose(offScrollingItemCol, Math.Ceiling(offScrollingItemCol)))
            {
                var offScrollingOffset = (offScrollingItemCol - Math.Floor(offScrollingItemCol)) * childSize.Height;
                offset = childSize.Height - offScrollingOffset;
            }

            AnimateVerticalOffsetSmoothly(offset);
        }

        public void PageUp()
        {
            double offset;
            if (AlignByScrollingDirection)
            {
                offset = CalPageUpOffsetWithBottomAlignment();
            }
            else
            {
                offset = CalPageUpOffsetWithTopAlignment();
            }

            AnimateVerticalOffset(-offset);
        }

        private double CalPageUpOffsetWithBottomAlignment()
        {
            var childSize = CalculateChildSize(_viewport);
            var inViewPortItemCount = ViewportHeight / childSize.Height;
            var fullInViewportItemCount = Math.Floor(inViewPortItemCount);

            var offset = fullInViewportItemCount * childSize.Height;
            var offScrollingItemCol = (ScrollOwner.ScrollableHeight - VerticalOffset) / childSize.Height;

            if (Math.Floor(offScrollingItemCol) < offScrollingItemCol &&
                !DoubleUtil.AreClose(offScrollingItemCol, Math.Ceiling(offScrollingItemCol)))
            {
                var offScrollingOffset = (offScrollingItemCol - Math.Floor(offScrollingItemCol)) * childSize.Height;
                var endurableOffset = childSize.Height - (childSize.Height * (inViewPortItemCount - fullInViewportItemCount));
                if (endurableOffset > offScrollingOffset)
                {
                    offset -= offScrollingOffset;
                }
                else
                {
                    offset += (childSize.Height - offScrollingOffset);
                }
            }

            return offset;
        }

        private double CalPageUpOffsetWithTopAlignment()
        {
            var childSize = CalculateChildSize(_viewport);
            var inViewPortItemCount = ViewportHeight / childSize.Height;
            var fullInViewportItemCount = Math.Floor(inViewPortItemCount);

            var offset = 0.0d;

            var topOffScrollingItemCol = VerticalOffset / childSize.Height;
            var bottomOffScrollingItemCol = (ScrollOwner.ScrollableHeight - VerticalOffset) / childSize.Height;

            if (Math.Floor(topOffScrollingItemCol) < topOffScrollingItemCol &&
                !DoubleUtil.AreClose(topOffScrollingItemCol, Math.Ceiling(topOffScrollingItemCol)))
            {
                var offScrollingOffset = childSize.Height * (inViewPortItemCount - fullInViewportItemCount);
                var topOffScrollingOffset = (topOffScrollingItemCol - Math.Floor(topOffScrollingItemCol)) * childSize.Height;
                var bottomOffScrollingOffset = (bottomOffScrollingItemCol - Math.Floor(bottomOffScrollingItemCol)) * childSize.Height;

                offset += topOffScrollingOffset;
                offset += (fullInViewportItemCount - 1) * childSize.Height;
            }
            else
            {
                offset += fullInViewportItemCount * childSize.Height;
            }

            return offset;
        }

        public void PageDown()
        {
            var childSize = CalculateChildSize(_viewport);
            var inViewPortItemCount = ViewportHeight / childSize.Height;
            var fullInViewportItemCount = Math.Floor(inViewPortItemCount);

            var offset = fullInViewportItemCount * childSize.Height;
            var offScrollingItemCol = VerticalOffset / childSize.Height;

            if (Math.Floor(offScrollingItemCol) < offScrollingItemCol)
            {
                var offScrollingOffset = (offScrollingItemCol - Math.Floor(offScrollingItemCol)) * childSize.Height;
                var endurableOffset = childSize.Height - (childSize.Height * (inViewPortItemCount - fullInViewportItemCount));

                if (endurableOffset < offScrollingOffset)
                {
                    offset += (childSize.Height - offScrollingOffset);
                }
                else
                {
                    offset -= offScrollingOffset;
                }
            }

            AnimateVerticalOffset(offset);
        }

        public void MouseWheelUp()
        {
            double lines = SystemParameters.WheelScrollLines;
            AnimateVerticalOffsetSmoothly(-lines * _scrollLineDelta);
        }

        public void MouseWheelDown()
        {
            double lines = SystemParameters.WheelScrollLines;
            AnimateVerticalOffsetSmoothly(lines * _scrollLineDelta);
        }

        public void SetVerticalOffset(double value)
        {
            SetCurrentValue(VerticalScrollOffsetProperty, value);
        }

        #region Not Implemented
        public void SetHorizontalOffset(double offset) { }
        public bool CanHorizontallyScroll
        {
            get
            {
                return false;
            }
            set { }
        }

        public void LineLeft() { }
        public void LineRight() { }
        public void MouseWheelLeft() { }
        public void MouseWheelRight() { }

        public void PageLeft() { }
        public void PageRight() { }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            // We can only work on visuals that are us or children.
            // An empty rect has no size or position.  We can't meaningfully use it.
            if (rectangle.IsEmpty
                || visual == null
                || visual == (Visual)this
                || !IsAncestorOf(visual))
            {
                return Rect.Empty;
            }

            rectangle = visual.TransformToAncestor(this).TransformBounds(rectangle);
            // We can't do any work unless we're scrolling.
            if (ScrollOwner == null)
            {
                return rectangle;
            }

            var viewportRect = new Rect(HorizontalOffset, VerticalOffset, ViewportWidth, ViewportHeight);

            if (viewportRect.IntersectsWith(rectangle) && !viewportRect.Contains(rectangle))
            {
                if (viewportRect.Y < rectangle.Y)
                {
                    var offset = (rectangle.Y + rectangle.Height) - (viewportRect.Y + viewportRect.Height);
                    //空出下一行项一定高度以使下次键盘向下导航调用能够成功。
                    AnimateVerticalOffsetSmoothly(offset + (rectangle.Height * 0.1));
                }
                else
                {
                    var offset = viewportRect.Y - rectangle.Y;
                    //空出上一行项一定高度以使下次键盘向上导航调用能够成功。
                    AnimateVerticalOffsetSmoothly(-offset - rectangle.Height * 0.1);
                }
            }

            return rectangle;
        }

        #endregion Not Implemented

        #endregion IScrollInfo
    }
}
