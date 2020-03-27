﻿/*
********************************************************************
*
*    曹旭升（sheng.c）
*    E-mail: cao.silhouette@msn.com
*    QQ: 279060597
*    https://github.com/iccb1013
*    http://shengxunwei.com
*
*    © Copyright 2017
*
********************************************************************/


using Linkup.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.Weixin.OfficialAccount
{
    /*
     * http://mp.weixin.qq.com/wiki/11/0e4b294685f817b95cbed85ba5e82b8f.html
     */
    public static class TokenApi
    {
        private static readonly HttpService _httpService = HttpService.Instance;

        /// <summary>
        /// 获取access token
        /// http://mp.weixin.qq.com/wiki/2/88b2bf1265a707c031e51f26ca5e6512.html
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static WeixinRequestApiResult<WeixinAccessTokenResult> GetAccessToken(string appid, string appSecret)
        {
            WeixinRequestApiResult<WeixinAccessTokenResult> result =
                new WeixinRequestApiResult<WeixinAccessTokenResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "GET";
            requestArgs.Url = String.Format(
               "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}",
               appid, appSecret);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Successful)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinAccessTokenResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 独立运行时获取网页授权access token
        /// http://mp.weixin.qq.com/wiki/9/01f711493b5a02f24b04365ac5d8fd95.html
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static WeixinRequestApiResult<WeixinWebAccessTokenResult> GetWebAccessToken(string appid, string appSecret, string code)
        {
            WeixinRequestApiResult<WeixinWebAccessTokenResult> result =
                new WeixinRequestApiResult<WeixinWebAccessTokenResult>();

            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(
                "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code",
               appid, appSecret, code);

            result.HttpRequestResult = _httpService.Request(args);

            if (result.HttpRequestResult.Successful)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinWebAccessTokenResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 作为第三方平台运营时获取网页授权access token
        /// https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&t=resource/res_list&verify=1&id=open1419318590&token=&lang=zh_CN
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static WeixinRequestApiResult<WeixinWebAccessTokenResult> GetThirdPartyWebAccessToken(string appid, string code, string component_appid, string component_access_token)
        {
            WeixinRequestApiResult<WeixinWebAccessTokenResult> result =
                new WeixinRequestApiResult<WeixinWebAccessTokenResult>();

            HttpRequestArgs args = new HttpRequestArgs();
            args.Method = "GET";
            args.Url = String.Format(
                "https://api.weixin.qq.com/sns/oauth2/component/access_token?appid={0}&code={1}&grant_type=authorization_code&component_appid={2}&component_access_token={3}",
               appid, code, component_appid, component_access_token);

            result.HttpRequestResult = _httpService.Request(args);

            if (result.HttpRequestResult.Successful)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinWebAccessTokenResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        public static WeixinRequestApiResult<WeixinGetJsApiTicketResult> GetJsApiTicket(string accessToken)
        {
            WeixinRequestApiResult<WeixinGetJsApiTicketResult> result =
                new WeixinRequestApiResult<WeixinGetJsApiTicketResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "GET";
            requestArgs.Url = String.Format(
               "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi",accessToken);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Successful)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinGetJsApiTicketResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }
    }
}
