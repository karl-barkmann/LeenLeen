// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Leen.Windows.Interaction
{
    /// <summary>
    /// Defines the interface for the Dialogs that will be activated by <see cref="IUIInteractionService"/>.
    /// </summary>
    public interface IWindow : IWin32Window
    {
        /// <summary>
        /// Ocurrs when the <see cref="IWindow"/> is closed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Gets or sets the title for the <see cref="IWindow"/>.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the content for the <see cref="IWindow"/>.
        /// </summary>
        object Content { get; set; }

        /// <summary>
        /// Gets or sets the owner control or handle of the <see cref="IWindow"/>.
        /// </summary>
        object Owner { get; set; }

        /// <summary>
        /// Gets or sets the height of the <see cref="IWindow"/>.
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// Gets or sets the width of the <see cref="IWindow"/>.
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.Windows.Style"/> to apply to the <see cref="IWindow"/>.
        /// </summary>
        Style Style { get; set; }

        /// <summary>
        ///  Gets or sets the <see cref="ImageSource"/> to apply to the <see cref="IWindow"/>.
        /// </summary>
        ImageSource Icon { get; set; }

        /// <summary>
        /// Opens the <see cref="IWindow"/>.
        /// </summary>
        bool? ShowDialog();

        /// <summary>
        /// Opens the <see cref="IWindow"/>.
        /// </summary>
        void Show();

        /// <summary>
        /// Gets or Sets the DataContext  for the <see cref="IWindow"/>.
        /// </summary>
        object DataContext { get; set; }

        /// <summary>
        /// Closes the <see cref="IWindow"/>.
        /// </summary>
        void Close();
    }
}