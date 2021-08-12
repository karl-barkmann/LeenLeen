using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Leen.Windows
{
    class TouchKeyboardProvider 
    {
        #region Private: Fields

        private static readonly string _virtualKeyboardPath;
        private static readonly bool _hasTouchScreen = HasTouchInput();

        #endregion

        static TouchKeyboardProvider()
        {
            var systemDisk = Path.GetPathRoot(System.Environment.GetFolderPath(Environment.SpecialFolder.System));
            if (Environment.Is64BitOperatingSystem)
            {
                _virtualKeyboardPath = Path.Combine(systemDisk, @"Program Files\Common Files", @"Microsoft Shared\ink\TabTip.exe");
            }
            else
            {
                _virtualKeyboardPath = Path.Combine(systemDisk, @"Program Files (x86)\Common Files", @"Microsoft Shared\ink\TabTip.exe");
            }
        }

        #region ITouchKeyboardProvider Methods

        public static void ShowTouchKeyboard(Action<Exception> callback)
        {
            if (!_hasTouchScreen) return;

            try
            {
                if (File.Exists(_virtualKeyboardPath))
                {
                    Process.Start(_virtualKeyboardPath);
                }
            }
            catch (Exception ex)
            {
                callback?.Invoke(ex);
            }
        }

        public static void HideTouchKeyboard()
        {
            if (!_hasTouchScreen) return;

            var nullIntPtr = new IntPtr(0);
            const uint wmSyscommand = 0x0112;
            var scClose = new IntPtr(0xF060);

            var keyboardWnd = FindWindow("IPTip_Main_Window", null);
            if (keyboardWnd != nullIntPtr)
            {
                SendMessage(keyboardWnd, wmSyscommand, scClose, nullIntPtr);
            }
        }

        #endregion

        #region Private: Win32 API Methods

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string sClassName, string sAppName);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Private: Methods

        private static bool HasTouchInput()
        {
            return Tablet.TabletDevices.Cast<TabletDevice>().Any(
                tabletDevice => tabletDevice.Type == TabletDeviceType.Touch);
        }

        #endregion
    }
}
