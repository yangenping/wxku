
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WeiXinLib.Model;

namespace WeiXinLib
{
    public delegate void JsApiTicketEventHandler(JsApiTicketModel jsApiTicketModel);

    /// <summary>
    /// JsApiTicket处理类(定时获取器)
    /// 每间隔指定的时间，向微信服务器请求JsApiTicket。
    /// jsapi_ticket是公众号用于调用微信JS接口的临时票据。
    /// 正常情况下，jsapi_ticket的有效期为7200秒，通过access_token来获取。
    /// 由于获取jsapi_ticket的api调用次数非常有限，频繁刷新jsapi_ticket会导致api调用受限，影响自身业务，开发者必须在自己的服务全局缓存jsapi_ticket 。 
    /// </summary>
    public class JsApiTicketTimer
    {
        /// <summary>
        /// JsApiTicket获取事件
        /// </summary>
        public event JsApiTicketEventHandler JsApiTicketEvent;

        private string appId = null;

        //private string accessToken = null;

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

        ///// <summary>
        ///// 访问令牌
        ///// </summary>
        //public string AccessToken
        //{
        //    get { return accessToken; }
        //    set { accessToken = value; }
        //}

        /// <summary>
        /// 授权令牌处理类(定时获取器)构造函数。
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="appSecret">应用密钥</param>
        public JsApiTicketTimer(string appId /*, string accessToken*/)
        {
            this.appId = appId;
            //this.accessToken = accessToken;
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
                            ConfigModel config = ConfigManager.GetConfig(this.appId);
                            JsApiTicketModel jsApiTicketModel = SendHandler.GetJsApiTicket(config.AccessToken);
                            jsApiTicketModel.appId = appId;

                            /**
                             * 为了重用，将JsApiTicket对象以事件的形式广播出去。
                             * 在这里，保存好JsApiTicket，以供项目在其他地方使用。
                             * 你可以保存到数据库中，也可以保存到全局变量中。
                             */

                            if (this.JsApiTicketEvent != null)
                            {
                                try
                                {
                                    this.JsApiTicketEvent(jsApiTicketModel);
                                }
                                catch (Exception ex)
                                { }
                            }

                            if (jsApiTicketModel.errcode!=0)
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
