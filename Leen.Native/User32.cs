using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Leen.Native.User32;

namespace Leen.Native
{
    /// <summary>
    /// user32 API。
    /// </summary>
    public static class User32
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public static readonly IntPtr HWND_TOP = new IntPtr(0);
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        //One should check msdn to understand what does these const mean.
        public const int
            GWL_HWNDPARENT = -8,
            GWL_STYLE = -16,
            GWL_EXSTYLE = -20;

        public const int
            WS_EX_TOPMOST = 0x6,
            WS_CHILD = 0x40000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000;

        #region Window Messages
        public const int
            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
            VK_MENU = 0x12,

            //Sent after a window has been moved.
            WM_MOVE = 0x003,

            // Sent to a window after its size has changed.
            WM_SIZE = 0x0005,
            WM_ACTIVATE = 0x0006,

            WM_MOUSEACTIVATE = 0x0021,

            WM_QUERYOPEN = 0x0013,
            WM_ERASEBKGND = 0x0014,
            WM_WINDOWPOSCHANGING = 0x0046,
            WM_WINDOWPOSCHANGED = 0x0047,

            //Notifies a window that its nonclient area is being destroyed.
            WM_NCDESTROY = 0x0082,
            WM_NCACTIVATE = 0x0086,
            WM_KEYFIRST = 0x0100,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_CHAR = 0x0102,
            WM_DEADCHAR = 0x0103,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_SYSCHAR = 0x0106,
            WM_SYSDEADCHAR = 0x0107,
            WM_KEYLAST = 0x0108,
            WM_SYSCOMMAND = 0x0112,

            WM_IME_STARTCOMPOSITION = 0x010D,
            WM_IME_ENDCOMPOSITION = 0x010E,
            WM_IME_COMPOSITION = 0x010F,
            WM_IME_KEYLAST = 0x010F,

            WM_WTSSESSION_CHANGE = 0x02b1;

        #endregion


        /*
         * System Menu Command Values
         */
        public const int
            SC_SIZE = 0xF000,
            SC_MOVE = 0xF010,
            SC_MINIMIZE = 0xF020,
            SC_MAXIMIZE = 0xF030,
            SC_NEXTWINDOW = 0xF040,
            SC_PREVWINDOW = 0xF050,
            SC_CLOSE = 0xF060,
            SC_VSCROLL = 0xF070,
            SC_HSCROLL = 0xF080,
            SC_MOUSEMENU = 0xF090,
            SC_KEYMENU = 0xF100,
            SC_ARRANGE = 0xF110,
            SC_RESTORE = 0xF120,
            SC_TASKLIST = 0xF130,
            SC_SCREENSAVE = 0xF140,
            SC_HOTKEY = 0xF150,
            SC_DEFAULT = 0xF160,
            SC_MONITORPOWER = 0xF170,
            SC_CONTEXTHELP = 0xF180,
            SC_SEPARATOR = 0xF00F;

        public enum SetWindowPosFlags : uint
        {
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOREDRAW = 0x0008,
            SWP_NOACTIVATE = 0x0010,
            SWP_FRAMECHANGED = 0x0020,
            SWP_DRAWFRAME = 0x0020,
            SWP_SHOWWINDOW = 0x0040,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_DEFERERASE = 0x2000,
            SWP_ASYNCWINDOWPOS = 0x4000,
        }

        public enum RedrawWindowFlags : uint
        {
            RDW_INVALIDATE = 0x0001,
            RDW_INTERNALPAINT = 0x0002,
            RDW_ERASE = 0x0004,
            RDW_VALIDATE = 0x0008,
            RDW_NOINTERNALPAINT = 0x0010,
            RDW_NOERASE = 0x0020,
            RDW_NOCHILDREN = 0x0040,
            RDW_ALLCHILDREN = 0x0080,
            RDW_UPDATENOW = 0x0100,
            RDW_ERASENOW = 0x0200,
            RDW_FRAME = 0x0400,
            RDW_NOFRAME = 0x0800,
        }

        [StructLayout(LayoutKind.Sequential), DebuggerDisplay("x={x}, y={y}, cx={cx}, cy={cy}, flags={flags}, hwnd={hwnd}, hdnwAfter={hwndInsertAfter}")]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            [MarshalAs(UnmanagedType.U4)]
            public SetWindowPosFlags flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public Point(int x,int y)
            {
                this.x = x;
                this.y = y;
            }

            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseHookStruct
        {
            public Point pt;
            public IntPtr hwnd;
            public uint wHitTestCode;
            public uint dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPStruct
        {
            public IntPtr lParam;
            public IntPtr wParam;
            public uint message;
            public IntPtr hwnd;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPRetStruct
        {
            public int lResult;
            public IntPtr lParam;
            public IntPtr wParam;
            public uint message;
            public IntPtr hwnd;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LASTINPUTINFO
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;

            [MarshalAs(UnmanagedType.U4)]
            public int dwTime;
        }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        #region P/Invoke Imports

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        /// <summary>
        /// Gets the maximum number of milliseconds that can elapse between a first click and a second click for the OS to consider the mouse action a double-click.
        /// </summary>
        /// <returns>The maximum amount of time, in milliseconds, that can elapse between a first click and a second click for the OS to consider the mouse action a double-click.</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
        public static extern uint GetDoubleClickTime();

        /// <summary>
        ///  获取系统最后一次输入信息。
        /// </summary>
        /// <param name="lastInputInfo">获取到的输入信息。</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO lastInputInfo);

        /// <summary>
        /// Places (posts) a message in the message queue associated with the thread that created the specified window and returns without waiting for the thread to process the message.
        /// </summary>
        /// <param name="hwnd">
        /// A handle to the window whose window procedure is to receive the message. 
        /// <para>
        /// The message is posted to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows. The message is not posted to child windows.
        /// </para>
        /// </param>
        /// <param name="wMsg">The message to be posted.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <returns>
        /// If the function succeeds, the return value is true.If the function fails, the return value is false.
        /// <para>
        /// To get extended error information, call <see cref="Kernel32.GetLastError"/>. 
        /// </para>
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool PostMessage(IntPtr hwnd, uint wMsg, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
        /// </summary>
        /// <param name="hwnd">
        /// A handle to the window whose window procedure will receive the message.
        /// <para>
        /// If this parameter is HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.
        /// </para>
        /// </param>
        /// <param name="msg">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// <para>
        /// To get extended error information, call <see cref="Kernel32.GetLastError"/>.
        /// </para>
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hwnd, uint msg, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// This function retrieves the handle to the top-level window whose class name and window name match the specified strings. This function does not search child windows. 
        /// </summary>
        /// <param name="lpClassName"> 
        /// A null-terminated string that specifies the class name or is an atom that identifies the class-name string. 
        /// <para>
        /// If lpClassName is an atom, it must be an atom returned from RegisterClass.
        /// </para>
        /// <para>
        /// If lpClassName is NULL, it finds any window whose title matches the lpWindowName parameter. 
        /// </para>
        /// </param>
        /// <param name="lpWindowName">A null-terminated string that specifies the window name (the window's title). . </param>
        /// <returns>A handle to the window that has the specified class name and window name indicates success. NULL indicates failure.
        /// <para>To get extended error information, call <see cref="Kernel32.GetLastError"/>. </para>
        /// </returns>
        [DllImport("User32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr FindWindow([MarshalAs(UnmanagedType.LPStr)]string lpClassName, [MarshalAs(UnmanagedType.LPStr)]string lpWindowName);

        /// <summary>
        /// Retrieves the window handle to the active window attached to the calling thread's message queue.
        /// </summary>
        /// <returns>The return value is the handle to the active window attached to the calling thread's message queue. Otherwise, the return value is NULL.</returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr GetActiveWindow();

        /// <summary>
        /// Changes the parent window of the specified child window.
        /// </summary>
        /// <param name="hWndChild">A handle to the child window.</param>
        /// <param name="hWndNewParent">A handle to the new parent window. If this parameter is NULL, the desktop window becomes the new parent window. If this parameter is HWND_MESSAGE, the child window becomes a message-only window.</param>
        /// <returns>If the function succeeds, the return value is a handle to the previous parent window.If the function fails, the return value is NULL.To get extended error information, call <see cref="Kernel32.GetLastError"/>.</returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr SetParent([In()] IntPtr hWndChild, [In()] IntPtr hWndNewParent);

        /// <summary>
        /// Retrieves a handle to the specified window's parent or owner.
        /// To retrieve a handle to a specified ancestor, use the <see cref="GetAncestor(IntPtr, uint)"/> function.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose parent window handle is to be retrieved.</param>
        /// <returns></returns>
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        /// <summary>
        /// Retrieves the handle to the ancestor of the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose ancestor is to be retrieved. If this parameter is the desktop window, the function returns NULL.</param>
        /// <param name="gaFlags">
        /// The ancestor to be retrieved. This parameter can be one of the following values.
        /// <![CDATA[
        ///     GA_PARENT：1     Retrieves the parent window. This does not include the owner, as it does with the GetParent function.
        ///     GA_ROOT：2       Retrieves the root window by walking the chain of parent windows.
        ///     GA_ROOTOWNER：3  Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent.
        /// ]]>
        /// </param>
        /// <returns></returns>
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which the user is currently working). The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
        /// </summary>
        /// <returns>The return value is a handle to the foreground window. The foreground window can be NULL in certain circumstances, such as when a window is losing activation.</returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Brings the thread that created the specified window into the foreground and activates the window. Keyboard input is directed to the window, and various visual cues are changed for the user. The system assigns a slightly higher priority to the thread that created the foreground window than it does to other threads.
        /// </summary>
        /// <param name="hWnd">A handle to the window that should be activated and brought to the foreground.</param>
        /// <returns></returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Changes an attribute of the specified window. 
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">The zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer. </param>
        /// <param name="dwNewLong">The replacement value.</param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified 32-bit integer. If the function fails, the return value is zero.To get extended error information, call <see cref="Kernel32.GetLastError"/> .</returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern int SetWindowLong([In()] IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Retrieves information about the specified window. The function also retrieves the 32-bit (DWORD) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">The zero-based offset to the value to be retrieved. Check msdn for more details.</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="hWndInsertAfter">
        /// A handle to the window to precede the positioned window in the Z order. This parameter must be a window handle or one of the following values.
        /// <![CDATA[
        ///     HWND_BOTTOM：1
        ///     HWND_NOTOPMOST：-2
        ///     HWND_TOP：0
        ///     HWND_TOPMOST:-1
        /// ]]>
        /// </param>
        /// <param name="X">The new position of the left side of the window, in client coordinates.</param>
        /// <param name="Y">The new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">The new width of the window, in pixels.</param>
        /// <param name="cy">The new height of the window, in pixels.</param>
        /// <param name="flags">The window sizing and positioning flags. </param>
        /// <returns></returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, [MarshalAs(UnmanagedType.U4)] SetWindowPosFlags flags);

        /// <summary>
        /// Sets the keyboard focus to the specified window. The window must be attached to the calling thread's message queue.
        /// </summary>
        /// <param name="hWnd">A handle to the window that will receive the keyboard input. If this parameter is NULL, keystrokes are ignored.</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        /// <summary>
        /// Activates a window. The window must be attached to the calling thread's message queue.
        /// </summary>
        /// <param name="hWnd">A handle to the top-level window to be activated.</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        /// <summary>
        /// Enables or disables mouse and keyboard input to the specified window or control. When input is disabled, the window does not receive input such as mouse clicks and key presses. When input is enabled, the window receives all input.
        /// </summary>
        /// <param name="hwnd">A handle to the window to be enabled or disabled.</param>
        /// <param name="enable">Indicates whether to enable or disable the window. If this parameter is TRUE, the window is enabled. If the parameter is FALSE, the window is disabled.</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool EnableWindow(IntPtr hwnd, bool enable);

        /// <summary>
        /// Determines whether the specified window is minimized (iconic).
        /// </summary>
        /// <param name="hwnd">A handle to the window to be tested.</param>
        /// <returns></returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool IsIconic(IntPtr hwnd);

        /// <summary>
        /// Determines whether a window is maximized.
        /// </summary>
        /// <param name="hwnd">A handle to the window to be tested.</param>
        /// <returns></returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool IsZoomed(IntPtr hwnd);

        /// <summary>
        /// Enables you to produce special effects when showing or hiding windows. There are four types of animation: roll, slide, collapse or expand, and alpha-blended fade.
        /// </summary>
        /// <param name="hwnd">A handle to the window to animate. The calling thread must own this window.</param>
        /// <param name="dwTime">The time it takes to play the animation, in milliseconds. Typically, an animation takes 200 milliseconds to play.</param>
        /// <param name="dwFlags">The type of animation. Check msdn for more details.</param>
        /// <returns>If the function succeeds, the return value is true.If the function fails, the return value is false. </returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool AnimateWindow(IntPtr hwnd, uint dwTime, uint dwFlags);

        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <param name="hwnd">A handle to the window.</param>
        /// <param name="nCmdShow">Controls how the window is to be shown.Check msdn for more details. </param>
        /// <returns>If the function succeeds, the return value is true.If the function fails, the return value is false.</returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        /// <summary>
        /// Flashes the specified window one time. It does not change the active state of the window.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be flashed. The window can be either open or minimized.</param>
        /// <param name="bInvert">
        /// If this parameter is TRUE, the window is flashed from one state to the other. If it is FALSE, the window is returned to its original state (either active or inactive).
        /// When an application is minimized and this parameter is TRUE, the taskbar window button flashes active/inactive.If it is FALSE, the taskbar window button flashes inactive, meaning that it does not change colors.
        /// It flashes, as if it were being redrawn, but it does not provide the visual invert clue to the user.
        /// </param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FlashWindow([In()] IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bInvert);

        /// <summary>
        /// Calls the default window procedure to provide default processing for any window messages that an application does not process. This function ensures that every message is processed. DefWindowProc is called with the same parameters received by the window procedure.
        /// </summary>
        /// <param name="hWnd">A handle to the window that received the message.</param>
        /// <param name="Msg">The message.</param>
        /// <param name="wParam">Additional message information. The content of this parameter depends on the value of the Msg parameter.</param>
        /// <param name="lParam">Additional message information. The content of this parameter depends on the value of the Msg parameter.</param>
        /// <returns>The return value is the result of the message processing and depends on the message.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr DefWindowProc([In()] IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain. You would install a hook procedure to monitor the system for certain types of events. These events are associated either with a specific thread or with all threads in the same desktop as the calling thread.
        /// </summary>
        /// <param name="idHook">The type of hook procedure to be installed.</param>
        /// <param name="lpfn">A pointer to the hook procedure. </param>
        /// <param name="hInstance">A handle to the DLL containing the hook procedure pointed to by the <paramref name="lpfn"/> parameter. </param>
        /// <param name="threadId">The identifier of the thread with which the hook procedure is to be associated. </param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure.If the function fails, the return value is NULL. To get extended error information, call <see cref="Kernel32.GetLastError"/>.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, NativeMethods.HookProc lpfn, IntPtr hInstance, uint threadId);

        /// <summary>
        /// Removes a hook procedure installed in a hook chain by the <see cref="SetWindowsHookEx(int, HookProc, IntPtr, uint)"/> function.
        /// </summary>
        /// <param name="hhk">A handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to <see cref="SetWindowsHookEx(int, HookProc, IntPtr, uint)"/>.</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        /// Passes the hook information to the next hook procedure in the current hook chain. A hook procedure can call this
        /// function either before or after processing the hook information.
        /// </summary>
        /// <param name="hhk">This parameter is ignored. </param>
        /// <param name="nCode">The hook code passed to the current hook procedure. The next
        ///     hook procedure uses this code to determine how to process the hook information.</param>
        /// <param name="wParam">The wParam value passed to the current hook procedure. The
        ///     meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
        /// <param name="lParam">The lParam value passed to the current hook procedure. The
        ///     meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
        /// <returns></returns>
        /// <remarks>
        ///     <para>
        ///         Hook procedures are installed in chains for particular hook types. <see cref="CallNextHookEx" /> calls the
        ///         next hook in the chain.
        ///     </para>
        ///     <para>
        ///         Calling CallNextHookEx is optional, but it is highly recommended; otherwise, other applications that have
        ///         installed hooks will not receive hook notifications and may behave incorrectly as a result. You should call
        ///         <see cref="CallNextHookEx" /> unless you absolutely need to prevent the notification from being seen by other
        ///         applications.
        ///     </para>
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// This function updates the specified rectangle or region in a window's client area.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be redrawn. If this parameter is NULL, the desktop window is updated.</param>
        /// <param name="lprcUpdate">A pointer to a RECT structure containing the coordinates, in device units, of the update rectangle. This parameter is ignored if the hrgnUpdate parameter identifies a region.</param>
        /// <param name="hrgnUpdate">A handle to the update region. If both the hrgnUpdate and lprcUpdate parameters are NULL, the entire client area is added to the update region.</param>
        /// <param name="flags">One or more redraw flags. </param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RedrawWindow(
           [In()] IntPtr hWnd,
           [In()] IntPtr lprcUpdate,
           [In()] IntPtr hrgnUpdate,
           [MarshalAs(UnmanagedType.U4)] RedrawWindowFlags flags);

        /// <summary>
        /// This function updates the client area of the specified window by sending a WM_PAINT message to the window if the window's update region is not empty. The function sends a WM_PAINT message directly to the window procedure of the specified window, bypassing the application queue. If the update region is empty, no message is sent.
        /// </summary>
        /// <param name="hWnd">Handle to the window to be updated.</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UpdateWindow(IntPtr hWnd);

        /// <summary>
        /// 获取窗口大小。
        /// </summary>
        /// <param name="hWnd">要获取的窗口句柄。</param>
        /// <param name="lpRect">要获取的窗口大小结构体。</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        /// <summary>
        /// 获取窗口工作区大小。
        /// </summary>
        /// <param name="hWnd">要获取的窗口句柄。</param>
        /// <param name="lpRect">要获取的窗口工作区大小结构体。</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

        /// <summary>
        /// Retrieves the specified system metric or system configuration setting.
        /// </summary>
        /// <param name="nIndex">The system metric or configuration setting to be retrieved. One should check msdn to figure out what value this parameter can be.</param>
        /// <returns>If the function succeeds, the return value is the requested system metric or configuration setting.If the function fails, the return value is 0.</returns>
        [DllImport("user32")]
        public static extern int GetSystemMetrics(int nIndex);

        /// <summary>
        /// Retrieves a handle to the desktop window. The desktop window covers the entire screen. The desktop window is the area on top of which other windows are painted.
        /// </summary>
        /// <returns>The return value is a handle to the desktop window.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        #endregion

        /// <summary>
        /// 设置窗口父窗体。
        /// </summary>
        /// <param name="hwnd">子窗体句柄。</param>
        /// <param name="hWndNewOwner">父窗体句柄。</param>
        public static void SetOwner(IntPtr hwnd, IntPtr hWndNewOwner)
        {
            SetWindowLong(hwnd, GWL_HWNDPARENT, hWndNewOwner.ToInt32());
        }

        /// <summary>
        /// 将指定窗体转换为子窗体。
        /// </summary>
        /// <param name="hwnd">A handle to the window.</param>
        public static void ConvertToChildWindow(IntPtr hwnd)
        {
            SetWindowLong(hwnd, GWL_STYLE, WS_CHILD | WS_CLIPCHILDREN);
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0,
                SetWindowPosFlags.SWP_FRAMECHANGED |
                SetWindowPosFlags.SWP_NOMOVE |
                SetWindowPosFlags.SWP_NOSIZE |
                SetWindowPosFlags.SWP_NOZORDER);
        }

        /// <summary>
        /// 获取系统距上一次接收输入起的空闲时间（毫秒)。
        /// </summary>
        /// <returns>返回输入空闲时间（毫秒）。</returns>
        public static int GetLastInputTime()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            int idleTime = 0;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                idleTime = Environment.TickCount - lastInputInfo.dwTime;
            }

            return idleTime;
        }

        public const int GW_CHILD = 5;
        public const int GW_HWNDFIRST = 0;
        public const int GW_HWNDLAST = 1;
        public const int GW_HWNDNEXT = 2;
        public const int GW_HWNDPREV = 3;
        public const int GW_OWNER = 4;
        public const int GWL_WNDPROC = -4;
        public const int HC_ACTION = 0;
        public const int HDI_HEIGHT = 1;
        public const int HDI_ORDER = 0x80;
        public const int HDI_WIDTH = 1;
        public const int HDM_FIRST = 0x1200;
        public const int HDM_GETITEMA = 0x1203;
        public const int HDM_GETITEMCOUNT = 0x1200;
        public const int HDM_GETITEMRECT = 0x1207;
        public const int HTCAPTION = 2;
        public const int HTHSCROLL = 6;
        public const int HTVSCROLL = 7;
        public const int LVM_FIRST = 0x1000;
        public const int LVM_GETHEADER = 0x101f;
        public const uint OBJID_ALERT = 0xfffffff6;
        public const uint OBJID_CARET = 0xfffffff8;
        public const uint OBJID_CLIENT = 0xfffffffc;
        public const uint OBJID_CURSOR = 0xfffffff7;
        public const uint OBJID_HSCROLL = 0xfffffffa;
        public const uint OBJID_MENU = 0xfffffffd;
        public const uint OBJID_NATIVEOM = 0xfffffff0;
        public const uint OBJID_QUERYCLASSNAMEIDX = 0xfffffff4;
        public const uint OBJID_SIZEGRIP = 0xfffffff9;
        public const uint OBJID_SOUND = 0xfffffff5;
        public const uint OBJID_SYSMENU = uint.MaxValue;
        public const uint OBJID_TITLEBAR = 0xfffffffe;
        public const uint OBJID_VSCROLL = 0xfffffffb;
        public const uint OBJID_WINDOW = 0;
        public const int WH_CALLWNDPROC = 4;

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hwnd, char[] className, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClientRect(IntPtr hwnd, [In, Out] ref Rectangle rect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetComboBoxInfo(IntPtr hwndCombo, ref COMBOBOXINFO info);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr handle);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetMessagePos();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetScrollBarInfo(IntPtr hwnd, uint idObject, ref SCROLLBARINFO psbi);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetScrollInfo(IntPtr hwnd, int bar, ref Win32ScrollInfo si);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool GetScrollRange(IntPtr hWnd, int nBar, ref int lpMinPos, ref int lpMaxPos);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindow(IntPtr hwnd, int uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetWindowRect(IntPtr hWnd, [In, Out] ref Rectangle rect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool InvalidateRect(IntPtr hwnd, ref Rectangle rect, bool bErase);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern int OffsetRect(ref RECT lpRect, int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool PtInRect(ref RECT lprc, Point pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr ReleaseDC(IntPtr handle, IntPtr hDC);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, int wParam, ref RECT lParam);

        [DllImport("user32.dll")]
        public static extern int SetScrollInfo(IntPtr hwnd, int fnBar, [In] ref Win32ScrollInfo lpsi, bool fRedraw);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool ValidateRect(IntPtr hwnd, ref Rectangle rect);

        public static int GET_X_LPARAM(int lParam)
        {
            return (lParam & 0xffff);
        }

        public static int GET_Y_LPARAM(int lParam)
        {
            return (lParam >> 0x10);
        }

        public static Point GetPointFromLPARAM(int lParam)
        {
            return new Point(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
        }

        public static int HIGH_ORDER(int param)
        {
            return (param >> 0x10);
        }

        public static int LOW_ORDER(int param)
        {
            return (param & 0xffff);
        }

        public enum ComboBoxButtonState
        {
            STATE_SYSTEM_INVISIBLE = 0x8000,
            STATE_SYSTEM_NONE = 0,
            STATE_SYSTEM_PRESSED = 8
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COMBOBOXINFO
        {
            public int cbSize;
            public User32.RECT rcItem;
            public User32.RECT rcButton;
            public User32.ComboBoxButtonState stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndEdit;
            public IntPtr hwndList;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HDITEM
        {
            public int mask;
            public int cxy;
            public string pszText;
            public IntPtr hbm;
            public int cchTextMax;
            public int fmt;
            public IntPtr lParam;
            public int iImage;
            public int iOrder;
            public uint type;
            public IntPtr pvFilter;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NCCALCSIZE_PARAMS
        {
            public User32.RECT Rect;
            public User32.WINDOWPOS WindowPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public int fErase;
            public Rectangle rcPaint;
            public int fRestore;
            public int fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public byte[] rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }

            public RECT(Rectangle rect)
            {
                this.Left = rect.Left;
                this.Top = rect.Top;
                this.Right = rect.Right;
                this.Bottom = rect.Bottom;
            }

            public override string ToString()
            {
                return string.Format("Left={0},Top={1},Right={2},Bottom={3}", new object[] { this.Left, this.Top, this.Right, this.Bottom });
            }

            public Rectangle Rect
            {
                get
                {
                    return new Rectangle(this.Left, this.Top, this.Right - this.Left, this.Bottom - this.Top);
                }
            }
            public System.Drawing.Size Size
            {
                get
                {
                    return new System.Drawing.Size(this.Right - this.Left, this.Bottom - this.Top);
                }
            }
            public static User32.RECT FromXYWH(int x, int y, int width, int height)
            {
                return new User32.RECT(x, y, x + width, y + height);
            }

            public static User32.RECT FromRectangle(Rectangle rect)
            {
                return new User32.RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLBARINFO
        {
            public uint cbSize;
            public User32.RECT rcScrollBar;
            public int dxyLineButton;
            public int xyThumbTop;
            public int xyThumbBottom;
            public int reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x18)]
            public uint[] rgstate;
        }

        public enum Win32ScrollBarMask
        {
            SIF_ALL = 0x17,
            SIF_DISABLENOSCROLL = 8,
            SIF_PAGE = 2,
            SIF_POS = 4,
            SIF_RANGE = 1,
            SIF_TRACKPOS = 0x10
        }

        public enum Win32ScrollEventType
        {
            SB_BOTTOM = 7,
            SB_ENDSCROLL = 8,
            SB_LEFT = 6,
            SB_LINEDOWN = 1,
            SB_LINELEFT = 0,
            SB_LINERIGHT = 1,
            SB_LINEUP = 0,
            SB_PAGEDOWN = 3,
            SB_PAGELEFT = 2,
            SB_PAGERIGHT = 3,
            SB_PAGEUP = 2,
            SB_RIGHT = 7,
            SB_THUMBPOSITION = 4,
            SB_THUMBTRACK = 5,
            SB_TOP = 6
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Win32ScrollInfo
        {
            public uint cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }

        public enum Win32ScrollOrientation
        {
            SB_HORZ,
            SB_VERT,
            SB_CTL,
            SB_BOTH
        }
    }
}

