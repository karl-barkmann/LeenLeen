using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Smart.Windows.Controls
{
    /// <summary>
    /// 可快速选择前一天或后一天的DatePicker。
    /// </summary>
    [TemplatePart(Name = "PART_UpButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_DownButton", Type = typeof(Button))]
    public class UpDownDatePicker : DatePicker
    {
        private Button btnUp;
        private Button btnDown;
        private DatePickerTextBox txtDate;
        private Popup popup;

        /// <summary>
        /// 可快速选择前一天或后一天的DatePicker。
        /// </summary>
        public UpDownDatePicker()
        { }

        /// <summary>
        /// 当应用新模板时生成 UpDownDatePicker 控件的可视化树。
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (btnUp != null)
                btnUp.Click -= btnUp_Click;
            if (btnDown != null)
                btnDown.Click -= btnDown_Click;
            if (txtDate != null)
            {
                txtDate.PreviewMouseLeftButtonDown -= txtDate_PreviewMouseLeftButtonDown;
                txtDate.LostFocus -= txtDate_LostFocus;
            }

            base.OnApplyTemplate();

            btnUp = GetTemplateChild("PART_UpButton") as Button;
            if (btnUp != null)
                btnUp.Click += btnUp_Click;
            btnDown = GetTemplateChild("PART_DownButton") as Button;
            if (btnDown != null)
                btnDown.Click += btnDown_Click;
            popup = GetTemplateChild("PART_Popup") as Popup;
            txtDate = GetTemplateChild("PART_TextBox") as DatePickerTextBox;
            if (txtDate != null)
            {
                if (txtDate.IsReadOnly)
                {
                    var dropDownButton = GetTemplateChild("PART_Button") as Button;
                    if (dropDownButton != null)
                        dropDownButton.Visibility = System.Windows.Visibility.Hidden;
                    txtDate.PreviewMouseLeftButtonDown += txtDate_PreviewMouseLeftButtonDown;
                    txtDate.LostFocus += txtDate_LostFocus;
                }
                else
                {
                    var dropDownButton = GetTemplateChild("PART_Button") as Button;
                    if (dropDownButton != null)
                        dropDownButton.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        #region 私有方法

        private void txtDate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtDate.InputHitTest(Mouse.GetPosition(txtDate)) != null)
                return;
            if (popup != null)
            {
                if (popup.InputHitTest(Mouse.GetPosition(popup)) != null)
                    return;
                if (popup.Child.InputHitTest(Mouse.GetPosition(popup.Child)) != null)
                    return;

                popup.IsOpen = false;
                popup.StaysOpen = false;
            }
        }

        private void txtDate_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (popup != null)
            {
                if (popup.IsOpen)
                {
                    popup.IsOpen = false;
                    popup.StaysOpen = false;
                }
                else
                {
                    popup.IsOpen = true;
                    popup.StaysOpen = true;
                }
            }

        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDate != null && SelectedDate.HasValue)
            {
                SelectedDate = SelectedDate.Value.AddDays(1);
            }
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDate != null && SelectedDate.HasValue)
            {
                SelectedDate = SelectedDate.Value.AddDays(-1);
            }
        }

        #endregion
    }
}
