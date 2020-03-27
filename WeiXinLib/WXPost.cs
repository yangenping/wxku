using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WeiXinLib.Model;
using WeiXinLib.Model.Messages;

namespace WeiXinLib
{
    public class WXPost: MessageHandler
    {
        public override ConfigModel GetConfig(HttpContext context)
        {

            return ConfigManager.GetConfigFirst();

        }

        public override ResponseMessage OnEventClickMessage(RequestEventClick requestEventClick)
        {
            throw new NotImplementedException();
        }

        public override ResponseMessage OnEventCustomerScanMessage(RequestEventCustomerScan requestEventCustomerScan)
        {
            throw new NotImplementedException();
        }

        public override ResponseMessage OnEventLocationMessage(RequestEventLocation requestEventLocation)
        {
            throw new NotImplementedException();
        }

        public override ResponseMessage OnEventScanMessage(RequestEventScan requestEventScan)
        {
            throw new NotImplementedException();
        }

        public override ResponseMessage OnEventSubscribeMessage(RequestEventSubscribe requestEventSubscribe)
        {
            throw new NotImplementedException();
        }

        public override ResponseMessage OnEventUnSubscribeMessage(RequestEventUnSubscribe requestEventUnSubscribe)
        {
            throw new NotImplementedException();
        }

        public override ResponseMessage OnEventViewMessage(RequestEventView requestEventView)
        {
            throw new NotImplementedException();
        }

        public override ResponseMessage OnImageMessage(RequestImage requestImage)
        {
            return new ResponseImage() { MediaId=requestImage.MediaId};
        }

        public override ResponseMessage OnLinkMessage(RequestLink requestLink)
        {
            return new ResponseText() { Content = "收到url" + requestLink.Url };
        }

        public override ResponseMessage OnLocationMessage(RequestLocation requestLocation)
        {
            return new ResponseText() { Content = "收到地址"+requestLocation.Location_X+"|"+requestLocation.Location_Y };
        }

        public override ResponseMessage OnTextMessage(RequestText requestText)
        {
            //return new ResponseText() { Content="收到消息"+ requestText.Content};

            return new ResponseNews() {ArticleCount=1,Articles=new List<ResponseNewsArticleItem>() { new ResponseNewsArticleItem() { Description="a",Title="b",PicUrl= "https://www.biedoul.com/Uploads/Images/4/57c413963e3d6.jpg", Url= "https://www.cnblogs.com/Violety/p/9814681.html" } } };
        }

        public override ResponseMessage OnVideoMessage(RequestVideo requestVideo)
        {
            return new 
                ResponseVideo() {MediaId=  requestVideo.MediaId ,Description="abbbbb",Title="a"};
        }

        public override ResponseMessage OnVoiceMessage(RequestVoice requestVoice)
        {
            return new ResponseVoice() { MediaId = requestVoice.MediaId };
        }
    }
}
