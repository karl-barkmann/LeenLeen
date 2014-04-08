using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunmei.Smart.Native;

namespace Demo
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(NativeMethodsUtil.IsAnyUserLogon() ? "有用户登陆" : "无用户登陆");
            Console.ReadLine();
        }
    }
}
