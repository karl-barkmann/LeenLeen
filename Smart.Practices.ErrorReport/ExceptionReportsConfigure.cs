using System.Configuration;

namespace Smart.Practices.ErrorReport
{
    /// <summary>
    /// 错误报告配置。
    /// </summary>
    public class ExceptionReportsConfigure : ConfigurationSection
    {
        /// <summary>
        /// 构造错误报告配置的实例。
        /// </summary>
        public ExceptionReportsConfigure()
        {
            AutoRestart = true;
            Folder = "";
            ReportDialogCloseSecond = 30;
            ReportServiceAddress = "services.xunmei.com";
            ReportServiceUrl = "http://{0}/Reports/v1.asmx";
            ShowReportDialog = true;
        }

        /// <summary>
        /// 获取或设置一个值，指示是否自动重启应用程序。
        /// </summary>
        [ConfigurationProperty("autoRestart", DefaultValue = true)]
        public bool AutoRestart
        {
            get
            {
                return (bool)base["autoRestart"];
            }
            set
            {
                base["autoRestart"] = value;
            }
        }

        /// <summary>
        /// 获取或设置错误报告保存路径。
        /// </summary>
        [ConfigurationProperty("folder", DefaultValue = "")]
        public string Folder
        {
            get
            {
                return (string)base["folder"];
            }
            set
            {
                base["folder"] = value;
            }
        }

        /// <summary>
        /// 获取或设置自动关闭时间。
        /// </summary>
        [IntegerValidator(MinValue = 0, MaxValue = 180), ConfigurationProperty("reportDialogCloseSecond", DefaultValue = 30)]
        public int ReportDialogCloseSecond
        {
            get
            {
                return (int)base["reportDialogCloseSecond"];
            }
            set
            {
                base["reportDialogCloseSecond"] = value;
            }
        }

        /// <summary>
        /// 获取或设置报告服务器地址。
        /// </summary>
        [ConfigurationProperty("reportServiceAddress", DefaultValue = "services.xunmei.com")]
        public string ReportServiceAddress
        {
            get
            {
                return (string)base["reportServiceAddress"];
            }
            set
            {
                base["reportServiceAddress"] = value;
            }
        }

        /// <summary>
        /// 获取或设置报告服务路径。
        /// </summary>
        [ConfigurationProperty("reportServiceUrl", DefaultValue = "http://{0}/Reports/v1.asmx")]
        public string ReportServiceUrl
        {
            get
            {
                return (string)base["reportServiceUrl"];
            }
            set
            {
                base["reportServiceUrl"] = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示是否发送错误报告到报告服务。
        /// </summary>
        [ConfigurationProperty("sendReport", DefaultValue = false)]
        public bool SendReport
        {
            get
            {
                return (bool)base["sendReport"];
            }
            set
            {
                base["sendReport"] = value;
            }
        }

        /// <summary>
        /// 获取设置一个值，指示是否显示错误报告窗口。
        /// </summary>
        [ConfigurationProperty("showReportDialog", DefaultValue = true)]
        public bool ShowReportDialog
        {
            get
            {
                return (bool)base["showReportDialog"];
            }
            set
            {
                base["showReportDialog"] = value;
            }
        }
    }
}

