using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeiXinLib.Model;

namespace WeiXinLib
{
    /// <summary>
    /// 微信公众号配置信息管理器。
    /// 也许，你的公众号配置信息原始存储在数据中，也许，是存储于配置文件中。但，无如如何，你都应该在系统启动时，
    /// 将你的公众号的配置信息读出来，构造成ConfigModel对象，然后传给本管理器。如果你在项目中需要管理多个公众账号，请全部构造并传入。
    /// 在这之后，程序都从这里读取账号配置信息，而不从原始存储设备中读取。
    /// </summary>
    public class ConfigManager
    {
        private static List<ConfigModel> configList = new List<ConfigModel>();

        /// <summary>
        /// 获取所有的账号配置信息
        /// </summary>
        /// <returns></returns>
        public static List<ConfigModel> GetConfig()
        {
            return configList;
        }

        /// <summary>
        /// 根据账号appId获取，该账号的配置信息.
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <returns>配置对象</returns>
        public static ConfigModel GetConfig(string appId)
        {
            return configList.FirstOrDefault(row=>row.AppId == appId);
        }

        public static ConfigModel GetConfigFirst()
        {
            return configList.FirstOrDefault();
        }

        /// <summary>
        /// 设置账号配置信息
        /// </summary>
        /// <param name="configModel">配置对象</param>
        /// <returns></returns>
        public static void SetConfig(ConfigModel configModel)
        {
            configList.RemoveAll(row => row.AppId == configModel.AppId);
            configList.Add(configModel);

        }

        /// <summary>
        /// 初始化账号配置信息。
        /// </summary>
        /// <param name="configList">账号列表</param>
        /// <returns></returns>
        public static void InitConfig(List<ConfigModel> configList)
        {
            if (configList == null) 
            { 
                ConfigManager.configList = new List<ConfigModel>();
                return;
            }

            ConfigManager.configList = configList;
        }

        /// <summary>
        /// 设置,更改受权访问令牌
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="access_token">受权访问令牌</param>
        public static void SetAccessToken(string appId,string access_token)
        {
            var config = configList.FirstOrDefault(row => row.AppId == appId);
            config.AccessToken = access_token;
        }

        /// <summary>
        /// 设置,更改JsApiTicket
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="access_token">受权访问令牌</param>
        public static void SetJsApiTicket(string appId, string ticket)
        {
            var config = configList.FirstOrDefault(row => row.AppId == appId);
            config.JsApiTicket = ticket;
        }


        /// <summary>
        /// 设置,更改ApiTicket
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="access_token">受权访问令牌</param>
        public static void SetApiTicket(string appId, string ticket)
        {
            var config = configList.FirstOrDefault(row => row.AppId == appId);
            config.ApiTicket = ticket;
        }
    }
}
