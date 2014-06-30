using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Smart.Common;

namespace Smart.Logging
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
        /// 若包括应用程序用户数据目录名名称(例: VipClient/xxx.log)，则文件将存放于%programdata%，否则存放于应用程序根目录。
        /// </remarks>
        /// </param>
        /// <param name="isAsync">是否启用线程池异步写入。</param>
        /// <exception cref="System.ArgumentException"/>
        public FileLogger(string logFile, bool isAsync)
        {
            if (String.IsNullOrEmpty(logFile))
            {
                throw new ArgumentException("logFile不能为空");
            }
            LogFile = logFile;
            Async = isAsync;
        }

        /// <summary>
        /// 获取或设置日志文件名(包括应用程序用户数据目录名)。
        /// <example>
        /// 例: VipClient/xxx.log
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
        /// <param name="message">日志信息。</param>
        /// <param name="args">格式化参数。</param>
        public void WriteMessage(string message, params object[] args)
        {
            Write("{0}[{1}]:{2}", Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId,
                String.Format(message, args));
        }

        private void Write(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(LogFile))
            {
                try
                {
                    string str = message;
                    if (args != null)
                    {
                        str = string.Format(CultureInfo.InvariantCulture, message, args);
                    }
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
                catch (Exception exception)
                {
                    if (exception.MustBeThrown())
                    {
                        throw;
                    }
                }
            }
        }

        private string GetAssemblyLoadingBase()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            if (!assembly.GlobalAssemblyCache)
            {
                string codeBase = assembly.GetName().CodeBase;
                Uri assemblyUri = new Uri(codeBase, UriKind.RelativeOrAbsolute);
                if (assemblyUri.Scheme == "file")
                {
                    return Path.GetDirectoryName(assemblyUri.LocalPath);
                }
            }
            return "";
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
                if (invalidFileNameChars.Contains(item))
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
