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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.Weixin.OfficialAccount.ThirdParty
{
    /// <summary>
    /// 作为第三方平台运行时的API
    /// https://open.weixin.qq.com/cgi-bin/showdocument?action=dir_list&t=resource/res_list&verify=1&id=open1419318587&lang=zh_CN
    /// </summary>
    public class ThirdPartyApi
    {
        private static readonly HttpService _httpService = HttpService.Instance;

        /// <summary>
        /// 获取第三方平台access_token
        /// </summary>
        public static WeixinRequestApiResult<WeixinThirdPartyGetAccessTokenResult> GetAccessToken(
            WeixinThirdPartyGetAccessTokenArgs args)
        {
            WeixinRequestApiResult<WeixinThirdPartyGetAccessTokenResult> result =
                new WeixinRequestApiResult<WeixinThirdPartyGetAccessTokenResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = "https://api.weixin.qq.com/cgi-bin/component/api_component_token";
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Successful)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinThirdPartyGetAccessTokenResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 获取预授权码
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        public static WeixinRequestApiResult<WeixinThirdPartyGetPreAuthCodeResult> GetPreAuthCode(
            string accessToken, string appid)
        {
            WeixinRequestApiResult<WeixinThirdPartyGetPreAuthCodeResult> result =
                new WeixinRequestApiResult<WeixinThirdPartyGetPreAuthCodeResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/component/api_create_preauthcode?component_access_token={0}",
                accessToken);
            requestArgs.Content = JsonConvert.SerializeObject(new { component_appid = appid });

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Successful)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinThirdPartyGetPreAuthCodeResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 使用授权码换取公众号的授权信息
        /// </summary>
        public static WeixinRequestApiResult<WeixinThirdPartyGetAuthorizationInfoResult> GetAuthorizationInfo(
            string accessToken, WeixinThirdPartyGetAuthorizationInfoArgs args)
        {
            WeixinRequestApiResult<WeixinThirdPartyGetAuthorizationInfoResult> result =
                new WeixinRequestApiResult<WeixinThirdPartyGetAuthorizationInfoResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/component/api_query_auth?component_access_token={0}",
                accessToken);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Successful)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinThirdPartyGetAuthorizationInfoResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 获取（刷新）授权公众号的令牌
        /// </summary>
        public static WeixinRequestApiResult<WeixinThirdPartyGetAuthorizerAccessTokenResult> GetAuthorizerAccessToken(
            string accessToken, WeixinThirdPartyGetAuthorizerAccessTokenArgs args)
        {
            WeixinRequestApiResult<WeixinThirdPartyGetAuthorizerAccessTokenResult> result =
                new WeixinRequestApiResult<WeixinThirdPartyGetAuthorizerAccessTokenResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/component/api_authorizer_token?component_access_token={0}",
                accessToken);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Successful)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinThirdPartyGetAuthorizerAccessTokenResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

        /// <summary>
        /// 获取授权方的账户信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static WeixinRequestApiResult<WeixinThirdPartyGetAuthorizerAccountInfoResult> GetAuthorizerAccountInfo(
            string accessToken, WeixinThirdPartyGetAuthorizerAccountInfoArgs args)
        {
            /*
             * 注意，这个接口官方页面给出的返回JSON有错误
             * qrcode_url 是 authorizer_info 的属性，而不是根级别的属性
             */

            WeixinRequestApiResult<WeixinThirdPartyGetAuthorizerAccountInfoResult> result =
                new WeixinRequestApiResult<WeixinThirdPartyGetAuthorizerAccountInfoResult>();

            HttpRequestArgs requestArgs = new HttpRequestArgs();
            requestArgs.Method = "POST";
            requestArgs.Url = String.Format(
                "https://api.weixin.qq.com/cgi-bin/component/api_get_authorizer_info?component_access_token={0}",
                accessToken);
            requestArgs.Content = JsonConvert.SerializeObject(args);

            result.HttpRequestArgs = requestArgs;
            result.HttpRequestResult = _httpService.Request(requestArgs);

            if (result.HttpRequestResult.Successful)
            {
                result.ApiResult = WeixinApiHelper.Parse<WeixinThirdPartyGetAuthorizerAccountInfoResult>(
                    result.HttpRequestResult.Content, ref result.ApiError);
            }

            return result;
        }

    }
}
