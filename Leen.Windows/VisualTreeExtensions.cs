using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace System.Windows
{
    /// <summary>
    /// 提供一组用于在可视化视图树上进行查找的扩展方法。
    /// </summary>
    public static class VisualTreeExtensions
    {
        /// <summary>
        /// 从当前依赖属性系统对象开始，在视图树自下而上的查找指定类型的依赖属性系统对象，返回第一个匹配的值。
        /// </summary>
        /// <typeparam name="T">要查找的依赖属性系统对象的类型。</typeparam>
        /// <param name="child">当前依赖属性系统对象。</param>
        /// <returns></returns>
        public static T GetVisualParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                return GetVisualParent<T>(parentObject);
            }
        }

        /// <summary>
        /// 从当前依赖属性系统对象开始，在视图树自下而上的查找指定类型的依赖属性系统对象（包括当前对象），返回第一个匹配的值。
        /// </summary>
        /// <typeparam name="T">要查找的依赖属性系统对象的类型。</typeparam>
        /// <param name="child">当前依赖属性系统对象。</param>
        /// <returns></returns>
        public static T GetVisual<T>(this DependencyObject child) where T : DependencyObject
        {
            if (child is T)
                return child as T;
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                return GetVisual<T>(parentObject);
            }
        }

        /// <summary>
        /// 从当前依赖属性系统对象开始，在视图树自上而下的查找指定类型的依赖属性系统对象，返回第一个匹配的值。
        /// </summary>
        /// <typeparam name="T">要查找的依赖属性系统对象的类型。</typeparam>
        /// <param name="d">当前依赖属性系统对象。</param>
        /// <returns></returns>
        public static T GetVisualChild<T>(this DependencyObject d) where T : DependencyObject
        {
            int childCount = VisualTreeHelper.GetChildrenCount(d);
            if (childCount == 0)
                return null;

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);
                if (child is T result)
                {
                    return result;
                }

                result = child.GetVisualChild<T>();
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// 从当前依赖属性系统对象开始，在视图树自上而下的查找指定类型的依赖属性系统对象，返回所有匹配的值。
        /// </summary>
        /// <typeparam name="T">要查找的依赖属性系统对象的类型。</typeparam>
        /// <param name="d">当前依赖属性系统对象。</param>
        /// <returns></returns>
        public static IEnumerable<T> GetVisualChildren<T>(this DependencyObject d) where T : DependencyObject
        {
            int childCount = VisualTreeHelper.GetChildrenCount(d);
            if (childCount == 0)
                return null;

            List<T> children = new List<T>();
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);
                if (child == null)
                {
                    continue;
                }
                if (child is T result)
                {
                    children.Add(result);
                }

                var results = child.GetVisualChildren<T>();
                if (results != null)
                {
                    children.AddRange(results);
                }
            }

            return children;
        }

        /// <summary>
        /// Counts type of <typeparamref name="T"/> visuals within the visual tree.
        /// </summary>
        /// <param name="visual">It's of type DependencyObject but we know it can only be a Visual or Visual3D. </param>
        /// <returns>Number of visuals in the visual tree.</returns>
        public static int GetVisualCount<T>(this DependencyObject visual) where T : Visual
        {
            int visualCount = 0;
            int childCount = VisualTreeHelper.GetChildrenCount(visual);

            for (int i = 0; i < childCount; i++)
            {
                DependencyObject childVisual = VisualTreeHelper.GetChild(visual, i);
                if (childVisual is T)
                {
                    visualCount += 1;
                }
                visualCount += GetVisualCount<T>(childVisual);
            }
            return visualCount;
        }

        /// <summary>
        /// Disconnect a viusal from it's parent.
        /// </summary>
        /// <param name="parent">parent visual in the visual tree.</param>
        /// <param name="visual">viusal which need to disconnect.</param>
        public static void Disconnect(this DependencyObject visual, DependencyObject parent)
        {
            if (visual == null)
            {
                throw new ArgumentNullException("visual");
            }

            if (parent == null)
                return;

            if (parent is Panel panel)
            {
                panel.Children.Remove(visual as UIElement);
                return;
            }

            if (parent is Decorator decorator)
            {
                if (decorator.Child == visual)
                {
                    decorator.Child = null;
                }
                return;
            }

            if (parent is ContentPresenter contentPresenter)
            {
                if (contentPresenter.Content == visual)
                {
                    contentPresenter.Content = null;
                }
                return;
            }

            if (parent is ContentControl contentControl)
            {
                if (contentControl.Content == visual)
                {
                    contentControl.Content = null;
                }
                return;
            }
        }

        /// <summary>
        /// Disconnect a viusal from it's parent then connect to a new parent.
        /// </summary>
        /// <param name="source">The old parent visual in the visual tree.</param>
        /// <param name="target">The new parent visual in the visual tree.</param>
        /// <param name="visual">viusal which need to disconnect.</param>
        public static void Connect(this DependencyObject visual, DependencyObject source, DependencyObject target)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (visual == null)
                return;

            if (visual == target)
            {
                return;
            }
            else
            {
                visual.Disconnect(source);
            }

            DependencyObject child = visual;
            if (target is Panel panel)
            {
                var element = child as UIElement;
                if (!panel.Children.Contains(element) && element != null)
                {
                    panel.Children.Add(element);
                }
                else
                {
                    throw new InvalidOperationException("visual is already a child of newParent!");
                }
                return;
            }

            if (target is Decorator decorator)
            {
                if (decorator.Child != child && decorator.Child != null)
                {
                    throw new InvalidOperationException("newParent contains other ui element!");
                }
                decorator.Child = child as UIElement;
                return;
            }

            if (target is ContentPresenter contentPresenter)
            {
                if (contentPresenter.Content != child && contentPresenter.Content != null)
                {
                    throw new InvalidOperationException("newParent contains other content!");
                }
                contentPresenter.Content = child;
                return;
            }

            if (target is ContentControl contentControl)
            {
                if (contentControl.Content != child && contentControl.Content != null)
                {
                    throw new InvalidOperationException("newParent contains other content!");
                }
                contentControl.Content = child;
                return;
            }
        }

        /// <summary>
        /// 从指定的 <see cref="Visual"/> 生成一张缩略图片并返回该图片的路径。
        /// </summary>
        /// <param name="visual">用以生成结果的呈现对象。</param>
        /// <param name="width">需要生成的图片宽度。</param>
        /// <param name="height">需要生成的图片高度。</param>
        /// <returns></returns>
        public static string GetThumbnailImage(this Visual visual, double width, double height)
        {
            if (width < 0 || width > double.PositiveInfinity || width == double.NaN)
            {
                throw new ArgumentException($"invalid {nameof(width)}", nameof(width));
            }

            if (height < 0 || height > double.PositiveInfinity || height == double.NaN)
            {
                throw new ArgumentException($"invalid {nameof(height)}", nameof(height));
            }

            Guid guid = Guid.NewGuid();
            string thumbnailPath = Path.Combine(
                Path.GetTempPath(),
                guid.ToString() + ".png");
            Image imgScreen = new Image()
            {
                Width = width,
                Height = height,
                Source = new DrawingImage(VisualTreeHelper.GetDrawing(visual))
            };
            using (FileStream stream = new FileStream(thumbnailPath, FileMode.Create))
            {
                DrawingVisual vis = new DrawingVisual();
                using (DrawingContext cont = vis.RenderOpen())
                {
                    cont.DrawImage(imgScreen.Source, new Rect(new Size(imgScreen.Width, imgScreen.Height)));
                }

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)imgScreen.Width,
                    (int)imgScreen.Height, 96d, 96d, PixelFormats.Default);
                rtb.Render(vis);

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(stream);
            }
            return thumbnailPath;
        }

        /// <summary>
        /// 从指定的 <see cref="Visual"/> 生成一个 <see cref="DrawingVisual"/>。
        /// </summary>
        /// <param name="visual">用以生成结果的呈现对象。</param>
        /// <param name="width">需要生成的可视对象宽度。</param>
        /// <param name="height">需要生成的可视对象高度。</param>
        /// <returns></returns>
        public static DrawingVisual CreateDrawingImage(this Visual visual, double width, double height)
        {
            if (width < 0 || width > double.PositiveInfinity || width == double.NaN)
            {
                throw new ArgumentException($"invalid {nameof(width)}", nameof(width));
            }

            if (height < 0 || height > double.PositiveInfinity || height == double.NaN)
            {
                throw new ArgumentException($"invalid {nameof(height)}", nameof(height));
            }

            var drawingVisual = new DrawingVisual();
            // open the Render of the DrawingVisual  
            using (var dc = drawingVisual.RenderOpen())
            {
                var drawingImage = new DrawingImage(VisualTreeHelper.GetDrawing(visual));
                var rectangle = new Rect
                {
                    X = 0,
                    Y = 0,
                    Width = width,
                    Height = height,
                };

                // draw the white background  
                dc.DrawRectangle(Brushes.White, null, rectangle);
                // draw the visual  
                //NOTE:  
                // instead of Creating one VisualBrush, it create and use one DrawingImage  
                // and use that DrawingImage , because VisualBrush has issue with WebBrowser  
                //   
                dc.DrawImage(drawingImage, rectangle);
            }

            return drawingVisual;
        }

        /// <summary>
        /// Counts all visuals within the visual tree.
        /// </summary>
        /// <param name="visual">It's of type DependencyObject but we know it can only be a Visual or Visual3D. </param>
        /// <returns>Number of visuals in the visual tree.</returns>
        private static int GetVisualCount(this DependencyObject visual)
        {
            int visualCount = 0;
            int childCount = VisualTreeHelper.GetChildrenCount(visual);

            for (int i = 0; i < childCount; i++)
            {
                DependencyObject childVisual = VisualTreeHelper.GetChild(visual, i);
                visualCount += GetVisualCount(childVisual);
            }
            visualCount += childCount;
            return visualCount;
        }
    }
}
