using Leen.Common;
using Leen.Practices.Mvvm;
using System;

namespace Leen.Practices.Tree
{
    /// <summary>
    /// 树形节点实体观察者。
    /// </summary>
    public abstract class TreeAwareViewModel : ViewModelBase, ITreeAwareViewModel
    {
        /// <summary>
        /// 当观察结束时调用。
        /// </summary>
        public void OnCompleted()
        {
        }

        /// <summary>
        /// 当发生异常时调用。
        /// </summary>
        /// <param name="error">描述错误的异常对象。</param>
        public void OnError(Exception error)
        {
        }

        /// <summary>
        /// 收到观察信息时调用。
        /// </summary>
        /// <param name="value">观察到的信息容器。</param>
        public void OnNext(ObservableContainer<INamedCascadeDataEntity> value)
        {
            switch (value.Action)
            {
                case ObservableAction.Create:
                    OnNodeCreate(value.Payload);
                    break;
                case ObservableAction.Update:
                    OnNodeUpdate(value.Payload);
                    break;
                case ObservableAction.Delete:
                    OnNodeDelete(value.Payload);
                    break;
            }
        }

        /// <summary>
        /// 当更新树节点实体时调用。
        /// </summary>
        /// <param name="payload">被更新的树节点实体。</param>
        protected abstract void OnNodeUpdate(INamedCascadeDataEntity payload);

        /// <summary>
        /// 当创建树节点实体时调用。
        /// </summary>
        /// <param name="payload">被创建的树节点实体。</param>
        protected abstract void OnNodeCreate(INamedCascadeDataEntity payload);

        /// <summary>
        /// 当删除树节点实体时调用。
        /// </summary>
        /// <param name="payload">被删除的树节点实体。</param>
        protected abstract void OnNodeDelete(INamedCascadeDataEntity payload);
    }
}
