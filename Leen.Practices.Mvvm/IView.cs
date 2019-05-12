// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Windows;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 定义一个视图。
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// 视图接口对应的WPF UI元素。
        /// </summary>
        FrameworkElement ActualView { get; }

        /// <summary>
        /// 获取或设置视图的数据上下文（视图模型）。
        /// </summary>
        object DataContext { get; set; }
    }
}
