using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeiXinLib.Model;
using Tencent;

namespace WeiXinLib
{
    /// <summary>
    /// 微信通信消息加解密类
    /// </summary>
    public class MessageCrypt
    {
        /// <summary>
        /// 表示不加密
        /// </summary>
        public const string ENCRYPT_TYPE_RAW = "raw";

        /// <summary>
        /// 表示aes加密
        /// </summary>
        public const string ENCRYPT_TYPE_AES = "aes";

        /// <summary>
        /// 配置信息
        /// </summary>
        public ConfigModel configModel { get; set; }

        //构造函数
	    // @param sToken: 公众平台上，开发者设置的Token
	    // @param sEncodingAESKey: 公众平台上，开发者设置的EncodingAESKey
	    // @param sAppID: 公众帐号的appid
        public MessageCrypt(ConfigModel configModel)
        {
            this.configModel = configModel;
        }

        /// <summary>
        /// 检验消息的真实性，并且获取解密后的明文
        /// </summary>
        /// <param name="sMsgSignature">签名串，对应URL参数的msg_signature</param>
        /// <param name="sTimeStamp">时间戳，对应URL参数的timestamp</param>
        /// <param name="sNonce">随机串，对应URL参数的nonce</param>
        /// <param name="sPostData">密文，对应POST请求的数据</param>
        /// <returns>返回解密后的明文</returns>
        public string DecryptMsg(string sMsgSignature, string sTimeStamp, string sNonce, string sPostData)
        {
            WXBizMsgCrypt wXBizMsgCrypt = new WXBizMsgCrypt(configModel.Token,configModel.EncodingAESKey,configModel.AppId);

            string sMsg = null;

            wXBizMsgCrypt.DecryptMsg(sMsgSignature, sTimeStamp, sNonce, sPostData, ref sMsg);

            return sMsg;
        }

        /// <summary>
        /// 检验消息的真实性，并且获取解密后的明文
        /// </summary>
        /// <param name="verifyModel">验证对象</param>
        /// <param name="sPostData">密文数据</param>
        /// <returns>解密后的明文</returns>
        public string DecryptMsg(VerifyModel verifyModel, string sPostData)
        {
            return DecryptMsg(verifyModel.MsgSignature, verifyModel.Timestamp, verifyModel.Nonce,sPostData);
        }

        /// <summary>
        /// 将企业号回复用户的消息加密打包
        /// </summary>
        /// <param name="sReplyMsg">企业号待回复用户的消息，xml格式的字符串</param>
        /// <param name="sTimeStamp">时间戳，可以自己生成，也可以用URL参数的timestamp</param>
        /// <param name="sNonce">随机串，可以自己生成，也可以用URL参数的nonce</param>
        /// <returns>返回加密后的可以直接回复用户的密文，包括msg_signature, timestamp, nonce, encrypt的xml格式的字符串</returns>
        public string EncryptMsg(string sReplyMsg, string sTimeStamp, string sNonce)
        {
            WXBizMsgCrypt wXBizMsgCrypt = new WXBizMsgCrypt(configModel.Token, configModel.EncodingAESKey, configModel.AppId);

            string sEncryptMsg = null;

            wXBizMsgCrypt.EncryptMsg(sReplyMsg, sTimeStamp, sNonce, ref sEncryptMsg);

            return sEncryptMsg;

        }

        /// <summary>
        /// 将回复消息加密打包
        /// </summary>
        /// <param name="verifyModel">验证对象</param>
        /// <param name="sReplyMsg">回复消息</param>
        /// <returns>加密后的密文</returns>
        public string EncryptMsg(VerifyModel verifyModel, string sReplyMsg)
        {
            return EncryptMsg(sReplyMsg,verifyModel.Timestamp,verifyModel.Nonce);
        }
    }
}
