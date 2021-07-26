using Leen.Common;
using System;

namespace Leen.Practices.Tree
{
    /// <summary>
    /// 定义树形节点实体观察者。
    /// </summary>
    public interface ITreeAwareViewModel : IObserver<ObservableContainer<INamedCascadeDataEntity>>
    {
    }
}
