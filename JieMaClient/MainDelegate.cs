using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JieMaClient
{
    class MainDelegate
    {
        public mainDelegate _mainDelegate;
        public delegate void mainDelegate(string type, Control control, params object[] objs);
        public MainDelegate()
        {
            _mainDelegate += new mainDelegate(onMainDelegate);
        }
        public void onMainDelegate(string type, Control control, params object[] objs)
        {
            try
            {
                if (objs == null) return;
                switch (type.ToString())
                {
                    case "TextBox":
                        {
                            TextBox t = (TextBox)control;
                            if (objs.Length > 0)
                            {
                                t.Text = objs[0].ToString();
                            }
                            break;
                        }
                    case "Label":
                        {
                            Label t = (Label)control;
                            if (objs.Length > 0)
                            {
                                t.Text = objs[0].ToString();
                            }
                            break;
                        }
                    case "Button":
                        {
                            Button b = (Button)control;
                            if (objs.Length > 0)
                            {
                                if (objs[0].ToString().Length > 0)
                                {
                                    b.Text = objs[0].ToString();
                                }
                            }
                            if (objs.Length > 1)
                            {
                                if (objs[1].ToString().ToLower().Equals("true") || objs[1].ToString().ToLower().Equals("false"))
                                {
                                    b.Enabled = (bool)objs[1];
                                }
                            }

                            break;
                        }
                    case "ComboBox":
                        {
                            ComboBox c = (ComboBox)control;
                            if (objs.Length > 0)
                            {
                                if (objs[0].ToString().Length > 0)
                                {
                                    c.Items.Add(objs[0].ToString());
                                    c.SelectedIndex = c.Items.Count - 1;
                                }
                            }

                            break;
                        }
                    case "Picter":
                        {
                            WebBrowser web = (WebBrowser)control;

                            break;
                        }
                }
            }
            catch { }
        }
    }
}
