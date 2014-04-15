using System;
using System.Diagnostics;

namespace Smart.Logging
{
    /// <summary>
    /// TraceSource��־��¼�ӿڡ�
    /// </summary>
    public class TraceSourceLogger : ILogger, IDisposable
    {
        private TraceSource traceSource;

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
            if (traceSource == null)
                throw new ArgumentNullException("traceSource");

            this.traceSource = traceSource;
        }

        /// <summary>
        /// ��¼��־��
        /// </summary>
        /// <param name="message">��־��Ϣ��</param>
        /// <param name="args">��ʽ��������</param>
        public void WriteMessage(string message, params object[] args)
        {
            this.traceSource.TraceInformation(message, args);
        }

        /// <summary>
        /// �ͷ���Դ��
        /// </summary>
        public void Dispose()
        {
            if (this.traceSource != null)
            {
                this.traceSource.Flush();
                this.traceSource.Close();
                this.traceSource = null;
            }
        }
    }
}
