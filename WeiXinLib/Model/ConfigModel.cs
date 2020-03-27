using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiXinLib.Model
{
    /// <summary>
    /// 微信公众号配置信息实体类
    /// </summary>
    public class ConfigModel
    {
        private string token;//token(我方令牌)
        //private string accessTokenUrl;//受权令牌获取地址
        private string appId;//应用ID
        private string appSecret;//应用密钥
        private string encodingAESKey;//消息加解密密钥
        private string encodingType;//加密方式
        private string accessToken;//受权令牌

        private string jsApiTicket;//票据 JsApiTicket
        private string apiTicket;//卡券/SDK ApiTicket

        /// <summary>
        /// 票据 JsApiTicket
        /// </summary>
        public string JsApiTicket
        {
            get { return jsApiTicket; }
            set { jsApiTicket = value; }
        }
        
        /// <summary>
        /// 卡券/SDK ApiTicket
        /// </summary>
        public string ApiTicket
        {
            get { return apiTicket; }
            set { apiTicket = value; }
        }
        
        /// <summary>
        /// 加密方式
        /// </summary>
        public string EncodingType
        {
            get { return encodingType; }
            set { encodingType = value; }
        }
        
        /// <summary>
        /// 微信受权访问令牌
        /// </summary>
        public string AccessToken
        {
            get { return accessToken; }
            set { accessToken = value; }
        }

        /// <summary>
        /// 消息加解密密钥
        /// </summary>
        public string EncodingAESKey
        {
            get { return encodingAESKey; }
            set { encodingAESKey = value; }
        }

        /// <summary>
        /// token(我方令牌)
        /// </summary>
        public string Token
        {
            get { return token; }
            set { token = value; }
        }

        ///// <summary>
        ///// 获取授权令牌(AccessToken)的地址
        ///// </summary>
        //public string AccessTokenUrl
        //{
        //    get { return accessTokenUrl; }
        //    set { accessTokenUrl = value; }
        //}

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId
        {
            get { return appId; }
            set { appId = value; }
        }

        /// <summary>
        /// 应用密钥
        /// </summary>
        public string AppSecret
        {
            get { return appSecret; }
            set { appSecret = value; }
        }

    }
}
