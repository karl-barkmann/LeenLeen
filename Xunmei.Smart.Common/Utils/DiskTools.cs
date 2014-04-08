using System;
using System.Collections.Generic;
using System.IO;

namespace Xunmei.Smart.Common.Utils
{
    public static class DiskTools
    {
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

