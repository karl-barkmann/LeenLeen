using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Leen.Media.Controls
{
    /// <summary>
    /// Introduces <see cref="MatrixAnimation"/> class, which performs linear, smooth animation of the Matrix type. 
    /// The animation supports translation, scaling and rotation along with easing functions. 
    /// </summary>
    public class MatrixAnimation : MatrixAnimationBase
    {
        /// <summary>
        /// Gets or sets base value of the animation.
        /// </summary>
        public Matrix? From
        {
            set { SetValue(FromProperty, value); }
            get { return (Matrix)GetValue(FromProperty); }
        }

        /// <summary>
        /// Dependency property of <see cref="From"/> property.
        /// </summary>
        public static DependencyProperty FromProperty =
            DependencyProperty.Register(nameof(From), typeof(Matrix?), typeof(MatrixAnimation),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets target value of the animation.
        /// </summary>
        public Matrix? To
        {
            set { SetValue(ToProperty, value); }
            get { return (Matrix)GetValue(ToProperty); }
        }

        /// <summary>
        /// Dependency property of <see cref="To"/> property.
        /// </summary>
        public static DependencyProperty ToProperty =
            DependencyProperty.Register(nameof(To), typeof(Matrix?), typeof(MatrixAnimation),
                new PropertyMetadata(null));

        /// <summary>
        /// Get or sets easing function of the animation.
        /// </summary>
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        /// <summary>
        /// Dependency property of <see cref="EasingFunction"/> property.
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(nameof(EasingFunction), typeof(IEasingFunction), typeof(MatrixAnimation),
                new UIPropertyMetadata(null));

        /// <summary>
        /// Instantiate a new instance of <see cref="MatrixAnimation"/> class.
        /// </summary>
        public MatrixAnimation()
        {
        }

        /// <summary>
        /// Instantiate a new instance of <see cref="MatrixAnimation"/> class.
        /// </summary>
        /// <param name="toValue">Target value of the animation.</param>
        /// <param name="duration">Animation duration.</param>
        public MatrixAnimation(Matrix toValue, Duration duration)
        {
            To = toValue;
            Duration = duration;
        }

        /// <summary>
        /// Instantiate a new instance of <see cref="MatrixAnimation"/> class.
        /// </summary>
        /// <param name="toValue">Target value of the animation.</param>
        /// <param name="duration">Animation duration.</param>
        /// <param name="fillBehavior">Fill behavior of the animation.</param>
        public MatrixAnimation(Matrix toValue, Duration duration, FillBehavior fillBehavior)
        {
            To = toValue;
            Duration = duration;
            FillBehavior = fillBehavior;
        }

        /// <summary>
        /// Instantiate a new instance of <see cref="MatrixAnimation"/> class.
        /// </summary>
        /// <param name="fromValue">Base value of the animation.</param>
        /// <param name="toValue">Target value of the animation.</param>
        /// <param name="duration">Animation duration.</param>
        public MatrixAnimation(Matrix fromValue, Matrix toValue, Duration duration)
        {
            From = fromValue;
            To = toValue;
            Duration = duration;
        }

        /// <summary>
        /// Instantiate a new instance of <see cref="MatrixAnimation"/> class.
        /// </summary>
        /// <param name="fromValue">Base value of the animation.</param>
        /// <param name="toValue">Target value of the animation.</param>
        /// <param name="duration">Animation duration.</param>
        /// <param name="fillBehavior">Fill behavior of the animation.</param>
        public MatrixAnimation(Matrix fromValue, Matrix toValue, Duration duration, FillBehavior fillBehavior)
        {
            From = fromValue;
            To = toValue;
            Duration = duration;
            FillBehavior = fillBehavior;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new MatrixAnimation();
        }

        protected override Matrix GetCurrentValueCore(Matrix defaultOriginValue, Matrix defaultDestinationValue, AnimationClock animationClock)
        {
            if (animationClock.CurrentProgress == null)
            {
                return Matrix.Identity;
            }

            var normalizedTime = animationClock.CurrentProgress.Value;
            if (EasingFunction != null)
            {
                normalizedTime = EasingFunction.Ease(normalizedTime);
            }

            var from = From ?? defaultOriginValue;
            var to = To ?? defaultDestinationValue;

            var newMatrix = new Matrix(
                    ((to.M11 - from.M11) * normalizedTime) + from.M11,
                    ((to.M12 - from.M12) * normalizedTime) + from.M12,
                    ((to.M21 - from.M21) * normalizedTime) + from.M21,
                    ((to.M22 - from.M22) * normalizedTime) + from.M22,
                    ((to.OffsetX - from.OffsetX) * normalizedTime) + from.OffsetX,
                    ((to.OffsetY - from.OffsetY) * normalizedTime) + from.OffsetY);

            return newMatrix;
        }
    }
}
