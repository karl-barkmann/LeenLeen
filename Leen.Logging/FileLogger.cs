using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Leen.Logging
{
    /// <summary>
    /// log文件日志记录接口。
    /// </summary>
    public class FileLogger : ILogger
    {
        private readonly object lockObject = new object();
        private string logFile = "";

        /// <summary>
        /// 构造log文件日志记录接口的实例。
        /// </summary>
        /// <param name="logFile">日志文件名。
        /// <remarks>
        /// 若包括应用程序用户数据目录名名称(例: xxx/xxx.log)，则文件将存放于%programdata%，否则存放于应用程序根目录。
        /// </remarks>
        /// </param>
        /// <param name="isAsync">是否启用线程池异步写入。</param>
        /// <exception cref="System.ArgumentException"/>
        public FileLogger(string logFile, bool isAsync)
        {
            if (String.IsNullOrEmpty(logFile))
            {
                throw new ArgumentException($"{nameof(logFile)}不能为空", nameof(logFile));
            }
            LogFile = logFile;
            Async = isAsync;
        }

        /// <summary>
        /// 获取或设置日志文件名(包括应用程序用户数据目录名)。
        /// <example>
        /// 例: xxx/xxx.log
        /// </example>
        /// </summary>
        public string LogFile
        {
            get { return logFile; }
            set
            {
                if (logFile != value)
                {
                    logFile = CheckPath(value);
                    if (!Path.IsPathRooted(logFile))
                    {
                        if (!String.IsNullOrEmpty(Path.GetDirectoryName(value)))
                        {
                            logFile = Path.Combine(Environment.GetFolderPath(
                                Environment.SpecialFolder.CommonApplicationData), value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置一个指示是否启用线程池异步写入。
        /// </summary>
        public bool Async
        {
            get;
            set;
        }

        /// <summary>
        /// 记录日志信息。
        /// </summary>
        /// <param name="message">日志消息内容。</param>
        /// <param name="category">描述日志内容的级别。</param>
        /// <param name="priority">描述日志内容的优先级。</param>
        public void Log(string message, LogLevel level, LogPriority priority)
        {
            Write($"{Thread.CurrentThread.Name}[{Thread.CurrentThread.ManagedThreadId}]:{message}");
        }

        private void Write(string message)
        {
            if (!string.IsNullOrEmpty(LogFile))
            {
                string str = message;
                StringBuilder builder = new StringBuilder(message.Length + 0x20);

                builder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff", CultureInfo.InvariantCulture));
                builder.Append(" ");
                builder.Append(str);
                string content = builder.ToString();
                string logFile = Path.Combine(Path.GetDirectoryName(LogFile),
                    String.Format("{0}{1}", DateTime.Now.ToString("yyyy-MM-dd"), Path.GetFileName(LogFile)));
                if (!string.IsNullOrEmpty(logFile))
                {
                    Action work = new Action(() =>
                    {
                        lock (lockObject)
                        {
                            int tryTimes = 0;
                            do
                            {
                                try
                                {
                                    if (!String.IsNullOrEmpty(Path.GetDirectoryName(LogFile)) &&
                                        !Directory.Exists(Path.GetDirectoryName(LogFile)))
                                    {
                                        Directory.CreateDirectory(Path.GetDirectoryName(LogFile));
                                    }

                                    using (StreamWriter writer = File.AppendText(logFile))
                                    {
                                        writer.WriteLine(content);
                                        break;
                                    }
                                }
                                catch (UnauthorizedAccessException)
                                {

                                }
                                catch (IOException)
                                {
                                    continue;
                                }
                            } while (tryTimes++ < 5);
                        }
                    });
                    if (Async)
                    {
                        ThreadPool.QueueUserWorkItem((state) => { work(); }, null);
                    }
                    else
                    {
                        work();
                    }
                }
            }
        }

        private string CheckPath(string logFile)
        {
            if (String.IsNullOrEmpty(logFile))
            {
                throw new ArgumentException("logFile不能为空");
            }

            var invalidFileNameChars = Path.GetInvalidFileNameChars();
            var invalidPathChars = Path.GetInvalidPathChars();

            var fileNameChars = Path.GetFileName(logFile).ToCharArray();
            foreach (var item in fileNameChars)
            {
                if (invalidFileNameChars.Contains(item))
                {
                    throw new ArgumentException("logFile包含无效字符");
                }
            }

            var ptahChars = Path.GetDirectoryName(logFile).ToCharArray();
            foreach (var item in ptahChars)
            {
                if (item == '\\')
                    continue;
                if (invalidPathChars.Contains(item))
                {
                    throw new ArgumentException("logFile包含无效字符");
                }
            }

            if (!Path.HasExtension(logFile))
            {
                logFile += ".log";
            }

            return logFile;
        }
    }
}
