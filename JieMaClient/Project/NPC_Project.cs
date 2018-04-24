using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JieMaClient
{
    class NPC_Project : BaseProject
    {
        public NPC_Project()
        {
            Console.WriteLine("NPC_Project");
        }
        //自动注册
        public void autoRegister()
        {
            //先把大写打开
            checkCapsLockStatus();
            //手机号
            if(!autoPhone(1050, 430))
            {
                return;
            }
            //人机识别
            autoIdentify(1050, 500);
            //密码
            autoPassword(1050, 610);
            //验证码
            autoVcode(1050, 550);
        }
        //手机号
        public bool autoPhone(float x, float y)
        {
            string phoneNumber = getPhoneNumber();
            if (phoneNumber != "")
            {
                //添加到手机号列表
                phoneNumbers.Add(phoneNumber);
                _form.ControlDelegate("ComboBox", _form.comboBox2, phoneNumber);
                //填写手机号
                input_str(x, y, phoneNumber);
                return true;
            }
            return false;
        }
        //人机识别
        public void autoIdentify(float x, float y)
        {
            //获取识别图片
            left_click(x, y);
            //延迟1.5秒后才获取图片
            Thread.Sleep(1500);
            //全屏截图
            int iWidth = Screen.PrimaryScreen.Bounds.Width;
            int iHeight = Screen.PrimaryScreen.Bounds.Height;
            Bitmap bit = new Bitmap(iWidth, iHeight);
            Graphics gc = Graphics.FromImage(bit);
            gc.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(iWidth, iHeight));
            //bit.Save("C:/Users/DELL/Desktop/bit.png");
            //智能检测
            List<Point> yuan1 = GraphicsProcessor.Instance.FindPicture(path + "\\yuan1.png", bit, new Rectangle(1000, 430, 1350 - 1000, 464 - 430), 0);
            List<Point> xing1 = GraphicsProcessor.Instance.FindPicture(path + "\\xing1.png", bit, new Rectangle(1000, 430, 1350 - 1000, 464 - 430), 0);
            List<Point> fang1 = GraphicsProcessor.Instance.FindPicture(path + "\\fang1.png", bit, new Rectangle(1000, 430, 1350 - 1000, 464 - 430), 0);
            List<Point_Struct> pointList = new List<Point_Struct>();
            if (yuan1.Count > 0)
            {
                Point_Struct ps = new Point_Struct();
                ps.name = path + "\\yuan2.png";
                ps.point = yuan1[0];
                pointList.Add(ps);
            }
            if (xing1.Count > 0)
            {
                Point_Struct ps = new Point_Struct();
                ps.name = path + "\\xing2.png";
                ps.point = xing1[0];
                pointList.Add(ps);
            }
            if (fang1.Count > 0)
            {
                Point_Struct ps = new Point_Struct();
                ps.name = path + "\\fang2.png";
                ps.point = fang1[0];
                pointList.Add(ps);
            }
            pointList.Sort(new PointComparer());
            for (int i = 0; i < pointList.Count; i++)
            {
                List<Point> points = GraphicsProcessor.Instance.FindPicture(pointList[i].name, bit, new Rectangle(1000, 271, 1299 - 1000, 430 - 271), 50);
                if (points.Count > 0)
                {
                    left_click(points[0].X, points[0].Y);
                }
            }
            //关闭
            gc.Dispose();
        }
        //密码
        public void autoPassword(float x, float y)
        {
            string password = getRandPassword();
            if (password != "")
            {
                //加入密码list
                passwords.Add(password);
                _form.ControlDelegate("ComboBox", _form.comboBox3, password);
                //填写密码
                left_click(550, y);
                Thread.Sleep(500);
                input_str(x, y, password);
            }
        }
        //验证码
        public void autoVcode(float x, float y)
        {
            //获取验证码
            _form.ControlDelegate("TextBox", _form.textBox1, "验证码获取中...");

            List<string> infoList = new List<string>();
            infoList.Add(_form.userName);
            infoList.Add(_form.token);
            infoList.Add(x.ToString());
            infoList.Add(y.ToString());
            ThreadPool.QueueUserWorkItem(new WaitCallback(getVcodeAndReleaseMobile), infoList);
        }
        private void getVcodeAndReleaseMobile(object obj)
        {
            List<string> infoList = (List<string>)obj;
            string userName = infoList[0];
            string token = infoList[1];
            float x = float.Parse(infoList[2]);
            float y = float.Parse(infoList[3]);
            string phone = phoneNumbers[phoneNumbers.Count - 1];
            string password = passwords[passwords.Count - 1];
            string retStrings = "";
            int count = 0;
            while (true)
            {
                string Url = "http://api.eobzz.com/httpApi.do";
                string postDataStr = "action=getVcodeAndReleaseMobile&uid=" + userName + "&token=" + token + "&mobile=" + phone;
                retStrings = HttpSingleton.Instance.HttpGet(Url, postDataStr);
                if (retStrings.Contains(phone))
                {
                    //解析验证码短信内容----【NPC】您的 NPC 验证码 : 1925
                    string sms = retStrings.Split('|')[1];
                    string[] split = sms.Split(' ');
                    string vcode = split[4];
                    //填写验证码
                    input_str(x, y, vcode);
                    //点击注册按钮
                    left_click(1050, 766);
                    //保存账号密码
                    saveUserPassword(phone, password);
                    _form.ControlDelegate("TextBox", _form.textBox1, sms);
                    break;
                }
                else
                {
                    count++;
                    string sms = "第" + count.ToString() + "次获取验证码";
                    _form.ControlDelegate("TextBox", _form.textBox1, sms);
                    Thread.Sleep(3000);
                }
            }
        }
    }
}
