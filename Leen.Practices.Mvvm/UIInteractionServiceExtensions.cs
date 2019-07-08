using Leen.Windows.Interaction;
using System.Windows;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// Mvvm交互接口扩展。
    /// </summary>
    public static class UIInteractionServiceExtensions
    {
        /// <summary>
        /// 显示提示消息。
        /// </summary>
        /// <param name="interactionService">交互接口。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="title">提示标题。</param>
        /// <param name="ownnerViewModel">显示提示框的父视图。</param>
        /// <param name="buttons">消息对话框显示的按钮。</param>
        /// <param name="icon">消息对话框显示的图标。</param>
        /// <returns></returns>
        public static MessageBoxResult ShowInfoMessage(this IUIInteractionService interactionService,
                                                   string message,
                                                   string title,
                                                   object ownnerViewModel,
                                                   MessageBoxButton buttons = MessageBoxButton.OK,
                                                   MessageBoxImage icon = MessageBoxImage.Information)
        {
            return interactionService.ShowMessage(message, title, buttons, icon, ownnerViewModel);
        }

        /// <summary>
        /// 显示错误消息。
        /// </summary>
        /// <param name="interactionService">交互接口。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="title">提示标题。</param>
        /// <param name="ownnerViewModel">显示提示框的父视图。</param>
        /// <param name="buttons">消息对话框显示的按钮。</param>
        /// <param name="icon">消息对话框显示的图标。</param>
        /// <returns></returns>
        public static MessageBoxResult ShowErrorMessage(this IUIInteractionService interactionService,
                                                        string message,
                                                        string title,
                                                        object ownnerViewModel,
                                                        MessageBoxButton buttons = MessageBoxButton.OK,
                                                        MessageBoxImage icon = MessageBoxImage.Error)
        {
            return interactionService.ShowMessage(message, title, buttons, icon, ownnerViewModel);
        }

        /// <summary>
        /// 显示警告消息。
        /// </summary>
        /// <param name="interactionService">交互接口。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="title">提示标题。</param>
        /// <param name="ownnerViewModel">显示提示框的父视图。</param>
        /// <param name="buttons">消息对话框显示的按钮。</param>
        /// <param name="icon">消息对话框显示的图标。</param>
        /// <returns></returns>
        public static MessageBoxResult ShowWarningMessage(this IUIInteractionService interactionService,
                                                       string message,
                                                       string title,
                                                       object ownnerViewModel,
                                                       MessageBoxButton buttons = MessageBoxButton.OK,
                                                       MessageBoxImage icon = MessageBoxImage.Warning)
        {
            return interactionService.ShowMessage(message, title, buttons, icon, ownnerViewModel);
        }

        /// <summary>
        /// 显示确认消息。
        /// </summary>
        /// <param name="interactionService">交互接口。</param>
        /// <param name="message">消息内容。</param>
        /// <param name="title">提示标题。</param>
        /// <param name="ownnerViewModel">显示提示框的父视图。</param>
        /// <param name="buttons">消息对话框显示的按钮。</param>
        /// <param name="icon">消息对话框显示的图标。</param>
        /// <returns></returns>
        public static MessageBoxResult ShowQuestionMessage(this IUIInteractionService interactionService,
                                                       string message,
                                                       string title,
                                                       object ownnerViewModel,
                                                       MessageBoxButton buttons = MessageBoxButton.OKCancel,
                                                       MessageBoxImage icon = MessageBoxImage.Question)
        {
            return interactionService.ShowMessage(message, title, buttons, icon, ownnerViewModel);
        }
    }
}
