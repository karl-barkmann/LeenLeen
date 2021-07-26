namespace Leen.Common
{
    /// <summary>
    /// 定义可观察信息容器，提供具体可观察动作的区分。
    /// </summary>
    /// <typeparam name="T">可观察信息的类型。</typeparam>
    public class ObservableContainer<T>
    {
        /// <summary>
        /// 构造可观察信息容器的实例。
        /// </summary>
        /// <param name="action">需通知的观察到的动作。</param>
        /// <param name="playload">观察到的目标。</param>
        public ObservableContainer(ObservableAction action, T playload)
        {
            Action = action;
            Payload = playload;
        }

        /// <summary>
        /// 获取观察到应用于观察目标的动作。
        /// </summary>
        public ObservableAction Action { get; }

        /// <summary>
        /// 获取观察到的目标。
        /// </summary>
        public T Payload { get; }
    }
}
