using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Microsoft.Practices.ServiceLocation;

namespace Smart.Practices.Mvvm
{
    /// <summary>
    /// 用户界面交互服务。
    /// </summary>
    public class InteractionService : IInteractionService
    {
        private readonly HashSet<FrameworkElement> views;
        private readonly IWindowViewModelMapping viewViewModelMapping;

        public InteractionService(IWindowViewModelMapping mapping=null)
        {
            views = new HashSet<FrameworkElement>();
            viewViewModelMapping = mapping;
        }

        #region 界面注册依赖属性

        public static bool GetIsRegistered(FrameworkElement obj)
        {
            return (bool)obj.GetValue(IsRegisteredProperty);
        }

        public static void SetIsRegistered(FrameworkElement obj, bool value)
        {
            obj.SetValue(IsRegisteredProperty, value);
        }

        public static readonly DependencyProperty IsRegisteredProperty =
            DependencyProperty.RegisterAttached("IsRegistered", typeof(bool), typeof(InteractionService), new UIPropertyMetadata(IsRegisteredPropertyChanged));

        private static void IsRegisteredPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(target)) return;
            FrameworkElement view = target as FrameworkElement;
            if (view != null)
            {
                bool newValue = (bool)e.NewValue;
                bool oldValue = (bool)e.OldValue;
                if (newValue)
                {
                    ServiceLocator.Current.GetInstance<IInteractionService>().Register(view);
                }
                else
                {
                    ServiceLocator.Current.GetInstance<IInteractionService>().Unregister(view);
                }
            }
        }

        #endregion

        public void Register(System.Windows.FrameworkElement view)
        {
            Window owner = GetOwnner(view);
            if (owner == null)
            {
                view.Loaded += LancyRegister;
                return;
            }
            
            owner.Closed += OwnerClosed;
            views.Add(view);
        }

        public void Unregister(System.Windows.FrameworkElement view)
        {
            views.Remove(view);
        }

        public void Show<T>(object viewModel, object ownnerViewModel = null) where T : System.Windows.Window
        {
            Show(viewModel, typeof(T), ownnerViewModel);
        }

        public void Show(object viewModel, object ownnerViewModel = null)
        {
            Type type = viewViewModelMapping.GetWindowTypeFromViewModelType(viewModel.GetType());
            Show(viewModel, type, ownnerViewModel);
        }

        public bool? ShowDialog<T>(object viewModel, object ownnerViewModel = null) where T : System.Windows.Window
        {
            return ShowDialog(viewModel, typeof(T), ownnerViewModel);
        }

        public bool? ShowDialog(object viewModel, object ownnerViewModel = null)
        {
            Type type = viewViewModelMapping.GetWindowTypeFromViewModelType(viewModel.GetType());
            return ShowDialog(viewModel, type, ownnerViewModel);
        }

        public MessageBoxResult ShowMessageBox(object ownerViewModel, string messageText, string caption, MessageBoxButton button, MessageBoxImage image,MessageBoxResult result, MessageBoxOptions options)
        {
            return MessageBox.Show(FindOwnnerWindow(ownerViewModel), messageText, caption, button, image, result, options);
        }

        private bool? ShowDialog(object viewModel, Type dialogType, object ownnerViewModel=null)
        {
            Window dialog = (Window)Activator.CreateInstance(dialogType);
            if(ownnerViewModel!=null)
                dialog.Owner = FindOwnnerWindow(ownnerViewModel);
            dialog.DataContext = viewModel;

            return dialog.ShowDialog();
        }

        private void Show(object viewModel, Type dialogType, object ownnerViewModel = null)
        {
            Window dialog = (Window)Activator.CreateInstance(dialogType);
            if (ownnerViewModel != null)
                dialog.Owner = FindOwnnerWindow(ownnerViewModel);
            dialog.DataContext = viewModel;

            dialog.Show();
        }

        private Window GetOwnner(FrameworkElement view)
        {
            return view as Window ?? Window.GetWindow(view);
        }

        private void OwnerClosed(object sender, EventArgs e)
        {
            Window owner = sender as Window;
            if (owner != null)
            {
                IEnumerable<FrameworkElement> windowViews = from view in views
                                                            where Window.GetWindow(view) == owner
                                                            select view;
                foreach (var view in windowViews.ToArray())
                {
                    Unregister(view);
                }
            }
        }

        private void LancyRegister(object sender, RoutedEventArgs e)
        {
            FrameworkElement view = sender as FrameworkElement;
            if (view != null)
            {
                view.Loaded -= LancyRegister;

                Register(view);
            }
        }

        private Window FindOwnnerWindow(object viewModel)
        {
            FrameworkElement view = views.SingleOrDefault(v => ReferenceEquals(v.DataContext, viewModel));
            if (view == null)
            {
                throw new ArgumentException("viewModel is not reference by any registered view.");
            }

            Window owner = GetOwnner(view);

            if (owner == null)
            {
                throw new InvalidOperationException("View which is reference by viewModel is not contained within a window.");
            }

            return owner;
        }
    }
}
