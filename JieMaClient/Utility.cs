using mshtml;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JieMaClient
{
    class Utility
    {
        /// <summary>  
        /// 返回指定WebBrowser中图片<IMG></IMG>中的图内容  
        /// </summary>  
        /// <param name="WebCtl">WebBrowser控件</param>  
        /// <param name="ImgeTag">IMG元素</param>  
        /// <returns>IMG对象</returns>  
        public static Image GetWebImage(Form1 form, WebBrowser WebCtl, HtmlElement ImgeTag)
        {
            Image image = null;
            form.Invoke((EventHandler)(delegate
            {
                // 这里写你的控件代码，比如
                HTMLDocument doc = (HTMLDocument)WebCtl.Document.DomDocument;
                HTMLBody body = (HTMLBody)doc.body;
                IHTMLControlRange rang = (IHTMLControlRange)body.createControlRange();
                IHTMLControlElement Img = (IHTMLControlElement)ImgeTag.DomElement; //图片地址  

                Image oldImage = Clipboard.GetImage();
                rang.add(Img);
                rang.execCommand("Copy", false, null);  //拷贝到内存  
                image = Clipboard.GetImage();
                try
                {
                    Clipboard.SetImage(oldImage);
                }
                catch
                {

                }
            }
            ));
            //HTMLDocument doc = (HTMLDocument)WebCtl.Document.DomDocument;
            //HTMLBody body = (HTMLBody)doc.body;
            //IHTMLControlRange rang = (IHTMLControlRange)body.createControlRange();
            //IHTMLControlElement Img = (IHTMLControlElement)ImgeTag.DomElement; //图片地址  

            //Image oldImage = Clipboard.GetImage();
            //rang.add(Img);
            //rang.execCommand("Copy", false, null);  //拷贝到内存  
            //Image numImage = Clipboard.GetImage();
            //try
            //{
            //    Clipboard.SetImage(oldImage);
            //}
            //catch
            //{

            //}

            return image;
        }
        /// <summary>
        /// 根据关键字获取JSON数据里面的信息
        /// </summary>
        /// <param name="jsonText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetTextByKey(string jsonText, string key)
        {
            JObject jsonObj = JObject.Parse(jsonText);
            string str = jsonObj[key].ToString();
            return str;
        }
        //MD5
        public static string MD5String(string str)
        {
            if (str == "") return str;
            byte[] b = System.Text.Encoding.Default.GetBytes(str);
            return MD5String(b);
        }
        public static string MD5String(byte[] b)
        {
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }
    }
}
