using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeiXinLib.Model.Send;
using Newtonsoft.Json;
using WeiXinLib.Model;
using WeiXinLib.Model.Menus;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using WeiXinLib.Model.User;
using WeiXinLib.Model.QR;

namespace WeiXinLib
{
    /// <summary>
    /// 微信消息发送功能类。
    /// 开发者要做的事情，只有2件：
    /// 1.创建SendHandler类的实例，并传入微信账号配置信息。
    /// 2.根据需要，调用相应的消息发送接口。
    /// </summary>
    public class SendHandler
    {
        #region 配置信息
        
        private ConfigModel configModel = null;

        public ConfigModel Config
        {
            get { return configModel; }
            set { configModel = value; }
        }
        
        #endregion

        #region 初始化及构造函数
        
        /// <summary>
        /// 注意：如果你用了空参构造函数。那么一定要额外设置好Config对象。
        /// </summary>
        private SendHandler() { }

        /// <summary>
        /// 带配置信息的构造参数。
        /// </summary>
        /// <param name="configModel"></param>
        public SendHandler(ConfigModel configModel) 
        {
            if (configModel == null || string.IsNullOrWhiteSpace(configModel.AccessToken))
            {
                throw new Exception("构造SendHandler，必须设置配置信息，并且AccessToken不能为空!因为发送消息需要AccessToken!");
            }

            Config = configModel;
            
        }
        
        #endregion

        #region 发送客服消息接口(发送一般消息)

        #region 发送文本
        
        /// <summary>
        /// 发送文本消息给客户
        /// </summary>
        /// <param name="message"></param>
        private void _SendText(TextMessage message)
        {
            string sendUrl = ApiUrl.GetSendCustomUrl(message.AccessToken);

            string result = HttpHelper.PostJson(sendUrl, JsonConvert.SerializeObject(message));
            
            ResultModel.CreateInstance(result).HasException();
        }

        /// <summary>
        /// 发送文本消息给客户
        /// </summary>
        /// <param name="message"></param>
        private void SendText(TextMessage message)
        {
            if (message.AccessToken == null)
            {
                message.AccessToken = Config.AccessToken;

            }
            _SendText(message);
        }

        /// <summary>
        /// 发送文本消息给客户
        /// </summary>
        /// <param name="touser">客户OpenID(发给谁？)</param>
        /// <param name="content">消息文本内容</param>
        public void SendText(string touser, string content)
        {
            TextMessage message = new TextMessage();

            message.AccessToken = Config.AccessToken;
            message.touser = touser;
            message.text = new Text { content = content };

            SendText(message);
        }
        #endregion

        #region 发送图片

        private void _SendImage(ImageMessage message)
        {
            string sendUrl = ApiUrl.GetSendCustomUrl(message.AccessToken);

            string result = HttpHelper.PostJson(sendUrl, JsonConvert.SerializeObject(message));

            ResultModel.CreateInstance(result).HasException();
        }

        private void SendImage(ImageMessage message)
        {
            if (message.AccessToken == null)
            {
                message.AccessToken = Config.AccessToken;

            }
            _SendImage(message);
        }

        /// <summary>
        /// 发送图片给客户
        /// </summary>
        /// <param name="touser">客户ID</param>
        /// <param name="media_id">图片ID</param>
        public void SendImage(string touser, string media_id)
        {
            ImageMessage message = new ImageMessage();

            message.AccessToken = Config.AccessToken;
            message.touser = touser;
            message.image = new Image { media_id = media_id };

            SendImage(message);
        }
        
        #endregion

        #region 发送图文

        private void _SendNews(NewsMessage message)
        {
            string sendUrl = ApiUrl.GetSendCustomUrl(message.AccessToken);

            string result = HttpHelper.PostJson(sendUrl, JsonConvert.SerializeObject(message));

            ResultModel.CreateInstance(result).HasException();
        }

        private void SendNews(NewsMessage message)
        {
            if (message.AccessToken == null)
            {
                message.AccessToken = Config.AccessToken;

            }
            _SendNews(message);
        }

        /// <summary>
        /// 发送图文给客户
        /// </summary>
        /// <param name="touser">客户ID</param>
        /// <param name="articles">文章列表</param>
        public void SendNews(string touser, List<Article> articles)
        {
            NewsMessage message = new NewsMessage();

            message.AccessToken = Config.AccessToken;
            message.touser = touser;
            message.news = new News { articles = articles };

            SendNews(message);
        }

        #endregion

        #endregion

        #region 自定义菜单接口

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="config"></param>
        private void _CreateMenu(MenuModel menu, ConfigModel config)
        {
            string sendUrl = ApiUrl.GetCreateMenuUrl(config.AccessToken);

            string json = JsonConvert.SerializeObject(menu, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            string result = HttpHelper.PostJson(sendUrl, json);

            ResultModel.CreateInstance(result).HasException();
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="menu">菜单对象</param>
        public void CreateMenu(MenuModel menu)
        {
            _CreateMenu(menu, this.Config);
        }

        #endregion

        #region 获取AccessToken接口
        /// <summary>
        /// 获取Access Token
        /// 公众号的全局唯一票据，公众号调用各接口时都需使用access_token。
        /// 正常情况下access_token有效期为7200秒，重复获取将导致上次获取的access_token失效。
        /// 由于获取access_token的api调用次数非常有限，建议开发者全局存储与更新access_token，频繁刷新access_token会导致api调用受限，影响自身业务。
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static AccessTokenModel GetAccessToken(ConfigModel config)
        {
           AccessTokenModel accessTokenModel = GetAccessToken(config.AppId, config.AppSecret);
           return accessTokenModel;
        }

        /// <summary>
        /// 获取Access Token
        /// 公众号的全局唯一票据，公众号调用各接口时都需使用access_token。
        /// 正常情况下access_token有效期为7200秒，重复获取将导致上次获取的access_token失效。
        /// 由于获取access_token的api调用次数非常有限，建议开发者全局存储与更新access_token，频繁刷新access_token会导致api调用受限，影响自身业务。
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="appSecret">应用密钥</param>
        /// <returns></returns>
        public static AccessTokenModel GetAccessToken(string appId, string appSecret)
        {
            string url = ApiUrl.GetAccessTokenUrl(appId, appSecret);

            String json = HttpHelper.ScrapeWebPage(url);

            JObject jo = JObject.Parse(json);

            AccessTokenModel accessTokenModel = jo.ToObject<AccessTokenModel>();

            return accessTokenModel;

        }

        #endregion

        #region 获取用户基本信息(UnionID机制)接口

        /// <summary>
        /// 获取用户基本信息（包括UnionID机制）
        /// </summary>
        /// <param name="openid">用户ID</param>
        /// <param name="lang">语言</param>
        /// <returns></returns>
        public UserInfoModel GetUserInfo(string openid, string lang = Constants.LANG_ZH_CN)
        {
            string url = ApiUrl.GetUserInfoUrl(Config.AccessToken, openid, lang);

            String json = HttpHelper.ScrapeWebPage(url);

            ResultModel.CreateInstance(json).HasException();

            JObject jo = JObject.Parse(json);

            UserInfoModel userInfoModel = jo.ToObject<UserInfoModel>();

            return userInfoModel;
        }

        /// <summary>
        /// 批量获取用户基本信息,开发者可通过该接口来批量获取用户基本信息。最多支持一次拉取100条。 
        /// </summary>
        /// <param name="batchgetUserInfoList">要请求的批量用户</param>
        /// <returns></returns>
        public List<UserInfoModel> BatchGetUserInfo(List<ParamBatchgetUserInfo>  batchgetUserInfoList)
        {
            string url = ApiUrl.BatchGetUserInfoUrl(Config.AccessToken);

            var paramBatchgetUserInfoList = new ParamBatchgetUserInfoList { user_list = batchgetUserInfoList };

            string json = HttpHelper.PostJson(url, JsonConvert.SerializeObject(paramBatchgetUserInfoList));

            ResultModel.CreateInstance(json).HasException();

            JObject jo = JObject.Parse(json);

            UserInfoList userInfoList = jo.ToObject<UserInfoList>();

            return userInfoList.user_info_list;
        }

        #endregion

        #region Ticket 票据操作接口。

        /// <summary>
        /// 获取JsApiTicket
        /// jsapi_ticket是公众号用于调用微信JS接口的临时票据。
        /// 正常情况下，jsapi_ticket的有效期为7200秒，通过access_token来获取。
        /// 由于获取jsapi_ticket的api调用次数非常有限，频繁刷新jsapi_ticket会导致api调用受限，影响自身业务，开发者必须在自己的服务全局缓存jsapi_ticket 。 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static JsApiTicketModel GetJsApiTicket(ConfigModel config)
        {
            JsApiTicketModel model = GetJsApiTicket(config.AccessToken);
            return model;
        }

        /// <summary>
        /// 获取JsApiTicket
        /// jsapi_ticket是公众号用于调用微信JS接口的临时票据。
        /// 正常情况下，jsapi_ticket的有效期为7200秒，通过access_token来获取。
        /// 由于获取jsapi_ticket的api调用次数非常有限，频繁刷新jsapi_ticket会导致api调用受限，影响自身业务，开发者必须在自己的服务全局缓存jsapi_ticket 。 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static JsApiTicketModel GetJsApiTicket(string accessToken)
        {
            string url = ApiUrl.GetJsApiTicketUrl(accessToken);

            String json = HttpHelper.ScrapeWebPage(url);

            JObject jo = JObject.Parse(json);

            JsApiTicketModel model = jo.ToObject<JsApiTicketModel>();

            return model;

        }

        /// <summary>
        /// 获取ApiTicket
        /// 卡券 api_ticket 是用于调用卡券相关接口的临时票据，有效期为 7200 秒，通过 access_token 来获取。
        /// 这里要注意与 jsapi_ticket 区分开来。由于获取卡券 api_ticket 的 api 调用次数非常有限，
        /// 频繁刷新卡券 api_ticket 会导致 api 调用受限，影响自身业务，开发者必须在自己的服务全局缓存卡券 api_ticket 。 
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ApiTicketModel GetApiTicket(ConfigModel config)
        {
            ApiTicketModel model = GetApiTicket(config.AccessToken);
            return model;
        }

        /// <summary>
        /// 获取ApiTicket
        /// 卡券 api_ticket 是用于调用卡券相关接口的临时票据，有效期为 7200 秒，通过 access_token 来获取。
        /// 这里要注意与 jsapi_ticket 区分开来。由于获取卡券 api_ticket 的 api 调用次数非常有限，
        /// 频繁刷新卡券 api_ticket 会导致 api 调用受限，影响自身业务，开发者必须在自己的服务全局缓存卡券 api_ticket 。 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static ApiTicketModel GetApiTicket(string accessToken)
        {
            string url = ApiUrl.GetApiTicketUrl(accessToken);

            String json = HttpHelper.ScrapeWebPage(url);

            JObject jo = JObject.Parse(json);

            ApiTicketModel model = jo.ToObject<ApiTicketModel>();

            return model;

        }


        #endregion

        #region 二维码操作接口

        /// <summary>
        ///  创建临时二维码
        ///  就算每次使用的id一样，生成的ticket也不会一样,图片不一样，如果扫描，返回的id却都一样。
        /// </summary>
        /// <param name="scene_id">场景值ID，临时二维码时为32位非0整型</param>
        /// <param name="expire_seconds">该二维码有效时间，以秒为单位。 最大不超过2592000（即30天），此字段如果不填，则默认有效期为30秒。 </param>
        /// <returns></returns>
        public QrCodeResultModel CreateQrCodeTemp(int scene_id, int? expire_seconds = null)
        {
            string url = ApiUrl.GetCreateQrUrl(Config.AccessToken);
            string postData = QrCodeModel.CreateQrCodeTempPostData(scene_id,expire_seconds) ;
            string json = HttpHelper.PostJson(url, postData);

            ResultModel.CreateInstance(json).HasException();

            JObject jo = JObject.Parse(json);
            QrCodeResultModel rModel = jo.ToObject<QrCodeResultModel>();

            return rModel;

        }

        /// <summary>
        /// 创建数字型id永久二维码
        /// 如果每次使用的id一样，生成的ticket就一样,图片一样，如果扫描，返回的id都一样。
        /// </summary>
        /// <param name="scene_id">场景值ID，最大值为100000（目前参数只支持1--100000）</param>
        /// <returns></returns>
        public QrCodeResultModel CreateQrCode(int scene_id)
        {
            string url = ApiUrl.GetCreateQrUrl(Config.AccessToken);
            string postData = QrCodeModel.CreateQrCodePostData(scene_id);
            string json = HttpHelper.PostJson(url, postData);

            ResultModel.CreateInstance(json).HasException();

            JObject jo = JObject.Parse(json);
            QrCodeResultModel rModel = jo.ToObject<QrCodeResultModel>();

            return rModel;
        }

        /// <summary>
        /// 创建字符型id永久二维码
        /// 如果每次使用的str id一样，生成的ticket就一样,图片一样，如果扫描，返回的str id都一样。
        /// </summary>
        /// <param name="scene_str">场景值ID（字符串形式的ID），字符串类型，长度限制为1到64，仅永久二维码支持此字段 </param>
        /// <returns></returns>
        public QrCodeResultModel CreateQrCodeByStr(string scene_str)
        {
            string url = ApiUrl.GetCreateQrUrl(Config.AccessToken);
            string postData = QrCodeModel.CreateQrCodeByStrPostData(scene_str);
            string json = HttpHelper.PostJson(url, postData);

            ResultModel.CreateInstance(json).HasException();

            JObject jo = JObject.Parse(json);
            QrCodeResultModel rModel = jo.ToObject<QrCodeResultModel>();

            return rModel;
        }

        #endregion

        #region 发送模板消息接口
        
        /// <summary>
        /// 发送模板消息 
        /// </summary>
        /// <param name="message"></param>
        private void _SendTemplate(TemplateMessage message)
        {
            string sendUrl = ApiUrl.GetSendTemplateUrl(message.AccessToken);

            string result = HttpHelper.PostJson(sendUrl, JsonConvert.SerializeObject(message));

            ResultModel.CreateInstance(result).HasException();
        }

        /// <summary>
        /// 发送模板消息 
        /// </summary>
        /// <param name="message"></param>
        private void SendTemplate(TemplateMessage message)
        {
            if (message.AccessToken == null)
            {
                message.AccessToken = Config.AccessToken;

            }
            _SendTemplate(message);
        }

        /// <summary>
        /// 发送模板消息 
        /// </summary>
        /// <param name="touser">用户openID，发给谁</param>
        /// <param name="template_id">模板ID</param>
        /// <param name="url">点详情时，跳转的URL</param>
        /// <param name="data">模板数据，以Key/Value的形式对应模板</param>
        public void SendTemplate(string touser,string template_id,string url, Dictionary<string, TemplateItem> data)
        {
            TemplateMessage message = new TemplateMessage();

            message.AccessToken = Config.AccessToken;
            message.touser = touser;
            message.template_id = template_id;
            message.url = url;
            message.data = data;

            SendTemplate(message);
        }


        #endregion
    }
}
