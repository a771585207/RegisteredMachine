using mshtml;
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
    class BXP_Project : BaseProject
    {

        public BXP_Project()
        {
            Console.WriteLine("BXP_Project");
        }
        //自动注册
        public void autoRegister()
        {
            //先把大写打开
            checkCapsLockStatus();
            //手机号
            if(!autoPhone(186, 177))
            {
                return;
            }
            //验证码检测
            if(!autoCheckCode(89, 221))
            {
                return;
            }
            //密码
            autoPassword(193, 338);
            //手机验证码
            autoVcode(168, 290);
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
        //验证码检测
        public bool autoCheckCode(float x, float y)
        {
            //取得验证码
            string checkCode = getCheckCode("GetCodePic");
            if (checkCode != "")
            {
                //填写验证码
                input_str(x, y, checkCode);
                return true;
            }
            return false;
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
                input_str(x, y, password);
            }
        }
        //手机验证码
        public void autoVcode(float x, float y)
        {
            //点击获取验证码
            left_click(1608, 290);
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

            //【币讯】您本次操作验证码:854348
            string sms = getCode(userName, token, phone);
            if (sms != "")
            {
                string[] split = sms.Split(':');
                string vcode = split[1];
                //填写验证码
                input_str(x, y, vcode);
                //点击注册按钮
                left_click(838, 405);
                //保存账号密码
                saveUserPassword(phone, password);
                _form.ControlDelegate("TextBox", _form.textBox1, sms);
            }
            else
            {
                MessageBox.Show("验证码获取超时", "提示", MessageBoxButtons.OK);
            }
        }
    }
}
