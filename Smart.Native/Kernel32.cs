using System.Runtime.InteropServices;

namespace Smart.Native
{
    /// <summary>
    /// kernel32 API。
    /// </summary>
    public static class Kernel32
    {
        public const uint SEM_FAILCRITICALERRORS = 1;
        public const uint SEM_NOALIGNMENTFAULTEXCEPT = 4;
        public const uint SEM_NOGPFAULTERRORBOX = 2;
        public const uint SEM_NOOPENFILEERRORBOX = 0x8000;

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        public static extern uint GetErrorMode();

        [DllImport("kernel32.dll")]
        public static extern int GetThreadErrorMode();

        [DllImport("kernel32.dll")]
        public static extern uint SetErrorMode(uint mode);

        [DllImport("kernel32.dll")]
        public static extern bool SetThreadErrorMode(int newMode, out int oldMode);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetDllDirectory(string lpPathName);
    }
}

