using System.Globalization;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// 实现一种使 <see cref="TextBox"/> 控件支持水印文字显示的行为。
    /// </summary>
    public class WatermarkBehavior : Behavior<TextBox>
    {
        private WatermarkAdorner _adorner;
        private AdornerLayer _adornerLayer;

        /// <summary>
        /// 获取或设置要显示的水印文字。
        /// </summary>
        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }

        /// <summary>
        /// 获取 <see cref="WatermarkText"/> 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register("WatermarkBehavior", typeof(string), typeof(WatermarkAdorner),
            new PropertyMetadata(string.Empty, OnWatermarkTextChanged));

        private static void OnWatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var watermarkBehavior = d as WatermarkBehavior;
            if (watermarkBehavior != null && e.OldValue != e.NewValue)
            {
                if (watermarkBehavior._adornerLayer != null)
                {
                    if (watermarkBehavior._adorner != null)
                    {
                        watermarkBehavior._adornerLayer.Remove(watermarkBehavior._adorner);
                    }
                    watermarkBehavior._adorner = new WatermarkAdorner(watermarkBehavior.AssociatedObject, watermarkBehavior.WatermarkText);
                    watermarkBehavior._adornerLayer.Add(watermarkBehavior._adorner);
                }
            }
        }

        /// <summary>
        /// 当行为附加到控件时发生。
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;

            if (AssociatedObject.IsLoaded)
            {
                AddAdorner();
            }
            else
            {
                AssociatedObject.Loaded += AssociatedObject_Loaded;
            }
        }

        /// <summary>
        /// 当行为从当前 <see cref="TextBox"/> 控件分离时发生。
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
            RemoveAdorner();
            base.OnDetaching();
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
            {
                AddAdorner();
            }
            else
            {
                RemoveAdorner();
            }
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AddAdorner();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            base.OnAttached();
        }

        private static AdornerLayer GetAdornerLayer(Visual dp)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(dp);
            if (adornerLayer == null)
            {
                var ownerWindow = Window.GetWindow(dp);
                if (ownerWindow != null)
                {
                    var adornerDecorator = ownerWindow.GetVisualChild<AdornerDecorator>();
                    if (adornerDecorator != null)
                    {
                        adornerLayer = adornerDecorator.AdornerLayer;
                    }
                }
            }

            return adornerLayer;
        }

        private void AddAdorner()
        {
            if (_adorner == null)
            {
                _adorner = new WatermarkAdorner(AssociatedObject, WatermarkText);
                _adornerLayer = GetAdornerLayer(AssociatedObject);
                if (_adornerLayer != null && !_adornerLayer.IsAncestorOf(_adorner))
                {
                    _adornerLayer.Add(_adorner);
                }
                _adorner.Hook();
            }
        }

        private void RemoveAdorner()
        {
            if (_adorner != null)
            {
                if (_adornerLayer != null)
                {
                    _adornerLayer.Remove(_adorner);
                }
                _adorner.Clear();
                _adorner = null;
            }
        }
    }

    internal class WatermarkAdorner : Adorner
    {
        private string watermark;
        private TextBox adornedTextBox;

        public WatermarkAdorner(UIElement target, string watermark)
            : base(target)
        {
            IsHitTestVisible = false;
            this.watermark = watermark;
            if (target is TextBox)
            {
                adornedTextBox = target as TextBox;
            }
        }

        void adornedTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            InvalidateVisual();
        }

        void adornedTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            InvalidateVisual();
        }

        void adornedTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            InvalidateVisual();
        }

        void adornedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (string.IsNullOrEmpty(adornedTextBox.Text) && !adornedTextBox.IsFocused)
            {
                var fmt = new FormattedText(watermark, CultureInfo.CurrentCulture,
                  FlowDirection.LeftToRight, new Typeface("微软雅黑"), 12, Brushes.Gray);
                fmt.SetFontStyle(FontStyles.Italic);
                dc.DrawText(fmt, new Point(4, (adornedTextBox.RenderSize.Height - fmt.Height) / 2));
            }
        }

        public void Hook()
        {
            adornedTextBox.Loaded += adornedTextBox_Loaded;
            adornedTextBox.TextChanged += adornedTextBox_TextChanged;
            adornedTextBox.GotFocus += adornedTextBox_GotFocus;
            adornedTextBox.LostFocus += adornedTextBox_LostFocus;
        }

        public void Clear()
        {
            adornedTextBox.Loaded -= adornedTextBox_Loaded;
            adornedTextBox.TextChanged -= adornedTextBox_TextChanged;
            adornedTextBox.GotFocus -= adornedTextBox_GotFocus;
            adornedTextBox.LostFocus -= adornedTextBox_LostFocus;
        }
    }
}
