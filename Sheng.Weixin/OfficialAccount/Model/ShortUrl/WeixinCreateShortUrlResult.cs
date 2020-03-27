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

namespace Sheng.Weixin.OfficialAccount
{
    /// <summary>
    /// 将一条长链接转成短链接。
    /// </summary>
    [DataContract]
    public class WeixinCreateShortUrlResult
    {
        [DataMember(Name = "short_url")]
        public string ShortUrl
        {
            get;
            set;
        }
    }
}
