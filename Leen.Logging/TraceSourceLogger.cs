using System;
using System.Diagnostics;

namespace Leen.Logging
{
    /// <summary>
    /// TraceSource��־��¼�ӿڡ�
    /// </summary>
    public class TraceSourceLogger : ILogger, IDisposable
    {
        private TraceSource _traceSource;

        /// <summary>
        /// ����TraceSource��־��¼�ӿڵ�ʵ����
        /// </summary>
        /// <param name="traceSourceName">TraceSource��������</param>
        public TraceSourceLogger(string traceSourceName)
            : this(new TraceSource(traceSourceName, SourceLevels.All))
        {

        }

        /// <summary>
        /// ����TraceSource��־��¼�ӿڵ�ʵ����
        /// </summary>
        /// <param name="traceSource"></param>
        public TraceSourceLogger(TraceSource traceSource)
        {
            _traceSource = traceSource ?? throw new ArgumentNullException("traceSource");
        }

        /// <summary>
        /// ��¼��־��Ϣ��
        /// </summary>
        /// <param name="message">��־��Ϣ���ݡ�</param>
        /// <param name="category">������־���ݵļ���</param>
        /// <param name="priority">������־���ݵ����ȼ���</param>
        public void Log(string message, LogLevel level, LogPriority priority)
        {
            _traceSource.TraceInformation(message);
        }

        /// <summary>
        /// �ͷ���Դ��
        /// </summary>
        public void Dispose()
        {
            if (_traceSource != null)
            {
                _traceSource.Flush();
                _traceSource.Close();
                _traceSource = null;
            }
        }
    }
}
