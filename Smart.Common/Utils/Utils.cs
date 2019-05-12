using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Leen.Common.Utils
{
    public static class Utils
    {
        public static string MapPath(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var baseDir = Path.GetDirectoryName(assembly.Location);
            string fullPath = null;
            if (path.StartsWith("~/"))
            {
                fullPath = Path.Combine(baseDir, path.Substring(2, path.Length - 2).Replace('/', '\\'));
            }
            else
            {
                fullPath = path.Replace('/', '\\');
            }
            return fullPath;
        }

        public static void AddEnvironmentPath(IEnumerable<string> searchPath)
        {
            string pathVar = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            IEnumerable<string> args = (from s in searchPath
                                        where !String.IsNullOrEmpty(s)
                                        select MapPath(s));
            pathVar += ";" + String.Join(";", args.ToArray());
            Environment.SetEnvironmentVariable("PATH", pathVar);
        }

        public static void AddEnvironmentPath(string searchPath)
        {
            string pathVar = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            pathVar += ";" + MapPath(searchPath);
            Environment.SetEnvironmentVariable("PATH", pathVar);
        }

        public static bool IsFibonacciNumber(int n)
        {
            double n1 = Math.Pow(n, 2) * 5 + 4;
            double n2 = Math.Pow(n, 2) * 5 - 4;

            double sqrtn1 = Math.Sqrt(n1);
            double sqrtn2 = Math.Sqrt(n2);

            if (Math.Floor(sqrtn1) == sqrtn1 || Math.Floor(sqrtn2) == sqrtn2)
            {
                return true;
            }

            return false;
        }
    }
}
