using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WeiXinLib.Model.Messages
{
    /// <summary>
    /// 发送的消息类型
    /// </summary>
    public enum SendMsgType
    {
        /// <summary>
        /// 1 文本消息
        /// </summary>
        Text,
        /// <summary>
        /// 2 图片消息
        /// </summary>
        Image,
        /// <summary>
        /// 3 语音消息
        /// </summary>
        Voice,
        /// <summary>
        /// 4 视频消息
        /// </summary>
        Video,
        /// <summary>
        /// 5 音乐消息
        /// </summary>
        Music,
        /// <summary>
        /// 6 图文消息
        /// </summary>
        News
        
    }

    /// <summary>
    /// 发送消息实体类
    /// </summary>
    public class SMessage
    {
        public SMessage()
        {
            
        }

        private string _touser = null;

        private SendMsgType _msgtype ;

        private string accessToken = null;

        /// <summary>
        /// 接收方帐号（一个OpenID）
        /// </summary>
        public string touser
        {
            get { return _touser; }
            set { _touser = value; }
        }

        /// <summary>
        /// 发送消息的类型
        /// </summary>
        [JsonIgnore]
        public SendMsgType MsgType
        {
            get { return _msgtype; }
            set { _msgtype = value; }
        }

        public string msgtype
        {
            get { return _msgtype.ToString().ToLower(); }
            set 
            {
                _msgtype = (SendMsgType)Enum.Parse(typeof(SendMsgType), value); 
            }
        }

        /// <summary>
        /// 授权令牌
        /// </summary>
        [JsonIgnore]
        public string AccessToken
        {
            get { return accessToken; }
            set { accessToken = value; }
        }

    }
}
