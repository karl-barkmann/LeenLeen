using CommonServiceLocator;
using Leen.Common;
using Leen.Logging;
using Leen.Windows.Interaction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
        private bool editing;
        private bool _isBusy;
        private string _busyMessage = "正在加载中...";
        private bool anyPropertyChanged;
        private bool _hasInitialized;
        private readonly static TaskCompletionSource<bool> s_TaskCompletionSource = new TaskCompletionSource<bool>();
        private Dictionary<string, object> _watchOnProperties;

        static ViewModelBase()
        {
            s_TaskCompletionSource.SetResult(true);
        }

        /// <summary>
        /// 构造ViewModelBase的实例。
        /// </summary>
        public ViewModelBase()
        {
            if (ServiceLocator.IsLocationProviderSet && ServiceLocator.Current.IsRegisterd<IUIInteractionService>())
            {
                UIService = ServiceLocator.Current.GetInstance<IUIInteractionService>();
            }
            else
            {
                UIService = new UIInteractionService();
            }
            if (ServiceLocator.IsLocationProviderSet && ServiceLocator.Current.IsRegisterd<ILogger>())
            {
                Logger = ServiceLocator.Current.GetInstance<ILogger>();
            }
            else
            {
                Logger = new DebugLogger();
            }
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
        /// 获取或设置一个值指示视图模型自开始编辑以来是否有属性已发生改变。
        /// </summary>
        public bool AnyPropertyChanged
        {
            get { return anyPropertyChanged; }
            protected set
            {
                if (anyPropertyChanged == value)
                    return;

                anyPropertyChanged = value;

                RaisePropertyChanged(nameof(AnyPropertyChanged));
            }
        }

        /// <summary>
        /// 获取一个值指示视图模型对应视图是否已加载。
        /// </summary>
        public bool IsLoaded { get; internal set; }

        /// <summary>
        /// 获取一个值指示视图模型对应视图是否已卸载。
        /// </summary>
        public bool IsUnloaded { get; internal set; }

        /// <summary>
        /// 获取或设置一个值指示当前视图模型是否正在编辑中。
        /// </summary>
        public bool IsEditing
        {
            get { return editing; }
            protected set
            {
                if (editing == value)
                    return;

                editing = value;

                RaisePropertyChanged(nameof(IsEditing));
            }
        }

        /// <summary>
        /// 获取或设置一个值指示当前视图是否正在忙碌状态。
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            protected internal set
            {
                SetProperty(ref _isBusy, value, () => IsBusy);
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
        /// 获取一个值指示该模型是否已成功初始化。
        /// </summary>
        public bool HasInitialized
        {
            get { return _hasInitialized; }
            internal set
            {
                SetProperty(ref _hasInitialized, value, () => HasInitialized);
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
        public virtual Task InitializeAsync()
        {
            WatchMethods();
            WatchProperties();
            return s_TaskCompletionSource.Task;
        }

        private void WatchProperties()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(prop => Attribute.IsDefined(prop, typeof(WatchOnAttribute)));
            foreach (var property in properties)
            {
                var propVal = property.GetValue(this, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty, null, null, null);
                if (propVal == null)
                {
                    continue;
                }
                if (typeof(RelayCommand).IsAssignableFrom(propVal.GetType()))
                {
                    var watchOn = property.GetCustomAttribute<WatchOnAttribute>();
                    if (watchOn != null)
                    {
                        if (_watchOnProperties == null)
                            _watchOnProperties = new Dictionary<string, object>();
                        _watchOnProperties.Add(property.Name, propVal);
                        Watch(property.PropertyType, watchOn.TargetProperty, () =>
                        {
                            if (propVal is RelayCommand command)
                            {
                                command.RaiseCanExecuteChanged();
                            }
                        }, false);
                    }
                }
            }
        }

        private void WatchMethods()
        {
            var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(method => Attribute.IsDefined(method, typeof(WatchOnAttribute)));
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var watchOn = method.GetCustomAttribute<WatchOnAttribute>();
                if ((parameters == null || parameters.Length < 1) && watchOn != null)
                {
                    Watch(watchOn.PropertyType, watchOn.TargetProperty, () =>
                    {
                        method.Invoke(this, null);
                    }, false);
                }
            }
        }

        /// <summary>
        /// 清除任何需要清除的资源，退订相关事件订阅。
        /// </summary>
        public virtual void CleanUp()
        { }

        /// <summary>
        /// 但初始化发生错误时调用。
        /// </summary>
        /// <param name="error">初始化时发生的异常。</param>
        protected virtual void OnInitializeError(Exception error)
        {

        }

        /// <summary>
        /// 但视图模型关联的视图加载是调用。
        /// </summary>
        protected internal virtual void OnLoad()
        {

        }

        /// <summary>
        /// 但视图模型关联的视图卸载是调用。
        /// </summary>
        protected internal virtual void OnUnload()
        {

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
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"),
        SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures"),
        SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected override void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            base.RaisePropertyChanged<T>(expression);
            AnyPropertyChanged = editing & true;
        }

        /// <summary>
        /// 通知属性值已更改。
        /// </summary>
        /// <param name="propertyName">指定属性名称。</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"),
        SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);
            AnyPropertyChanged = editing & true && propertyName != nameof(IsEditing);
        }

        internal void NotifyInitializeError(Exception ex)
        {
            OnInitializeError(ex);
        }
    }
}
