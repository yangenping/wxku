using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiXinLib.Model.Messages
{
    /// <summary>
    /// 接收到的消息类型
    /// </summary>
    public enum RequestMsgType
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
        /// 5 地理位置消息
        /// </summary>
        Location,
        /// <summary>
        /// 6 链接消息
        /// </summary>
        Link,
        /// <summary>
        /// 7 事件类型消息
        /// </summary>
        Event

    }

    /// <summary>
    /// 接收到的事件类型
    /// </summary>
    public enum RequestEventType
    {
        /// <summary>
        /// 1 关注(订阅)
        /// 2 非用户扫描二维码时，也会使用这个事件。如果用户还未关注公众号，则用户可以关注公众号，关注后微信会将带场景值关注事件推送给开发者。
        /// </summary>
        Subscribe,
        /// <summary>
        /// 取消关注(取消订阅) 
        /// </summary>
        UnSubscribe,
        /// <summary>
        /// 2 扫描带参数二维码事件
        /// </summary>
        Scan,
        /// <summary>
        /// 3 上报地理位置事件
        /// </summary>
        Location,
        /// <summary>
        /// 4 自定义菜单事件
        /// 5 点击菜单拉取消息时的事件推送
        /// </summary>
        Click,
        /// <summary>
        /// 6 点击菜单跳转链接时的事件推送
        /// </summary>
        View,
        /// <summary>
        /// 模版消息发送任务完成后事件推送
        /// </summary>
        TemplateSendJobFinish

    }

    /// <summary>
    /// 响应的消息类型
    /// </summary>
    public enum ResponseMsgType
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
    /// 接收/响应的消息实体类
    /// </summary>
    public class RMessage
    {
        public RMessage()
        {
            CreateTime = DateTime.Now;
        }

        private string toUserName = null;

        private string fromUserName = null;

        private DateTime createTime ;

        /// <summary>
        /// (不可空)接收方帐号（收到的OpenID）
        /// </summary>
        public string ToUserName
        {
            get { return toUserName; }
            set { toUserName = value; }
        }

        /// <summary>
        /// (不可空)发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName
        {
            get { return fromUserName; }
            set { fromUserName = value; }
        }

        /// <summary>
        /// (不可空)消息创建时间 戳 （整型）
        /// </summary>
        public DateTime CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }

    }
}
