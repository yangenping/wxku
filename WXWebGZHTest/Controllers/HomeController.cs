using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using WeiXinLib;
using System.IO;
namespace WXWebGZHTest.Controllers
{
    public class HomeController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get|HttpVerbs.Post)]
        public ActionResult Index()
        {


          string s=  MenuMgr.CreateBtn();

            LogFactory.GetLog(typeof(HomeController)).Info("接收");
            if (HttpContext.Request.HttpMethod == "GET")
            { 
               var v= Verify.CreateVerifyModel(HttpContext.ApplicationInstance.Context);
                if (v.Signature == null)
                {
                    return Content("null");
                }
                //  v.Token = WXTokenHelper.GetTokenBYXml();
                v.Token = ConfigManager.GetConfigFirst().Token;
                if (Verify.IsFromWeiXin(v))
                {
                    return Content(v.Echostr);
                }
                return Content("false");
            }
            else if (HttpContext.Request.HttpMethod == "POST") {

                WXPost p = new WXPost();
                p.ProcessRequest(this.HttpContext.ApplicationInstance.Context);
            }
            return Content("");

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}