using System;
using System.Configuration;
using System.IO;

namespace Leen.Common.Utils
{
    /// <summary>
    /// 应用程序配置帮助类。
    /// </summary>
    public static class ConfigurationHelper
    {
        private static readonly FileSystemWatcher watcher = new FileSystemWatcher(
            AppDomain.CurrentDomain.BaseDirectory,
            AppDomain.CurrentDomain.FriendlyName + ".config");

        /// <summary>
        /// 当配置项发生变化时引发。
        /// </summary>
        public static event FileSystemEventHandler ConfigurationChanged;

        static ConfigurationHelper()
        {
            watcher.NotifyFilter = NotifyFilters.Size;
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 加载应用程序配置节点。
        /// </summary>
        /// <param name="configSection">配置节点名称。</param>
        /// <returns></returns>
        public static ConfigurationSection Load(string configSection)
        {
            return (ConfigurationManager.GetSection(configSection) as ConfigurationSection);
        }

        /// <summary>
        /// 保存应用程序配置节点。
        /// </summary>
        /// <param name="configSection">配置节点。</param>
        public static void Save(ConfigurationSection configSection)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.Sections.Remove(configSection.SectionInformation.Name);
            configuration.Sections.Add(configSection.SectionInformation.Name, configSection);
            configuration.Save();
        }

        private static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                ConfigurationChanged?.Invoke(typeof(ConfigurationHelper), e);
            }
        }
    }
}

