
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using WeiXinLib.Model;

namespace WeiXinLib
{
    public delegate void AccessTokenEventHandler(AccessTokenModel accessTokenModel);

    /// <summary>
    /// 第三方授权令牌处理类(定时获取器)
    /// 每间隔指定的时间，向微信服务器请求授权令牌。
    /// access_token是公众号的全局唯一票据，公众号调用各接口时都需使用access_token。
    /// 正常情况下access_token有效期为7200秒，重复获取将导致上次获取的access_token失效。
    /// 由于获取access_token的api调用次数非常有限，建议开发者全局存储与更新access_token，频繁刷新access_token会导致api调用受限，影响自身业务。
    /// </summary>
    public class AccessTokenTimer
    {
        /// <summary>
        /// 访问令牌事件
        /// </summary>
        public event AccessTokenEventHandler AccessTokenEvent;

        private string appId = null;

        private string appSecret = null;

        private int millisecondsTimeout = 1000 * 7000; //access_token有效期为7200秒

        private int millisecondsErrorContinue = 1000 * 10;//出错后，重新请求的时间间隔。

        private static object lockStart = new object();

        private System.Threading.Thread thread = null;

        private bool enabled = false;

        public bool Enabled
        {
            get { return enabled; }
            //set { enabled = value; }
        }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId
        {
            get { return appId; }
            set { appId = value; }
        }

        /// <summary>
        /// 应用密钥
        /// </summary>
        public string AppSecret
        {
            get { return appSecret; }
            set { appSecret = value; }
        }

        /// <summary>
        /// 授权令牌处理类(定时获取器)构造函数。
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="appSecret">应用密钥</param>
        public AccessTokenTimer(string appId, string appSecret)
        {
            this.appId = appId;
            this.appSecret = appSecret;

        }

        public void Start()
        {
            lock (lockStart)
            {
                if (thread != null)
                {
                    return;
                }
                enabled = true;
            }

            thread = new System.Threading.Thread(
                () =>
                {
                    while (enabled)
                    {
                        try
                        {
                            AccessTokenModel accessTokenModel = SendHandler.GetAccessToken(appId,appSecret);
                            accessTokenModel.appId = appId;
                            string path = System.Web.HttpContext.Current.Server.MapPath("~/Token/token.xml");
                            XDocument x = XDocument.Load(path);
                            x.Root.Element("token").Value = accessTokenModel.access_token;
                            x.Save(path);
                            /**
                             * 为了重用，将AccessToken对象以事件的形式广播出去。
                             * 在这里，保存好AccessToken，以供项目在其他地方使用。
                             * 你可以保存到数据库中，也可以保存到全局变量中。
                             */

                            if (this.AccessTokenEvent != null)
                            {
                                try
                                {
                                    this.AccessTokenEvent(accessTokenModel);
                                }
                                catch (Exception ex)
                                { }
                            }

                            if (accessTokenModel.errcode!=0)
                            {
                                System.Threading.Thread.Sleep(millisecondsErrorContinue);//如果出现异常,X秒后重新请求.

                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            System.Threading.Thread.Sleep(millisecondsErrorContinue);//如果出现异常,X秒后重新请求.

                            continue;
                        }
                        finally
                        {

                        }

                        System.Threading.Thread.Sleep(millisecondsTimeout);
                        
                    }
                });
            thread.Start();
        }

        public void Stop()
        {
            lock (lockStart)
            {
                if (thread != null)
                {
                    enabled = false;
                    thread = null;
                }
            }
        }

    }
}
