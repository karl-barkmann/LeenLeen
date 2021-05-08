using System.Linq;
using System.Windows.Controls;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// 自动调整列宽度的数据网格控件。
    /// <remarks>
    /// 按Column的单位为Star的宽度自动设置实际宽度。
    /// </remarks>
    /// </summary>
    public class DataGridColumnAdjustableBehavior : Behavior<DataGrid>
    {
        /// <summary>
        /// 在行为附加到 AssociatedObject 后调用。
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
        }

        /// <summary>
        /// 在行为与其 AssociatedObject 分离时（但在它实际发生之前）调用。
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
        }

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AssociatedObject.Columns == null || !AssociatedObject.Columns.Any())
                return;

            AssociatedObject.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double fixedWidth = AssociatedObject.Columns.Where(c => !c.Width.IsStar).Sum(c => c.Width.DesiredValue);
            double starValue = AssociatedObject.Columns.Where(c => c.Width.IsStar).Sum(c => c.Width.Value);

            var horizontalScrollbarVisibility = ScrollViewer.GetHorizontalScrollBarVisibility(AssociatedObject);
            /* REMAIN 2 PIXEL */
            double eachWidth = (e.NewSize.Width - fixedWidth - 0 - (horizontalScrollbarVisibility == ScrollBarVisibility.Disabled? 0 : SystemParameters.ScrollWidth)) / starValue;

            for (int i = 0; i < AssociatedObject.Columns.Count; i++)
            {
                if (AssociatedObject.Columns[i].Width.IsStar)
                {
                    AssociatedObject.Columns[i].Width = eachWidth * AssociatedObject.Columns[i].Width.Value;
                }
            }
        }
    }
}
