// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Windows;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// ����һ����ͼ��
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// ��ͼ�ӿڶ�Ӧ��WPF UIԪ�ء�
        /// </summary>
        FrameworkElement ActualView { get; }

        /// <summary>
        /// ��ȡ��������ͼ�����������ģ���ͼģ�ͣ���
        /// </summary>
        object DataContext { get; set; }
    }
}
