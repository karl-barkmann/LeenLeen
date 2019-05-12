using Microsoft.Win32;
using Leen.Native;
using System;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Leen.Common.Utils
{
    /// <summary>
    /// 提供一组用于注册表访问的帮助方法。
    /// </summary>
    public static class RegistryHelper
    {
        /// <summary>
        /// 检测指定的注册表子节点是否存在。
        /// </summary>
        /// <param name="baseKey">注册表根节点类型。</param>
        /// <param name="subKeyName">要打开的注册表子节点名称。例如：SOFTWARE\DCCS\iSOC</param>
        /// <returns>若存在指定子节点则返回true，否则返回false。</returns>
        public static bool FindIfIsExists(RegistryHive baseKey, string subKeyName)
        {
            var registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey rootKey = RegistryKey.OpenBaseKey(baseKey, registryView))
            {
                using (var subKey = rootKey.OpenSubKey(subKeyName))
                {
                    return subKey != null;
                }
            }
        }

        /// <summary>
        /// 检测指定的注册表项是否存在。
        /// </summary>
        /// <param name="baseKey">注册表根节点类型。</param>
        /// <param name="subKeyName">要打开的注册表子节点名称。例如：SOFTWARE\DCCS\iSOC</param>
        /// <param name="configKeyName">要从 <paramref name="subKeyName"/> 子节点上获取的注册表项名称，
        /// 例如：获取SOFTWARE\DCCS\iSOC\perferedlanguage项，则项的名称为perferedlanguage。
        /// <para>若要获取子节点的默认值，请使用<see cref="string.Empty"/>。</para></param>
        /// <returns>若存在指定注册表项则返回true，否则返回false。</returns>
        public static bool FindIfIsExists(RegistryHive baseKey, string subKeyName, string configKeyName)
        {
            var registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey rootKey = RegistryKey.OpenBaseKey(baseKey, registryView))
            {
                using (var subKey = rootKey.OpenSubKey(subKeyName))
                {
                    if (subKey == null)
                    {
                        return false;
                    }

                    var configValue = subKey.GetValue(configKeyName);
                    return configValue != null;
                }
            }
        }

        /// <summary>
        /// 删除指定的注册表子节点。
        /// </summary>
        /// <param name="baseKey">注册表根节点类型。</param>
        /// <param name="subKeyName">要打开的注册表子节点名称。例如：SOFTWARE\DCCS\iSOC</param>
        /// <param name="throwOnMissingSubKey">尝试删除不存在的子节点时是否引发异常。</param>
        /// <param name="recursive">是否需要以递归方式删除指定的子项和任何子级子项。</param>
        public static void DeleteSubKey(RegistryHive baseKey, string subKeyName, bool throwOnMissingSubKey = false, bool recursive = true)
        {
            var registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey rootKey = RegistryKey.OpenBaseKey(baseKey, registryView))
            {
                if (recursive)
                {
                    rootKey.DeleteSubKeyTree(subKeyName, throwOnMissingSubKey);
                }
                else
                {
                    rootKey.DeleteSubKey(subKeyName, throwOnMissingSubKey);
                }
            }
        }

        /// <summary>
        /// 获取注册表项的值。
        /// <para>当位于64位操作系统上时此方法将以64位视图方式打开注册表。</para>
        /// </summary>
        /// <param name="baseKey">注册表根节点类型。</param>
        /// <param name="subKeyName">要打开的注册表子节点名称。例如：SOFTWARE\DCCS\iSOC</param>
        /// <param name="configKeyName">要从 <paramref name="subKeyName"/> 子节点上获取的注册表项名称，
        /// 例如：获取SOFTWARE\DCCS\iSOC\perferedlanguage项，则项的名称为perferedlanguage。
        /// <para>若要获取子节点的默认值，请使用<see cref="string.Empty"/>。</para></param>
        /// <returns>获取到的注册表项的值。</returns>
        public static object GetConfigValue(RegistryHive baseKey, string subKeyName, string configKeyName)
        {
            var registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey rootKey = RegistryKey.OpenBaseKey(baseKey, registryView))
            {
                using (var subKey = rootKey.OpenSubKey(subKeyName))
                {
                    if (subKey == null)
                    {
                        return null;
                    }

                    return subKey.GetValue(configKeyName, null);
                }
            }
        }

        /// <summary>
        /// 设置注册表项的值，若指定的子节点不存在将创建对应节点。
        /// <para>当位于64位操作系统上时此方法将以64位视图方式打开注册表。</para>
        /// </summary>
        /// <param name="baseKey">注册表根节点类型。</param>
        /// <param name="subKeyName">要打开的注册表子节点名称。例如：SOFTWARE\Microsoft\Windows</param>
        /// <param name="configKeyName">要从 <paramref name="subKeyName"/> 子节点上设置的注册表项名称。
        /// 例如：设置SOFTWARE\DCCS\iSOC\perferedlanguage项，则项的名称为perferedlanguage。
        /// <para>若要设置子节点的默认值，请使用<see cref="string.Empty"/>。</para></param>
        /// <param name="configVal">要设置的注册表项的值。</param>
        public static void SetConfigValue(RegistryHive baseKey, string subKeyName, string configKeyName, object configVal)
        {
            var registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey rootKey = RegistryKey.OpenBaseKey(baseKey, registryView))
            {
                using (var subKey = rootKey.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.SetValue))
                {
                    if (subKey == null)
                    {
                        using (var createdSubKey = rootKey.CreateSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree))
                        {
                            createdSubKey.SetValue(configKeyName, configVal);
                        }
                    }
                    else
                    {
                        subKey.SetValue(configKeyName, configVal);
                    }
                }
            }
        }

        /// <summary>
        /// 删除注册表项。
        /// </summary>
        /// <param name="baseKey">注册表根节点类型。</param>
        /// <param name="subKeyName">要打开的注册表子节点名称。例如：SOFTWARE\Microsoft\Windows</param>
        /// <param name="configKeyName">要从 <paramref name="subKeyName"/> 子节点上设置的注册表项名称。
        /// 例如：设置SOFTWARE\DCCS\iSOC\perferedlanguage项，则项的名称为perferedlanguage。
        /// <para>若要设置子节点的默认值，请使用<see cref="string.Empty"/>。</para></param>
        public static void DeleteConfig(RegistryHive baseKey, string subKeyName, string configKeyName)
        {
            var registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey rootKey = RegistryKey.OpenBaseKey(baseKey, registryView))
            {
                using (var subKey = rootKey.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.SetValue))
                {
                    if (subKey == null)
                    {
                        return;
                    }

                    subKey.DeleteValue(configKeyName);
                }
            }
        }

        /// <summary>
        /// 为文件类型设置默认应用程序。
        /// </summary>
        /// <param name="appPath">打开文件的默认应用程序路径。</param>
        /// <param name="extension">要设置文件的扩展名。</param>
        /// <param name="programId">打开文件的默认应用程序名称。</param>
        /// <param name="description">打开文件的默认应用程序描述。</param>
        /// <returns></returns>
        public static bool SetDefaultProgram(string appPath, string extension, string programId, string description)
        {
            try
            {
                //1 设置自定义文件的双击打开
                SetConfigValue(RegistryHive.ClassesRoot, programId, string.Empty, description);
                SetConfigValue(RegistryHive.ClassesRoot, $"{programId}\\shell\\open\\command", string.Empty, $"{appPath} \"%1\"");
                //2 设置自定义文件的默认图标 
                SetConfigValue(RegistryHive.ClassesRoot, $"{programId}\\DefaultIcon", string.Empty, $"{appPath},0");

                //3 新增的扩展名和设置关联
                DeleteSubKey(RegistryHive.ClassesRoot, extension);
                SetConfigValue(RegistryHive.ClassesRoot, extension, string.Empty, programId);

                //判断 UserChoice是否存在
                var subKeyName = @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + extension + @"\UserChoice";
                if (FindIfIsExists(RegistryHive.CurrentUser, subKeyName))
                {
                    if (RightsControl(extension))
                    {
                        SetConfigValue(RegistryHive.CurrentUser, subKeyName, string.Empty, programId);
                    }
                    else
                    {
                        return false;
                    }
                }
                //通知系统，文件关联已经是作用，不然可能要等到系统重启才看到效果
                Shell32.SHChangeNotify(Shell32.HChangeNotifyEventID.SHCNE_ASSOCCHANGED, Shell32.HChangeNotifyFlags.SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool RightsControl(string subKeyName)
        {
            try
            {
                NTAccount name = new NTAccount(Environment.UserDomainName, Environment.UserName);
                RegistryKey keyUser = Registry.CurrentUser;
                keyUser = keyUser.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions);
                RegistrySecurity rs = keyUser.GetAccessControl();
                RegistryAccessRule denyRule = rs.GetAccessRules(true, false, typeof(NTAccount))
                                                .OfType<RegistryAccessRule>()
                                                .FirstOrDefault(r => r.AccessControlType == AccessControlType.Deny && (r.IdentityReference as NTAccount) == name);

                if (denyRule != null && rs.RemoveAccessRule(denyRule))
                {
                    keyUser.SetAccessControl(rs);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
