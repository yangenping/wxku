using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeiXinLib.Model.Messages;
using Newtonsoft.Json;

namespace WeiXinLib.Model.Send
{
    /// <summary>
    /// 文本消息
    /// </summary>
    public class TextMessage : SMessage
    {
        public TextMessage()
        {
            this.MsgType = SendMsgType.Text;
        }

        /// <summary>
        /// 文本对象
        /// </summary>
        public Text text { get; set; }
    }

    /// <summary>
    /// 文本内容类
    /// </summary>
    public class Text
    {
        public string content { get; set; }
    }

    /// <summary>
    /// 图片消息
    /// </summary>
    public class ImageMessage : SMessage
    {
        public ImageMessage()
        {
            this.MsgType = SendMsgType.Image;
        }

        /// <summary>
        /// 图片对象
        /// </summary>
        public Image image { get; set; }
    }

    /// <summary>
    /// 图片内容类
    /// </summary>
    public class Image
    {
        /// <summary>
        /// 图片ID
        /// </summary>
        public string media_id { get; set; }
    }

    /// <summary>
    /// 图文消息
    /// </summary>
    public class NewsMessage : SMessage
    {
        public NewsMessage()
        {
            this.MsgType = SendMsgType.News;
        }

        /// <summary>
        /// 图文对象
        /// </summary>
        public News news { get; set; }
    }
    
    /// <summary>
    /// 图文内容类
    /// </summary>
    public class News
    {
        /// <summary>
        /// 文章列表
        /// </summary>
        public List<Article> articles { get; set; }
    }

    /// <summary>
    /// 文章内容类
    /// </summary>
    public class Article
    {
        /// <summary>
        /// 标题 
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 点击后跳转的链接
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 图文消息的图片链接，支持JPG、PNG格式，较好的效果为大图640*320，小图80*80 
        /// </summary>
        public string picurl { get; set; }
    }


    /// <summary>
    /// 语音消息
    /// </summary>
    public class VoiceMessage : SMessage
    {
        public VoiceMessage()
        {
            this.MsgType = SendMsgType.Voice;
        }

        /// <summary>
        /// 语音对象
        /// </summary>
        public Voice voice { get; set; }
    }

    /// <summary>
    /// 语音内容类
    /// </summary>
    public class Voice
    {
        /// <summary>
        /// 语音ID
        /// </summary>
        public string media_id { get; set; }
    }
    
    /// <summary>
    /// 模板消息实体类
    /// </summary>
    public class TemplateMessage
    {
        /// <summary>
        /// 接收方帐号（一个OpenID）
        /// </summary>
        public string touser { get; set; }
        
        /// <summary>
        /// 授权令牌
        /// </summary>
        [JsonIgnore]
        public string AccessToken { get; set; }
        
        public string template_id { get; set; }

        public string url { get; set; }

        public Dictionary<string, TemplateItem> data { get; set; }
    }

    /// <summary>
    /// 模板节点实体类
    /// </summary>
    public class TemplateItem 
    {
        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 值颜色
        /// </summary>
        public string color { get; set; }
    }
}
