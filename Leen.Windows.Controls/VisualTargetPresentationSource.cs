using System;
using System.Windows;
using System.Windows.Media;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 提供一种跨UI线程边界来呈现内容的互操作方案。
    /// </summary>
    public class VisualTargetPresentationSource : PresentationSource
    {
        private VisualTarget _visualTarget;

        /// <summary>
        /// 构造 <see cref="VisualTargetPresentationSource"/> 类的实例。
        /// </summary>
        /// <param name="hostVisual">表示连接到另一UI线程创建的视图的对象。</param>
        public VisualTargetPresentationSource(HostVisual hostVisual)
        {
            _visualTarget = new VisualTarget(hostVisual);
        }

        /// <summary>
        /// 获取获取设置根视图。
        /// </summary>
        public override Visual RootVisual
        {
            get => _visualTarget.RootVisual;
            set
            {
                Visual oldRoot = _visualTarget.RootVisual;
                _visualTarget.RootVisual = value;
                RootChanged(oldRoot, value);
                if (value is UIElement rootElement)
                {
                    rootElement.Measure(new Size(Double.PositiveInfinity,
                                                 Double.PositiveInfinity));
                    rootElement.Arrange(new Rect(rootElement.DesiredSize));
                }
            }
        }

        /// <summary>
        /// 获取一个值指示此对象是否已进行资源回放。
        /// </summary>
        public override bool IsDisposed => false;

        /// <summary>
        /// 获取呈现源的的可视目标。
        /// </summary>
        /// <returns>返回此呈现源的可视目标。</returns>
        protected override CompositionTarget GetCompositionTargetCore()
        {
            return _visualTarget;
        }
    }
}
