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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sheng.Weixin.OfficialAccount.ThirdParty
{
    /// <summary>
    /// 获取第三方平台access_token
    /// </summary>
    [DataContract]
    public class WeixinThirdPartyGetPreAuthCodeResult
    {
        [DataMember(Name = "pre_auth_code")]
        public string PreAuthCode
        {
            get;
            set;
        }

        /// <summary>
        /// 微信API返回的单位是秒
        /// </summary>
        [DataMember(Name = "expires_in")]
        public int ExpiresIn
        {
            get;
            set;
        }
    }
}
