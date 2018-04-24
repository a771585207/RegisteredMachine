﻿using mshtml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JieMaClient
{
    public struct ProjectInfo
    {
        public string name;
        public string pid;
        public int webLoadTotalCount;
        public string url;
    };
    class BaseProject
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hWnd, out RECT lpRect);
        const int MOUSEEVENTF_MOVE = 0x0001; //移动鼠标
        const int MOUSEEVENTF_LEFTDOWN = 0x0002; //模拟鼠标左键按下
        const int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008; //模拟鼠标右键按下
        const int MOUSEEVENTF_RIGHTUP = 0x0010; //模拟鼠标右键抬起
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //模拟鼠标中键按下
        const int MOUSEEVENTF_MIDDLEUP = 0x0040; //模拟鼠标中键抬起
        const int MOUSEEVENTF_ABSOLUTE = 0x8000; //标示是否采用绝对坐标
        [DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int mouse_event(int dwFlags, uint dx, uint dy, uint cButtons, int dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        [DllImport("user32.dll", EntryPoint = "GetKeyboardState")]
        public static extern int GetKeyboardState(byte[] pbKeyState);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("User32")]
        public static extern IntPtr GetDC(IntPtr h);
        [DllImport("gdi32")]
        public static extern uint GetPixel(IntPtr h, Point p);

        /// <summary>
        /// 属性
        /// </summary>
        public IntPtr _ptr;
        public Form1 _form;
        public float width;
        public float height;
        public string path;
        public List<string> phoneNumbers = new List<string>();
        public List<string> passwords = new List<string>();

        public BaseProject()
        {
            _ptr = default(IntPtr);
            Console.WriteLine("BaseProject");
        }
        public bool init(Form1 form)
        {
            bool flag = false;
            _form = form;
            width = _form.Bounds.Width;
            height = _form.Bounds.Height;

            _ptr = FindWindow(null, "Form1");
            if (_ptr.ToInt32() != 0)
            {
                MoveWindow(_ptr, 0, 0, (int)width, (int)height, true);
                //显示并激活窗体
                ShowWindow(_ptr, 5);
                SetForegroundWindow(_ptr);
                flag = true;
            }

            return flag;
        }
        //根据项目自动注册
        public void autoRegister(object obj)
        {
            string projectName = obj.ToString();
            switch (projectName)
            {
                case "NPC":
                    {
                        NPC_Project npc = new NPC_Project();
                        npc.init(_form);
                        npc.path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + projectName;
                        npc.autoRegister();
                    }
                    break;
                case "BXP":
                    {
                        BXP_Project bxp = new BXP_Project();
                        bxp.init(_form);
                        bxp.path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + projectName;
                        bxp.autoRegister();
                    }
                    break;
                default:
                    break;
            }
        }
        //获取相应项目的手机号
        public string getPhoneNumber()
        {
            //获取手机号
            string Url = "http://api.eobzz.com/httpApi.do";
            string postDataStr = "action=getMobilenum&pid=" + _form.projectInfo.pid + "&uid=" + _form.userName + "&token=" + _form.token + "&mobile=&size=1";//&province=广东&phoneType=CMCC";
            string retStrings = HttpSingleton.Instance.HttpGet(Url, postDataStr);
            string phoneNumber = "";
            if (retStrings.Contains(_form.token))
            {
                phoneNumber = retStrings.Split('|')[0];
                return phoneNumber;
            }
            else
            {
                MessageBox.Show("速码平台没钱了", "提示", MessageBoxButtons.OK);
            }
            return "";
        }
        //获取随机密码
        public string getRandPassword()
        {
            //随机密码
            string pool = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";//abcdefghijklmnopqrstuvwxyz
            string password = "";
            Random rand = new Random();
            int number = rand.Next(8, 12);
            for (int i = 0; i < number; i++)
            {
                int num = rand.Next(0, 35);
                password += pool[num];
            }
            return password;
        }
        //保存账号密码
        public void saveUserPassword(string user, string password)
        {
            //获取和设置包含该应用程序的目录的名称。(推荐)
            path += "\\records.txt";
            if (!System.IO.File.Exists(path))
            {
                FileStream file = System.IO.File.Create(path);
                file.Dispose();
            }
            string[] lines1 = System.IO.File.ReadAllLines(path);
            string[] lines2 = new string[lines1.Count() + 1];
            for (int i = 0; i < lines1.Count(); i++)
            {
                lines2[i] = lines1[i];
            }
            lines2[lines1.Count()] = _form.projectInfo.name + ":账号 = " + user + " 密码 = " + password;
            System.IO.File.WriteAllLines(path, lines2);
        }
        //测试按钮
        public string getCheckCode(string doucmentName)
        {
            //取得验证码
            HtmlElement ImgeTag = null;
            _form.Invoke((EventHandler)(delegate
            {
                // 这里写你的控件代码，比如
                ImgeTag = _form.webBrowser1.Document.All[doucmentName];
            }
            ));
            Image image = Utility.GetWebImage(_form, _form.webBrowser1, ImgeTag);
            string pathCheckCode = path;
            pathCheckCode += "\\checkCode.png";
            image.Save(pathCheckCode);
            //pictureBox1.Image = image;
            //填写验证码
            string checkCode = "";
            string str = NetRecognizePic.CJY_RecognizeFile(pathCheckCode.Trim(), "a771585207", Utility.MD5String("a395583102"), "895186", "1005", "0", "0", "");
            string strerr = Utility.GetTextByKey(str, "err_str");
            if (strerr == "OK")
            {
                checkCode = Utility.GetTextByKey(str, "pic_str");
                checkCode = checkCode.ToUpper();
            }
            else
            {
                MessageBox.Show("超级鹰验证码识别平台没钱", "提示", MessageBoxButtons.OK);
            }
            return checkCode;
        }
        //在文本控件内输入字符
        public void input_str(float x, float y, string str)
        {
            left_click(x, y);
            for (int i = 0; i < str.Length; i++)
            {
                keybd_click(str[i]);
            }
        }
        //左键点击
        public bool left_click(float x, float y)
        {
            if (_ptr.ToInt32() == 0)
            {
                return false;
            }
            RECT rect = new RECT();
            GetWindowRect(_ptr, out rect);
            x = rect.left + x;
            y = rect.top + y;

            Thread.Sleep(50);
            SetCursorPos((int)x, (int)y);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, 0);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);

            return true;
        }
        //键盘点击
        public bool keybd_click(char str)
        {
            if (_ptr.ToInt32() == 0)
            {
                return false;
            }
            Thread.Sleep(50);
            Keys k = (Keys)str;
            keybd_event((byte)k, 0, 0, 0);
            keybd_event((byte)k, 0, 2, 0);

            return true;
        }
        //是否大写状态
        public void checkCapsLockStatus()
        {
            byte[] bs = new byte[256];
            GetKeyboardState(bs);
            bool flag = (bs[0x14] == 1);
            if (!flag)
            {
                Thread.Sleep(50);
                keybd_event((byte)Keys.CapsLock, 0, 0, 0);
                keybd_event((byte)Keys.CapsLock, 0, 2, 0);
            }
        }
    }
}