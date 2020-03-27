using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace WeiXinLib.Model.Send
{
    /// <summary>
    /// 微信公众返回的结果数据。
    /// </summary>
    public class ResultModel
    {
        //{"errcode":0,"errmsg":"ok"}

        /// <summary>
        /// 错误码
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 原始字符串
        /// </summary>
        public string originalResult { get; set; }

        public ResultModel ()
        {

        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="result">结果字符串</param>
        /// <returns></returns>
        public static ResultModel CreateInstance(string result)
        {
            JObject jo = JObject.Parse(result);
            ResultModel resultModel = jo.ToObject<ResultModel>();
            resultModel.originalResult = result;
            return resultModel;
        }

        /// <summary>
        /// 如果是异常结果（代表失败的结果），将抛出异常。
        /// </summary>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public void HasException()
        {
            if (IsException() == true)
            {
                throw new WeiXinLib.Helper.Common.WxPublicException(this);
            }
        }

        /// <summary>
        /// 判断这个结果，是否是异常结果（代表失败的结果）
        /// </summary>
        /// <returns></returns>
        public bool IsException()
        {
            if (this.errcode != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
