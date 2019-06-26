using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// ViewLocationProvider定义了如何根据视图模型的类型来获取视图类型，并使用构造方法构造视图的实例，
    /// 并维护视图实例的生命周期，确保视图在关闭释放相关资源，同时提供使用视图模型查找其对应视图所在窗体。
    /// ViewLocationProvider维护通过ViewLocator设置依赖属性IsRegistered为True的视图。
    /// ViewLocationProvider通过以下策略来使用视图模型定位并创建视图：首先查找视图类型映射的视图类型是否存在；
    /// 如果不存在则通过基于视图模型类型名称的解析规则来获得视图类型。当获取到视图类型后使用视图构造工厂来构造视图。
    /// ViewLocationProvider也提供默认对视图模型类型到视图类型的解析重载，或添加直接的类型映射；同时提供默认视图构造工厂的重载。
    /// </summary>
    public static class ViewLocationProvider
    {
        private static readonly Dictionary<Type, Type> viewViewModelMappings = new Dictionary<Type, Type>();
        private static readonly Dictionary<object, Type> viewAliasMapping = new Dictionary<object, Type>();
        private static Func<Type, IView> defaultViewFactory = type => (IView)Activator.CreateInstance(type);
        private static readonly HashSet<IView> views = new HashSet<IView>();

        private static Func<Type, Type> defaultViewModelTypeToViewTypeResolver =
            viewModelType =>
            {
                var viewModelName = viewModelType.FullName;
                viewModelName = viewModelName.Replace(".ViewModels.", ".Views.");
                viewModelName = viewModelName.Replace(".ViewModel.", ".View.");
                if (viewModelName.EndsWith("ViewModel"))
                {
                    int index = viewModelName.LastIndexOf("ViewModel");
                    viewModelName = viewModelName.Remove(index);
                }

                if (viewModelName.EndsWith("Window"))
                {
                    var viewModelAssemblyName = viewModelType.Assembly.FullName;
                    var viewName = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewModelName,
                        viewModelAssemblyName);
                    return Type.GetType(viewName);
                }
                else
                {
                    var viewModelAssemblyName = viewModelType.Assembly.FullName;
                    var viewName = String.Format(CultureInfo.InvariantCulture, "{0}View, {1}", viewModelName,
                        viewModelAssemblyName);
                    return Type.GetType(viewName);
                }
            };

        /// <summary>
        /// 设置默认视图构造方法。
        /// </summary>
        /// <param name="viewFactory">视图构造方法。</param>
        public static void SetDefaultViewFactory(Func<Type, IView> viewFactory)
        {
            defaultViewFactory = viewFactory;
        }

        /// <summary>
        /// 设置默认视图模型类型到视图类型的解析方法。
        /// </summary>
        /// <param name="viewModelTypeToViewTypeResolver">视图模型类型到视图类型的解析方法。</param>
        public static void SetDefaultViewModelTypeToViewTypeResolver(Func<Type, Type> viewModelTypeToViewTypeResolver)
        {
            defaultViewModelTypeToViewTypeResolver = viewModelTypeToViewTypeResolver;
        }

        /// <summary>
        /// 映射视图模型类型到视图类型。
        /// </summary>
        /// <param name="viewModelType">视图模型的类型。</param>
        /// <param name="viewType">对应的视图类型。</param>
        public static void Map(Type viewModelType, Type viewType)
        {
            if (viewViewModelMappings.ContainsKey(viewModelType))
            {
                throw new InvalidOperationException(string.Format("{0} has been mapped to {1}", viewType, viewModelType));
            }

            viewViewModelMappings.Add(viewModelType, viewType);
        }

        /// <summary>
        /// 注册视图别名。
        /// <para>允许我们通过视图别名来查找视图并建立视图。</para>
        /// </summary>
        /// <param name="viewAlias">视图别名。</param>
        /// <param name="viewType">对应的视图类型。</param>
        public static void RegisterViewAlias(string viewAlias, Type viewType)
        {
            if (viewAliasMapping.ContainsKey(viewAlias))
            {
                throw new InvalidOperationException(string.Format("{0} has owned alias {1}", viewType, viewAlias));
            }
            viewAliasMapping.Add(viewAlias, viewType);
        }

        /// <summary>
        /// 查找视图模型对应视图所在的窗体。
        /// </summary>
        /// <param name="viewModel">视图对应的视图模型。</param>
        /// <returns>返回视图所在的窗体。</returns>
        public static Window FindOwnerWindow(object viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            IView view = views.SingleOrDefault(v => ReferenceEquals(v.DataContext, viewModel));
            if (view == null)
            {
                throw new ArgumentException($"The view model {viewModel} of type {viewModel.GetType()} is not referenced by any registered view.");
            }

            if(view is Window dialogView)
            {
                return dialogView;
            }

            //In the cross-process situation,
            //root view may not contained within a Window.
            if (IsRootView(view))
            {
                return null;
            }

            Window owner = GetOwnner(view.ActualView);
            if (owner == null)
            {
                throw new ArgumentException($"The view which is referenced by the view model {viewModel} of type {viewModel.GetType()} is not contained within a window.");
            }

            return owner;
        }

        /// <summary>
        /// 查找视图所在的窗体。
        /// </summary>
        /// <param name="view">视图。</param>
        /// <returns>返回视图所在的窗体。</returns>
        public static Window FindOwnerWindow(IView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (view is Window dialogView)
            {
                return dialogView;
            }

            //In the cross-process situation,
            //root view may not contained within a Window.
            if (IsRootView(view))
            {
                return null;
            }

            Window owner = GetOwnner(view.ActualView);
            if (owner == null)
            {
                throw new ArgumentException($"The view is not contained within a window.");
            }

            return owner;
        }

        /// <summary>
        /// 查找视图模型对应视图所在的窗体。
        /// </summary>
        /// <param name="viewModel">视图对应的内容呈现对象。</param>
        /// <returns>返回视图所在的窗体。</returns>
        public static PresentationSource FindOwnnerSource(object viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            IView view = views.SingleOrDefault(v => ReferenceEquals(v.DataContext, viewModel));
            if (view == null)
            {
                throw new ArgumentException($"The view model {viewModel} of type {viewModel.GetType()} is not referenced by any registered view.");
            }

            var presentationSource = PresentationSource.FromVisual(view.ActualView);

            return presentationSource;
        }

        internal static void Register(IView view)
        {
            var source = PresentationSource.FromVisual(view.ActualView);
            if (source != null && view.ActualView.IsLoaded)
            {
                views.Add(view);

                InitializeAsync(view);

                return;
            }

            Window owner = GetOwnner(view.ActualView);
            if (owner == null)
            {
                view.ActualView.Loaded += LancyRegister;
                return;
            }

            //Visual tree has a up to down initializing 
            if (owner.IsLoaded)
            {
                views.Add(view);

                InitializeAsync(view);

                owner.Closing += owner_Closing;
                owner.Closed += OwnerClosed;
            }
            else
            {
                view.ActualView.Loaded += LancyRegister;
            }
        }

        internal static void Unregister(IView view)
        {
            if (views.Remove(view))
            {
                view.ActualView.Unloaded -= targetView_Unloaded;
                //In case of target view will load again.
                //Such as TabControl TabItem content view
                if (ViewLocator.GetIsRegistered(view.ActualView))
                {
                    view.ActualView.Loaded += LancyRegister;
                }
                else
                {
                    if (view is IDisposable disposableView)
                    {
                        disposableView.Dispose();
                    }

                    //clean up resources,and event subscribution
                    if (view.DataContext is ViewModelBase viewModel)
                    {
                        viewModel.CleanUp();
                    }
                    if (view.DataContext is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        internal static bool IsRootViewModel(object viewModel)
        {
            IView view = views.SingleOrDefault(v => ReferenceEquals(v.DataContext, viewModel));
            if (view == null)
            {
                throw new ArgumentException($"The view model {viewModel} of type {viewModel.GetType()} is not referenced by any registered view.");
            }

            return IsRootView(view);
        }

        internal static bool IsRootView(IView view)
        {
            var hwndSource = (HwndSource)PresentationSource.FromVisual(view.ActualView);
            if (hwndSource != null && hwndSource.RootVisual == view.ActualView)
            {
                return true;
            }

            return false;
        }

        internal static IView GetViewForViewModel(object viewModel, string viewAlias = null)
        {
            IView view = FindViewForViewModel(viewModel, false);
            if (view == null)
            {
                Type viewType = ResolveViewType(viewModel, viewAlias);

                view = defaultViewFactory(viewType);
                if (view == null)
                {
                    throw new ArgumentException($"can not resolve view {viewType} throw view factory", nameof(viewModel));
                }

                if (!string.IsNullOrEmpty(viewAlias))
                {
                    ViewLocator.SetAlias(view.ActualView, viewAlias);
                }

                view.DataContext = viewModel;
            }

            if (!views.Contains(view))
            {
                Register(view);
            }

            return view;
        }

        internal static IView FindViewForViewModel(object viewModel, bool ensureView = true)
        {
            IView view = views.SingleOrDefault(v =>
            {
                bool equal = false;

                v.ActualView.Dispatcher.Invoke(() =>
                {
                    equal = ReferenceEquals(v.DataContext, viewModel);
                });

                return equal;
            });

            if (view == null && ensureView)
            {
                throw new ArgumentException($"Can not find any view's datacontext matching with {viewModel}.Here is some clue：\r\n" +
                    $"1、You are not passing a valid view model ;" +
                    $"2、The view that's related to view model has been closed；" +
                    $"3、The view type does not implement {nameof(IView)} interaface;",
                    nameof(viewModel));
            }

            return view;
        }

        private static Type ResolveViewType(object viewModel, string viewAlias)
        {
            Type viewType = null;
            var viewModelType = viewModel.GetType();
            if (!string.IsNullOrEmpty(viewAlias))
            {
                if (viewAliasMapping.ContainsKey(viewAlias))
                {
                    viewType = viewAliasMapping[viewAlias];
                }
                else
                {
                    throw new ArgumentException($"can not resolve view type by alias {viewAlias}", nameof(viewAlias));
                }
            }

            if (viewType == null)
            {
                if (viewViewModelMappings.ContainsKey(viewModelType))
                {
                    viewType = viewViewModelMappings[viewModelType];
                }
            }

            if (viewType == null)
            {
                viewType = defaultViewModelTypeToViewTypeResolver(viewModelType);
                if (viewType == null)
                {
                    throw new ArgumentException($"can not resolve view type by {viewModel.GetType()}");
                }
            }

            if (!typeof(IView).IsAssignableFrom(viewType))
            {
                throw new InvalidOperationException($"The view type \"{viewType}\" does not implement {nameof(IView)} interaface;");
            }

            return viewType;
        }

        private static Window GetOwnner(FrameworkElement view)
        {
            var ownner = view as Window ?? Window.GetWindow(view);
            if (ownner == null)
                ownner = view.GetVisualParent<Window>();

            return ownner;
        }

        private static void OwnerClosed(object sender, EventArgs e)
        {
            if (sender is Window ownner)
            {
                IEnumerable<IView> windowViews = from view in views
                                                 where GetOwnner(view.ActualView) == ownner
                                                    || view.ActualView.GetVisualParent<Window>() == ownner
                                                 select view;
                foreach (var view in windowViews.ToArray())
                {
                    Unregister(view);
                }

                ownner.Closed -= OwnerClosed;
            }
        }

        static void owner_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is Window ownner)
            {
                IEnumerable<IView> windowViews = from view in views
                                                 where GetOwnner(view.ActualView) == ownner
                                                    || view.ActualView.GetVisualParent<Window>() == ownner
                                                 select view;
                foreach (var view in windowViews.ToArray())
                {
                    Unregister(view);
                }
                ownner.Closing -= owner_Closing;
            }
        }

        private static void LancyRegister(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement target)
            {
                target.Loaded -= LancyRegister;
                target.Unloaded += targetView_Unloaded;
                IView targetView = (IView)target;
                Register(targetView);
            }
        }

        private static void targetView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement target)
            {
                Unregister((IView)target);
            }
        }

        private static void InitializeAsync(IView targetView)
        {
            if (targetView.DataContext is ViewModelBase vm)
            {
                var task = vm.InitializeAsync();

                if (task.Status == TaskStatus.Created)
                {
                    task.Start();
                }
            }
        }
    }
}
