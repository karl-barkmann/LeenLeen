using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Leen.Common;
using Leen.Common.Utils;

namespace Leen.Practices.ErrorReport
{
    /// <summary>
    /// 错误报告管理。
    /// </summary>
    public class ExceptionReportsManager
    {
        #region 成员变量

        private static ExceptionReportsManager instance = null;
        private readonly static object syncRoot = new object();

        private ExceptionReportsConfigure config;
        private string[] hostAddress;
        private IExceptionReportView view;
        private AbortableBackgroundWorker backgroundWork;

        #endregion

        #region 构造函数

        private ExceptionReportsManager()
        {
            backgroundWork = new AbortableBackgroundWorker();
            backgroundWork.DoWork += backgroundWork_DoWork;
            backgroundWork.RunWorkerCompleted += backgroundWork_RunWorkerCompleted;
            view = new ExceptionReportForm();
            view.Closed += view_Closed;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.domain_UnhandledException);
            Application.ThreadException += new ThreadExceptionEventHandler(this.application_ThreadException);
            try
            {
                IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                if (hostAddresses.Length > 0)
                {
                    this.hostAddress = new string[hostAddresses.Length];
                    for (int i = 0; i < hostAddresses.Length; i++)
                    {
                        this.hostAddress[i] = hostAddresses[i].ToString();
                    }
                }
                else
                {
                    this.hostAddress = new string[] { "无本机IP。" };
                }
            }
            catch
            {
                this.hostAddress = new string[] { "获取本机IP失败。" };
            }
            this.config = null;
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 获取错误报告管理的唯一实例。
        /// </summary>
        public static ExceptionReportsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new ExceptionReportsManager();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取应用程序配置中名为"exceptionReports"类型为<see cref="ExceptionReportsConfigure"/>的配置节点。
        /// </summary>
        public void LoadConfiguration()
        {
            this.config = (ExceptionReportsConfigure)ConfigurationHelper.Load("exceptionReports");
        }

        /// <summary>
        /// 配置错误报告。
        /// </summary>
        /// <param name="configuration">错误报告配置信息。</param>
        public void Config(ExceptionReportsConfigure configuration)
        {
            this.config = configuration;
        }

        /// <summary>
        /// 重载错误报告界面。
        /// </summary>
        /// <param name="view">错误报告视图接口。</param>
        public void OverrideReportView(IExceptionReportView view)
        {
            if (this.view != null)
            {
                view.Closed -= view_Closed;
            }
            this.view = view;
            view.Closed += view_Closed;
        }

        #endregion

        #region 私有方法

        private void backgroundWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            view.CloseReport();
        }

        private void backgroundWork_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(config.ReportDialogCloseSecond * 1000);
        }

        private void view_Closed(object sender, EventArgs e)
        {
            if (view.AutoRestart)
            {
                RestartApplication();
            }
        }

        private void application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            this.HandleException(e.Exception);
        }

        private void domain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.HandleException(e.ExceptionObject as Exception);
        }

        protected virtual bool FilterException(Exception ex)
        {
            return true;
        }

        private string GenerateReport(Exception ex)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("异常信息：{0}{1}", ex.Message, Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.AppendFormat("发生时间：{0}{1}", DateTime.Now, Environment.NewLine);
            builder.AppendFormat("主机名称：{0}{1}", Environment.MachineName, Environment.NewLine);
            builder.AppendFormat("主机IP：{0}{1}", string.Join(",", this.hostAddress), Environment.NewLine);
            builder.AppendFormat("操作系统：{0}{1}", Environment.OSVersion, Environment.NewLine);
            builder.AppendFormat("系统运行时长：{0}分{1}", Environment.TickCount / 0xea60, Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.AppendFormat("应用程序名称：{0}{1}", AppDomain.CurrentDomain.FriendlyName, Environment.NewLine);
            builder.AppendFormat("应用程序版本：{0}{1}", Application.ProductVersion, Environment.NewLine);
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                builder.AppendFormat("内存占用：物理{0} 虚拟：{1}{2}", DiskTools.GetFriendlySize(currentProcess.WorkingSet64), DiskTools.GetFriendlySize(currentProcess.VirtualMemorySize64), Environment.NewLine);
                builder.AppendFormat("应用程序线程数：{0}{1}", currentProcess.Threads.Count, Environment.NewLine);
                builder.AppendFormat("应用程序句柄数：{0}{1}", currentProcess.HandleCount, Environment.NewLine);
            }
            builder.AppendFormat("硬盘磁盘信息{0}", Environment.NewLine);
            try
            {
                string[] logicalDrives = Environment.GetLogicalDrives();
                string format = "{0,-10}\t总空间：{1}\t可用空间：{2}{3}";
                foreach (string str2 in logicalDrives)
                {
                    DriveInfo info = new DriveInfo(str2);
                    if ((info.DriveType == DriveType.Fixed) || (info.DriveType == DriveType.Network))
                    {
                        builder.AppendFormat(format, new object[] { str2, DiskTools.GetFriendlySize(info.TotalSize), DiskTools.GetFriendlySize(info.AvailableFreeSpace), Environment.NewLine });
                    }
                }
            }
            catch
            {
            }
            builder.AppendFormat("{0}{1}{0}", Environment.NewLine, ex);
            return builder.ToString();
        }

        private string GetPath(string orgPath)
        {
            string pattern = @"\${specialfolder:folder=(\w+)}";
            Match match = Regex.Match(orgPath, pattern);
            if (match.Success && System.Enum.IsDefined(typeof(Environment.SpecialFolder), match.Groups[1].Value))
            {
                Environment.SpecialFolder folder = (Environment.SpecialFolder)System.Enum.Parse(typeof(Environment.SpecialFolder), match.Groups[1].Value);
                string folderPath = Environment.GetFolderPath(folder);
                return Regex.Replace(orgPath, pattern, folderPath);
            }
            return orgPath;
        }

        private void HandleException(Exception ex)
        {
            if ((this.config != null) && this.FilterException(ex))
            {
                string text = this.GenerateReport(ex);
                this.WriteReportFile(text);
                this.ShowReportDialog(text);
            }
        }

        private void RestartApplication()
        {
            Application.Restart();
        }

        private void ShowReportDialog(string message)
        {
            if (this.config.ShowReportDialog)
            {
                view.ShowReport(message, config.ReportDialogCloseSecond, config.AutoRestart);
                if (!view.ModalDialog)
                {
                    backgroundWork.RunWorkerAsync();
                    backgroundWork.Abort();
                }
            }
        }

        private void WriteReportFile(string text)
        {
            string path;
            if (this.config.Folder == string.Empty)
            {
                path = Application.StartupPath + @"\Reports";
            }
            else
            {
                path = this.GetPath(this.config.Folder);
            }
            StreamWriter writer = null;
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string str2 = string.Format("{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));
                writer = System.IO.File.AppendText(Path.Combine(path, str2));
                writer.WriteLine();
                writer.WriteLine("-------------------------------------------------------");
                writer.Write(text);
                writer.WriteLine();
                writer.WriteLine("-------------------------------------------------------");
                writer.Flush();
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        #endregion
    }
}

