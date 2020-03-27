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
    [DataContract]
    public class WeixinGroupMessageGetStatusResult
    {
        [DataMember(Name = "msg_id")]
        public int MsgId
        {
            get;
            set;
        }

        [DataMember(Name = "msg_status")]
        public string Status
        {
            get;
            set;
        }
    }
}
