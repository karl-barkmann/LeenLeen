
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
}
