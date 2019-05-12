using System;
using System.Collections;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 提供一种实现跨UI线程程序视图的控件基类。
    /// </summary>
    public abstract class UIThreadHostControl : FrameworkElement
    {
        /// <summary>
        /// 获取呈现的另一UI线程创建的框架元素。
        /// </summary>
        public FrameworkElement TargetElement { get; private set; }

        /// <summary>
        /// 获取连接到另一UI线程的视图对象。
        /// </summary>
        public HostVisual HostVisual { get; private set; }

        /// <summary>
        /// 获取提供跨线程边界将一个可视化树连接到另一个可视化树的功能的互操作对象。
        /// </summary>
        public VisualTargetPresentationSource VisualTarget { get; private set; }

        /// <summary>
        /// 获取另一UI线程的 <see cref="Dispatcher"/> 对象。
        /// </summary>
        public Dispatcher TargetDispatcher => TargetElement?.Dispatcher;

        /// <summary>
        /// 获取跨UI线程创建并呈现的元素的渲染大小。
        /// </summary>
        public Size TargetRenderSize { get; private set; }

        /// <summary>
        /// 获取此元素的子视图数量。
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return HostVisual != null ? 1 : 0;
            }
        }

        /// <summary>
        /// 获取此元素的包含的逻辑对象集合。
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (HostVisual != null)
                {
                    yield return HostVisual;
                }
            }
        }

        /// <summary>
        /// 需要在另一UI线程创建的框架元素。
        /// </summary>
        /// <returns></returns>
        protected abstract FrameworkElement CreateUIThreadControl();

        /// <summary>
        /// 获取指定索引处的视图对象。
        /// </summary>
        /// <param name="index">要获取的视图对象的索引。</param>
        /// <returns>返回指定索引处的视图对象。</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
            {
                return HostVisual;
            }

            throw new IndexOutOfRangeException(nameof(index));
        }

        /// <summary>
        /// 在布局系统进行测量时调用。
        /// </summary>
        /// <param name="constraint">布局系统测定的元素大小。</param>
        /// <returns>返回此元素测量完成后计算出的需求大小。</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            var uiSize = new Size();

            if (TargetElement != null)
            {
                TargetElement.Dispatcher.Invoke(
                    () =>
                    {
                        TargetElement.Measure(constraint);
                    },
                    DispatcherPriority.Background);
                uiSize.Width = TargetElement.DesiredSize.Width;
                uiSize.Height = TargetElement.DesiredSize.Height;
            }

            return uiSize;
        }

        /// <summary>
        /// 在布局系统进行排列定位时调用。
        /// </summary>
        /// <param name="finalSize">布局系统在测量完成后得到的控件最终大小。</param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (TargetElement != null)
            {
                TargetElement.Dispatcher.BeginInvoke(
                    () =>
                    {
                        TargetElement.Arrange(new Rect(finalSize));
                    },
                    DispatcherPriority.Background);
            }
            return finalSize;
        }

        /// <summary>
        /// 在此元素初始化完后调用。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnInitialized(EventArgs e)
        {
            Loaded += (sender, args) =>
            {
                LoadUIThreadControl();
            };

            Unloaded += (sender, args) =>
            {
                UnloadUIThreadControl();
            };

            base.OnInitialized(e);
        }

        private void CreateControlOnUIThread()
        {
            TargetElement = CreateUIThreadControl();

            if (TargetElement == null)
            {
                return;
            }
            TargetElement.SizeChanged += TargetElement_SizeChanged;
            VisualTarget = new VisualTargetPresentationSource(HostVisual)
            {
                RootVisual = TargetElement
            };
            Dispatcher.Run();
        }

        private void LoadUIThreadControl()
        {
            if (TargetDispatcher != null)
            {
                return;
            }

            HostVisual = new HostVisual();

            AddLogicalChild(HostVisual);
            AddVisualChild(HostVisual);

            var uiThread = new Thread(CreateControlOnUIThread)
            {
                IsBackground = true
            };
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
        }

        private void UnloadUIThreadControl()
        {
            if (TargetDispatcher == null)
            {
                return;
            }

            TargetElement.SizeChanged -= TargetElement_SizeChanged;
            TargetDispatcher.InvokeShutdown();

            RemoveLogicalChild(HostVisual);
            RemoveVisualChild(HostVisual);

            HostVisual = null;
            TargetElement = null;
        }

        private void TargetElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TargetRenderSize = TargetElement.RenderSize;
        }
    }
}
