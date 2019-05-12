using System;
using System.Windows;
using System.Windows.Media;

namespace Leen.Windows.Interaction
{
    /// <summary>
    /// Declares the Attached Properties and Behaviors for implementing popup window.
    /// </summary>
    public class WindowPopupBehavior
    {
        /// <summary>
        /// The <see cref="Style"/> to set to the popup window.
        /// </summary>
        public static readonly DependencyProperty ContainerWindowStyleProperty =
          DependencyProperty.RegisterAttached(
                              "ContainerWindowStyle",
                              typeof(Style), 
                              typeof(WindowPopupBehavior),
                              new PropertyMetadata(defaultValue: null));

        /// <summary>
        /// The title to set to the popup window.
        /// </summary>
        public static readonly DependencyProperty ContainerWindowTitleProperty =
            DependencyProperty.RegisterAttached(
                                "ContainerWindowTitle",
                                typeof(string),
                                typeof(WindowPopupBehavior),
                                new PropertyMetadata(defaultValue: null));

        /// <summary>
        /// The icon to set to the popup window.
        /// </summary>
        public static readonly DependencyProperty ContainerWindowIconProperty =
            DependencyProperty.RegisterAttached(
                                "ContainerWindowIcon", 
                                typeof(ImageSource), 
                                typeof(WindowPopupBehavior),
                                new PropertyMetadata(defaultValue: null));

        /// <summary>
        /// The window height set to the popup window.
        /// </summary>
        public static readonly DependencyProperty ContainerWindowHeightProperty =
            DependencyProperty.RegisterAttached(
                                "ContainerWindowHeight", 
                                typeof(double), 
                                typeof(WindowPopupBehavior), 
                                new PropertyMetadata(0.0d));

        /// <summary>
        /// The window width set to the popup window.
        /// </summary>
        public static readonly DependencyProperty ContainerWindowWidthProperty =
            DependencyProperty.RegisterAttached(
                                "ContainerWindowWidth",
                                typeof(double), 
                                typeof(WindowPopupBehavior),
                                new PropertyMetadata(0.0d));

        /// <summary>
        /// Gets the title for the popup window.
        /// </summary>
        /// <param name="owner">Owner of the attached dependency property.</param>
        /// <returns></returns>
        public static string GetContainerWindowTitle(DependencyObject owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return (string)owner.GetValue(ContainerWindowTitleProperty);
        }

        /// <summary>
        /// Sets the title for the popup window.
        /// </summary>
        /// <param name="owner">Owner of the attached dependency property.</param>
        /// <param name="value">The title for the popup window.</param>
        public static void SetContainerWindowTitle(DependencyObject owner, string value)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(ContainerWindowTitleProperty, value);
        }

        /// <summary>
        /// Gets the <see cref="Style"/> for the popup window.
        /// </summary>
        /// <param name="owner">Owner of the attached dependency property.</param>
        /// <returns>The <see cref="Style"/> for the popup window.</returns>
        public static Style GetContainerWindowStyle(DependencyObject owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            return owner.GetValue(ContainerWindowStyleProperty) as Style;
        }

        /// <summary>
        /// Sets the <see cref="Style"/> for the popup window.
        /// </summary>
        /// <param name="owner">Owner of the attached dependency property.</param>
        /// <param name="style"><see cref="Style"/> for the popup window.</param>
        public static void SetContainerWindowStyle(DependencyObject owner, Style style)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            owner.SetValue(ContainerWindowStyleProperty, style);
        }

        /// <summary>
        /// Gets the icon for the popup window.
        /// </summary>
        /// <param name="owner">Owner of the attached dependency property.</param>
        public static ImageSource GetContainerWindowIcon(DependencyObject owner)
        {
            return (ImageSource)owner.GetValue(ContainerWindowIconProperty);
        }

        /// <summary>
        /// Sets the icon for the popup window.
        /// </summary>
        /// <param name="owner">Owner of the attached dependency property.</param>
        /// <param name="value">Icon <see cref="ImageSource"/> for the popup window.</param>
        public static void SetContainerWindowIcon(DependencyObject owner, ImageSource value)
        {
            owner.SetValue(ContainerWindowIconProperty, value);
        }

        /// <summary>
        ///  Gets the window height for the popup window.
        /// </summary>
        /// <param name="owner">Owner of the attached dependency property.</param>
        /// <returns></returns>
        public static double GetContainerWindowHeight(DependencyObject owner)
        {
            return (double)owner.GetValue(ContainerWindowHeightProperty);
        }

        /// <summary>
        ///  Sets the window height for the popup window.
        /// </summary>
        /// <param name="owner">Owner of the attached dependency property.</param>
        /// <param name="height"></param>
        public static void SetContainerWindowHeight(DependencyObject owner, double height)
        {
            owner.SetValue(ContainerWindowHeightProperty, height);
        }

        /// <summary>
        ///  Gets the window width for the popup window.
        /// </summary>
        /// <param name="owner">Owner of the attached dependency property.</param>
        /// <returns></returns>
        public static double GetContainerWindowWidth(DependencyObject owner)
        {
            return (double)owner.GetValue(ContainerWindowWidthProperty);
        }

        /// <summary>
        ///  Sets the window width for the popup window.
        /// </summary>
        /// <param name="owner">Owner of the attached dependency property.</param>
        /// <param name="width"></param>
        public static void SetContainerWindowWidth(DependencyObject owner, double width)
        {
            owner.SetValue(ContainerWindowWidthProperty, width);
        }
    }
}
