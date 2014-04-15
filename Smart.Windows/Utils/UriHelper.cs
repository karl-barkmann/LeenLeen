using System;

namespace Smart.Windows.Utils
{
    /// <summary>
    /// WPF资源文件Uri帮助类。
    /// </summary>
    public class UriHelper
    {
        /// <summary>
        /// 获取执行应用程序程序集中的资源文件Uri路径。
        /// <para>如果你需要一个用于设计界面绑定的资源文件路径，应该使用 <seealso cref="PackReferencedAssemblyUriPath(string,string)"/> 等方法的重载。</para>
        /// </summary>
        /// <param name="resourceFileName">资源文件文件名(包括扩展名)。</param>
        /// <returns></returns>
        public static string PackAppResourceUriPath(string resourceFileName)
        {
            if (String.IsNullOrEmpty(resourceFileName))
                return String.Empty;
            string path = String.Format("/{0}", resourceFileName);
            string uriPath = PackUriPath("siteoforigin", path);
            return uriPath;
        }

        /// <summary>
        /// 获取执行应用程序程序集中的资源文件Uri路径。
        /// <para>如果你需要一个用于设计界面绑定的资源文件路径，应该使用 <seealso cref="PackReferencedAssemblyUriPath(string,string)"/> 等方法的重载。</para>
        /// </summary>
        /// <param name="subFolderPath">子文件夹路径。</param>
        /// <param name="resourceFileName">资源文件文件名(包括扩展名)。</param>
        /// <returns></returns>
        public static string PackAppResourceUriPath(string subFolderPath, string resourceFileName)
        {
            if (String.IsNullOrEmpty(resourceFileName))
                return String.Empty;
            if (String.IsNullOrEmpty(subFolderPath))
                return PackAppResourceUriPath(resourceFileName);

            if (subFolderPath.StartsWith(@"/"))
                subFolderPath = subFolderPath.Remove(0, 1);
            if (subFolderPath.EndsWith(@"/"))
                subFolderPath = subFolderPath.Remove(subFolderPath.Length - 1, 1);

            string path = String.Format("/{0}/{1}", subFolderPath, resourceFileName);
            string uriPath = PackUriPath("siteoforigin", path);
            return uriPath;
        }

        /// <summary>
        /// 获取当前程序集中的资源文件Uri路径。
        /// <para>如果你需要一个用于设计界面绑定的资源文件路径，应该使用 <seealso cref="PackReferencedAssemblyUriPath(string,string)"/> 等方法的重载。</para>
        /// </summary>
        /// <param name="resourceFileName">资源文件文件名(包括扩展名)。</param>
        /// <returns></returns>
        public static string PackLocalAssemblyUriPath(string resourceFileName)
        {
            if (String.IsNullOrEmpty(resourceFileName))
                return String.Empty;

            string path = String.Format("/{0}", resourceFileName);
            string uriPath = PackUriPath("application", path);
            return uriPath;
        }

        /// <summary>
        /// 获取当前程序集中的资源文件Uri路径。
        /// <para>如果你需要一个用于设计界面绑定的资源文件路径，应该使用 <seealso cref="PackReferencedAssemblyUriPath(string,string)"/> 等方法的重载。</para>
        /// </summary>
        /// <param name="subFolderPath">子文件夹路径。</param>
        /// <param name="resourceFileName">资源文件文件名(包括扩展名)。</param>
        /// <returns></returns>
        public static string PackLocalAssemblyUriPath(string subFolderPath, string resourceFileName)
        {
            if (String.IsNullOrEmpty(resourceFileName))
                return String.Empty;
            if (String.IsNullOrEmpty(subFolderPath))
                return PackLocalAssemblyUriPath(resourceFileName);

            if (subFolderPath.StartsWith(@"/"))
                subFolderPath = subFolderPath.Remove(0, 1);
            if (subFolderPath.EndsWith(@"/"))
                subFolderPath = subFolderPath.Remove(subFolderPath.Length - 1, 1);

            string path = String.Format("/{0}/{1}", subFolderPath, resourceFileName);
            string uriPath = PackUriPath("application", path);
            return uriPath;
        }

        /// <summary>
        /// 获取指定程序集中的资源文件Uri路径。
        /// </summary>
        /// <param name="assemblyName">程序集名称。</param>
        /// <param name="resourceFileName">资源文件文件名(包括扩展名)。</param>
        /// <returns></returns>
        public static string PackReferencedAssemblyUriPath(string assemblyName, string resourceFileName)
        {
            if (String.IsNullOrEmpty(resourceFileName) || String.IsNullOrEmpty(assemblyName))
                return String.Empty;

            string path = String.Format("/{0};component/{1}", assemblyName, resourceFileName);
            string uriPath = PackUriPath("application", path);
            return uriPath;
        }

        /// <summary>
        /// 获取指定程序集中的资源文件Uri路径。
        /// </summary>
        /// <param name="assemblyName">程序集名称。</param>
        /// <param name="subFolderPath">子文件夹路径。</param>
        /// <param name="resourceFileName">资源文件文件名(包括扩展名)。</param>
        /// <returns></returns>
        public static string PackReferencedAssemblyUriPath(string assemblyName, string subFolderPath, string resourceFileName)
        {
            if (String.IsNullOrEmpty(resourceFileName))
                return String.Empty;
            if (String.IsNullOrEmpty(subFolderPath))
                return PackReferencedAssemblyUriPath(assemblyName, resourceFileName);

            if (subFolderPath.StartsWith(@"/"))
                subFolderPath = subFolderPath.Remove(0, 1);
            if (subFolderPath.EndsWith(@"/"))
                subFolderPath = subFolderPath.Remove(subFolderPath.Length - 1, 1);

            string path = String.Format("/{0};component/{1}/{2}", assemblyName, subFolderPath, resourceFileName);
            string uriPath = PackUriPath("application", path);
            return uriPath;
        }

        /// <summary>
        /// 获取指定版本的程序集中的资源文件Uri路径。
        /// </summary>
        /// <param name="assemblyName">程序集名称。</param>
        /// <param name="assemblyVersion">程序集版本字符串。
        /// <example>ie:v1.0.0.1</example>
        /// </param>
        /// <param name="subFolderPath">子文件夹路径。</param>
        /// <param name="resourceFileName">资源文件文件名(包括扩展名)。</param>
        /// <returns></returns>
        public static string PackReferencedAssemblyUriPath(string assemblyName, string assemblyVersion, string subFolderPath, string resourceFileName)
        {
            if (String.IsNullOrEmpty(resourceFileName))
                return String.Empty;
            if (String.IsNullOrEmpty(assemblyVersion))
                return PackReferencedAssemblyUriPath(assemblyName, subFolderPath, resourceFileName);
            if (String.IsNullOrEmpty(subFolderPath))
                return PackReferencedAssemblyUriPath(assemblyName, resourceFileName);

            if (subFolderPath.StartsWith(@"/"))
                subFolderPath = subFolderPath.Remove(0, 1);
            if (subFolderPath.EndsWith(@"/"))
                subFolderPath = subFolderPath.Remove(subFolderPath.Length - 1, 1);

            string path = String.Format("/{0};{1};component/{2}/{3}", assemblyName, assemblyVersion, subFolderPath, resourceFileName);
            string uriPath = PackUriPath("application", path);
            return uriPath;
        }

        #region 私有方法

        private static string PackUriPath(string authority, string path)
        {
            return String.Format("pack://{0}:,,,{1}", authority, path);
        }

        #endregion
    }
}
