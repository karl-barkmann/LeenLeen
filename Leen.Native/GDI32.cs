using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Leen.Native
{
    /// <summary>
    /// gdi32 API。
    /// </summary>
    public static class GDI32
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);
        
        [DllImport("gdi32.dll")]
        public static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
        
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);
        
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDCA([MarshalAs(UnmanagedType.LPStr)] string lpszDriver, [MarshalAs(UnmanagedType.LPStr)] string lpszDevice, [MarshalAs(UnmanagedType.LPStr)] string lpszOutput, int lpInitData);
        
        [DllImport("gdi32.dll")]
        public static extern int DeleteDC(IntPtr hdc);
        
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [StructLayout(LayoutKind.Explicit)]
        public struct RGB
        {
            [FieldOffset(2)]
            public byte Blue;

            [FieldOffset(1)]
            public byte Green;

            [FieldOffset(0)]
            public byte Red;

            public static GDI32.RGB FromArgb(Color color)
            {
                return new GDI32.RGB { Red = color.R, Green = color.G, Blue = color.B };
            }

            public int ToInt()
            {
                return ((this.Red + (this.Green * 0x100)) + (this.Blue * 0x10000));
            }
        }
    }
}

