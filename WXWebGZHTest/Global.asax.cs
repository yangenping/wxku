using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WeiXinLib;

namespace WXWebGZHTest
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
       
            LogFactory.LogInitialize();
            var log = LogFactory.GetLog(typeof(MvcApplication));
            log.Info("程序启动！");

     
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            WeiXinLib.Model.ConfigModel config = new WeiXinLib.Model.ConfigModel();
            config.AppId = "wx5dcd4ac4d2b015b7";
            config.AppSecret = "b7f1f039925663fb84f649a8d105e683";
            
            config.Token = "yangenping";
            WeiXinLib.ConfigManager.SetConfig(config);
            
        }
    }
}
