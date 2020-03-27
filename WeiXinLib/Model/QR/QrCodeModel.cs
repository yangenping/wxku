using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiXinLib.Model.QR
{

    public class QrCodeModel{

        /*
        /// <summary>
        /// 该二维码有效时间，以秒为单位。 最大不超过2592000（即30天），此字段如果不填，则默认有效期为30秒。 
        /// </summary>
        public int? expire_seconds {get;set;}

        /// <summary>
        /// 二维码类型，QR_SCENE为临时,QR_LIMIT_SCENE为永久,QR_LIMIT_STR_SCENE为永久的字符串参数值 
        /// </summary>
        public string action_name { get; set; }

        /// <summary>
        /// 场景值ID，临时二维码时为32位非0整型，永久二维码时最大值为100000（目前参数只支持1--100000）
        /// </summary>
        public int? scene_id { get; set; }

        /// <summary>
        /// 场景值ID（字符串形式的ID），字符串类型，长度限制为1到64，仅永久二维码支持此字段 
        /// </summary>
        public string scene_str { get; set; }
         */

        /// <summary>
        ///  临时二维码
        ///  POST数据例子：{"expire_seconds": 604800, "action_name": "QR_SCENE", "action_info": {"scene": {"scene_id": 123}}}
        /// </summary>
        /// <param name="scene_id">场景值ID，临时二维码时为32位非0整型</param>
        /// <param name="expire_seconds">该二维码有效时间，以秒为单位。 最大不超过2592000（即30天），此字段如果不填，则默认有效期为30秒。 </param>
        /// <returns></returns>
        public static string CreateQrCodeTempPostData(int scene_id, int? expire_seconds = null)
        {
            var qrCodeModel = new
            {
                expire_seconds = expire_seconds,
                action_name = "QR_SCENE",
                action_info = new { scene = new { scene_id = scene_id } }
            };

            var jSetting = new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore };
            return Newtonsoft.Json.JsonConvert.SerializeObject(qrCodeModel, jSetting);

        }

        /// <summary>
        /// 永久二维码,数字形式的ID
        /// 无参数，POST数据例子：{"action_name": "QR_LIMIT_SCENE", "action_info": {"scene": {"scene_id": 123}}}
        /// </summary>
        /// <param name="scene_id">场景值ID，最大值为100000（目前参数只支持1--100000）</param>
        /// <returns></returns>
        public static string CreateQrCodePostData(int scene_id)
        {
            var qrCodeModel = new
            {
                action_name = "QR_LIMIT_SCENE",
                action_info = new { scene = new { scene_id = scene_id } }
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(qrCodeModel);
        }

        /// <summary>
        /// 永久二维码,字符串形式的ID
        /// 有参数，{"action_name": "QR_LIMIT_STR_SCENE", "action_info": {"scene": {"scene_str": "123"}}}
        /// </summary>
        /// <param name="scene_str">场景值ID（字符串形式的ID），字符串类型，长度限制为1到64，仅永久二维码支持此字段 </param>
        /// <returns></returns>
        public static string CreateQrCodeByStrPostData(string scene_str)
        {
            var qrCodeModel = new
            {
                action_name = "QR_LIMIT_STR_SCENE",
                action_info = new { scene = new { scene_str = scene_str } }
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(qrCodeModel);
        }

    }

    public class QrCodeResultModel
    {
        //正确的Json返回结果,示例: 
        //{"ticket":"gQH47joAAAAAAAAAASxodHRwOi8vd2VpeGluLnFxLmNvbS9xL2taZ2Z3TVRtNzJXV1Brb3ZhYmJJAAIEZ23sUwMEmm3sUw==","expire_seconds":60,"url":"http:\/\/weixin.qq.com\/q\/kZgfwMTm72WWPkovabbI"}

        //错误的Json返回示例:
        //{"errcode":40013,"errmsg":"invalid appid"}

        public string ticket { get; set; }
        public int? expire_seconds { get; set; }
        public string url { get; set; }
    }
}
