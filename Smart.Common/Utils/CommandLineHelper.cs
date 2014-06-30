using System;
using System.Collections.Generic;

namespace Smart.Common.Utils
{   
    /// <summary>
    /// 命令行解析帮助者。
    /// </summary>
    /// <remarks>
    /// '-' - 匹配命令
    /// ' ' - 匹配参数
    /// ie. -debug launch
    /// </remarks>
    public static class CommandLineHelper
    {
        /// <summary>
        /// 获取用户命令列表。
        /// </summary>
        /// <param name="args">命令行参数。</param>
        /// <returns></returns>
        public static IEnumerable<CommandLine> GetCommands(string[] args)
        {
            List<CommandLine> commands = new List<CommandLine>();

            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string verb = args[i];
                    if (verb.IndexOf(("-")) == 0)
                    {
                        CommandLine command = new CommandLine(verb, String.Empty);
                        if (args[i + 1].IndexOf(("-")) != 0)
                        {
                            command.Parameter = args[i + 1];
                            i++;
                        }
                        commands.Add(command);
                    }
                }
            }

            return commands;
        }

        /// <summary>
        /// 获取用户命令。
        /// </summary>
        /// <param name="args">命令行参数。</param>
        /// <param name="verb">命令代号。</param>
        /// <returns></returns>
        public static CommandLine GetCommand(string[] args, string verb)
        {
            CommandLine command = null;

            if (args != null && args.Length > 0)
            {
                if (verb.IndexOf('-') != 0)
                    verb = "-" + verb;

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == verb)
                    {
                        command = new CommandLine(verb, String.Empty);
                        if (args[i + 1].IndexOf(("-")) != 0)
                        {
                            command.Parameter = args[i + 1];
                        }
                        break;
                    }
                }
            }

            return command;
        }

        /// <summary>
        /// 代表一条用户命令。
        /// </summary>
        public class CommandLine
        {
            /// <summary>
            /// 构造用户命令对象的实例。
            /// </summary>
            /// <param name="verb">命令代号。</param>
            /// <param name="parameter">命令参数。</param>
            public CommandLine(string verb, string parameter)
            {
                Verb = verb;
                Parameter = parameter;
            }

            /// <summary>
            /// 命令。
            /// </summary>
            public string Verb { get; set; }

            /// <summary>
            /// 命令参数。
            /// </summary>
            public string Parameter { get; set; }
        }
    }
}
