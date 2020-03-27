using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeiXinLib.Model.Send;

namespace WeiXinLib.Helper.Common
{
    /// <summary>
    /// 微信公众平台异常类
    /// </summary>
    public class WxPublicException : Exception
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="message">错误消息</param>
        public WxPublicException(string message): base(message)
        {
            
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="message">错误消息</param>
        public WxPublicException(int code, string message): base(message)
        {
            this.Code = code;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="resultModel">结果对象函数</param>
        public WxPublicException(ResultModel resultModel)
            : this(resultModel.errcode, resultModel.errmsg)
        {

        }

        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; set; }
        
    }
}
