using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiXinLib.Model
{
    /// <summary>
    /// 验证数据实体
    /// </summary>
    public class VerifyModel
    {
        private string token = null;

        private string signature = null;

        private string timestamp = null;

        private string nonce = null;

        private string echostr = null;

        private string encryptType = null;

        private string msgSignature = null;

        /// <summary>
        /// 加密类型。
        /// url上无encrypt_type参数或者其值为raw时表示为不加密；
        /// encrypt_type为aes时，表示aes加密（暂时只有raw和aes两种值)。
        /// 公众帐号开发者根据此参数来判断微信公众平台发送的消息是否加密。 
        /// </summary>
        public string EncryptType
        {
            get { return encryptType; }
            set { encryptType = value; }
        }

        /// <summary>
        /// 加密消息体的签名。
        /// 当启用AES加密时，使用此签名。
        /// </summary>
        public string MsgSignature
        {
            get { return msgSignature; }
            set { msgSignature = value; }
        }

        /// <summary>
        /// Token令牌
        /// </summary>
        public string Token
        {
            get { return token; }
            set { token = value; }
        }

        /// <summary>
        /// 微信加密签名。
        /// 明文模式时，使用此签名。
        /// </summary>
        public string Signature
        {
            get { return signature; }
            set { signature = value; }
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        /// <summary>
        /// 随机数
        /// </summary>
        public string Nonce
        {
            get { return nonce; }
            set { nonce = value; }
        }

        /// <summary>
        /// 回音/暗号应答
        /// 当提交申请接入时,与验证有关的参数,会多出一个echostr. 
        /// </summary>
        public string Echostr
        {
            get { return echostr; }
            set { echostr = value; }
        }

    }
}
