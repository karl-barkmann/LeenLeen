using System;
using System.Configuration;
using System.IO;

namespace Leen.Common.Utils
{
    public static class ConfigurationHelper
    {
        private static FileSystemWatcher watcher;

        public static  event FileSystemEventHandler ConfigurationChanged;

        static ConfigurationHelper()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filter = AppDomain.CurrentDomain.FriendlyName + ".config";
            watcher = new FileSystemWatcher(baseDirectory, filter);
            watcher.NotifyFilter = NotifyFilters.Size;
            watcher.Changed += new FileSystemEventHandler(ConfigurationHelper.watcher_Changed);
            watcher.EnableRaisingEvents = true;
        }

        public static ConfigurationSection Load(string configSection)
        {
            return (ConfigurationManager.GetSection(configSection) as ConfigurationSection);
        }

        public static void Save(ConfigurationSection configSection)
        {
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.Sections.Remove(configSection.SectionInformation.Name);
            configuration.Sections.Add(configSection.SectionInformation.Name, configSection);
            configuration.Save();
        }

        private static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if ((e.ChangeType == WatcherChangeTypes.Changed) && (ConfigurationChanged != null))
            {
                ConfigurationChanged(typeof(ConfigurationHelper), e);
            }
        }
    }
}

