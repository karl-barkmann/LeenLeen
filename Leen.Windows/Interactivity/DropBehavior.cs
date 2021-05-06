
using Leen.Windows.Utils;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows.Interactivity
{
    /// <summary>
    /// 使目标元素支持鼠标拖放。
    /// </summary>
    public class DropBehavior : CommandBehaviorBase<UIElement>
    {
        #region DataFormatProperty

        /// <summary>
        /// 拖拽放的可接收自定义数据格式。
        /// </summary>
        public string DataFormat
        {
            get { return (string)GetValue(DataFormatProperty); }
            set { SetValue(DataFormatProperty, value); }
        }

        /// <summary>
        /// 拖拽放的可接收自定义数据格式。
        /// </summary>
        public static readonly DependencyProperty DataFormatProperty =
            DependencyProperty.Register("DataFormat", typeof(string), typeof(DropBehavior), new PropertyMetadata(DataFormats.StringFormat));

        #endregion

        /// <summary>
        /// 当附加到目标元素时。
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.Drop += AssociatedObject_Drop;
            AssociatedObject.DragEnter += AssociatedObject_DragEnter;
            base.OnAttached();
        }

        /// <summary>
        /// 当从目标元素分离时。
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.Drop -= AssociatedObject_Drop;
            AssociatedObject.DragEnter -= AssociatedObject_DragEnter;
            base.OnDetaching();
        }
      
        #region Event Handlers

        private void AssociatedObject_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormat))
            {
                e.Effects = DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (e.Data.GetDataPresent(DataFormat))
            {
                var data = GetProperDroppingData(e.Data);
                if (Command != null && (Command.CanExecute(data) || Command.CanExecute(CommandParameter)))
                {
                    var bomb = CommandParameter ?? data;
                    if (bomb is IMouseAwareBomb mouseAwareBomb)
                        mouseAwareBomb.SetMousePosition(e.GetPosition(AssociatedObject));
                    Command.Execute(bomb);
                    e.Handled = true;
                }
            }
        }

        #endregion

        /// <summary>
        /// 提取适当的拖放数据。
        /// </summary>
        /// <param name="droppingData"></param>
        /// <returns></returns>
        protected virtual object GetProperDroppingData(IDataObject droppingData)
        {
            return droppingData.GetData(DataFormat);
        }
    }

    /// <summary>
    /// 使数据表表格支持鼠标拖放。
    /// </summary>
    public class DataGridDropBehavior : DropBehavior
    {
        /// <summary>
        /// 是否识别鼠标拖放目标行元素。
        /// </summary>
        public bool DetectDropingTarget
        {
            get { return (bool)GetValue(DetectDropingTargetProperty); }
            set { SetValue(DetectDropingTargetProperty, value); }
        }

        /// <summary>
        /// <see cref="DetectDropingTarget"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty DetectDropingTargetProperty =
            DependencyProperty.Register(nameof(DetectDropingTarget), typeof(bool), typeof(DataGridDropBehavior), new PropertyMetadata(true));


        /// <summary>
        /// 提取适当的拖放数据。
        /// </summary>
        /// <param name="droppingData"></param>
        /// <returns></returns>
        protected override object GetProperDroppingData(IDataObject droppingData)
        {
            if (DetectDropingTarget)
            {
                if (!(AssociatedObject is DataGrid))
                {
                    throw new ArgumentException("AssociatedObject is not a instance of 'DataGrid'!");
                }

                var mousePos = AssociatedObject.PointFromScreen(MouseUtil.GetMousePosition());
                HitTestResult testResult = VisualTreeHelper.HitTest(AssociatedObject, mousePos);
                if (testResult == null || testResult.VisualHit == null)
                {
                    return null;
                }

                var dataGridRow = testResult.VisualHit.GetVisualParent<DataGridRow>();
                return dataGridRow.DataContext;
            }
            return base.GetProperDroppingData(droppingData);
        }
    }
}
