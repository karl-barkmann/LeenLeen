using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Smart.Native
{
    /// <summary>
    /// user32 API。
    /// </summary>
    public static class User32
    {
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
        public const int SC_MOVE = 0xf010;
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
        public static extern int GetClientRect(IntPtr hwnd, ref RECT lpRect);

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
        public static extern bool GetWindowRect(IntPtr hWnd, [In, Out] ref RECT rect);

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
        public static extern bool UpdateWindow(IntPtr hwnd);

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

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndAfter;
            public int X;
            public int Y;
            public int CX;
            public int CY;
            public uint Flags;
        }
    }
}

