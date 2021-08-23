// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Windows;

#if NETFX_CORE
using Windows.UI.Xaml;
#endif
namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// This class defines the attached property and related change handler that calls the ViewModelLocator in Prism.Mvvm.
    /// </summary>
    public static class ViewModelLocator
    {
        #region Attached property with convention-or-mapping based approach

        /// <summary>
        /// The AutoWireViewModel attached property.
        /// </summary>
        public static DependencyProperty AutoWireViewModelProperty =
            DependencyProperty.RegisterAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator),
            new UIPropertyMetadata(false, AutoWireViewModelChanged));

        private static void AutoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            if (d is IView view)
                ViewModelLocationProvider.AutoWireViewModelChanged(view);
        }

        /// <summary>
        /// Gets the value of the AutoWireViewModel attached property.
        /// </summary>
        /// <param name="obj">The dependency object that has this attached property.</param>
        /// <returns><c>True</c> if view model autowiring is enabled; otherwise, <c>false</c>.</returns>
        public static bool GetAutoWireViewModel(DependencyObject obj)
        {
            if (obj != null)
            {
                return (bool)obj.GetValue(AutoWireViewModelProperty);
            }
            return false;
        }

        /// <summary>
        /// Sets the value of the AutoWireViewModel attached property.
        /// </summary>
        /// <param name="obj">The dependency object that has this attached property.</param>
        /// <param name="value">if set to <c>true</c> the view model wiring will be performed.</param>
        public static void SetAutoWireViewModel(DependencyObject obj, bool value)
        {
            if (obj != null)
            {
                obj.SetValue(AutoWireViewModelProperty, value);
            }
        }

        #endregion

        /// <summary>
        /// Gets the value of the Alias attached property.
        /// </summary>
        /// <remarks>
        /// We can support multiple views over the same View-Model by setting a ViewModelLocator.Alias in Xaml.
        /// </remarks>
        /// <param name="obj">The dependency object that has this attached property.</param>
        /// <returns>The view's view model alias.</returns>
        internal static string GetAlias(DependencyObject obj)
        {
            return (string)obj.GetValue(AliasProperty);
        }

        /// <summary>
        /// Sets the value of the Alias attached property.
        /// </summary>
        /// <param name="obj">The dependency object that has this attached property.</param>
        /// <param name="value">The view's view model alias.</param>
        internal static void SetAlias(DependencyObject obj, string value)
        {
            obj.SetValue(AliasProperty, value);
        }

        /// <summary>
        /// The Alias attached property.
        /// </summary>
        internal static readonly DependencyProperty AliasProperty =
            DependencyProperty.RegisterAttached("Alias", typeof(string), typeof(ViewModelLocator), new PropertyMetadata(null));
    }
}
