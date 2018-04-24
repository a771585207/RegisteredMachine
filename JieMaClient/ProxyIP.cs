using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace JieMaClient
{
    public class ProxyIP
    {
        private static readonly ProxyIP instance = new ProxyIP();

        // 显示的static 构造函数
        //没必要标记类型 - 在field初始化以前
        static ProxyIP()
        {

        }

        private ProxyIP()
        {

        }

        public static ProxyIP Instance
        {
            get
            {
                return instance;
            }
        }
        public struct Struct_INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        };
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        //重设代理地址
        public void RefreshIESettings(string strProxy)
        {
            const int INTERNET_OPTION_PROXY = 38;
            const int INTERNET_OPEN_TYPE_PROXY = 3;

            Struct_INTERNET_PROXY_INFO struct_IPI;

            // Filling in structure 
            struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
            struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
            struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");

            // Allocating memory 
            IntPtr intptrStruct = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI));

            // Converting structure to IntPtr 
            Marshal.StructureToPtr(struct_IPI, intptrStruct, true);

            bool iReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, Marshal.SizeOf(struct_IPI));
        }
        //重设代理地址
        public bool changeProxyIP()
        {
            string url = "http://webapi.http.zhimacangku.com/getip";
            string postDataStr = "num=1&type=2&pro=&city=0&yys=0&port=1&time=1&ts=1&ys=1&cs=1&lb=1&sb=0&pb=4&mr=1&regions=";
            string results = HttpSingleton.Instance.HttpGet(url, postDataStr);

            JObject jo = JObject.Parse(results);
            string success = jo["success"].ToString();
            if (success == "True")
            {
                string data = jo["data"].ToString();
                JArray ja = (JArray)JsonConvert.DeserializeObject(data);
                string ip = ja[0]["ip"].ToString();
                string port = ja[0]["port"].ToString();
                ProxyIP proxyIP = new ProxyIP();
                proxyIP.RefreshIESettings(ip + ":" + port);

                return true;
            }
            else
            {
                string msg = Utility.GetTextByKey(results, "msg");
                if (msg.Contains("设置为白名单"))
                {
                    //请将113.92.34.148设置为白名单！
                    int IndexofA = msg.IndexOf("将");
                    int IndexofB = msg.IndexOf("设");
                    string ip = msg.Substring(IndexofA + 1, IndexofB - IndexofA - 1);
                    string bai_url = "http://web.http.cnapi.cc/index/index/save_white";
                    string bai_postDataStr = "neek=41352&appkey=3198bd3864d13676ca7775d006f58c4b&white=" + ip;
                    string result = HttpSingleton.Instance.HttpGet(bai_url, bai_postDataStr);
                    return changeProxyIP();
                }
                else
                {
                    MessageBox.Show(msg, "提示", MessageBoxButtons.OK);
                }
            }
            return false;
        }
    }
}
