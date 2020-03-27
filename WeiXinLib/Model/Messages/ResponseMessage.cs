using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiXinLib.Model.Messages
{
    /// <summary>
    /// 响应消息实体
    /// </summary>
    public class ResponseMessage :RMessage
    {
        public ResponseMessage()
        {
            
        }

        /// <summary>
        /// (不可空)消息类型 
        /// </summary>
        public ResponseMsgType MsgType { get; set; }

    }

    /// <summary>
    /// 文本消息
    /// </summary>
    public class ResponseText : ResponseMessage
    {
        public ResponseText()
        {
            this.MsgType = ResponseMsgType.Text;
        }

        /// <summary>
        /// (不可空)文本消息内容
        /// </summary>
        public string Content { get; set; }
    }

    /// <summary>
    /// 图片消息
    /// </summary>
    public class ResponseImage : ResponseMessage
    {
        public ResponseImage()
        {
            this.MsgType = ResponseMsgType.Image;
        }

        /// <summary>
        /// (不可空)通过上传多媒体文件，得到的id。  
        /// </summary>
        public string MediaId { get; set; }
    }

    /// <summary>
    /// 语音消息
    /// </summary>
    public class ResponseVoice : ResponseMessage
    {
        public ResponseVoice()
        {
            this.MsgType = ResponseMsgType.Voice;
        }

        /// <summary>
        /// (不可空)通过上传多媒体文件，得到的id 
        /// </summary>
        public string MediaId { get; set; }
        
    }

    /// <summary>
    /// 视频消息
    /// </summary>
    public class ResponseVideo : ResponseMessage
    {
        public ResponseVideo()
        {
            this.MsgType = ResponseMsgType.Video;
        }

        /// <summary>
        /// (不可空)通过上传多媒体文件，得到的id 
        /// </summary>
        public string MediaId { get; set; }
        /// <summary>
        /// 视频消息的标题 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 视频消息的描述 
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// 音乐消息
    /// </summary>
    public class ResponseMusic : ResponseMessage
    {
        public ResponseMusic()
        {
            this.MsgType = ResponseMsgType.Music;
        }

        /// <summary>
        /// 音乐标题  
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 音乐描述 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 音乐链接
        /// </summary>
        public string MusicURL { get; set; }
        /// <summary>
        /// 高质量音乐链接，WIFI环境优先使用该链接播放音乐
        /// </summary>
        public string HQMusicUrl { get; set; }
        /// <summary>
        /// (不可空)缩略图的媒体id，通过上传多媒体文件，得到的id
        /// </summary>
        public string ThumbMediaId { get; set; }
        
    }

    /// <summary>
    /// 图文消息
    /// </summary>
    public class ResponseNews : ResponseMessage
    {
        public ResponseNews()
        {
            this.MsgType = ResponseMsgType.News;
        }

        /// <summary>
        /// (不可空)图文消息个数，限制为10条以内 
        /// </summary>
        public int ArticleCount { get; set; }

        private List<ResponseNewsArticleItem> _articles = null;

        /// <summary>
        /// (不可空)多条图文消息信息，默认第一个item为大图,注意，如果图文数超过10，则将会无响应 
        /// </summary>
        public List<ResponseNewsArticleItem> Articles
        {
            get { return _articles; }
            set {
                if (value != null)
                {
                    ArticleCount = value.Count;
                }
                _articles = value; 
            }
        }

    }

    /// <summary>
    /// 图文消息信息内容
    /// </summary>
    public class ResponseNewsArticleItem
    {
        /// <summary>
        /// 图文消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 图文消息描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200 
        /// </summary>
        public string PicUrl { get; set; }
        /// <summary>
        /// 点击图文消息跳转链接
        /// </summary>
        public string Url { get; set; }
    }

}
