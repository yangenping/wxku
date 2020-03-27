using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiXinLib.Model.Messages
{
    /// <summary>
    /// 接收消息实体
    /// </summary>
    public class RequestMessage : RMessage
    {
        /// <summary>
        /// 信息类型
        /// </summary>
        public RequestMsgType MsgType { get; set; }

    }

    /// <summary>
    /// 普通消息实体
    /// </summary>
    public class RequestGeneralMessage : RequestMessage
    {
        /// <summary>
        /// 消息id，64位整型 
        /// </summary>
        public string MsgId { get; set; }
    }

    /// <summary>
    /// 文本消息
    /// </summary>
    public class RequestText : RequestGeneralMessage
    {
        public RequestText()
        {
            this.MsgType = RequestMsgType.Text;
        }

        public string Content { get; set; }
    }

    /// <summary>
    /// 图片消息
    /// </summary>
    public class RequestImage : RequestGeneralMessage
    {
        public RequestImage()
        {
            this.MsgType = RequestMsgType.Image;
        }

        /// <summary>
        /// 图片链接
        /// </summary>
        public string PicUrl { get; set; }
        /// <summary>
        /// 图片消息媒体id，可以调用多媒体文件下载接口拉取数据。 
        /// </summary>
        public string MediaId { get; set; }
    }

    /// <summary>
    /// 语音消息
    /// </summary>
    public class RequestVoice : RequestGeneralMessage
    {
        public RequestVoice()
        {
            this.MsgType = RequestMsgType.Voice;
        }

        /// <summary>
        /// 语音消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// 语音格式，如amr，speex等
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 语音识别结果，UTF8编码 
        /// 开通语音识别功能，用户每次发送语音给公众号时，微信会在推送的语音消息XML数据包中，增加一个Recongnition字段。 
        /// </summary>
        public string Recognition { get; set; }
    }

    /// <summary>
    /// 视频消息
    /// </summary>
    public class RequestVideo : RequestGeneralMessage
    {
        public RequestVideo()
        {
            this.MsgType = RequestMsgType.Video;
        }

        /// <summary>
        /// 频消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string ThumbMediaId { get; set; }
    }

    /// <summary>
    /// 地理位置消息
    /// </summary>
    public class RequestLocation : RequestGeneralMessage
    {
        public RequestLocation()
        {
            this.MsgType = RequestMsgType.Location;
        }

        /// <summary>
        /// 地理位置维度
        /// </summary>
        public string Location_X { get; set; }
        /// <summary>
        /// 地理位置经度
        /// </summary>
        public string Location_Y { get; set; }
        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public string Scale { get; set; }
        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label { get; set; }

    }

    /// <summary>
    /// 链接消息
    /// </summary>
    public class RequestLink : RequestGeneralMessage
    {
        public RequestLink()
        {
            this.MsgType = RequestMsgType.Link;
        }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 消息链接
        /// </summary>
        public string Url { get; set; }

    }

    /// <summary>
    /// 事件消息
    /// </summary>
    public class RequestEventMessage : RequestMessage
    {
        public RequestEventMessage()
        {
            this.MsgType = RequestMsgType.Event;
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        public RequestEventType Event { get; set; }

    }

    /// <summary>
    /// 关注事件
    /// </summary>
    public class RequestEventSubscribe : RequestEventMessage
    {
        public RequestEventSubscribe()
        {
            this.Event = RequestEventType.Subscribe;
        }
    }

    /// <summary>
    /// 取消关注事件
    /// </summary>
    public class RequestEventUnSubscribe : RequestEventMessage
    {
        public RequestEventUnSubscribe()
        {
            this.Event = RequestEventType.UnSubscribe;
        }
    }

    /// <summary>
    /// 二维码扫描事件
    /// 用户已关注时的事件推送
    /// </summary>
    public class RequestEventScan : RequestEventMessage
    {
        public RequestEventScan()
        {
            this.Event = RequestEventType.Scan;
        }

        /// <summary>
        /// 事件KEY值，是一个32位无符号整数，即创建二维码时的二维码scene_id
        /// </summary>
        public string EventKey { get; set; }

        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片 
        /// </summary>
        public string Ticket { get; set; }
    }

    /// <summary>
    /// 用户未关注时，进行关注后的事件推送
    /// 非常用户二维码扫描事件
    /// </summary>
    public class RequestEventCustomerScan : RequestEventMessage
    {
        public RequestEventCustomerScan()
        {
            this.Event = RequestEventType.Subscribe;
        }

        /// <summary>
        /// 事件KEY值，qrscene_为前缀，后面为二维码的参数值 
        /// </summary>
        public string EventKey { get; set; }

        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片 
        /// </summary>
        public string Ticket { get; set; }

        //private string paramValue;

        /// <summary>
        /// 二维码参数值
        /// </summary>
        public string ParamValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EventKey) == false)
                {
                    return EventKey.Replace("qrscene_", "");
                }
                else
                {
                    return null;
                }
            }
        }

    }

    /// <summary>
    /// 上报地理位置事件
    /// </summary>
    public class RequestEventLocation : RequestEventMessage
    {
        public RequestEventLocation()
        {

            this.Event = RequestEventType.Location;
        }

        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public string Latitude { get; set; }
        /// <summary>
        /// 地理位置经度
        /// </summary>
        public string Longitude { get; set; }
        /// <summary>
        /// 地理位置精度
        /// </summary>
        public string Precision { get; set; }
    }

    /// <summary>
    /// 自定义菜单事件
    /// 点击菜单拉取消息时的事件
    /// </summary>
    public class RequestEventClick : RequestEventMessage
    {
        public RequestEventClick()
        {
            this.Event = RequestEventType.Click;
        }

        /// <summary>
        /// 事件KEY值，与自定义菜单接口中KEY值对应 
        /// </summary>
        public string EventKey { get; set; }

    }

    /// <summary>
    /// 自定义菜单事件
    /// 点击菜单跳转链接时的事件
    /// </summary>
    public class RequestEventView : RequestEventMessage
    {
        public RequestEventView()
        {
            this.Event = RequestEventType.View;
        }

        /// <summary>
        /// 事件KEY值，设置的跳转URL 
        /// </summary>
        public string EventKey { get; set; }

    }

    /// <summary>
    /// 模版消息发送任务状态
    /// </summary>
    public enum TemplateSendJobFinishStatus
    {
        /// <summary>
        /// 送达成功
        /// </summary>
        Success,
        /// <summary>
        /// 送达由于用户拒收（用户设置拒绝接收公众号消息）而失败
        /// </summary>
        FailedUserBlock,
        /// <summary>
        /// 送达由于其他原因失败
        /// </summary>
        FailedSystemFailed
    }

    public class RequestEventTemplateSendJobFinish : RequestEventMessage
    {
        public RequestEventTemplateSendJobFinish()
        {
            this.Event = RequestEventType.TemplateSendJobFinish;
        }

        /// <summary>
        /// 发送状态  
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 发送状态  
        /// </summary>
        public TemplateSendJobFinishStatus StatusEnum
        {
            get {
                switch (this.Status)
                {
                    case "success":
                        return TemplateSendJobFinishStatus.Success;
                    case "failed:user block":
                        return TemplateSendJobFinishStatus.FailedUserBlock;
                    case "failed: system failed":
                        return TemplateSendJobFinishStatus.FailedSystemFailed;
                    default:
                        throw new Exception("没有比配的状态，可能是第三方变更了SDK约定") ;
                }
            }
            
        }

        /// <summary>
        /// 消息id，64位整型 
        /// </summary>
        public string MsgId { get; set; }
    }

}
