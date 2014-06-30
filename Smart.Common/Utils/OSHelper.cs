
namespace Smart.Common.Utils
{
    /// <summary>
    /// 操作系统帮助类
    /// </summary>
    public class OSHelper
    {
        /// <summary>
        /// 操作系统是否低于vista
        /// </summary>
        /// <returns>低于vista返回true，否则false</returns>
        public static bool IsLowerVista()
        {
            var osInfo = System.Environment.OSVersion;
            if ((int)osInfo.Platform < 2) return true;
            if ((int)osInfo.Platform == 2 && osInfo.Version.Major < 6)
                return true;

            return false;
        }
    }
}
