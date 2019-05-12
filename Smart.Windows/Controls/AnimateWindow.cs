using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 可以动画控制 widht，height，top，left的窗体 (还不成熟 建议不要乱用)
    /// </summary>
    public class AnimateWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimateWindow"/> class.
        /// </summary>
        public AnimateWindow()
        {
            WindowStyle = System.Windows.WindowStyle.None;
        }

        #region TargetWidth

        private double targetWidth;

        /// <summary>
        /// Gets or sets the value of the <see cref="TargetWidth" />
        /// property. This is a dependency property.
        /// </summary>
        public double TargetWidth
        {
            get { return targetWidth; }
            set
            {
                if (targetWidth == value) return;
                targetWidth = value;
                Animate(this, Window.WidthProperty, Width, TargetWidth);
            }
        }

        #endregion

        #region TargetHeight

        private double targetHeight;

        /// <summary>
        /// Gets or sets the value of the <see cref="TargetHeight" />
        /// property. This is a dependency property.
        /// </summary>
        public double TargetHeight
        {
            get { return targetHeight; }
            set
            {
                if (targetHeight == value) return;
                targetHeight = value;
                Animate(this, Window.HeightProperty, Height, TargetHeight);
            }
        }

        #endregion

        #region TargetTop

        private double targetTop;

        /// <summary>
        /// Gets or sets the value of the <see cref="TargetTop" />
        /// property. This is a dependency property.
        /// </summary>
        public double TargetTop
        {
            get { return targetTop; }
            set
            {
                if (targetTop == value)
                    return;
                targetTop = value;



                if (Double.IsNaN(Top))
                    Top = 0;
                Animate(this, Window.TopProperty, Top, TargetTop);
                OnPropertyChanged("TargetTop");
            }
        }

        #endregion

        #region TargetLeft

        private double targetLeft;

        /// <summary>
        /// Gets or sets the value of the <see cref="TargetLeft" />
        /// property. This is a dependency property.
        /// </summary>
        public double TargetLeft
        {
            get { return targetLeft; }
            set
            {
                if (targetLeft == value) return;
                targetLeft = value;

                if (Double.IsNaN(Left))
                    Left = 0;
                Animate(this, Window.LeftProperty, Left, TargetLeft);
            }
        }

        #endregion

        /// <summary>
        /// Animates the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="property">The property.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        protected virtual void Animate(DependencyObject target, DependencyProperty property, double from, double to)
        {
            DoubleAnimationUsingKeyFrames translationAnimation = new DoubleAnimationUsingKeyFrames();
            translationAnimation.Duration = TimeSpan.FromSeconds(0.5);

            translationAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(from, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2))));

            translationAnimation.KeyFrames.Add(new SplineDoubleKeyFrame(to, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5))));

            Storyboard sb = new Storyboard();

            Storyboard.SetTarget(translationAnimation, target);
            Storyboard.SetTargetProperty(translationAnimation, new PropertyPath(property));

            sb.Children.Add(translationAnimation);
            sb.Begin();
        }

        /// <summary>
        /// 在更改属性值时发生。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
