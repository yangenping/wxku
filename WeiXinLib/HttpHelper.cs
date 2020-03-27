using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;

namespace WeiXinLib
{
    /// <summary>
    /// Http操作工具类
    /// </summary>
    public class HttpHelper
    {
        /// <summary>
        /// 发起Http请求，并获取其响应结果，将响应结果以字符串形式返回。
        /// </summary>
        /// <param name="url">URL请求地址</param>
        /// <returns>响应结果</returns>
        public static string ScrapeWebPage(string url)
        {
            //System.Net.ServicePointManager.UseNagleAlgorithm = true;
            //System.Net.ServicePointManager.Expect100Continue = true;
            //System.Net.ServicePointManager.CheckCertificateRevocationList = true;
            //System.Net.ServicePointManager.DefaultConnectionLimit = System.Net.ServicePointManager.DefaultPersistentConnectionLimit;

            System.Net.HttpWebRequest request = null;
            System.Net.HttpWebResponse response = null;
            System.IO.Stream responseStream = null;
            System.IO.Stream requestStream = null;
            System.IO.StreamReader reader = null;
            string html = null;

            try
            {
                //create request (which supports http compression)
                request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                request.Headers = new System.Net.WebHeaderCollection();
                request.CookieContainer = new System.Net.CookieContainer();
                request.AllowAutoRedirect = true;
                request.Referer = url;
                request.Method = "GET";
                request.Accept = "*/*";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Pipelined = true;
                request.KeepAlive = true;
                request.Headers.Add(System.Net.HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
                request.Headers["Accept-Encoding"] = "gzip,deflate";

                //request.IfModifiedSince = DateTime.?;
                request.Timeout = 2000;

                //数据是否缓冲 false 提高效率 
                request.AllowWriteStreamBuffering = false;
                //最大连接数
                request.ServicePoint.ConnectionLimit = 65500;
                //是否使用 Nagle 不使用 提高效率
                request.ServicePoint.UseNagleAlgorithm = false;
                request.ServicePoint.Expect100Continue = false;

                //get response.
                response = (System.Net.HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                Encoding encode = Encoding.UTF8;
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                    responseStream = new System.IO.Compression.GZipStream(responseStream, System.IO.Compression.CompressionMode.Decompress);
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                    responseStream = new System.IO.Compression.DeflateStream(responseStream, System.IO.Compression.CompressionMode.Decompress);

                //read html.
                reader = new System.IO.StreamReader(responseStream, encode);
                html = reader.ReadToEnd();
            }
            catch
            {
                //throw;
            }
            finally
            {//dispose of objects.
                request = null;
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (requestStream != null)
                {
                    requestStream.Close();
                    requestStream.Dispose();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream.Dispose();
                }
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
            return html;
        }

        /// <summary>
        /// 将JSON字符串POST到某个URL地址上。
        /// </summary>
        /// <param name="url">目标URL</param>
        /// <param name="json">JSON字符串</param>
        /// <returns>返回通信响应原始结果</returns>
        public static string PostJson(string url,string json)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (Exception ex) 
            {
                return null;
            }
        }
    }
}
