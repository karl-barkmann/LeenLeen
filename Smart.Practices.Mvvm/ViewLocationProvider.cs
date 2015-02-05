using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Smart.Practices.Mvvm
{
    public static class ViewLocationProvider
    {
        private static Dictionary<Type, Type> viewViewModelMappings = new Dictionary<Type, Type>();

        private static Func<Type, IView> defaultViewFactory = type => (IView)Activator.CreateInstance(type);

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
                var viewModelAssemblyName = viewModelType.Assembly.FullName;
                var viewName = String.Format(CultureInfo.InvariantCulture, "{0}View, {1}", viewModelName,
                    viewModelAssemblyName);
                return Type.GetType(viewName);
            };


        private static readonly HashSet<IView> views = new HashSet<IView>();

        public static void SetDefaultViewFactory(Func<Type, IView> viewFactory)
        {
            defaultViewFactory = viewFactory;
        }

        public static void SetDefaultViewModelTypeToViewTypeResolver(Func<Type, Type> viewModelTypeToViewTypeResolver)
        {
            defaultViewModelTypeToViewTypeResolver = viewModelTypeToViewTypeResolver;
        }

        public static void Map(Type viewModelType,Type viewType)
        {
            if (viewViewModelMappings.ContainsKey(viewModelType))
            {
                viewViewModelMappings[viewModelType] = viewType;
            }
            else
            {
                viewViewModelMappings.Add(viewModelType, viewType);
            }
        }

        public static Window FindOwnnerWindow(object viewModel)
        {
            IView view = views.SingleOrDefault(v => ReferenceEquals(v.DataContext, viewModel));
            if (view == null)
            {
                throw new ArgumentException("viewModel is not reference by any registered view.");
            }

            Window owner = GetOwnner(view.ActualView);
            if (owner == null)
            {
                throw new ArgumentException("View which is reference by viewModel is not contained within a window.");
            }

            return owner;
        }

        internal static void Register(IView view)
        {
            Window owner = GetOwnner(view.ActualView);
            if (owner == null)
            {
                view.ActualView.Loaded += LancyRegister;
                return;
            }

            owner.Closing += owner_Closing;
            owner.Closed += OwnerClosed;
            views.Add(view);
        }

        internal static void Unregister(IView view)
        {
            views.Remove(view);
            view.ActualView.Unloaded -= targetView_Unloaded;
            //In case of target view will load again.
            //Such as TabControl TabItem content view
            if (ViewLocator.GetIsRegistered(view.ActualView))
            {
                view.ActualView.Loaded += LancyRegister;
            }

            //clean up resources,and event subscribution
            var viewModel = view.DataContext as IViewModel;
            if (viewModel != null)
            {
                viewModel.CleanUp();
            }
            var disposable = view.DataContext as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }

            var disposableView = view as IDisposable;
            if(disposableView!=null)
            {
                disposableView.Dispose();
            }
        }

        internal static IView GetViewForViewModel(object viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");

            IView view = FindViewForViewModel(viewModel);
            if (view == null)
            {
                Type viewType = null;
                var viewModelType = viewModel.GetType();
                if (viewViewModelMappings.ContainsKey(viewModelType))
                {
                    viewType = viewViewModelMappings[viewModelType];
                }

                if (viewType == null)
                {
                    viewType = defaultViewModelTypeToViewTypeResolver(viewModelType);
                    if (viewType == null)
                    {
                        throw new ArgumentNullException("can not resolve view type by viewModel");
                    }
                }

                view = defaultViewFactory(viewType);
                if (view == null)
                {
                    throw new ArgumentNullException("can not resolve view throw view factory");
                }
            }
            view.DataContext = viewModel;
            return view;
        }

        private static Window GetOwnner(FrameworkElement view)
        {
            var ownner = view as Window ?? Window.GetWindow(view);
            if (ownner == null)
                ownner = GetVisualParent<Window>(view);

            return ownner;
        }

        private static IView FindViewForViewModel(object viewModel)
        {
            IView view = views.SingleOrDefault(v => ReferenceEquals(v.DataContext, viewModel));
            return view;
        }

        private static void OwnerClosed(object sender, EventArgs e)
        {
            Window ownner = sender as Window;
            if (ownner != null)
            {
                IEnumerable<IView> windowViews = from view in views
                                                 where GetOwnner(view.ActualView) == ownner 
                                                    || GetVisualParent<Window>(view.ActualView) == ownner
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
            Window ownner = sender as Window;
            if (ownner != null)
            {
                IEnumerable<IView> windowViews = from view in views
                                                 where GetOwnner(view.ActualView) == ownner
                                                    || GetVisualParent<Window>(view.ActualView) == ownner
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
            FrameworkElement target = sender as FrameworkElement;
            if (target != null)
            {
                target.Loaded -= LancyRegister;
                target.Unloaded += targetView_Unloaded;
                Register((IView) target);
            }
        }

        static void targetView_Unloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement target = sender as FrameworkElement;
            if (target != null)
            {
                Unregister((IView)target);
            }
        }

        private static T GetVisualParent<T>(DependencyObject child) where T : Visual
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return GetVisualParent<T>(parentObject);
            }
        }
    }
}
