﻿using Microsoft.Win32;
using mshtml;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JieMaClient
{
    class Utility
    {
        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        /// <summary>
        /// 使用InternetSetOption操作wininet.dll清除webbrowser里的cookie
        /// </summary>
        public static void SuppressWininetBehavior(int num)
        {
            //清除IE临时文件
            //ShellExecute(IntPtr.Zero, "open", "rundll32.exe", " InetCpl.cpl,ClearMyTracksByProcess " + num.ToString(), "", 0);
            //WebCtl.Document.Cookie.Remove(0, (WebCtl.Document.Cookie.Count() - 1));
        }
        [DllImport("wininet.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetOption(int hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public static unsafe void SuppressWininetBehavior()
        {
            int option = (int)3/* INTERNET_SUPPRESS_COOKIE_PERSIST*/;
            int* optionPtr = &option;
            bool success = InternetSetOption(0, 81/*INTERNET_OPTION_SUPPRESS_BEHAVIOR*/, new IntPtr(optionPtr), sizeof(int));
            if (!success)
            {
                MessageBox.Show("Something went wrong !>?");
            }
        }

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

            return image;
        }
        public static Image GetWebImage_test(WebBrowser WebCtl, string doucmentName)
        {
            HtmlElement ImgeTag = WebCtl.Document.All[doucmentName];
            HTMLDocument doc = (HTMLDocument)WebCtl.Document.DomDocument;
            HTMLBody body = (HTMLBody)doc.body;
            IHTMLControlRange rang = (IHTMLControlRange)body.createControlRange();
            IHTMLControlElement Img = (IHTMLControlElement)ImgeTag.DomElement; //图片地址  

            Image oldImage = Clipboard.GetImage();
            rang.add(Img);
            rang.execCommand("Copy", false, null);  //拷贝到内存  
            Image numImage = Clipboard.GetImage();
            try
            {
                Clipboard.SetImage(oldImage);
            }
            catch
            {

            }

            return numImage;
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
        public static void registryKeyCheck()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            if (key != null)
            {
                key.SetValue("JieMaClient.exe", 11001, RegistryValueKind.DWord);
                key.SetValue("JieMaClient.vshost.exe", 11001, RegistryValueKind.DWord);//调试运行需要加上，否则不起作用
            }

            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            if (key != null)
            {
                key.SetValue("JieMaClient.exe", 11001, RegistryValueKind.DWord);
                key.SetValue("JieMaClient.vshost.exe", 11001, RegistryValueKind.DWord);//调试运行需要加上，否则不起作用
            }
        }
    }
}
