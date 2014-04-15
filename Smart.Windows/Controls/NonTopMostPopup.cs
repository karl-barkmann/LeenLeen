using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Smart.Windows.Controls
{
    /// <summary>
    /// 不会始终TopMost的 Popuup
    /// </summary>
    public class NonTopmostPopup : Popup
    {
        /// <summary>
        /// Is Topmost dependency property
        /// </summary>
        public static readonly DependencyProperty IsTopmostProperty = DependencyProperty.Register("IsTopmost", typeof(bool), typeof(NonTopmostPopup), new FrameworkPropertyMetadata(false, OnIsTopmostChanged));

        private bool? _appliedTopMost;
        private bool _alreadyLoaded;
        private Window _parentWindow;

        /// <summary>
        /// Get/Set IsTopmost
        /// </summary>
        public bool IsTopmost
        {
            get { return (bool)GetValue(IsTopmostProperty); }
            set { SetValue(IsTopmostProperty, value); }
        }

        /// <summary>
        /// ctor
        /// </summary>
        public NonTopmostPopup()
        {
            Loaded += OnPopupLoaded;
            Unloaded += OnPopupUnloaded;
        }

        void OnPopupLoaded(object sender, RoutedEventArgs e)
        {
            if (_alreadyLoaded)
                return;

            _alreadyLoaded = true;

            if (Child != null)
            {
                Child.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnChildPreviewMouseLeftButtonDown), true);
            }

            _parentWindow = Window.GetWindow(this);

            if (_parentWindow == null)
                return;

            _parentWindow.Activated += OnParentWindowActivated;
            _parentWindow.Deactivated += OnParentWindowDeactivated;
        }

        private void OnPopupUnloaded(object sender, RoutedEventArgs e)
        {
            if (_parentWindow == null)
                return;
            _parentWindow.Activated -= OnParentWindowActivated;
            _parentWindow.Deactivated -= OnParentWindowDeactivated;
        }

        void OnParentWindowActivated(object sender, EventArgs e)
        {
            // Debug.WriteLine("Parent Window Activated");
            SetTopmostState(true);
        }

        void OnParentWindowDeactivated(object sender, EventArgs e)
        {
            //   Debug.WriteLine("Parent Window Deactivated");

            if (IsTopmost == false)
            {
                SetTopmostState(IsTopmost);
            }
        }

        void OnChildPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //  Debug.WriteLine("Child Mouse Left Button Down");

            SetTopmostState(true);

            if (!_parentWindow.IsActive && IsTopmost == false)
            {
                _parentWindow.Activate();
                //      Debug.WriteLine("Activating Parent from child Left Button Down");
            }
        }

        private static void OnIsTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var thisobj = (NonTopmostPopup)obj;

            thisobj.SetTopmostState(thisobj.IsTopmost);
        }

        /// <summary>
        /// 对 <see cref="P:System.Windows.Controls.Primitives.Popup.IsOpen"/> 属性的值从 false 更改为 true 的情况进行响应。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnOpened(EventArgs e)
        {
            if (PlacementTarget == null && Parent == null) return;

            Window window = Window.GetWindow(PlacementTarget ?? Parent);

            if (window != null)
            {
                window.LocationChanged -= new EventHandler(win_LocationChanged);
                window.LocationChanged += new EventHandler(win_LocationChanged);
                window.SizeChanged -= new SizeChangedEventHandler(window_SizeChanged);
                window.SizeChanged += new SizeChangedEventHandler(window_SizeChanged);
            }

            SetTopmostState(IsTopmost);
            //ResreshPoint();
            base.OnOpened(e);
        }

        void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsOpen)
            {
                //ResreshPoint();
                UpdatePosition();
            }
        }

        void win_LocationChanged(object sender, EventArgs e)
        {
            if (IsOpen)
            {
                //ResreshPoint();
                UpdatePosition();
            }
        }

        /// <summary>
        /// 逆天的反射私有方法
        /// </summary>
        public void UpdatePosition()
        {
            try
            {
                var mi = typeof(Popup).GetMethod("UpdatePosition", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(this, null);
            }
            catch (TargetInvocationException)
            {

            }
            catch (InvalidOperationException)
            {

            }
        }

        private double tempAcutalWidth;
        private BindingBase tempWidthBindingBase;

        private void ResreshPoint()
        {
            var child = Child as FrameworkElement;
            var parentOrTarget = (PlacementTarget as FrameworkElement ?? Parent as FrameworkElement);
            var target = GetNotNanWidthChild(child);

            //刷新坐标，减少超出屏幕的宽度,popup不能移到屏幕外
            var upperLeft = parentOrTarget.PointToScreen(new Point(0, 0));
            var upperRight = parentOrTarget.PointToScreen(new Point(target.ActualWidth, 0));

            double tempWidth = 0;

            bool isChanged = false;

            if (upperLeft.X < 0)
            {
                var xoffsetLeft = Math.Min(0, upperLeft.X);
                tempWidth = xoffsetLeft + target.ActualWidth;
                if (tempWidth < 0)
                    tempWidth = 0;
                target.Margin = new Thickness(xoffsetLeft, 0, 0, 0);

                tempAcutalWidth = Width;
                isChanged = true;
            }
            else if (upperRight.X >= SystemParameters.PrimaryScreenWidth)
            {
                var xoffsetRight = upperRight.X - SystemParameters.PrimaryScreenWidth;
                tempWidth = target.ActualWidth - xoffsetRight;
                target.Margin = new Thickness(0, 0, 0, 0);

                tempAcutalWidth = Width;
                isChanged = true;
            }

            if (isChanged)
            {                
                if (tempWidthBindingBase == null)
                    tempWidthBindingBase = BindingOperations.GetBindingBase(target, WidthProperty);
                if (tempWidthBindingBase == null)
                {
                    tempAcutalWidth = Width;
                }

                if (target != null)
                    target.Width = tempAcutalWidth;
            }
            else
            {
                target.Margin = new Thickness(0, 0, 0, 0);

                if (target != null)
                {
                    if (tempWidthBindingBase != null)
                        target.SetBinding(WidthProperty, tempWidthBindingBase);
                    else if (tempAcutalWidth != 0)
                        target.Width = tempAcutalWidth;
                }
            }
            //var upperLeft=temp
        }

        private FrameworkElement GetNotNanWidthChild(FrameworkElement control)
        {
            if (!Double.IsNaN(control.Width) || BindingOperations.GetBindingBase(control, WidthProperty) != null)
            {
                return control;
            }

            int childCount = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(control, i) as FrameworkElement;
                if (!Double.IsNaN(child.Width) || BindingOperations.GetBindingBase(child, WidthProperty) != null)
                {
                    return child;
                }
                else
                {
                    var result = GetNotNanWidthChild(child);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }



        private void SetTopmostState(bool isTop)
        {
            // Don’t apply state if it’s the same as incoming state
            if (_appliedTopMost.HasValue && _appliedTopMost == isTop)
            {
                //  return;
            }

            if (Child == null)
                return;

            var hwndSource = (PresentationSource.FromVisual(Child)) as HwndSource;

            if (hwndSource == null)
                return;
            var hwnd = hwndSource.Handle;

            RECT rect;

            if (!GetWindowRect(hwnd, out rect))
                return;

            //    Debug.WriteLine("setting z-order " + isTop);

            if (isTop)
            {
                SetWindowPos(hwnd, HWND_TOPMOST, rect.Left, rect.Top, (int)Width, (int)Height, TOPMOST_FLAGS);
            }
            else
            {
                // Z-Order would only get refreshed/reflected if clicking the
                // the titlebar (as opposed to other parts of the external
                // window) unless I first set the popup to HWND_BOTTOM
                // then HWND_TOP before HWND_NOTOPMOST
                SetWindowPos(hwnd, HWND_BOTTOM, rect.Left, rect.Top, (int)Width, (int)Height, TOPMOST_FLAGS);
                SetWindowPos(hwnd, HWND_TOP, rect.Left, rect.Top, (int)Width, (int)Height, TOPMOST_FLAGS);
                SetWindowPos(hwnd, HWND_NOTOPMOST, rect.Left, rect.Top, (int)Width, (int)Height, TOPMOST_FLAGS);
            }

            _appliedTopMost = isTop;
        }

        #region P/Invoke imports & definitions
#pragma warning disable 1591 //Xml-doc
#pragma warning disable 169 //Never used-warning
        // ReSharper disable InconsistentNaming
        // Imports etc. with their naming rules

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
        int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        private const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOZORDER = 0x0004;
        const UInt32 SWP_NOREDRAW = 0x0008;
        const UInt32 SWP_NOACTIVATE = 0x0010;

        const UInt32 SWP_FRAMECHANGED = 0x0020; /* The frame changed: send WM_NCCALCSIZE */
        const UInt32 SWP_SHOWWINDOW = 0x0040;
        const UInt32 SWP_HIDEWINDOW = 0x0080;
        const UInt32 SWP_NOCOPYBITS = 0x0100;
        const UInt32 SWP_NOOWNERZORDER = 0x0200; /* Don’t do owner Z ordering */
        const UInt32 SWP_NOSENDCHANGING = 0x0400; /* Don’t send WM_WINDOWPOSCHANGING */

        const UInt32 TOPMOST_FLAGS =
            SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOSIZE | SWP_NOMOVE | SWP_NOREDRAW | SWP_NOSENDCHANGING;

        // ReSharper restore InconsistentNaming
#pragma warning restore 1591
#pragma warning restore 169
        #endregion
    }
}
