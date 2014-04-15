using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Smart.Windows.Controls
{
    /// <summary>
    /// 自动动画到实际高度大小的面板，初始高度为MinHeight
    /// </summary>
    public class HeightExpander : ContentControl
    {
        /// <summary>
        /// 
        /// </summary>
        public HeightExpander()
        {
            Height = MinHeight;
        }

        #region Duration

        /// <summary>
        /// The <see cref="Duration" /> dependency property's name.
        /// </summary>
        public const string DurationPropertyName = "Duration";

        /// <summary>
        /// 获取或设置 动画过度 单位秒        
        /// </summary>
        public double Duration
        {
            get { return (double)GetValue(DurationProperty); }
            set
            {
                SetValue(DurationProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Duration" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(DurationPropertyName, typeof(double), typeof(HeightExpander), new PropertyMetadata());

        #endregion

        /// <summary>
        /// The <see cref="IsExpanded" /> dependency property's name.
        /// </summary>
        public const string IsExpandedPropertyName = "IsExpanded";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsExpanded" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }
            set
            {
                SetValue(IsExpandedProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsExpanded" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
            IsExpandedPropertyName,
            typeof(bool),
            typeof(HeightExpander),
            new PropertyMetadata((new PropertyChangedCallback(PropertyChangedCallback))));

        static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeightExpander sender = (HeightExpander)d;

            DoubleAnimationUsingKeyFrames translationAnimation = new DoubleAnimationUsingKeyFrames();
            translationAnimation.Duration = TimeSpan.FromSeconds(0.7);

            if ((bool)e.NewValue)
            {
                var temp = sender.Content as FrameworkElement;
                if (temp != null)
                {
                    sender.Animate(sender, HeightProperty, sender.MinHeight, temp.ActualHeight);
                }
            }
            else
            {
                sender.Animate(sender, HeightProperty, sender.ActualHeight, sender.MinHeight);
            }
        }

        void Animate(DependencyObject target, DependencyProperty property, double from, double to)
        {
            DoubleAnimationUsingKeyFrames translationAnimation = new DoubleAnimationUsingKeyFrames();
            translationAnimation.Duration = TimeSpan.FromSeconds(0.7);

            translationAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(from, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));

            translationAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(to, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(Duration))));

            Storyboard sb = new Storyboard();

            Storyboard.SetTarget(translationAnimation, target);
            Storyboard.SetTargetProperty(translationAnimation, new PropertyPath(property));

            sb.Children.Add(translationAnimation);
            sb.Begin();
        }
    }
}
