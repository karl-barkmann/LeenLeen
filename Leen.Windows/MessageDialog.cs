using Microsoft.Windows.Shell;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Leen.Windows
{
    /// <summary>
    /// 定义一个通用消息对话框窗体。
    /// </summary>
    [TemplateVisualState(GroupName = "ActivationStates", Name = "Blink")]
    [TemplateVisualState(GroupName = "ActivationStates", Name = "Normal")]
    [TemplatePart(Name = "PART_BtnYes", Type = typeof(Button))]
    [TemplatePart(Name = "PART_BtnNo", Type = typeof(Button))]
    [TemplatePart(Name = "PART_BtnCancel", Type = typeof(Button))]
    public class MessageDialog : BlinkWindow
    {
        private Button _btnYes;
        private Button _btnNo;
        private Button _btnCancel;

        /// <summary>
        /// 构造 <see cref="System.Windows.MessageBox"/> 类的实例。
        /// </summary>
        public MessageDialog()
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindowExcuted));
        }

        /// <summary>
        /// 获取或设置提示消息文本。
        /// </summary>
        public object Message
        {
            get { return GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// <see cref="Message"/> 的依赖属性定义。
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message),
                                        typeof(object),
                                        typeof(MessageDialog),
                                        new PropertyMetadata(string.Empty));

        /// <summary>
        /// 应用控件模板。
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (_btnYes != null)
                _btnYes.Click -= OnBtnYesClick;
            if (_btnNo != null)
                _btnNo.Click -= OnBtnNoClick;
            if (_btnCancel != null)
                _btnCancel.Click -= OnBtnCancel_Click;

            base.OnApplyTemplate();

            _btnYes = GetTemplateChild("PART_BtnYes") as Button;
            if (_btnYes != null)
                _btnYes.Click += OnBtnYesClick;
            _btnNo = GetTemplateChild("PART_BtnNo") as Button;
            if (_btnNo != null)
                _btnNo.Click += OnBtnNoClick;
            _btnCancel = GetTemplateChild("PARt_BtnCancel") as Button;
            if (_btnCancel != null)
                _btnCancel.Click += OnBtnCancel_Click;
        }

        /// <summary>
        /// 显示消息提示对话框。
        /// </summary>
        /// <param name="message">消息提示内容。</param>
        /// <param name="title">消息提示标题。</param>
        /// <param name="buttons">消息提示可选按钮。</param>
        /// <param name="owner">父窗体。</param>
        /// <returns></returns>
        public static bool? Show(string message, string title, MessageBoxButton buttons, IWin32Window owner)
        {
            var dlg = new MessageDialog();
            dlg.Message = message;
            dlg.Title = title;

            if (owner == null)
            {
                dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            else
            {
                var interopHelper = new WindowInteropHelper(dlg);
                interopHelper.EnsureHandle();
                interopHelper.Owner = owner.Handle;
            }

            ApplyButtons(buttons, dlg);
            return dlg.ShowDialog();
        }

        /// <summary>
        /// 显示消息提示对话框。
        /// </summary>
        /// <param name="message">消息提示内容。</param>
        /// <param name="title">消息提示标题。</param>
        /// <param name="buttons">消息提示可选按钮。</param>
        /// <param name="owner">父窗体。</param>
        /// <returns></returns>
        public static bool? Show(string message, string title, MessageBoxButton buttons, Window owner)
        {
            var dlg = new MessageDialog();
            dlg.Message = message;
            dlg.Title = title;
            dlg.Owner = owner;
            ApplyButtons(buttons, dlg);
            return dlg.ShowDialog();
        }

        private void CloseWindowExcuted(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private static void ApplyButtons(MessageBoxButton buttons, MessageDialog dlg)
        {
            if (buttons == MessageBoxButton.OK)
            {
                dlg._btnYes.Visibility = Visibility.Visible;
                dlg._btnYes.Content = "确定";
                dlg._btnNo.Visibility = Visibility.Collapsed;
                dlg._btnCancel.Visibility = Visibility.Collapsed;
            }
            else if (buttons == MessageBoxButton.OKCancel)
            {
                dlg._btnYes.Visibility = Visibility.Visible;
                dlg._btnYes.Content = "确定";
                dlg._btnNo.Visibility = Visibility.Collapsed;
                dlg._btnCancel.Visibility = Visibility.Visible;
                dlg._btnCancel.Content = "取消";
            }
            else if (buttons == MessageBoxButton.YesNo)
            {
                dlg._btnYes.Visibility = Visibility.Visible;
                dlg._btnYes.Content = "是";
                dlg._btnNo.Visibility = Visibility.Visible;
                dlg._btnNo.Content = "否";
                dlg._btnCancel.Visibility = Visibility.Collapsed;
            }
            else if (buttons == MessageBoxButton.YesNoCancel)
            {
                dlg._btnYes.Visibility = Visibility.Visible;
                dlg._btnYes.Content = "是";
                dlg._btnNo.Visibility = Visibility.Visible;
                dlg._btnNo.Content = "否";
                dlg._btnCancel.Visibility = Visibility.Visible;
                dlg._btnCancel.Content = "取消";
            }
        }

        private void OnBtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            Close();
        }

        private void OnBtnNoClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnBtnYesClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
