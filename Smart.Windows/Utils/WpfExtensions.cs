using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Leen.Native;

namespace Leen.Windows.Utils
{
    /// <summary>
    /// WPF通用扩展方法。
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 移除指定依赖属性的绑定。
        /// </summary>
        /// <typeparam name="TControl"></typeparam>
        /// <param name="ctrl"></param>
        /// <param name="dp">对象依赖属性。</param>
        public static void ClearBinding<TControl>(this TControl ctrl, DependencyProperty dp) where TControl : DependencyObject
        {
            BindingOperations.ClearBinding(ctrl, dp);
        }

        /// <summary>
        /// 移除所有绑定。
        /// </summary>
        /// <typeparam name="TControl"></typeparam>
        /// <param name="ctrl"></param>
        public static void ClearAll<TControl>(this TControl ctrl) where TControl : DependencyObject
        {
            BindingOperations.ClearAllBindings(ctrl);
        }

        /// <summary>
        /// 绑定按钮命令。
        /// </summary>
        /// <param name="button">依赖属性.</param>
        /// <param name="bindCommand">目标命令.</param>
        /// <param name="commandParameter">命令参数.</param>
        public static void BindCommand(this ButtonBase button, ICommand bindCommand, Object commandParameter = null)
        {
            if (button == null) return;

            button.Command = bindCommand;
            button.CommandParameter = commandParameter;
        }

        /// <summary>
        /// Binds the property.
        /// </summary>
        /// <param name="element">The dependency object.</param>
        /// <param name="dependencyProperty">The dependency property.</param>
        /// <param name="binding">The binding.</param>
        public static void BindProperty(this FrameworkElement element, DependencyProperty dependencyProperty, Binding binding)
        {
            if (element == null)
                return;

            element.SetBinding(dependencyProperty, binding);
        }

        /// <summary>
        /// 从<paramref name="parent"/>可视对象树中查找<paramref name="childName"/>指定名称的子控件。
        /// </summary>
        /// <typeparam name="T">子控件的类型。</typeparam>
        /// <param name="parent">子控件的祖先。</param>
        /// <param name="childName">子控件的名称</param>
        /// <returns></returns>
        public static T FindChild<T>(this DependencyObject parent, string childName)
            where T : DependencyObject
        {
            if (parent == null)
                return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                T childType = child as T;

                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null)
                        break;
                }
                else if (!String.IsNullOrEmpty(childName))
                {
                    var frameWorkElement = child as FrameworkElement;
                    if (frameWorkElement != null && frameWorkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }
            return foundChild;
        }

        /// <summary>
        /// 返回第一满足条件的子元素
        /// </summary>
        /// <typeparam name="T">子元素类型</typeparam>
        /// <param name="refrence">要查找的容器</param>
        /// <returns></returns>
        public static T FindChild<T>(this DependencyObject refrence)
            where T : DependencyObject
        {
            if (refrence == null) return null;

            int childCount = VisualTreeHelper.GetChildrenCount(refrence);

            DependencyObject child = null;
            for (int i = 0; i < childCount; i++)
            {
                child = VisualTreeHelper.GetChild(refrence, i);
                if (child != null)
                {
                    if (child is T)
                        return (T)child;
                    else
                        child = FindChild<T>(child);
                }
            }

            return (T)child;
        }

        /// <summary>
        /// 查找第一个满足条件的父元素。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="refrence"></param>
        /// <returns></returns>
        public static T FindParent<T>(this DependencyObject refrence) where T : DependencyObject
        {
            if (refrence == null) return null;

            DependencyObject parent = VisualTreeHelper.GetParent(refrence);
            while (parent != null && (parent as T) == null)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return (T)parent;
        }

        /// <summary> 
        /// Converts a <see cref="System.Drawing.Bitmap"/> into a WPF <see cref="BitmapSource"/>. 
        /// </summary> 
        /// <param name="source">Bitmap to convert</param> 
        /// <returns>Converted BitmapSource</returns> 
        public static BitmapSource ToBitmapSource(this Bitmap source)
        {
            var hBitmap = source.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, 
                    
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                GDI32.DeleteObject(hBitmap);
            }
        }
    }
}
