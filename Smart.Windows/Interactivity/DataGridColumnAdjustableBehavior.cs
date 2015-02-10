using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Smart.Windows.Interactivity
{
    /// <summary>
    /// 自动调整列宽度的数据网格控件。
    /// <remarks>
    /// 按Column的单位为Star的宽度自动设置实际宽度。
    /// </remarks>
    /// </summary>
    public class DataGridColumnAdjustableBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
        }

        void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AssociatedObject.Columns == null || !AssociatedObject.Columns.Any())
                return;

            AssociatedObject.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            double fixWidth = AssociatedObject.Columns.Where(c => c.Visibility!= Visibility.Collapsed && !c.Width.IsStar).Sum(c => c.Width.DesiredValue);
            double starValue = AssociatedObject.Columns.Where(c => c.Visibility != Visibility.Collapsed && c.Width.IsStar).Sum(c => c.Width.Value);

            var verticalScrollbarVisibility = ScrollViewer.GetVerticalScrollBarVisibility(AssociatedObject);
            /* REMAIN 2 PIXEL */
            double eachWidth = (e.NewSize.Width - fixWidth - 2 - (verticalScrollbarVisibility == ScrollBarVisibility.Disabled ? 0 : SystemParameters.ScrollWidth)) / starValue;

            for (int i = 0; i < AssociatedObject.Columns.Count; i++)
            {
                if (AssociatedObject.Columns[i].Width.IsStar)
                {
                    AssociatedObject.Columns[i].Width = eachWidth * AssociatedObject.Columns[i].Width.Value;
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
        }
    }
}
