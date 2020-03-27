using Linkup.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeiXinLib.Model.Menus;

namespace WeiXinLib
{
    public class MenuMgr
    {
        public static string CreateBtn()
        {
            string url = ApiUrl.URL_CREATE_MENU;
            string token = WXTokenHelper.GetTokenBYXml();
            MenuModel m = new MenuModel();
            m.button = new List<ButtonModel>();
            m.button.Add(new ClickButtonModel("功能1", "a"));
            m.button.Add(new ClickButtonModel("功能2", "b"));
            ButtonModel s = new ButtonModel("功能3");
                s.sub_button.Add(new ClickButtonModel("sub1", "z1"));
            s.sub_button.Add(new ClickButtonModel("sub2", "z2"));
            m.button.Add(s);
            string mumes = JsonHelper.Serializer(m);
            return HttpHelper.PostJson(url + token, mumes);
        }
    }
}
