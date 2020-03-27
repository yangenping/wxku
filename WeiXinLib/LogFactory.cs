using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinLib
{
  
    public static class LogFactory
    {
        /// <summary>
        /// 解析Xml的路径
        /// </summary>
        public const string Config = "~/config/log4net.config";

        /// <summary>
        /// 错误日志的标签
        /// </summary>
        public const string Error = "ErrorLogger";

        /// <summary>
        /// 日志记录的标签
        /// </summary>
        public const string Info = "InfoLogger";

        /// <summary>
        /// 初始化
        /// </summary>
        public static void LogInitialize()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(System.Web.HttpContext.Current.Server.MapPath(Config)));
        }

        public static log4net.ILog GetLog(Type t)
        {
             return log4net.LogManager.GetLogger(t);
        }

        public static log4net.ILog GetLogInfo()
        {
            return log4net.LogManager.GetLogger(Info);
        }



        public static log4net.ILog GetLogErr()
        {
            return log4net.LogManager.GetLogger(Error);
        }
    }
}
