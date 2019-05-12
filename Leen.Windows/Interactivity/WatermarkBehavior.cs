using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media;
using Leen.Windows.Utils;

namespace Leen.Windows.Interactivity
{
    public class WatermarkBehavior : Behavior<TextBox>
    {
        private WatermarkAdorner adorner;

        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }

        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register("WatermarkBehavior", typeof(string), typeof(WatermarkAdorner),
            new PropertyMetadata(String.Empty, OnWatermarkTextChanged));

        private static void OnWatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var watermarkBehavior = d as WatermarkBehavior;
            if (watermarkBehavior != null && e.OldValue != e.NewValue)
            {
                AdornerLayer adornerLayer = GetAdornerLayer(watermarkBehavior.AssociatedObject);
                if (adornerLayer != null)
                {
                    if (watermarkBehavior.adorner != null)
                    {
                        adornerLayer.Remove(watermarkBehavior.adorner);
                    }
                    watermarkBehavior.adorner = new WatermarkAdorner(watermarkBehavior.AssociatedObject, watermarkBehavior.WatermarkText);
                    adornerLayer.Add(watermarkBehavior.adorner);
                }
            }
        }

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

        void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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

        protected override void OnDetaching()
        {
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
            RemoveAdorner();
            base.OnDetaching();
        }

        private static AdornerLayer GetAdornerLayer(Visual dp)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(dp);
            if (adornerLayer == null)
            {
                var ownerWindow = Window.GetWindow(dp);
                if (ownerWindow != null)
                {
                    var decorator = ownerWindow.FindChild<AdornerDecorator>();
                    if (decorator != null)
                    {
                        adornerLayer = decorator.AdornerLayer;
                    }
                }
            }

            return adornerLayer;
        }

        private void AddAdorner()
        {
            if (adorner == null)
            {
                adorner = new WatermarkAdorner(AssociatedObject, WatermarkText);
            }
            AdornerLayer adornerLayer = GetAdornerLayer(AssociatedObject);
            if (adornerLayer != null && !adornerLayer.IsAncestorOf(adorner))
            {
                adornerLayer.Add(adorner);
            }
            adorner.Hook();
        }

        private void RemoveAdorner()
        {
            if (adorner != null)
            {
                AdornerLayer adornerLayer = GetAdornerLayer(AssociatedObject);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(adorner);
                }
                adorner.Clear();
            }
        }
    }

    public class WatermarkAdorner : Adorner
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
            if (String.IsNullOrEmpty(adornedTextBox.Text) && !adornedTextBox.IsFocused)
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
