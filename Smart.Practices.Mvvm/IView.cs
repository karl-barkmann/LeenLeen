// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Windows;
namespace Smart.Practices.Mvvm
{
    public interface IView
    {
        FrameworkElement ActualView { get; }

        object DataContext { get; set; }
    }
}
