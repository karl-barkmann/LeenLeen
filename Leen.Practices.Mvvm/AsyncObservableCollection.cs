using System.Collections.Specialized;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// 表示一个动态数据集合，在添加项、移除项或刷新整个列表时，此集合将提供通知。
    /// <typeparam name="T">集合中的数据类型</typeparam>
    /// <para>
    /// 对比常规的 ObservableCollection 集合，此集合允许在非集合所有者线程上操作它，同时应用了Framework4.5版本中的 EnableCollectionSynchronization 接口。
    /// 更多细节 http://stackoverflow.com/questions/14336750/upgrading-to-net-4-5-an-itemscontrol-is-inconsistent-with-its-items-source
    /// </para>
    /// </summary>
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private static object _syncLock = new object();

        /// <summary>
        /// 表示一个动态数据集合，在添加项、移除项或刷新整个列表时，此集合将提供通知。
        /// 此集合允许在非集合所有者线程上操作（添加、移除或刷新）它。
        /// </summary>
        public AsyncObservableCollection()
            : base()
        {
            enableCollectionSynchronization(this, _syncLock);
        }

        /// <summary>
        /// 当集合发生变化时调用。
        /// </summary>
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// 当集合发生变化时
        /// </summary>
        /// <param name="e">集合变化事件参数。</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            using (BlockReentrancy())
            {
                var eh = CollectionChanged;
                if (eh == null) return;

                var dispatcher = (from NotifyCollectionChangedEventHandler nh in eh.GetInvocationList()
                                  let dpo = nh.Target as DispatcherObject
                                  where dpo != null
                                  select dpo.Dispatcher).FirstOrDefault();

                if (dispatcher != null && dispatcher.CheckAccess() == false)
                {
                    dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => OnCollectionChanged(e)));
                }
                else
                {
                    foreach (NotifyCollectionChangedEventHandler nh in eh.GetInvocationList())
                        nh.Invoke(this, e);
                }
            }
        }

        private static void enableCollectionSynchronization(IEnumerable collection, object lockObject)
        {
            var method = typeof(BindingOperations).GetMethod("EnableCollectionSynchronization",
                                    new Type[] { typeof(IEnumerable), typeof(object) });
            if (method != null)
            {
                // It's .NET 4.5
                method.Invoke(null, new object[] { collection, lockObject });
            }
        }
    }
}
