using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// 使目标元素支持拖拽并能够显示拖拽预览。
    /// </summary>
    public class DragBehavior : CommandBehaviorBase<UIElement>
    {
        #region BombProperty

        /// <summary>
        /// 拖拽时的自定义数据。
        /// </summary>
        public object Bomb
        {
            get { return (object)GetValue(BombProperty); }
            set { SetValue(BombProperty, value); }
        }

        /// <summary>
        /// 拖拽时的自定义数据。
        /// </summary>
        public static readonly DependencyProperty BombProperty =
            DependencyProperty.Register(nameof(Bomb), typeof(object), typeof(DragBehavior), new PropertyMetadata(null));

        #endregion

        #region DataFormatProperty

        /// <summary>
        /// 拖拽时的自定义数据格式。
        /// </summary>
        public string DataFormat
        {
            get { return (string)GetValue(DataFormatProperty); }
            set { SetValue(DataFormatProperty, value); }
        }

        /// <summary>
        /// 拖拽时的自定义数据格式。
        /// </summary>
        public static readonly DependencyProperty DataFormatProperty =
            DependencyProperty.Register("DataFormat", typeof(string), typeof(DragBehavior), new PropertyMetadata(DataFormats.StringFormat));

        #endregion

        #region IsPreviewEnabledProperty

        /// <summary>
        /// 获取或设置一个值指示是否启用拖拽预览。
        /// </summary>
        public bool IsPreviewEnabled
        {
            get { return (bool)GetValue(IsPreviewEnabledProperty); }
            set { SetValue(IsPreviewEnabledProperty, value); }
        }

        /// <summary>
        /// 获取或设置一个值指示是否启用拖拽预览。
        /// </summary>
        public static readonly DependencyProperty IsPreviewEnabledProperty =
            DependencyProperty.Register("IsPreviewEnabled", typeof(bool), typeof(DragBehavior), new PropertyMetadata(true));

        #endregion

        #region PreviewProperty

        /// <summary>
        /// 获取或设置拖拽预览。
        /// </summary>
        public object Preview
        {
            get { return (object)GetValue(PreviewProperty); }
            set { SetValue(PreviewProperty, value); }
        }

        /// <summary>
        /// 获取拖拽预览的依赖属性。
        /// </summary>
        public static readonly DependencyProperty PreviewProperty =
            DependencyProperty.Register(nameof(Preview), typeof(object), typeof(DragBehavior), new PropertyMetadata(null));

        #endregion

        #region KeepRelativeMousePositionProperty

        /// <summary>
        /// 获取或设置一个值，指示鼠标预览时是否保留启动拖拽时的鼠标位置。
        /// </summary>
        public bool KeepRelativeMousePosition
        {
            get { return (bool)GetValue(KeepRelativeMousePositionProperty); }
            set { SetValue(KeepRelativeMousePositionProperty, value); }
        }

        /// <summary>
        /// KeepRelativeMousePosition 依赖属性。
        /// </summary>
        public static readonly DependencyProperty KeepRelativeMousePositionProperty =
            DependencyProperty.Register(nameof(KeepRelativeMousePosition), typeof(bool), typeof(DragBehavior), new PropertyMetadata(true));

        #endregion

        private Point? _lastMouseDown;
        private DragAdorner _adorner;

        /// <summary>
        /// 当附加完成时。
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.GiveFeedback += AssociatedObject_GiveFeedback;
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_MouseDown;
            AssociatedObject.PreviewMouseMove += AssociatedObject_MouseMove;
            base.OnAttached();
        }

        /// <summary>
        /// 查找合适的拖拽元素。
        /// </summary>
        /// <returns>返回当前附加的目标元素。</returns>
        protected virtual UIElement FindProperDraggingElement(MouseEventArgs e)
        {
            if (Preview is UIElement preview)
                return preview;
            return AssociatedObject;
        }

        /// <summary>
        /// 正在分离时。
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.GiveFeedback -= AssociatedObject_GiveFeedback;
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_MouseDown;
            AssociatedObject.PreviewMouseMove -= AssociatedObject_MouseMove;
            base.OnDetaching();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public int X;
            public int Y;
        };

        private static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        private void AssociatedObject_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (_adorner != null && IsPreviewEnabled)
            {
                UIElement element = sender as UIElement;
                var pos = element.PointFromScreen(GetMousePosition());
                _adorner.UpdatePosition(pos);
            }
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed && Bomb != null && _adorner == null)
                {
                    var draggingElement = FindProperDraggingElement(e);
                    if (draggingElement != null)
                    {
                        Point currentPosition = e.GetPosition(AssociatedObject);

                        if (_lastMouseDown.HasValue && ((Math.Abs(currentPosition.X - _lastMouseDown.Value.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                            (Math.Abs(currentPosition.Y - _lastMouseDown.Value.Y) > SystemParameters.MinimumVerticalDragDistance)))
                        {
                            AdornerLayer adornerLayer = null;
                            if (IsPreviewEnabled)
                            {
                                var hwndSource = PresentationSource.FromVisual(AssociatedObject);
                                var adornerDecorator = hwndSource.RootVisual.GetVisualChild<AdornerDecorator>();
                                if (adornerDecorator != null)
                                {
                                    adornerLayer = adornerDecorator.AdornerLayer;
                                    if (adornerLayer != null)
                                    {
                                        var offset = e.GetPosition(AssociatedObject);
                                        if (!KeepRelativeMousePosition)
                                            offset = new Point(0, 0);
                                        _adorner = new DragAdorner(draggingElement, offset);
                                        adornerLayer.Add(_adorner);
                                    }
                                }
                            }
                            try
                            {
                                var effects = DragDrop.DoDragDrop(draggingElement, new DataObject(DataFormat, Bomb), DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
                                if (effects.HasFlag(DragDropEffects.Move) || effects.HasFlag(DragDropEffects.Copy) || effects.HasFlag(DragDropEffects.Link))
                                {
                                    ExecuteCommand(CommandParameter);
                                }
                            }
                            finally
                            {
                                //Indicate dragging is over.
                                _lastMouseDown = null;
                                if (adornerLayer != null)
                                {
                                    adornerLayer.Remove(_adorner);
                                    _adorner = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _lastMouseDown = e.GetPosition(AssociatedObject);
            }
        }

        private class DragAdorner : Adorner
        {
            private readonly Brush vbrush;

            private Point location;

            private Point offset;

            public DragAdorner(UIElement adornedElement, Point offset)

                : base(adornedElement)
            {
                this.offset = offset;
                vbrush = new VisualBrush(AdornedElement);
                vbrush.Opacity = .7;

                IsHitTestVisible = false;
            }

            public void UpdatePosition(Point location)
            {
                this.location = location;
                if (Parent is AdornerLayer adornerLayer)
                {
                    adornerLayer.Update(AdornedElement);
                }
            }

            protected override void OnRender(DrawingContext dc)
            {
                var p = location;
                p.Offset(-offset.X, -offset.Y);
                dc.DrawRectangle(vbrush, null, new Rect(p, this.RenderSize));
            }
        }
    }

    /// <summary>
    /// 使数据表格支持拖拽。
    /// </summary>
    public class DataGridDragBehavior : DragBehavior
    {
        /// <summary>
        /// 查找合适的拖拽元素。
        /// </summary>
        /// <returns>返回鼠标按下处的行。</returns>
        protected override UIElement FindProperDraggingElement(MouseEventArgs e)
        {
            if (!(AssociatedObject is DataGrid dataGrid))
            {
                throw new ArgumentException("AssociatedObject is not a type of 'DataGrid'!");
            }

            if (dataGrid.SelectedIndex < 0 || dataGrid.Items.Count < 1)
            {
                return null;
            }

            HitTestResult testResult = VisualTreeHelper.HitTest(AssociatedObject, e.GetPosition(AssociatedObject));
            if (testResult == null || testResult.VisualHit == null)
            {
                return null;
            }
            //User is scrolling a scrollviewr inside a DataGridRow
            var scrollViewer = testResult.VisualHit.GetVisualParent<ScrollViewer>();
            if (scrollViewer != null)
            {
                var outterDataGridRow = scrollViewer.GetVisualParent<DataGridRow>();
                if (outterDataGridRow != null)
                {
                    var outterItem = dataGrid.ItemContainerGenerator.ItemFromContainer(outterDataGridRow);
                    //We check if user is dragging a DataGridRow which inside a detail datagrid
                    if (outterDataGridRow != null && (outterItem != null && outterItem != DependencyProperty.UnsetValue))
                    {
                        return null;
                    }
                }
            }

            var dataGridRow = testResult.VisualHit.GetVisualParent<DataGridRow>();
            if (dataGridRow == null)
            {
                //Since we are not dragging a DataGridRow
                //Maybe user is scrolling a scroll bar or just clicking around
                return null;
            }

            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);

            if (dataGridRow == row)
            {
                //This's the row which user wish to drag.
                return row;
            }

            return null;
        }
    }
}
