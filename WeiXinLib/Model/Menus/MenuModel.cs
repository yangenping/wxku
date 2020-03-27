using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiXinLib.Model.Menus
{
    /// <summary>
    /// 基础菜单按钮实体类
    /// 它不属于任何类型.
    /// 它是基础按钮，可以有子节点.
    /// </summary>
    public class ButtonModel
    {
        /// <summary>
        /// 点击型(菜单的响应动作类型)
        /// </summary>
        public const string TYPE_CLICK = "click";
        /// <summary>
        /// 跳转型(菜单的响应动作类型)
        /// </summary>
        public const string TYPE_VIEW = "view";

        /// <summary>
        /// 构造默认的按钮
        /// </summary>
        public ButtonModel() { }

        /// <summary>
        /// 用名称构造菜单按钮
        /// </summary>
        /// <param name="name"></param>
        public ButtonModel(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 名称
        /// 一级菜单最多4个汉字
        /// 二级菜单最多7个汉字，多出来的部分将会以“...”代替
        /// (菜单标题，不超过16个字节，子菜单不超过40个字节 )
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 默认无类型。
        /// 菜单的响应动作类型：click或view
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 特定类型的按钮不可以有子节点。
        /// 子节点.二级菜单数组，个数应为1~5个 
        /// </summary>
        public List<ButtonModel> sub_button { get; set; }

    }

    /// <summary>
    /// 点击型的按钮
    /// </summary>
    public class ClickButtonModel : ButtonModel
    {
        public ClickButtonModel()
        {
            this.type = ButtonModel.TYPE_CLICK;
        }

        public ClickButtonModel(string name, string key)
            : this()
        {
            this.name = name;
            this.key = key;
        }

        /// <summary>
        /// 菜单KEY值，用于消息接口推送，不超过128字节 
        /// </summary>
        public string key { get; set; }
    }

    /// <summary>
    /// 跳转型的按钮
    /// </summary>
    public class ViewButtonModel : ButtonModel
    {
        public ViewButtonModel()
        {
            this.type = ButtonModel.TYPE_VIEW;
        }

        public ViewButtonModel(string name, string url)
            : this()
        {
            this.name = name;
            this.url = url;
        }

        /// <summary>
        /// 菜单的URL.view类型必须
        /// 网页链接，用户点击菜单可打开链接，不超过256字节 
        /// </summary>
        public string url { get; set; }
    }

    /// <summary>
    /// 菜单实体类
    /// 它只包含第一级菜单按钮组(根节点).
    /// </summary>
    public class MenuModel
    {
        /// <summary>
        /// 第一级菜单按钮组(根节点)，个数应为1~3个 
        /// </summary>
        public List<ButtonModel> button { get; set; }
    }

}
