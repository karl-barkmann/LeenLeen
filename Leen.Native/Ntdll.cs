using System;
using System.Runtime.InteropServices;

namespace Leen.Native
{
    /// <summary>
    /// ntdll.dll P/Invoke native methods.
    /// </summary>
    public static class Ntdll
    {
        /// <summary>
        /// The memcpy function copies count bytes of src to dest. 
        /// </summary>
        /// <param name="dest">New buffer.</param>
        /// <param name="source">Buffer to copy from.</param>
        /// <param name="length">Number of characters to copy.</param>
        /// <returns>returns the value of dest.</returns>
        [DllImport("ntdll.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Memcpy(IntPtr dest, IntPtr source, int length);
    }
}
