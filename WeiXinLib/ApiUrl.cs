using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiXinLib
{
    /// <summary>
    /// 微信第三方接口地址。
    /// </summary>
    public class ApiUrl
    {
        /// <summary>
        /// 发送客服消息的URL接口
        /// http请求方式: POST
        /// </summary>
        public const string URL_SEND_CUSTOM = @"https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=";

        /// <summary>
        /// 自定义菜单创建接口 URL
        /// POST（请使用https协议）
        /// </summary>
        public const string URL_CREATE_MENU = @"https://api.weixin.qq.com/cgi-bin/menu/create?access_token=";

        /// <summary>
        /// 获取access token 的URL接口
        /// </summary>
        public const string URL_GET_ACCESS_TOKEN = @"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";

        /// <summary>
        /// 获取用户基本信息（包括UnionID机制）的URL接口
        /// </summary>
        public const string URL_GET_USER_INFO =  @"https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang={2}";

        /// <summary>
        ///  批量获取用户基本信息。开发者可通过该接口来批量获取用户基本信息。最多支持一次拉取100条。 
        /// </summary>
        public const string URL_BATCH_GET_USER_INFO = @"https://api.weixin.qq.com/cgi-bin/user/info/batchget?access_token={0}";

        /// <summary>
        /// 获取接口票据Ticket的URL接口
        /// </summary>
        public const string URL_GET_TICKET = @"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type={1}";

        /// <summary>
        /// 创建二维码的URL接口
        /// </summary>
        public const string URL_CREATE_QR = @"https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}";

        /// <summary>
        /// 发送模板消息URL
        /// </summary>
        public const string URL_SEND_TEMPLATE = @"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";

        /// <summary>
        /// 创建发送客服消息的URL接口
        /// </summary>
        /// <param name="accessToken">授权访问令牌</param>
        /// <returns></returns>
        public static string GetSendCustomUrl(string accessToken)
        {
            return URL_SEND_CUSTOM + accessToken;
        }

        /// <summary>
        /// 创建生成菜单的接口URL
        /// </summary>
        /// <param name="accessToken">授权访问令牌</param>
        /// <returns></returns>
        public static string GetCreateMenuUrl(string accessToken)
        {
            return URL_CREATE_MENU + accessToken;
        }

        /// <summary>
        /// 获取access token 接口的URL.
        /// http请求方式: GET
        /// </summary>
        /// <param name="appId">应用ID，第三方用户唯一凭证 </param>
        /// <param name="appSecret">应用密钥，第三方用户唯一凭证密钥，即appsecret </param>
        /// <returns></returns>
        public static string GetAccessTokenUrl(string appId, string appSecret)
        {
            return string.Format(URL_GET_ACCESS_TOKEN, appId, appSecret);
        }

        /// <summary>
        /// 获取用户基本信息（包括UnionID机制）的URL接口
        /// </summary>
        /// <param name="accessToken">授权访问令牌</param>
        /// <param name="appId">应用ID</param>
        /// <param name="lang">以哪种语言返回结果(返回国家地区语言版本，zh_CN 简体，zh_TW 繁体，en 英语 )</param>
        /// <returns></returns>
        public static string GetUserInfoUrl(string accessToken, string openid, string lang = Constants.LANG_ZH_CN)
        {
            return string.Format(URL_GET_USER_INFO, accessToken, openid, lang);
        }

        /// <summary>
        /// 批量获取用户基本信息。开发者可通过该接口来批量获取用户基本信息。最多支持一次拉取100条。 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string BatchGetUserInfoUrl(string accessToken)
        {
            return string.Format(URL_BATCH_GET_USER_INFO, accessToken);
        }

        /// <summary>
        /// 获取JsApi Ticket 票据接口的URL。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string GetJsApiTicketUrl(string accessToken)
        {
            return string.Format(URL_GET_TICKET, accessToken, "jsapi");
        }

        /// <summary>
        /// 获取Api Ticket 票据(卡券)接口的URL。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string GetApiTicketUrl(string accessToken)
        {
            return string.Format(URL_GET_TICKET, accessToken, "wx_card");
        }

        /// <summary>
        /// 获取创建二维码接口的URL。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string GetCreateQrUrl(string accessToken)
        {
            return string.Format(URL_CREATE_QR, accessToken);
        }

        /// <summary>
        /// 获取发送模板消息URL
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string GetSendTemplateUrl(string accessToken)
        {
            return string.Format(URL_SEND_TEMPLATE, accessToken);
        }

    }
}
