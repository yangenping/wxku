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
using System.Text;
using System.Threading.Tasks;

namespace Sheng.Weixin.Pay
{
    /// <summary>
    /// 查询订单
    /// https://pay.weixin.qq.com/wiki/doc/api/jsapi.php?chapter=9_2
    /// </summary>
    public class WeixinPayOrderQueryArgs
    {
        /// <summary>
        /// 公众账号ID
        /// 微信分配的公众账号ID（企业号corpid即为此appId）
        /// appid
        /// </summary>
        public string AppId
        {
            get;
            set;
        }

        /// <summary>
        /// 商户号
        /// 微信支付分配的商户号
        /// mch_id
        /// </summary>
        public string MchId
        {
            get;
            set;
        }

        //二选一

        /// <summary>
        /// 微信订单号
        /// 微信的订单号，优先使用
        /// transaction_id
        /// </summary>
        public string TransactionId
        {
            get;
            set;
        }

        /// <summary>
        /// 商户订单号
        /// 商户系统内部的订单号，当没提供transaction_id时需要传这个。
        /// out_trade_no
        /// </summary>
        public string OutTradeNo
        {
            get;
            set;
        }

        ///// <summary>
        ///// 随机字符串
        ///// 随机字符串，不长于32位。推荐随机数生成算法
        ///// nonce_str
        ///// </summary>
        //public string NonceStr
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 签名
        ///// 签名，详见签名生成算法
        ///// sign
        ///// </summary>
        //public string Sign
        //{
        //    get;
        //    set;
        //}

    }
}
