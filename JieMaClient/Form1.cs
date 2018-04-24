using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using mshtml;

namespace JieMaClient
{
    public partial class Form1 : Form
    {
        BaseProject baseProject = new BaseProject();
        MainDelegate mainDelegate = new MainDelegate();

        public string phonePlatform;
        public string userName;
        public string token;
        public int webLoadCount;
        public ProjectInfo projectInfo;

        //test url
        public string testUrl = "";

        //预建好相关需要
        public static ProjectInfo[] projectInfos = {
            new ProjectInfo{name =  "BXP", pid = "48997|16531", webLoadTotalCount = 1, url = "http://www.bitcquan.com/Register?parentId=110367"},
            new ProjectInfo{name =  "NPC", pid = "44560|14957", webLoadTotalCount = 4, url = "http://1aau.com/i/411129248"},
            new ProjectInfo{name =  "ShareWallet", pid = "45673|15335", webLoadTotalCount = 1, url = "https://ssl.zc.qq.com/v3/index-chs.html"},
            new ProjectInfo{name =  "QQ", pid = "8141|15335", webLoadTotalCount = 1, url = "https://ssl.zc.qq.com/v3/index-chs.html"},
        };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //初始化
            button1.Enabled = true;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            button4.Enabled = false;
            //让执行脚本错误停止弹出
            webBrowser1.ScriptErrorsSuppressed = true;
            //加载项目列表
            for (int i = 0; i < projectInfos.Count(); i++)
            {
                comboBox1.Items.Add(projectInfos[i].name);
            }
            comboBox1.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            baseProject.init(this);
        }

        //登录
        private void button1_Click(object sender, EventArgs e)
        {
            string Url = "";
            string postDataStr = "";
            switch (phonePlatform)
            {
                case "速码":
                    {
                        Url = "http://api.eobzz.com/httpApi.do";
                        postDataStr = "action=loginIn&uid=" + sumaUser.Text + "&pwd=" + sumaPassword.Text;
                    }
                    break;
                case "易码":
                    {
                        Url = "http://api.fxhyd.cn/UserInterface.aspx";
                        postDataStr = "action=login&username=" + sumaUser.Text + "&password=" + sumaPassword.Text;
                    }
                    break;
                default:
                    break;
            }
            string retStrings = HttpSingleton.Instance.HttpGet(Url, postDataStr);
            string[] retString = retStrings.Split('|');
            if (retString[0] == sumaUser.Text || retString[0] == "success")
            {
                //成功，开放下一步操作
                button1.Text = "登录成功";
                button1.Enabled = false;
                comboBox4.Enabled = false;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                button4.Enabled = true;
                sumaUser.Enabled = false;
                sumaUser.Enabled = false;
                userName = sumaUser.Text;
                token = retString[1];
            }
            else
            {
                button1.Text = "登录失败";
            }
        }
        //自动注册
        private void button4_Click(object sender, EventArgs e)
        {
            //禁止重新选择项目
            //comboBox1.Enabled = false;

            webLoadCount = 0;
            string url = testUrl.Equals("") ? projectInfo.url : testUrl;
            //改变代理IP
            if(!ProxyIP.Instance.changeProxyIP())
            {
                return;
            }
            //打开网页
            webBrowser1.Navigate(url);
        }
        //网页
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webLoadCount++;
            System.Console.WriteLine("Hello world!");
            ((WebBrowser)sender).Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);
            if (webLoadCount == projectInfo.webLoadTotalCount)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(baseProject.autoRegister), projectInfo.name);
            }
        }
        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            // Ignore the error and suppress the error dialog box. 
            e.Handled = true;
        }
        //开打网页按钮
        private void button2_Click(object sender, EventArgs e)
        {
            webLoadCount = 0;
            string url = testUrl.Equals("") ? projectInfo.url : testUrl;
            //改变代理IP
            //ProxyIP.Instance.changeProxyIP();
            projectInfo.webLoadTotalCount = 1000;

            webBrowser1.Navigate(url);
        }
        //测试按钮
        private void button3_Click(object sender, EventArgs e)
        {
            
        }
        //手机接码平台
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            //0：速码，1：易码
            phonePlatform = comboBox4.Text;
        }
        //项目选择
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            projectInfo = projectInfos[comboBox1.SelectedIndex];
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        //UI委托
        public void ControlDelegate(string type, Control control, params object[] objs)
        {
            try
            {
                Invoke(mainDelegate._mainDelegate, type, control, objs);
            }
            catch { }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            testUrl = ((TextBox)sender).Text; //ToString();
        }

        private void sumaUser_TextChanged(object sender, EventArgs e)
        {

        }

        private void sumaPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
        //定时器
        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
