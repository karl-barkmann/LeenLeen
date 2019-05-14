using Leen.Logging;
using Leen.Windows.Interaction;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using Leen.Common;
using CommonServiceLocator;
using System.Runtime.CompilerServices;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 视图模型基类定义了界面交互、日志记录接口，同时提供属性值变更记录。
    /// <remarks>
    /// 通常视图模型应当继承该类，任何资源释放应当在<see cref="CleanUp"/>方法中完成。
    /// </remarks>
    /// </summary>
    [Serializable]
    public class ViewModelBase : BindableBase
    {
        private bool editing;
        private bool anyPropertyChanged;
        private readonly static TaskCompletionSource<bool> s_TaskCompletionSource = new TaskCompletionSource<bool>();

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
        /// 获取或设置一个值指示视图模型中的属性是否已有改变。
        /// </summary>
        public bool AnyPropertyChanged
        {
            get { return anyPropertyChanged; }
            protected set
            {
                if (anyPropertyChanged == value)
                    return;

                anyPropertyChanged = value;

                RaisePropertyChangedWith("AnyPropertyChanged");
            }
        }

        /// <summary>
        /// 获取或设置一个值指示当前视图模型是否正在编辑中。
        /// </summary>
        public bool IsEditing
        {
            get { return editing; }
            set
            {
                if (editing == value)
                    return;

                editing = value;

                RaisePropertyChangedWith("IsEditing");
            }
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
            return s_TaskCompletionSource.Task;
        }

        /// <summary>
        /// 清除任何需要清除的资源，退订相关事件订阅。
        /// </summary>
        public virtual void CleanUp()
        { }

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
        protected override void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);
            AnyPropertyChanged = editing & true && propertyName != "IsEditing";
        }

        /// <summary>
        /// 通知属性值已更改。
        /// </summary>
        /// <param name="propertyName">默认为自动解析属性名称，亦可指定属性名称。</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected override void RaisePropertyChangedWith(string propertyName)
        {
            base.RaisePropertyChangedWith(propertyName);
            AnyPropertyChanged = editing & true && propertyName != "IsEditing";
        }
    }
}
