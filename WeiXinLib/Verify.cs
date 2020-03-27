using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;
using WeiXinLib.Model;
using System.Web.Security;

namespace WeiXinLib
{
    /// <summary>
    /// 微信通信消息校验类
    /// </summary>
    public class Verify
    {
        /// <summary>
        /// 从Http请求中获取并构造验证数据。
        /// </summary>
        /// <param name="context">Http请求实例</param>
        /// <returns>验证数据实体类</returns>
        public static VerifyModel CreateVerifyModel(HttpContext context)
        {
            //平时,与验证有关的参数,只有这3个.
            string signature = context.Request["signature"];
            string timestamp = context.Request["timestamp"];
            string nonce = context.Request["nonce"];
            //当提交申请接入时,与验证有关的参数,会多出一个echostr. 
            string echostr = context.Request["echostr"];

            string encrypt_type = context.Request["encrypt_type"];
            string msg_signature = context.Request["msg_signature"];

            VerifyModel verifyModel = new VerifyModel();

            verifyModel.Signature = signature;
            verifyModel.Timestamp = timestamp;
            verifyModel.Nonce = nonce;
            verifyModel.Echostr = echostr;

            return verifyModel;
        }

        /// <summary>
        /// 使用Http请求与使用token令牌构造验证数据。
        /// </summary>
        /// <param name="context">Http请求实例</param>
        /// <param name="token">令牌</param>
        /// <returns>验证数据实体类</returns>
        public static VerifyModel CreateVerifyModel(HttpContext context, string token)
        {
            VerifyModel verifyModel = CreateVerifyModel(context);
            verifyModel.Token = token;
            return verifyModel;
        }

        /// <summary>
        /// (附带Http响应输出)验证该请求是否来自微信。
        /// </summary>
        /// <param name="context">Http请求实例</param>
        /// <param name="token">令牌</param>
        /// <returns>是true/否false</returns>
        public static bool IsFromWeiXin(HttpContext context, VerifyModel verifyModel)
        {
            if (Verify.IsFromWeiXin(verifyModel) == false)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("欢迎访问久邻接口,请问你是微信吗?");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 验证VerifyModel的值，是否正常。
        /// </summary>
        /// <param name="verifyModel"></param>
        /// <returns></returns>
        public static bool VerifyModelValues(VerifyModel verifyModel)
        {
            if ((string.IsNullOrWhiteSpace(verifyModel.Signature) && string.IsNullOrWhiteSpace(verifyModel.MsgSignature)) //两个Signature都为空，就不应该了。
                || string.IsNullOrWhiteSpace(verifyModel.Token)
                || string.IsNullOrWhiteSpace(verifyModel.Timestamp)
                || string.IsNullOrWhiteSpace(verifyModel.Nonce))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 验证VerifyModel的值，是否正常。并且带http输出。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="verifyModel"></param>
        /// <returns></returns>
        public static bool VerifyModelValues(HttpContext context, VerifyModel verifyModel)
        {
            if (VerifyModelValues(verifyModel) == false)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 验证该请求是否来自微信。
        /// </summary>
        /// <param name="verifyModel"></param>
        /// <returns></returns>
        public static bool IsFromWeiXin(VerifyModel verifyModel)
        {
            return Verify.IsFromWeiXin(verifyModel.Signature, verifyModel.Token, verifyModel.Timestamp, verifyModel.Nonce); 
        }

        /// <summary>
        /// 验证该请求是否来自微信.
        /// </summary>
        /// <param name="signature">微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。 </param>
        /// <param name="token">之前配置过的Token</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <returns>是true/否false</returns>
    
        public static bool IsFromWeiXin(string signature,string token,string timestamp, string nonce)
        {
            string codingStr = CodingSinature(token, timestamp, nonce);

            //3. 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信
            if (signature.ToUpper().Equals(codingStr.ToUpper()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否首次接入，是否请求接入。
        /// </summary>
        /// <param name="echostr">回音/暗号</param>
        /// <returns>是true/否false</returns>
        public static bool IsRequestJoin(string echostr)
        {
            //当提交申请接入时,与验证有关的参数,会多出一个echostr. 
            //不为空时，就是请求接入。
            if (string.IsNullOrWhiteSpace(echostr) == false) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// 将token、timestamp、nonce三个参数进行加密
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <returns>加密后的字符串形式</returns>
        public static string CodingSinature(string token, string timestamp, string nonce)
        {
            //加密/校验流程如下：
            //1. 将token、timestamp、nonce三个参数进行字典序排序
            //2. 将三个参数字符串拼接成一个字符串进行sha1加密
            //3. 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信

            List<string> list = new List<string>();
            list.Add(token);
            list.Add(timestamp);
            list.Add(nonce);
            list.Sort();

            string codingStr = list[0] + list[1] + list[2];


            codingStr = CalculateSHA1(codingStr);

            return codingStr;

        }

        /// <summary>
        /// 将token、timestamp、nonce三个参数进行加密
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string CodingSinature(HttpContext context, string token)
        {
            VerifyModel verifyModel = CreateVerifyModel(context, token);
            return CodingSinature(verifyModel);
        }

        /// <summary>
        /// 将token、timestamp、nonce三个参数进行加密
        /// </summary>
        /// <param name="verifyModel"></param>
        /// <returns></returns>
        public static string CodingSinature(VerifyModel verifyModel)
        {
            return CodingSinature(verifyModel.Token, verifyModel.Timestamp, verifyModel.Nonce);
        }

        /// <summary>
        /// 对字符串进行SHA1加密
        /// </summary>
        /// <param name="text">原字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string CalculateSHA1(string text)
        {
            return CalculateSHA1(text, Encoding.ASCII);
        }

        /// <summary>
        /// 对字符串进行SHA1加密
        /// </summary>
        /// <param name="text">原字符串</param>
        /// <param name="encoding">原字符的编码格式</param>
        /// <returns>加密后的字符串</returns>
        public static string CalculateSHA1(string text, Encoding encoding)
        {
            byte[] buffer = encoding.GetBytes(text);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            string tmpStr = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");


           //var  tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(text, "SHA1");
           //  tmpStr = tmpStr.ToLower();
            return tmpStr;
        }

        /// <summary>
        /// 消息模式(明文/密文)，表示消息是否加密(是否密文)。
        /// </summary>
        /// <param name="verifyModel">验证对象</param>
        /// <returns>是/true 否/false</returns>
        public static bool IsEncrypt(VerifyModel verifyModel)
        {
            return IsEncrypt(verifyModel.EncryptType);
        }

        /// <summary>
        /// 消息模式(明文/密文)，表示消息是否加密(是否密文)。
        /// </summary>
        /// <param name="encryptType">加密标识</param>
        /// <returns>是/true 否/false</returns>
        public static bool IsEncrypt(string encryptType)
        {
            
            // 加密类型。
            // url上无encrypt_type参数或者其值为raw时表示为不加密；
            // encrypt_type为aes时，表示aes加密（暂时只有raw和aes两种值)。
            // 公众帐号开发者根据此参数来判断微信公众平台发送的消息是否加密。 
           
            if (string.IsNullOrWhiteSpace(encryptType)
                || MessageCrypt.ENCRYPT_TYPE_RAW == encryptType )
            {
                return false;
            }
            else if (MessageCrypt.ENCRYPT_TYPE_AES == encryptType)
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
