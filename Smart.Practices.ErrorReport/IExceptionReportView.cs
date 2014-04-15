using System;

namespace Smart.Practices.ErrorReport
{
    /// <summary>
    /// 错误报告视图接口。
    /// </summary>
    public interface IExceptionReportView
    {
        /// <summary>
        /// 当错误报告界面关闭时发生。
        /// </summary>
        event EventHandler<EventArgs> Closed;

        /// <summary>
        /// 获取一个值，指示是否自动重启应用程序。
        /// </summary>
        bool AutoRestart { get; }

        /// <summary>
        /// 获取一个值，指示错误报告界面是否以"模式"窗口形式显示。
        /// </summary>
        bool ModalDialog { get; }


        /// <summary>
        /// 显示错误报告。
        /// </summary>
        /// <param name="errorMessage">错误报告内容。</param>
        /// <param name="closeSeconds">错误报告界面自动关闭倒计时长。</param>
        /// <param name="autoRestart">一个值，指示自动重启应用程序。</param>
        void ShowReport(string errorMessage, int closeSeconds, bool autoRestart);

        /// <summary>
        /// 关闭错误报告界面。
        /// </summary>
        void CloseReport();
    }
}
