using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiXinLib;
using WeiXinLib.Model;
using WeiXinLib.Model.Messages;
using System.Web.SessionState;
using System.Text;
using System.IO;

namespace WeiXinLib
{
    /// <summary>
    /// 微信消息接收类。
    /// 开发者要做的事情，只有3件：
    /// 1.必须继承自本MessageHandler类。
    /// 2.实现GetConfig方法，返回微信账号配置信息。
    /// 3.根据需要，处理接收到的各种消息（其实就是实现各OnXXXMessage方法）。
    /// </summary>
    public abstract class MessageHandler : IHttpHandler, IRequiresSessionState
    {
        #region 内部字段数据块
        private VerifyModel verifyModel{ get; set; }
        #endregion

        #region 公开的入口及配置信息

        /// <summary>
        /// 配置信息
        /// </summary>
        public ConfigModel configModel { get; set; }

        /// <summary>
        /// 标准Http处理入口。
        /// </summary>
        /// <param name="context"></param>
        public virtual void ProcessRequest(HttpContext context)
        {

            //初始化
             Init(context);

            //验证消息
            if (VerifyMessage(context) == false) return;

            //处理消息
            ProcessMessage(context);

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 获取微信账号配置信息。
        /// (必须实现本方法)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract ConfigModel GetConfig(HttpContext context);

        #endregion

        #region 初始化/验证/初步消息处理
        
        /// <summary>
        /// 初始化运行环境
        /// </summary>
        /// <param name="context"></param>
        protected virtual void Init(HttpContext context)
        {
            configModel = GetConfig(context);
        }

        /// <summary>
        /// 验证消息
        /// </summary>
        protected virtual bool VerifyMessage(HttpContext context)
        {

            VerifyModel verifyModel = Verify.CreateVerifyModel(context, configModel.Token);//构造验证数据.
            this.verifyModel = verifyModel;

            if (Verify.VerifyModelValues(context,verifyModel) == false)
            {
                return false;
            }

            //是否申请接入(首次接入)
            if (Verify.IsRequestJoin(verifyModel.Echostr) == true)
            {
                AllowJoinWeiXin(context, verifyModel.Echostr);
                return false;//是接入请求,表明该消息无业务意义，只是一个握手而已，流程可以到此结束。
            }

            if (Verify.IsEncrypt(verifyModel) == true)//密文模式
            {
                return true; //返回，交由腾讯验证类去处理。
            }
            else //是明文模式，交由我们自己处理。
            {
                //是否来自微信?
                if (Verify.IsFromWeiXin(context, verifyModel) == false)
                {
                    return false;//不是微信，验证不通过。
                }
            }
            
            return true;
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="context"></param>
        protected virtual void ProcessMessage(HttpContext context)
        {
            try
            {
                System.Xml.XmlDocument doc = GetRequestXml(context, verifyModel);
                if (doc == null) { return; }

                string msgType = doc.GetElementsByTagName("MsgType")[0].InnerText.ToLower();

                #region 消息类型判断
                
                switch (msgType)
                {
                    case "text"://1文本消息
                        _OnTextMessage(context, doc);
                        break;

                    case "image"://2图片消息
                        _OnImageMessage(context, doc);
                        break;

                    case "voice"://3语音消息
                        _OnVoiceMessage(context, doc);
                        break;

                    case "video"://4视频消息
                        _OnVideoMessage(context, doc);
                        break;

                    case "link"://5链接消息
                        _OnLinkMessage(context, doc);
                        break;

                    case "location"://6位置消息
                        _OnLocationMessage(context, doc);
                        break;

                    case "event"://7事件类型消息

                        #region 事件类型判断

                        string eventAction = doc.GetElementsByTagName("Event")[0].InnerText.ToLower();
                        switch (eventAction) //事件也分好多种事件
                        {
                            case "subscribe"://关注订阅事件、也可能是(未关注的)客户的扫描事件.
                                if(doc.GetElementsByTagName("EventKey").Count >0 && doc.GetElementsByTagName("Ticket").Count>0)
                                {
                                    //如果subscribe事件里，含有EventKey与Ticket标签，说明这是(未关注的)客户的扫描事件。
                                    _OnEventCustomerScanMessage(context, doc);
                                }else
                                {
                                    //否则，说明这仅仅只是客户的关注事件。
                                    _OnEventSubscribeMessage(context, doc);
                                }
                                break;
                            case "unsubscribe"://接收到客户的取消关注事件
                                _OnEventUnSubscribeMessage(context, doc);
                                break;

                            case "scan"://接收到(已经关注的)客户的扫描事件
                                _OnEventScanMessage(context, doc);
                                break;

                            case "location"://接收到客户发来的地理位置事件
                                _OnEventLocationMessage(context, doc);
                                break;

                            case "click"://接收到客户的点击菜单按钮事件
                                _OnEventClickMessage(context, doc);
                                break;

                            case "view"://接收到客户的点击菜单按钮要跳转界面的事件
                                _OnEventViewMessage(context, doc);
                                break;
                            case "templatesendjobfinish"://模版消息发送任务完成后，微信服务器会将是否送达成功作为通知
                                _OnTemplateSendJobFinish(context, doc);
                                break;
                            default:
                                break;
                        }

                        break;
                        #endregion

                    default:
                        break;
                }
                #endregion
            }
            catch (Exception e)
            {
                /*
                 * 假如服务器无法保证在五秒内处理并回复，必须直接回复空串（是指回复一个空字符串，而不是一个XML结构体中content字段的内容为空，请切勿误解），
                 * 微信服务器不会对此作任何处理，并且不会发起重试。。这种情况下，可以使用客服消息接口进行异步回复。
                 */
                SendResponse(context, "");
            }
        }

        /// <summary>
        /// 获取请求的XML数据
        /// </summary>
        /// <param name="context"></param>
        /// <param name="verifyModel"></param>
        /// <returns></returns>
        private System.Xml.XmlDocument GetRequestXml(HttpContext context, VerifyModel verifyModel)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            if (Verify.IsEncrypt(verifyModel) == false) //明文消息
            {
                doc.Load(context.Request.InputStream); 
                return doc ;
            }
            else //密文消息
            {
                MessageCrypt mc = new MessageCrypt(configModel);
                string strRequest = GetRequestString(context.Request.InputStream);
                string strMsg = mc.DecryptMsg(verifyModel, strRequest);//解密
                
                if (string.IsNullOrWhiteSpace(strMsg))
                {
                    return null;
                }

                doc.LoadXml(strMsg);
                return doc;
            }
            
        }

        /// <summary>
        /// 获取请求的String数据
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        private string GetRequestString(System.IO.Stream inputStream)
        {
            String strmContents;
            int strLen, readCount;
            
            // Find number of bytes in stream.
            strLen = Convert.ToInt32(inputStream.Length);

            // Create a byte array.
            byte[] byteArray = new byte[strLen];

            // Read stream into byte array.
            readCount = inputStream.Read(byteArray, 0, strLen);
            
            // Convert byte array to a text string.
            strmContents = System.Text.Encoding.UTF8.GetString(byteArray);
            
            return strmContents;
        }

        /// <summary>
        /// 允许来自微信的请求接入(微信官方称之为首次接入)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="echostr"></param>
        private void AllowJoinWeiXin(HttpContext context, string echostr)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(echostr);
        }

        #endregion

        #region 消息内部分发接口(包括解析数据结构)

        /// <summary>
        /// 接收到客户发来的文本消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnTextMessage(HttpContext context, System.Xml.XmlDocument doc) 
        {
 //<xml>
 //<ToUserName><![CDATA[toUser]]></ToUserName>
 //<FromUserName><![CDATA[fromUser]]></FromUserName> 
 //<CreateTime>1348831860</CreateTime>
 //<MsgType><![CDATA[text]]></MsgType>
 //<Content><![CDATA[this is a test]]></Content>
 //<MsgId>1234567890123456</MsgId>
 //</xml>
            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;
            string content = doc.GetElementsByTagName("Content")[0].InnerText;
            string msgId = doc.GetElementsByTagName("MsgId")[0].InnerText;
            
            RequestText rqText = new RequestText();

            rqText.ToUserName = toUserName;
            rqText.FromUserName = fromUserName;
            rqText.CreateTime = new DateTime(long.Parse(createTime));
            rqText.Content = content;
            rqText.MsgId = msgId;
            
            ResponseMessage rpMsg = OnTextMessage(rqText);

            SendResponseMessage(context, rpMsg, rqText);
        }
        /// <summary>
        /// 接收到客户发来的图片消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnImageMessage(HttpContext context, System.Xml.XmlDocument doc) {

//            < xml >
//  < ToUserName >< ![CDATA[toUser]] ></ ToUserName >
//  < FromUserName >< ![CDATA[fromUser]] ></ FromUserName >
//  < CreateTime > 1348831860 </ CreateTime >
//  < MsgType >< ![CDATA[image]] ></ MsgType >
//  < PicUrl >< ![CDATA[this is a url]] ></ PicUrl >
//  < MediaId >< ![CDATA[media_id]] ></ MediaId >
//  < MsgId > 1234567890123456 </ MsgId >
//</ xml >

            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;
            string picurl = doc.GetElementsByTagName("PicUrl")[0].InnerText;
            string mediaId= doc.GetElementsByTagName("MediaId")[0].InnerText;
            string msgId = doc.GetElementsByTagName("MsgId")[0].InnerText;

            RequestImage rqText = new RequestImage();

            rqText.ToUserName = toUserName;
            rqText.FromUserName = fromUserName;
            rqText.CreateTime = new DateTime(long.Parse(createTime));
            rqText.PicUrl= picurl;
            rqText.MediaId = mediaId;
            rqText.MsgId = msgId;


            ResponseMessage rpMsg = OnImageMessage(rqText);

            SendResponseMessage(context, rpMsg, rqText);

        }
        /// <summary>
        /// 接收到客户发来的语音消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnVoiceMessage(HttpContext context, System.Xml.XmlDocument doc) {

//            < xml >
//  < ToUserName >< ![CDATA[toUser]] ></ ToUserName >
//  < FromUserName >< ![CDATA[fromUser]] ></ FromUserName >
//  < CreateTime > 1357290913 </ CreateTime >
//  < MsgType >< ![CDATA[voice]] ></ MsgType >
//  < MediaId >< ![CDATA[media_id]] ></ MediaId >
//  < Format >< ![CDATA[Format]] ></ Format >
//  < MsgId > 1234567890123456 </ MsgId >
//</ xml >

            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;

            string mediaId = doc.GetElementsByTagName("MediaId")[0].InnerText;
            string format = doc.GetElementsByTagName("Format")[0].InnerText;
            string msgId = doc.GetElementsByTagName("MsgId")[0].InnerText;

            RequestVoice rqText = new RequestVoice();

            rqText.ToUserName = toUserName;
            rqText.FromUserName = fromUserName;
            rqText.CreateTime = new DateTime(long.Parse(createTime));

            rqText.Format = format;
            rqText.MediaId = mediaId;
            rqText.MsgId = msgId;

            rqText.MsgId = msgId;

            ResponseMessage rpMsg = OnVoiceMessage(rqText);

            SendResponseMessage(context, rpMsg, rqText);

        }
        /// <summary>
        /// 接收到客户发来的视频消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnVideoMessage(HttpContext context, System.Xml.XmlDocument doc) {


//            < xml >
//  < ToUserName >< ![CDATA[toUser]] ></ ToUserName >
//  < FromUserName >< ![CDATA[fromUser]] ></ FromUserName >
//  < CreateTime > 1357290913 </ CreateTime >
//  < MsgType >< ![CDATA[video]] ></ MsgType >
//  < MediaId >< ![CDATA[media_id]] ></ MediaId >
//  < ThumbMediaId >< ![CDATA[thumb_media_id]] ></ ThumbMediaId >
//  < MsgId > 1234567890123456 </ MsgId >
//</ xml >

            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;

            string mediaId = doc.GetElementsByTagName("MediaId")[0].InnerText;
            string ThumbMediaId = doc.GetElementsByTagName("ThumbMediaId")[0].InnerText;
            string msgId = doc.GetElementsByTagName("MsgId")[0].InnerText;

            RequestVideo rqText = new RequestVideo();

            rqText.ToUserName = toUserName;
            rqText.FromUserName = fromUserName;
            rqText.CreateTime = new DateTime(long.Parse(createTime));

            rqText.ThumbMediaId = ThumbMediaId;
            rqText.MediaId = mediaId;
            rqText.MsgId = msgId;

            rqText.MsgId = msgId;

            ResponseMessage rpMsg = OnVideoMessage(rqText);

            SendResponseMessage(context, rpMsg, rqText);
        }
        /// <summary>
        /// 接收到客户发来的地理位置消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnLocationMessage(HttpContext context, System.Xml.XmlDocument doc) 
        {  
//            <xml>
//<ToUserName><![CDATA[toUser]]></ToUserName>
//<FromUserName><![CDATA[fromUser]]></FromUserName>
//<CreateTime>1351776360</CreateTime>
//<MsgType><![CDATA[location]]></MsgType>
//<Location_X>23.134521</Location_X>
//<Location_Y>113.358803</Location_Y>
//<Scale>20</Scale>
//<Label><![CDATA[位置信息]]></Label>
//<MsgId>1234567890123456</MsgId>
//</xml> 
            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;
            string location_X = doc.GetElementsByTagName("Location_X")[0].InnerText;
            string location_Y = doc.GetElementsByTagName("Location_Y")[0].InnerText;
            string scale = doc.GetElementsByTagName("Scale")[0].InnerText;
            string label = doc.GetElementsByTagName("Label")[0].InnerText;
            string msgId = doc.GetElementsByTagName("MsgId")[0].InnerText;

            RequestLocation rqMsg = new RequestLocation();

            rqMsg.ToUserName = toUserName;
            rqMsg.FromUserName = fromUserName;
            rqMsg.CreateTime = new DateTime(long.Parse(createTime));
            rqMsg.Location_X = location_X;
            rqMsg.Location_Y = location_Y;
            rqMsg.Label = label;
            rqMsg.MsgId = msgId;

            ResponseMessage rpMsg = OnLocationMessage(rqMsg);

            SendResponseMessage(context, rpMsg, rqMsg);

        }
        /// <summary>
        /// 接收到客户发来的链接消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnLinkMessage(HttpContext context, System.Xml.XmlDocument doc) {

            //            < xml >
            //  < ToUserName >< ![CDATA[toUser]] ></ ToUserName >
            //  < FromUserName >< ![CDATA[fromUser]] ></ FromUserName >
            //  < CreateTime > 1351776360 </ CreateTime >
            //  < MsgType >< ![CDATA[link]] ></ MsgType >
            //  < Title >< ![CDATA[公众平台官网链接]] ></ Title >
            //  < Description >< ![CDATA[公众平台官网链接]] ></ Description >
            //  < Url >< ![CDATA[url]] ></ Url >
            //  < MsgId > 1234567890123456 </ MsgId >
            //</ xml >

            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;

           
            string Title = doc.GetElementsByTagName("Title")[0].InnerText;
            string Url = doc.GetElementsByTagName("Url")[0].InnerText;
            string Description = doc.GetElementsByTagName("Description")[0].InnerText;
            
            string msgId = doc.GetElementsByTagName("MsgId")[0].InnerText;

            RequestLink rqText = new RequestLink();

            rqText.ToUserName = toUserName;
            rqText.FromUserName = fromUserName;
            rqText.CreateTime = new DateTime(long.Parse(createTime));

            rqText.Title = Title;
            rqText.Url = Url;
            rqText.Description = Description;
         
            rqText.MsgId = msgId;


            ResponseMessage rpMsg = OnLinkMessage(rqText);

            SendResponseMessage(context, rpMsg, rqText);

        }
        /// <summary>
        /// 接收到客户的关注事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnEventSubscribeMessage(HttpContext context, System.Xml.XmlDocument doc) 
        {
//            <xml>
//<ToUserName><![CDATA[toUser]]></ToUserName>
//<FromUserName><![CDATA[FromUser]]></FromUserName>
//<CreateTime>123456789</CreateTime>
//<MsgType><![CDATA[event]]></MsgType>
//<Event><![CDATA[subscribe]]></Event>
//</xml>
            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;
            
            RequestEventSubscribe rqMsg = new RequestEventSubscribe();

            rqMsg.ToUserName = toUserName;
            rqMsg.FromUserName = fromUserName;
            rqMsg.CreateTime = new DateTime(long.Parse(createTime));

            ResponseMessage rpMsg = OnEventSubscribeMessage(rqMsg);

            SendResponseMessage(context, rpMsg, rqMsg);
        }
        /// <summary>
        /// 接收到客户的取消关注事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnEventUnSubscribeMessage(HttpContext context, System.Xml.XmlDocument doc) 
        {
            //            <xml>
            //<ToUserName><![CDATA[toUser]]></ToUserName>
            //<FromUserName><![CDATA[FromUser]]></FromUserName>
            //<CreateTime>123456789</CreateTime>
            //<MsgType><![CDATA[event]]></MsgType>
            //<Event><![CDATA[subscribe]]></Event>
            //</xml>
            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;

            RequestEventUnSubscribe rqMsg = new RequestEventUnSubscribe();

            rqMsg.ToUserName = toUserName;
            rqMsg.FromUserName = fromUserName;
            rqMsg.CreateTime = new DateTime(long.Parse(createTime));

            ResponseMessage rpMsg = OnEventUnSubscribeMessage(rqMsg);

            SendResponseMessage(context, rpMsg, rqMsg);
        }
        /// <summary>
        /// 接收到(已经关注的)客户的扫描事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnEventScanMessage(HttpContext context, System.Xml.XmlDocument doc) 
        {
            //            <xml>
            //<ToUserName><![CDATA[toUser]]></ToUserName>
            //<FromUserName><![CDATA[FromUser]]></FromUserName>
            //<CreateTime>123456789</CreateTime>
            //<MsgType><![CDATA[event]]></MsgType>
            //<Event><![CDATA[SCAN]]></Event>
            //<EventKey><![CDATA[SCENE_VALUE]]></EventKey>
            //<Ticket><![CDATA[TICKET]]></Ticket>
            //</xml>
            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;
            string eventKey = doc.GetElementsByTagName("EventKey")[0].InnerText;
            string ticket = doc.GetElementsByTagName("Ticket")[0].InnerText;

            RequestEventScan rqMsg = new RequestEventScan();

            rqMsg.ToUserName = toUserName;
            rqMsg.FromUserName = fromUserName;
            rqMsg.CreateTime = new DateTime(long.Parse(createTime));
            rqMsg.EventKey = eventKey;
            rqMsg.Ticket = ticket;

            ResponseMessage rpMsg = OnEventScanMessage(rqMsg);

            SendResponseMessage(context, rpMsg, rqMsg);
        }
        /// <summary>
        /// 接收到(未关注的)客户的扫描事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnEventCustomerScanMessage(HttpContext context, System.Xml.XmlDocument doc) 
        {
//            <xml><ToUserName><![CDATA[toUser]]></ToUserName>
//<FromUserName><![CDATA[FromUser]]></FromUserName>
//<CreateTime>123456789</CreateTime>
//<MsgType><![CDATA[event]]></MsgType>
//<Event><![CDATA[subscribe]]></Event>
//<EventKey><![CDATA[qrscene_123123]]></EventKey>
//<Ticket><![CDATA[TICKET]]></Ticket>
//</xml>
            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;
            string eventKey = doc.GetElementsByTagName("EventKey")[0].InnerText;
            string ticket = doc.GetElementsByTagName("Ticket")[0].InnerText;

            RequestEventCustomerScan rqMsg = new RequestEventCustomerScan();

            rqMsg.ToUserName = toUserName;
            rqMsg.FromUserName = fromUserName;
            rqMsg.CreateTime = new DateTime(long.Parse(createTime));
            rqMsg.EventKey = eventKey;
            rqMsg.Ticket = ticket;

            ResponseMessage rpMsg = OnEventCustomerScanMessage(rqMsg);

            SendResponseMessage(context, rpMsg, rqMsg);
        }
        /// <summary>
        /// 接收到客户的地理位置事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnEventLocationMessage(HttpContext context, System.Xml.XmlDocument doc) 
        {  
//            <xml>
//<ToUserName><![CDATA[toUser]]></ToUserName>
//<FromUserName><![CDATA[fromUser]]></FromUserName>
//<CreateTime>123456789</CreateTime>
//<MsgType><![CDATA[event]]></MsgType>
//<Event><![CDATA[LOCATION]]></Event>
//<Latitude>23.137466</Latitude>
//<Longitude>113.352425</Longitude>
//<Precision>119.385040</Precision>
//</xml>
            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;
            string latitude = doc.GetElementsByTagName("Latitude")[0].InnerText;
            string longitude = doc.GetElementsByTagName("Longitude")[0].InnerText;
            string precision = doc.GetElementsByTagName("Precision")[0].InnerText;

            RequestEventLocation rqMsg = new RequestEventLocation();

            rqMsg.ToUserName = toUserName;
            rqMsg.FromUserName = fromUserName;
            rqMsg.CreateTime = new DateTime(long.Parse(createTime));
            rqMsg.Latitude = latitude;
            rqMsg.Longitude = longitude;
            rqMsg.Precision = precision;

            ResponseMessage rpMsg = OnEventLocationMessage(rqMsg);

            SendResponseMessage(context, rpMsg, rqMsg);
        }
        /// <summary>
        /// 接收到客户的点击菜单按钮事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnEventClickMessage(HttpContext context, System.Xml.XmlDocument doc) 
        {  
//        <xml>
//<ToUserName><![CDATA[toUser]]></ToUserName>
//<FromUserName><![CDATA[FromUser]]></FromUserName>
//<CreateTime>123456789</CreateTime>
//<MsgType><![CDATA[event]]></MsgType>
//<Event><![CDATA[CLICK]]></Event>
//<EventKey><![CDATA[EVENTKEY]]></EventKey>
//</xml>
            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;
            string eventKey = doc.GetElementsByTagName("EventKey")[0].InnerText;

            RequestEventClick rqMsg = new RequestEventClick();

            rqMsg.ToUserName = toUserName;
            rqMsg.FromUserName = fromUserName;
            rqMsg.CreateTime = new DateTime(long.Parse(createTime));
            rqMsg.EventKey = eventKey;
            
            ResponseMessage rpMsg = OnEventClickMessage(rqMsg);

            SendResponseMessage(context, rpMsg, rqMsg);
        }
        /// <summary>
        /// 接收到客户的点击菜单按钮要跳转界面的事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnEventViewMessage(HttpContext context, System.Xml.XmlDocument doc) 
        {
//            <xml>
//<ToUserName><![CDATA[toUser]]></ToUserName>
//<FromUserName><![CDATA[FromUser]]></FromUserName>
//<CreateTime>123456789</CreateTime>
//<MsgType><![CDATA[event]]></MsgType>
//<Event><![CDATA[VIEW]]></Event>
//<EventKey><![CDATA[www.qq.com]]></EventKey>
//</xml>
            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;
            string eventKey = doc.GetElementsByTagName("EventKey")[0].InnerText;

            RequestEventView rqMsg = new RequestEventView();

            rqMsg.ToUserName = toUserName;
            rqMsg.FromUserName = fromUserName;
            rqMsg.CreateTime = new DateTime(long.Parse(createTime));
            rqMsg.EventKey = eventKey;

            ResponseMessage rpMsg = OnEventViewMessage(rqMsg);

            SendResponseMessage(context, rpMsg, rqMsg);
        }

        /// <summary>
        /// 在模版消息发送任务完成后，微信服务器会将是否送达成功作为通知
        /// </summary>
        /// <param name="context"></param>
        /// <param name="doc"></param>
        private void _OnTemplateSendJobFinish(HttpContext context, System.Xml.XmlDocument doc) 
        {
//<xml>
//          <ToUserName><![CDATA[gh_7f083739789a]]></ToUserName>
//          <FromUserName><![CDATA[oia2TjuEGTNoeX76QEjQNrcURxG8]]></FromUserName>
//          <CreateTime>1395658920</CreateTime>
//          <MsgType><![CDATA[event]]></MsgType>
//          <Event><![CDATA[TEMPLATESENDJOBFINISH]]></Event>
//          <MsgID>200163836</MsgID>
//          <Status><![CDATA[success]]></Status>
//          </xml>
            string toUserName = doc.GetElementsByTagName("ToUserName")[0].InnerText;
            string fromUserName = doc.GetElementsByTagName("FromUserName")[0].InnerText;
            string createTime = doc.GetElementsByTagName("CreateTime")[0].InnerText;
            string msgID = doc.GetElementsByTagName("MsgID")[0].InnerText;
            string status = doc.GetElementsByTagName("Status")[0].InnerText;

            RequestEventTemplateSendJobFinish rqMsg = new RequestEventTemplateSendJobFinish();

            rqMsg.ToUserName = toUserName;
            rqMsg.FromUserName = fromUserName;
            rqMsg.CreateTime = new DateTime(long.Parse(createTime));
            rqMsg.MsgId = msgID;
            rqMsg.Status = status;

            OnTemplateSendJobFinish(rqMsg);

            //SendResponse(context, "");//无需响应
        }
        
        #endregion

        #region 消息外部分发接口
		
        /// <summary>
        ///  接收到客户发来的文本消息
        /// </summary>
        /// <param name="requestText"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnTextMessage(RequestText requestText);
        /// <summary>
        /// 接收到客户发来的图片消息
        /// </summary>
        /// <param name="requestImage"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnImageMessage(RequestImage requestImage);
        /// <summary>
        /// 接收到客户发来的语音消息
        /// </summary>
        /// <param name="requestVoice"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnVoiceMessage(RequestVoice requestVoice);
        /// <summary>
        /// 接收到客户发来的视频消息
        /// </summary>
        /// <param name="requestVideo"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnVideoMessage(RequestVideo requestVideo);
        /// <summary>
        /// 接收到客户发来的地理位置消息
        /// </summary>
        /// <param name="requestLocation"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnLocationMessage(RequestLocation requestLocation);
        /// <summary>
        /// 接收到客户发来的链接消息
        /// </summary>
        /// <param name="requestLink"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnLinkMessage(RequestLink requestLink);
        /// <summary>
        /// 接收到客户的关注事件
        /// </summary>
        /// <param name="requestEventSubscribe"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnEventSubscribeMessage(RequestEventSubscribe requestEventSubscribe);
        /// <summary>
        /// 接收到客户的取消关注事件
        /// </summary>
        /// <param name="requestEventUnSubscribe"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnEventUnSubscribeMessage(RequestEventUnSubscribe requestEventUnSubscribe);
        /// <summary>
        /// 接收到客户的扫描事件
        /// </summary>
        /// <param name="requestEventScan"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnEventScanMessage(RequestEventScan requestEventScan);
        /// <summary>
        /// 接收到非关注客户的扫描事件
        /// </summary>
        /// <param name="requestEventCustomerScan"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnEventCustomerScanMessage(RequestEventCustomerScan requestEventCustomerScan);
        /// <summary>
        /// 接收到客户的地理位置事件
        /// </summary>
        /// <param name="requestEventLocation"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnEventLocationMessage(RequestEventLocation requestEventLocation);
        /// <summary>
        /// 接收到客户的点击菜单按钮事件
        /// </summary>
        /// <param name="requestEventClick"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnEventClickMessage(RequestEventClick requestEventClick);
        /// <summary>
        /// 接收到客户的点击菜单按钮要跳转界面的事件
        /// </summary>
        /// <param name="requestEventView"></param>
        /// <returns></returns>
        public abstract ResponseMessage OnEventViewMessage(RequestEventView requestEventView);

        /// <summary>
        /// 模版消息发送任务完成后，微信服务器会将是否送达成功作为通知。
        /// </summary>
        /// <param name="requestEventTemplateSendJobFinish"></param>
        public virtual void OnTemplateSendJobFinish(RequestEventTemplateSendJobFinish requestEventTemplateSendJobFinish) {

        }

        #endregion

        #region 发送响应客户消息

        /// <summary>
        /// 发送Http数据
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        private void SendResponse(HttpContext context,string data)
        {
            if (Verify.IsEncrypt(verifyModel) == true //属于密文模式
                && string.IsNullOrWhiteSpace(data) == false) //并且数据不为空
            {
                MessageCrypt mc = new MessageCrypt(configModel);
                data = mc.EncryptMsg(verifyModel, data);//加密
            }

            WriteResponse(context, data);

        }

        /// <summary>
        /// 写入Http响应流。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        private void WriteResponse(HttpContext context, string data)
        {
            context.Response.Charset = "UTF-8";
            context.Response.ContentType = "text/xml";
            context.Response.Write(data);
        }

        /// <summary>
        /// 发送响应消息。
        /// </summary>
        /// <param name="responseMessage"></param>
        private void SendResponseMessage(HttpContext context, ResponseMessage responseMessage)
        {
            if (responseMessage == null) { return; }

            switch (responseMessage.MsgType)
            {
                case ResponseMsgType.Text:
                    SendResponseText(context, (ResponseText)responseMessage);
                    break;
                case ResponseMsgType.Image:
                    SendResponseImage(context, (ResponseImage)responseMessage);
                    break;
                case ResponseMsgType.Voice:
                    SendResponseVoice(context, (ResponseVoice)responseMessage);
                    break;
                case ResponseMsgType.Video:
                    SendResponseVideo(context, (ResponseVideo)responseMessage);
                    break;
                case ResponseMsgType.Music:
                    SendResponseMusic(context, (ResponseMusic)responseMessage);
                    break;
                case ResponseMsgType.News:
                    SendResponseNews(context, (ResponseNews)responseMessage);
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// 发送响应消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseMessage"></param>
        /// <param name="requestMessage"></param>
        private void SendResponseMessage(HttpContext context, ResponseMessage responseMessage, RequestMessage requestMessage) 
        {
            if (responseMessage == null) 
            {
                /*
                 * 假如服务器无法保证在五秒内处理并回复，必须直接回复空串（是指回复一个空字符串，而不是一个XML结构体中content字段的内容为空，请切勿误解），
                 * 微信服务器不会对此作任何处理，并且不会发起重试。。这种情况下，可以使用客服消息接口进行异步回复。
                 */
                SendResponse(context, "");
                return; 
            }

            //转换发送接收的角色
            responseMessage.FromUserName = requestMessage.ToUserName;
            responseMessage.ToUserName = requestMessage.FromUserName;

            SendResponseMessage(context, responseMessage);
        }
        
        /// <summary>
        /// 响应文本
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseText"></param>
        private void SendResponseText(HttpContext context, ResponseText responseText) 
        {
            string data = @"
<xml>
<ToUserName><![CDATA[{0}]]></ToUserName>
<FromUserName><![CDATA[{1}]]></FromUserName>
<CreateTime>{2}</CreateTime>
<MsgType><![CDATA[{3}]]></MsgType>
<Content><![CDATA[{4}]]></Content>
</xml>";

            data = string.Format(data, 
                responseText.ToUserName, //{0}
                responseText.FromUserName, //{1}
                responseText.CreateTime.Ticks,//{2}
                responseText.MsgType.ToString().ToLower(), //{3}
                responseText.Content//{4}
                );

            SendResponse(context, data);

        }

        /// <summary>
        /// 响应图片
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseImage"></param>
        private void SendResponseImage(HttpContext context, ResponseImage responseImage) 
        { 
            string data = @"
<xml>
<ToUserName><![CDATA[{0}]]></ToUserName>
<FromUserName><![CDATA[{1}]]></FromUserName>
<CreateTime>{2}</CreateTime>
<MsgType><![CDATA[{3}]]></MsgType>
<Image>
<MediaId><![CDATA[{4}]]></MediaId>
</Image>
</xml>";

            data = string.Format(data,
                responseImage.ToUserName, //{0}
                responseImage.FromUserName, //{1}
                responseImage.CreateTime.Ticks,//{2}
                responseImage.MsgType.ToString().ToLower(), //{3}
                responseImage.MediaId//{4}
                );

            SendResponse(context, data);

        }

        /// <summary>
        /// 响应语音
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseVoice"></param>
        private void SendResponseVoice(HttpContext context, ResponseVoice responseVoice) 
        {
            string data = @"
<xml>
<ToUserName><![CDATA[{0}]]></ToUserName>
<FromUserName><![CDATA[{1}]]></FromUserName>
<CreateTime>{2}</CreateTime>
<MsgType><![CDATA[{3}]]></MsgType>
<Voice>
<MediaId><![CDATA[{4}]]></MediaId>
</Voice>
</xml>";

            data = string.Format(data,
                responseVoice.ToUserName, //{0}
                responseVoice.FromUserName, //{1}
                responseVoice.CreateTime.Ticks,//{2}
                responseVoice.MsgType.ToString().ToLower(), //{3}
                responseVoice.MediaId//{4}
                );

            SendResponse(context, data);
        }
        /// <summary>
        /// 响应视频
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseVideo"></param>
        private void SendResponseVideo(HttpContext context, ResponseVideo responseVideo) { }
        /// <summary>
        /// 响应音乐
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseMusic"></param>
        private void SendResponseMusic(HttpContext context, ResponseMusic responseMusic) { }
        /// <summary>
        /// 响应图文
        /// </summary>
        /// <param name="context"></param>
        /// <param name="responseNews"></param>
        private void SendResponseNews(HttpContext context, ResponseNews responseNews) {
            string data = @"
<xml>
<ToUserName><![CDATA[{0}]]></ToUserName>
<FromUserName><![CDATA[{1}]]></FromUserName>
<CreateTime>{2}</CreateTime>
<MsgType><![CDATA[{3}]]></MsgType>
<ArticleCount>{4}</ArticleCount>
<Articles>
{5}
</Articles>
</xml> ";

            string dataArticleItem = @"
<item>
<Title><![CDATA[{0}]]></Title> 
<Description><![CDATA[{1}]]></Description>
<PicUrl><![CDATA[{2}]]></PicUrl>
<Url><![CDATA[{3}]]></Url>
</item>";

            StringBuilder sbArticles = new StringBuilder();

            foreach (var item in responseNews.Articles)
            {
                sbArticles.Append(string.Format(dataArticleItem, item.Title, item.Description, item.PicUrl, item.Url));
            }

            data = string.Format(data,
                responseNews.ToUserName, //{0}
                responseNews.FromUserName, //{1}
                responseNews.CreateTime.Ticks,//{2}
                responseNews.MsgType.ToString().ToLower(), //{3}
                responseNews.ArticleCount,//{4},
                sbArticles//{5}
                );

            SendResponse(context, data);
        }
        
        #endregion

    }
}
