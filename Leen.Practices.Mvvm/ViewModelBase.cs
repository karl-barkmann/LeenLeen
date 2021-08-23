using CommonServiceLocator;
using Leen.Common;
using Leen.Common.Utils;
using Leen.Logging;
using Leen.Windows.Interaction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 视图模型基类定义了界面交互、日志记录接口，同时提供属性值变更记录。
    /// <remarks>
    /// 通常视图模型应当继承该类，任何资源释放应当在<see cref="CleanUp"/>方法中完成。
    /// </remarks>
    /// </summary>
    [Serializable]
    public class ViewModelBase : ValidationBase
    {
        private byte _internalStateFlags = 0x00;
        private const byte IsBusyMask = 0x01;
        private const byte IsEditingMask = 0x02;
        private const byte AnyPropertyChangedMask = 0x04;

        private byte _lifetimeStateFlags = 0x00;
        private const byte IsLoadedMask = 0x01;
        private const byte IsLoadingMask = 0x02;
        private const byte IsUnloadedMask = 0x04;
        private const byte IsUnloadingMask = 0x08;
        private const byte IsInitializingMask = 0x10;
        private const byte HasInitializedMask = 0x20;
        private const byte KeepAliveMask = 0x40;

        private List<IWatcher> _watchers;
        private string _busyMessage = "正在加载中...";
        private readonly static TaskCompletionSource<bool> s_TaskCompletionSource = new TaskCompletionSource<bool>();

        static ViewModelBase()
        {
            s_TaskCompletionSource.SetResult(true);
        }

        /// <summary>
        /// 构造<see cref="ViewModelBase"/>的实例。
        /// </summary>
        public ViewModelBase()
        {

        }

        /// <summary>
        /// 获取或设置UI交互接口。
        /// </summary>
        public virtual IUIInteractionService UIService { get; set; }

        /// <summary>
        /// 获取或设置日志记录接口。
        /// </summary>
        public virtual ILogger Logger { get; set; }

        /// <summary>
        /// 获取一个值，指示当前视图模型是否是在设计器中加载（Microsoft Visual Studio XAML UI Designer or Blend）。
        /// </summary>
        public static bool IsInDesignMode
        {
            get { return DesignerProperties.GetIsInDesignMode(new DependencyObject()); }
        }

        /// <summary>
        /// 获取一个值指示视图模型对应视图是否已加载。
        /// </summary>
        public bool IsLoaded 
        {
            get { return GetLifetimeStateFlag(IsLoadedMask); }
            internal set
            {
                SetLifetimeStateFlag(value, IsLoadedMask);
            }
        }

        /// <summary>
        /// 获取一个值指示视图模型对应视图是否正在加载中。
        /// </summary>
        public bool IsLoading 
        {
            get { return GetLifetimeStateFlag(IsLoadingMask); }
            internal set
            {
                SetLifetimeStateFlag(value, IsLoadingMask);
            }
        }

        /// <summary>
        /// 获取一个值指示视图模型对应视图是否已卸载。
        /// </summary>
        public bool IsUnloaded
        {
            get { return GetLifetimeStateFlag(IsUnloadedMask); }
            internal set
            {
                SetLifetimeStateFlag(value, IsUnloadedMask);
            }
        }

        /// <summary>
        /// 获取一个值指示视图模型对应视图是否正在卸载中。
        /// </summary>
        public bool IsUnloading
        {
            get { return GetLifetimeStateFlag(IsUnloadingMask); }
            internal set
            {
                SetLifetimeStateFlag(value, IsUnloadingMask);
            }
        }

        /// <summary>
        /// 获取一个值指示该模型是否已成功初始化。
        /// </summary>
        public bool HasInitialized
        {
            get { return GetLifetimeStateFlag(HasInitializedMask); }
            internal set
            {
                SetLifetimeStateFlag(value, HasInitializedMask);
            }
        }

        /// <summary>
        /// 获取一个值指示该模型是否正在初始化中。
        /// </summary>
        public bool IsInitializing
        {
            get { return GetLifetimeStateFlag(IsInitializingMask); }
            internal set
            {
                SetLifetimeStateFlag(value, IsInitializingMask);
            }
        }

        /// <summary>
        /// 获取一个值指示该模型是否需要在视图卸载时继续保持存活，否则执行资源清除。
        /// </summary>
        public bool KeepAlive
        {
            get { return GetLifetimeStateFlag(KeepAliveMask); }
            protected set
            {
                SetLifetimeStateFlag(value, KeepAliveMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值指示当前视图模型是否正在编辑中。
        /// </summary>
        public bool IsEditing
        {
            get { return GetInternalStateFlag(IsEditingMask); }
            protected internal set
            {
                SetInternalStateFlag(value, IsEditingMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值指示视图模型自开始编辑以来是否有属性已发生改变。
        /// </summary>
        public bool AnyPropertyChanged
        {
            get { return GetInternalStateFlag(AnyPropertyChangedMask); }
            protected internal set
            {
                SetInternalStateFlag(value, AnyPropertyChangedMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值指示当前视图是否正在忙碌状态。
        /// </summary>
        public bool IsBusy
        {
            get { return GetInternalStateFlag(IsBusyMask); }
            protected internal set
            {
                SetInternalStateFlag(value, IsBusyMask);
            }
        }

        /// <summary>
        /// 获取或设置忙碌状态提示。
        /// </summary>
        public string BusyMessage
        {
            get { return _busyMessage; }
            protected internal set
            {
                SetProperty(ref _busyMessage, value, () => BusyMessage);
            }
        }

        /// <summary>
        /// 获取指定类型的已注册接口。
        /// </summary>
        /// <typeparam name="T">接口类型。</typeparam>
        /// <returns></returns>
        public static T GetInstance<T>()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }

        /// <summary>
        /// 获取指定类型的已注册接口。
        /// </summary>
        /// <typeparam name="T">接口类型。</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetInstances<T>()
        {
            return ServiceLocator.Current.GetAllInstances<T>();
        }

        /// <summary>
        /// 获取指定类型及名称的已注册接口。
        /// </summary>
        /// <typeparam name="T">接口类型。</typeparam>
        /// <returns></returns>
        public static T GetInstance<T>(string serviceName)
        {
            return ServiceLocator.Current.GetInstance<T>(serviceName);
        }

        /// <summary>
        /// 异步初始化后台数据，通常是一些耗时操作。
        /// <para>
        /// 当UI交互接口在处理交互请求时，会自动处理这些初始化。
        /// </para>
        /// <para>
        /// 简单的初始化建议直接在构造函数进行。
        /// </para>
        /// </summary>
        /// <returns></returns>
        public Task InitializeAsync()
        {
            WatchMethods();
            WatchProperties();
            return OnInitializeAsync();
        }

        /// <summary>
        /// 清除任何需要清除的资源，退订相关事件订阅。
        /// </summary>
        public virtual void CleanUp()
        {
            if (_watchers != null)
            {
                _watchers.ForEach(x => x.Dispose());
                _watchers.Clear();
            }
        }

        /// <summary>
        /// 但视图模型关联的视图加载是调用。
        /// </summary>
        protected internal virtual Task OnLoadAsync()
        {
            return s_TaskCompletionSource.Task;
        }

        /// <summary>
        /// 但视图模型关联的视图卸载是调用。
        /// </summary>
        protected internal virtual Task OnUnloadAsync()
        {
            return s_TaskCompletionSource.Task;
        }

        /// <summary>
        /// 开始记录属性变更。
        /// </summary>
        protected virtual void BeginEdit()
        {
            IsEditing = true;
        }

        /// <summary>
        /// 忽略属性变更,并重置<see cref="AnyPropertyChanged"/> 属性变更标识。
        /// </summary>
        protected virtual void EndEdit()
        {
            IsEditing = false;
            AnyPropertyChanged = false;
        }

        /// <summary>
        /// 通知属性值已更改。
        /// </summary>
        /// <typeparam name="T">描述属性的类型。</typeparam>
        /// <param name="expression">用户获取属性名称的表达式。</param>
        protected override void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            base.RaisePropertyChanged<T>(expression);
            AnyPropertyChanged = IsEditing & true;
        }

        /// <summary>
        /// 通知属性值已更改。
        /// </summary>
        /// <param name="propertyName">指定属性名称。</param>
        protected override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);
            AnyPropertyChanged = IsEditing & true && propertyName != nameof(IsEditing);
        }

        /// <summary>
        /// 当初始化时调用。
        /// </summary>
        /// <returns></returns>
        protected virtual Task OnInitializeAsync()
        {
            return s_TaskCompletionSource.Task;
        }

        private void WatchProperties()
        {
            _watchers = new List<IWatcher>();
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(prop => Attribute.IsDefined(prop, typeof(WatchOnAttribute)));
            foreach (var property in properties)
            {
                var propVal = ReflectionHelper.GetPropValue(this, property.Name);
                if (propVal == null)
                {
                    continue;
                }

                if (typeof(INotifyCanExecuteChangedCommand).IsAssignableFrom(propVal.GetType()))
                {
                    var raiseCommandCanExecute = new Action(() =>
                    {
                        if (propVal is INotifyCanExecuteChangedCommand command)
                        {
                            UIService.InvokeIfNeeded(() =>
                            {
                                command.RaiseCanExecuteChanged();
                            });
                        }
                    });

                    var watchOnList = property.GetCustomAttributes<WatchOnAttribute>();
                    if (watchOnList != null)
                    {
                        foreach (var watchOn in watchOnList)
                        {
                            IWatcher watcher;
                            if (watchOn.PropertyType == null)
                            {
                                watcher = Watch(watchOn.PropertyType, watchOn.TargetProperty, raiseCommandCanExecute);
                            }
                            else
                            {
                                watcher = DeepWatch(watchOn.PropertyType, watchOn.TargetProperty, raiseCommandCanExecute);
                            }
                            _watchers.Add(watcher);
                        }
                    }
                }
            }
        }

        private void WatchMethods()
        {
            _watchers = new List<IWatcher>();
            var methods = GetType().GetMethods(BindingFlags.Public
                                               | BindingFlags.NonPublic
                                               | BindingFlags.Instance).Where(method => Attribute.IsDefined(method, typeof(WatchOnAttribute)));
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var watchOnList = method.GetCustomAttributes<WatchOnAttribute>();
                if (watchOnList != null)
                {
                    foreach (var watchOn in watchOnList)
                    {
                        var invokeMethod = new Action<object, object>((oldVal, newVal) =>
                        {
                            if (parameters == null || parameters.Length < 1)
                            {
                                method.Invoke(this, null);
                            }
                            else if (parameters.Length == 1 && (watchOn.PropertyType == null || parameters[0].ParameterType == watchOn.PropertyType))
                            {
                                method.Invoke(this, new object[1] { newVal });
                            }
                            else if (parameters.Length == 2 && (watchOn.PropertyType == null || parameters.All(x => x.ParameterType == watchOn.PropertyType)))
                            {
                                method.Invoke(this, new object[2] { oldVal, newVal });
                            }
                        });
                        IWatcher watcher;
                        if (watchOn.PropertyType == null)
                        {
                            watcher = Watch(watchOn.PropertyType, watchOn.TargetProperty, invokeMethod);
                        }
                        else
                        {
                            watcher = DeepWatch(watchOn.PropertyType, watchOn.TargetProperty, invokeMethod);
                        }
                        _watchers.Add(watcher);
                    }
                }
            }
        }

        private bool GetInternalStateFlag(byte mask)
        {
            return (_internalStateFlags & mask) == mask;
        }

        private bool SetInternalStateFlag(bool? value, byte mask, [CallerMemberName] string propertyName = null)
        {
            if (GetInternalStateFlag(mask) == value)
            {
                return false;
            }

            if (value.HasValue && value.Value)
            {
                _internalStateFlags |= mask;
            }
            else
            {
                _internalStateFlags ^= mask;
            }

            if (propertyName != null)
                RaisePropertyChanged(propertyName);

            return true;
        }

        private bool GetLifetimeStateFlag(byte mask)
        {
            return (_lifetimeStateFlags & mask) == mask;
        }

        private bool SetLifetimeStateFlag(bool? value, byte mask, [CallerMemberName] string propertyName = null)
        {
            if (GetLifetimeStateFlag(mask) == value)
            {
                return false;
            }

            if (value.HasValue && value.Value)
            {
                _lifetimeStateFlags |= mask;
            }
            else
            {
                _lifetimeStateFlags ^= mask;
            }

            if (propertyName != null)
                RaisePropertyChanged(propertyName);

            return true;
        }
    }
}
