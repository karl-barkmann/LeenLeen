using Microsoft.Win32;

namespace Leen.Media
{
    /// <summary>
    /// 媒体渲染组件的工具箱类。
    /// </summary>
    public static class MediaKit
    {
        /// <summary>
        /// 检测本地计算机是否已安装DirectX运行时。
        /// </summary>
        /// <returns></returns>
        public static bool CheckDirectXInstalltion()
        {
            int directxMajorVersion = 0;
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DirectX"))
            {
                if (key == null)
                {
                    return false;
                }

                string versionStr = key.GetValue("Version") as string;
                if (!string.IsNullOrEmpty(versionStr))
                {
                    var versionComponents = versionStr.Split('.');
                    if (versionComponents.Length > 1)
                    {
                        int directXLevel;
                        if (int.TryParse(versionComponents[1], out directXLevel))
                        {
                            directxMajorVersion = directXLevel;
                        }
                    }
                }
            }

            return directxMajorVersion >= 9;
        }
    }
}
