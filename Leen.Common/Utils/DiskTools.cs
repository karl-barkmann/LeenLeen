using System;
using System.Collections.Generic;
using System.IO;

namespace Leen.Common.Utils
{
    public static class DiskTools
    {
        public static string GetFriendlySize(long size)
        {
            if (size >= 0x10000000000L)
            {
                return string.Format("{0:F}TB", size / 1.099512E+12f);
            }
            if (size >= 0x40000000L)
            {
                return string.Format("{0:F}GB", size / 1.073742E+09f);
            }
            if (size >= 0x100000L)
            {
                return string.Format("{0:F}MB", size / 1048576f);
            }
            if (size >= 0x400L)
            {
                return string.Format("{0:F}KB", size / 1024f);
            }
            return (size.ToString() + "B");
        }

        public static long GetAllAvailableFreeSpace()
        {
            long num = 0L;
            foreach (DriveInfo info in GetAvailableDrives())
            {
                num += info.AvailableFreeSpace;
            }
            return num;
        }

        public static long GetAllDiskTotalSize()
        {
            long num = 0L;
            foreach (DriveInfo info in GetAvailableDrives())
            {
                num += info.TotalSize;
            }
            return num;
        }

        public static DriveInfo[] GetAvailableDrives()
        {
            return GetAvailableDrives(null);
        }

        public static DriveInfo[] GetAvailableDrives(string[] excludeDrivers)
        {
            List<DriveInfo> list = new List<DriveInfo>();
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                bool flag = false;
                foreach (DriveInfo info in drives)
                {
                    if ((info.DriveType == DriveType.Fixed) || (info.DriveType == DriveType.Network))
                    {
                        if (excludeDrivers != null)
                        {
                            flag = false;
                            foreach (string str in excludeDrivers)
                            {
                                if (info.Name == str)
                                {
                                    flag = true;
                                }
                            }
                        }
                        if (!flag)
                        {
                            list.Add(info);
                        }
                    }
                }
            }
            catch (IOException)
            {
            }
            return list.ToArray();
        }

        public static long GetAvailableFreeSpace(string driveName)
        {
            long availableFreeSpace = 0L;
            try
            {
                DriveInfo info = new DriveInfo(driveName);
                availableFreeSpace = info.AvailableFreeSpace;
            }
            catch (Exception)
            {
            }
            return availableFreeSpace;
        }

        public static string GetDriveLetter(string path)
        {
            if (path == null)
            {
                return null;
            }
            int index = path.IndexOf(Path.VolumeSeparatorChar);
            if (index == -1)
            {
                return null;
            }
            return (path.Substring(0, index + 1) + Path.DirectorySeparatorChar);
        }

        public static long GetTotalSize(string driveName)
        {
            long totalSize = 0L;
            try
            {
                DriveInfo info = new DriveInfo(driveName);
                totalSize = info.TotalSize;
            }
            catch (IOException)
            {
            }
            return totalSize;
        }

        public static bool IsSystemPartition(string driveName)
        {
            return Environment.SystemDirectory.StartsWith(driveName);
        }

        public static string SystemPartition
        {
            get
            {
                return GetDriveLetter(Environment.SystemDirectory);
            }
        }
    }
}

