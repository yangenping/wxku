using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Web;
namespace WeiXinLib
{
   public  class WXTokenHelper
    {
        #region 请求Url，不发送数据

        /// <summary>
        /// 请求Url，不发送数据
        /// </summary>
        public static string RequestUrl(string url)
        {
            return RequestUrl(url, "POST");
        }

        #endregion

        #region 请求Url，不发送数据

        /// <summary>
        /// 请求Url，不发送数据
        /// </summary>
        public static string RequestUrl(string url, string method)
        {
            // 设置参数
            var request = WebRequest.Create(url) as HttpWebRequest;
            var cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = method;
            request.ContentType = "text/html";
            request.Headers.Add("charset", "utf-8");
            //发送请求并获取相应回应数据
            var response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            var sr = new StreamReader(responseStream, Encoding.UTF8);
            //返回结果网页（html）代码
            string content = sr.ReadToEnd();

            return content;
        }

        #endregion



        #region 获取Json字符串某节点的值

        /// <summary>
        /// 获取Json字符串某节点的值
        /// </summary>
        public static string GetJsonValue(string jsonStr, string key)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(jsonStr))
            {
                key = "\"" + key.Trim('"') + "\"";
                int index = jsonStr.IndexOf(key) + key.Length + 1;
                if (index > key.Length + 1)
                {
                    //先截逗号，若是最后一个，截"｝"号，取最小值
                    int end = jsonStr.IndexOf(',', index);
                    if (end == -1)
                    {
                        end = jsonStr.IndexOf('}', index);
                    }
                    result = jsonStr.Substring(index, end - index);
                    result = result.Trim(new[] { '"', ' ', '\"' }); //过滤引号或空格
                }
            }
            return result;
        }

        #endregion

        #region 验证Token是否过期

        /// <summary>
        /// 验证Token是否过期
        /// </summary>
        public static bool TokenExpired(string access_token)
        {
            string jsonStr =
            RequestUrl(string.Format("https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}",
            access_token));
            if (GetJsonValue(jsonStr, "errcode") == "42001")
            {
                return true;
            }
            return false;
        }

        #endregion

        #region 获取Token

        /// <summary>
        /// 获取Token
        /// </summary>
        public static string GetToken(string appid, string secret)
        {
            string strJson =
            RequestUrl(
            string.Format(
            "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}",
            appid, secret));
            return GetJsonValue(strJson, "access_token");
        }

        #endregion

        //获取Openid

        public static string GetOpenId(string appid, string secret, string code)
        {
            string strJson =
            RequestUrl(
            string.Format(
            "https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code",
            appid, secret, code));
            //LogUtil.WriteLog(strJson);
            return GetJsonValue(strJson, "openid");
        }

        public static string GetTokenBYXml(string path=null)
        {


            if (string.IsNullOrWhiteSpace(path))
            {
                path = "~/Token/token.xml";
                path = HttpContext.Current.Server.MapPath(path);
            }


            XDocument p = XDocument.Load(path);
   
            string token =  p.Root.Element("token").Value;

            if (!TokenExpired(token))
            {
                token = GetToken(ConfigManager.GetConfigFirst().AppId, ConfigManager.GetConfigFirst().AppSecret);
                p.Root.Element("token").Value = token;
                p.Save(path);
            }
            else
            {
                
            }

            return token;



        }
    }
}
