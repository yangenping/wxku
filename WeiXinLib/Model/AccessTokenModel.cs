using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiXinLib.Model
{
    /// <summary>
    /// 受权令牌实体类
    /// access_token是公众号的全局唯一票据，公众号调用各接口时都需使用access_token。
    /// 正常情况下access_token有效期为7200秒，重复获取将导致上次获取的access_token失效。
    /// 由于获取access_token的api调用次数非常有限，建议开发者全局存储与更新access_token，频繁刷新access_token会导致api调用受限，影响自身业务。
    /// </summary>
    public class AccessTokenModel
    {
        //{"access_token":"ACCESS_TOKEN","expires_in":7200}
        //{"errcode":40013,"errmsg":"invalid appid"}
        
        public string access_token;
        public int? expires_in;
        public int errcode = 0;
        public string errmsg;

        public string appId ;
    }
}
