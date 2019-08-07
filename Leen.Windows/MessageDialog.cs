using Microsoft.Windows.Shell;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

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
    [TemplatePart(Name = "PART_IconHolder", Type = typeof(Path))]
    public class MessageDialog : BlinkWindow
    {
        private Button _btnYes;
        private Button _btnNo;
        private Button _btnCancel;
        private Path _pathIcon;

        private const string Error = "M512 0C229.376 0 0 229.376 0 512s229.376 512 512 512 512-229.376 512-512S794.624 0 512 0z m218.624 672.256c15.872 15.872 15.872 41.984 0 57.856-8.192 8.192-18.432 11.776-29.184 11.776s-20.992-4.096-29.184-11.776L512 569.856l-160.256 160.256c-8.192 8.192-18.432 11.776-29.184 11.776s-20.992-4.096-29.184-11.776c-15.872-15.872-15.872-41.984 0-57.856L454.144 512 293.376 351.744c-15.872-15.872-15.872-41.984 0-57.856 15.872-15.872 41.984-15.872 57.856 0L512 454.144l160.256-160.256c15.872-15.872 41.984-15.872 57.856 0 15.872 15.872 15.872 41.984 0 57.856L569.856 512l160.768 160.256z";
        private const string Info = "M22 0C9.86959 0 0 9.86959 0 22C0 34.1304 9.86959 44 22 44C34.1304 44 44 34.1304 44 22C44 9.86959 34.1304 0 22 0L22 0ZM22 3.38462C32.2993 3.38462 40.6154 11.7007 40.6154 22C40.6154 32.2993 32.2993 40.6154 22 40.6154C11.7007 40.6154 3.38462 32.2993 3.38462 22C3.38462 11.7007 11.7007 3.38462 22 3.38462L22 3.38462ZM20.3077 11.8462L20.3077 15.2308L23.6923 15.2308L23.6923 11.8462L20.3077 11.8462ZM20.3077 18.6154L20.3077 32.1538L23.6923 32.1538L23.6923 18.6154L20.3077 18.6154Z";
        private const string Warning = "M26.5032 36.9266C25.8077 37.6369 24.9664 37.99 23.9886 37.99C23.0108 37.99 22.1492 37.6369 21.45 36.9266C20.7267 36.2129 20.3799 35.3599 20.3799 34.3417C20.3799 33.346 20.7267 32.465 21.45 31.7567C22.1492 31.0184 23.0108 30.6633 23.9886 30.6633C24.9664 30.6633 25.8058 31.0183 26.5032 31.7567C27.2282 32.465 27.5751 33.346 27.5751 34.3417C27.5751 35.3599 27.2282 36.2129 26.5032 36.9266ZM21.4098 12.5442C22.0832 11.8096 22.9485 11.4302 23.9927 11.4302C25.0406 11.4302 25.904 11.7852 26.5793 12.5442C27.2323 13.2787 27.5792 14.2049 27.5792 15.3208C27.5792 16.2676 26.1807 23.3125 25.6955 28.4355L22.3175 28.4355C21.9208 23.3106 20.384 16.2657 20.384 15.3208C20.384 14.2293 20.7328 13.3031 21.4098 12.5442ZM46.9824 36.4983L28.1623 3.05886C25.858 -1.01962 22.1054 -1.01962 19.803 3.05886L0.982908 36.4983C-1.31956 40.575 0.56226 43.9189 5.17457 43.9189L42.8129 43.9189C47.4013 43.9189 49.2886 40.575 46.9824 36.4983Z";
        private const string Question = "M21.642683,29.199997 C22.385555,29.199997 23.12843,29.542854 23.499866,29.885714 24.05702,30.4 24.242738,30.914284 24.242738,31.599998 24.242738,32.285711 24.05702,32.799997 23.499866,33.314285 22.942709,33.828571 22.385555,33.999998 21.642683,33.999998 20.899812,33.999998 20.342657,33.828571 19.785503,33.314285 19.228349,32.799997 19.042631,32.285711 19.042631,31.599998 19.042631,30.914284 19.228349,30.228571 19.785503,29.885714 20.342657,29.371426 20.899812,29.199997 21.642683,29.199997 z M22.199838,9.9999996 C24.614174,9.9999996 26.471354,10.514285 27.9571,11.885714 29.257126,13.085714 30.000002,14.628571 30.000002,16.685713 29.999998,18.228571 29.628562,19.599999 28.699972,20.62857 28.328536,20.971428 27.399946,21.82857 25.728481,23.199997 24.98561,23.714286 24.614174,24.4 24.242738,24.914285 23.871303,25.599998 23.685584,26.285711 23.685584,27.142857 L23.685584,27.657143 19.785503,27.657143 19.785503,27.142857 C19.785503,25.942855 20.156939,24.914285 20.528376,24.057142 21.08553,23.199997 22.385555,21.82857 24.614174,19.942856 L25.171328,19.257141 C25.728481,18.571427 26.099917,17.714285 26.099917,16.857141 26.099917,15.657142 25.728481,14.799999 24.98561,14.114285 24.242738,13.428571 23.314149,13.085714 22.014119,13.085714 20.342657,13.085714 19.228349,13.599999 18.485476,14.457143 18.114039,14.971428 17.928322,15.657142 17.742604,16.342856 17.556886,17.199999 16.814012,17.885714 15.885423,17.885714 14.585395,17.885714 13.842524,16.685713 14.028241,15.657142 14.399677,14.114285 14.956831,12.914285 16.071141,12.057142 17.556886,10.685713 19.599785,9.9999996 22.199838,9.9999996 z M22,2.7866669 C11.44,2.7866669 2.7866669,11.44 2.7866669,22 2.7866669,32.559999 11.44,41.213333 22,41.213333 32.559999,41.213333 41.213333,32.559999 41.213333,22 41.213333,11.44 32.559999,2.7866669 22,2.7866669 z M22,0 C34.173334,0 44,9.8266667 44,22 44,34.173334 34.173334,44 22,44 9.8266668,44 0,34.173334 0,22 0,9.8266667 9.8266668,0 22,0 z";

        /// <summary>
        /// 构造 <see cref="MessageBox"/> 类的实例。
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


        #region Icon

        /// <summary>
        /// Gets or sets the <see cref="Icon"/> value.
        /// </summary>
        public new MessageBoxImage Icon
        {
            get { return (MessageBoxImage)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Icon"/> property.
        /// </summary>
        public new static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                                nameof(Icon),
                                typeof(MessageBoxImage),
                                typeof(MessageDialog),
                                new PropertyMetadata(MessageBoxImage.None, IconPropertyChanged));

        private static void IconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MessageDialog)d).OnIconChanged((MessageBoxImage)e.OldValue, (MessageBoxImage)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Icon"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Icon"/>.</param>
        /// <param name="newValue">New value of <see cref="Icon"/>.</param>
        protected virtual void OnIconChanged(MessageBoxImage oldValue, MessageBoxImage newValue)
        {

        }

        #endregion


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

            _pathIcon = GetTemplateChild("PART_IconHolder") as Path;
            _btnYes = GetTemplateChild("PART_BtnYes") as Button;
            if (_btnYes != null)
                _btnYes.Click += OnBtnYesClick;
            _btnNo = GetTemplateChild("PART_BtnNo") as Button;
            if (_btnNo != null)
                _btnNo.Click += OnBtnNoClick;
            _btnCancel = GetTemplateChild("PART_BtnCancel") as Button;
            if (_btnCancel != null)
                _btnCancel.Click += OnBtnCancel_Click;
        }

        /// <summary>
        /// 显示消息提示对话框。
        /// </summary>
        /// <param name="message">消息提示内容。</param>
        /// <param name="title">消息提示标题。</param>
        /// <param name="buttons">消息提示可选按钮。</param>
        /// <param name="image">消息提示图标。</param>
        /// <param name="owner">父窗体。</param>
        /// <returns></returns>
        public static bool? Show(string message, string title, MessageBoxButton buttons, MessageBoxImage image, IWin32Window owner)
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
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                var interopHelper = new WindowInteropHelper(dlg);
                interopHelper.EnsureHandle();
                interopHelper.Owner = owner.Handle;
            }
            ApplyStyle(dlg);
            dlg.ApplyTemplate();
            ApplyButtons(buttons, dlg);
            ApplyImages(image, dlg);
            return dlg.ShowDialog();
        }

        /// <summary>
        /// 显示消息提示对话框。
        /// </summary>
        /// <param name="message">消息提示内容。</param>
        /// <param name="title">消息提示标题。</param>
        /// <param name="buttons">消息提示可选按钮。</param>
        /// <param name="image">消息提示图标。</param>
        /// <param name="owner">父窗体。</param>
        /// <returns></returns>
        public static bool? Show(string message, string title, MessageBoxButton buttons, MessageBoxImage image, Window owner)
        {
            var dlg = new MessageDialog();
            dlg.Message = message;
            dlg.Title = title;
            dlg.Owner = owner;
            if (owner == null)
                dlg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            else
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ApplyStyle(dlg);
            dlg.ApplyTemplate();
            ApplyButtons(buttons, dlg);
            ApplyImages(image, dlg);
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
                if (dlg._btnYes != null)
                {
                    dlg._btnYes.Visibility = Visibility.Visible;
                    dlg._btnYes.Content = "确定";
                }
                if (dlg._btnNo != null)
                    dlg._btnNo.Visibility = Visibility.Collapsed;
                if (dlg._btnCancel != null)
                    dlg._btnCancel.Visibility = Visibility.Collapsed;
            }
            else if (buttons == MessageBoxButton.OKCancel)
            {
                if (dlg._btnYes != null)
                {
                    dlg._btnYes.Visibility = Visibility.Visible;
                    dlg._btnYes.Content = "确定";
                }
                if (dlg._btnNo != null)
                    dlg._btnNo.Visibility = Visibility.Collapsed;
                if (dlg._btnCancel != null)
                {
                    dlg._btnCancel.Visibility = Visibility.Visible;
                    dlg._btnCancel.Content = "取消";
                }
            }
            else if (buttons == MessageBoxButton.YesNo)
            {
                if (dlg._btnYes != null)
                {
                    dlg._btnYes.Visibility = Visibility.Visible;
                    dlg._btnYes.Content = "是";
                }
                if (dlg._btnNo != null)
                {
                    dlg._btnNo.Visibility = Visibility.Visible;
                    dlg._btnNo.Content = "否";
                }
                if (dlg._btnCancel != null)
                    dlg._btnCancel.Visibility = Visibility.Collapsed;
            }
            else if (buttons == MessageBoxButton.YesNoCancel)
            {
                if (dlg._btnYes != null)
                {
                    dlg._btnYes.Visibility = Visibility.Visible;
                    dlg._btnYes.Content = "是";
                }

                if (dlg._btnNo != null)
                {
                    dlg._btnNo.Visibility = Visibility.Visible;
                    dlg._btnNo.Content = "否";
                }

                if (dlg._btnCancel != null)
                {
                    dlg._btnCancel.Visibility = Visibility.Visible;
                    dlg._btnCancel.Content = "取消";
                }
            }
        }


        private static void ApplyStyle(MessageDialog dlg)
        {
            var style = Application.Current.TryFindResource(typeof(MessageDialog)) as Style;
            dlg.Style = style;
        }

        private static void ApplyImages(MessageBoxImage image, MessageDialog dlg)
        {
            if (dlg._pathIcon == null)
                return;
            if (image == MessageBoxImage.Question)
            {
                dlg._pathIcon.Data = Geometry.Parse(Question);
                var color = (Color)ColorConverter.ConvertFromString("#325BF1");
                var fill = new SolidColorBrush(color);
                fill.Freeze();
                dlg._pathIcon.Fill = fill;
            }
            else if (image == MessageBoxImage.Error || image == MessageBoxImage.Hand || image == MessageBoxImage.Stop)
            {
                dlg._pathIcon.Data = Geometry.Parse(Error);
                dlg._pathIcon.Data = Geometry.Parse(Error);
                var color = (Color)ColorConverter.ConvertFromString("#F52C2C");
                var fill = new SolidColorBrush(color);
                fill.Freeze();
                dlg._pathIcon.Fill = fill;
            }
            else if (image == MessageBoxImage.Warning || image == MessageBoxImage.Exclamation)
            {
                dlg._pathIcon.Data = Geometry.Parse(Warning);
                dlg._pathIcon.Data = Geometry.Parse(Warning);
                var color = (Color)ColorConverter.ConvertFromString("#FFD814");
                var fill = new SolidColorBrush(color);
                fill.Freeze();
                dlg._pathIcon.Fill = fill;
            }
            else if (image == MessageBoxImage.Information || image == MessageBoxImage.Asterisk)
            {
                dlg._pathIcon.Data = Geometry.Parse(Info);
                dlg._pathIcon.Data = Geometry.Parse(Info);
                var color = (Color)ColorConverter.ConvertFromString("#39A225");
                var fill = new SolidColorBrush(color);
                fill.Freeze();
                dlg._pathIcon.Fill = fill;
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
