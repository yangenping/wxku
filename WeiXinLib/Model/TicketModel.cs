using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiXinLib.Model
{
    /// <summary>
    /// 基础票据Model
    /// </summary>
    public class TicketModel
    {
        public string ticket;
        public int? expires_in;
        public int errcode = 0;
        public string errmsg;
        public string appId;

    }

    /// <summary>
    /// jsapi_ticket是公众号用于调用微信JS接口的临时票据。
    /// 正常情况下，jsapi_ticket的有效期为7200秒，通过access_token来获取。
    /// 由于获取jsapi_ticket的api调用次数非常有限，频繁刷新jsapi_ticket会导致api调用受限，影响自身业务，开发者必须在自己的服务全局缓存jsapi_ticket 。 
    /// </summary>
    public class JsApiTicketModel : TicketModel
    {
        //成功{"errcode":0,"errmsg":"ok","ticket":"bxLdikRXVbTPdHSM05e5u5sUoXNKd8-41ZO3MhKoyN5OfkWITDGgnr2fwJ0m9E8NYzWKVZvdVtaUgWvsdshFKA","expires_in":7200}
        //失败{"errcode":40001,"errmsg":"invalid credential, access_token is invalid or not latest hint: [v9sQfa0315vr22]"}

    }

    /// <summary>
    /// 卡券 api_ticket 是用于调用卡券相关接口的临时票据，有效期为 7200 秒，通过 access_token 来获取。
    /// 这里要注意与 jsapi_ticket 区分开来。由于获取卡券 api_ticket 的 api 调用次数非常有限，
    /// 频繁刷新卡券 api_ticket 会导致 api 调用受限，影响自身业务，开发者必须在自己的服务全局缓存卡券 api_ticket 。 
    /// </summary>
    public class ApiTicketModel : TicketModel
    {

        //成功{"errcode":0,"errmsg":"ok","ticket":"bxLdikRXVbTPdHSM05e5u5sUoXNKdvsdshFKA","expires_in":7200}
        //失败{"errcode":40001,"errmsg":"invalid credential, access_token is invalid or not latest hint: [sdc0200vr23]"}

    }
}
