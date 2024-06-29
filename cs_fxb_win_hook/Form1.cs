using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace cs_fxb_win_hook
{
    public partial class Form1 : Form
    {
        private WebBrowser webBrowser1;
        public Form1()
        {
            InitializeComponent();

            InitializeWebBrowser();
        
            int SH = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height; // 设置窗体的尺寸。
            int SW = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            this.Size = new System.Drawing.Size(SW, SH);
        }

        private void browser_NewWindow3(ref bool Cancel, string bstrUrl)
        {
            Cancel = true;
            webBrowser1.Navigate(bstrUrl);
        }

        private System.Uri GetMainURL()
        {
            string thisPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            string mainURL = string.Empty; // 从 ini 文件载入“主页”。
            try
            {
                mainURL = MyGlobal.fIni.GetValueOfKey("主页");
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                mainURL = thisPath + @"error page\no mainURL.html";
            }
            System.Uri h = new System.Uri(mainURL.Replace(@"\", "/"));

            return h;
        }

        private void webBrowser1_DocumentCompleted(
            object sender,
            WebBrowserDocumentCompletedEventArgs e)
        {
            // Unload事件的对象是当前载入的页面，所以要在载入完成后才存在此对象。
            // 判断在融入完成即DocumentCompleted事件被触发，所以在DocumentCompleted中关联Unload事件。
            webBrowser1.Document.Window.Unload += new HtmlElementEventHandler(Window_Unload);
        }

        void Window_Unload(object sender, HtmlElementEventArgs e)
        {
            if (webBrowser1.Document == null) // 执行页面关闭后，该项为null。
            {
                this.Controls.Remove(webBrowser1);
                InitializeWebBrowser();
            }
        }

        private void InitializeWebBrowser()
        {
            this.webBrowser1 = new WebBrowser();

            this.SuspendLayout();

            this.webBrowser1.AllowWebBrowserDrop = false;
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(284, 262);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.Url = new System.Uri("http://www.163.com", System.UriKind.Absolute);
            this.webBrowser1.DocumentCompleted +=
                new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(
                    this.webBrowser1_DocumentCompleted);
            this.webBrowser1.NewWindow3 += new WebBrowser.NewWindow3EventHandler(this.browser_NewWindow3);
            
            this.ResumeLayout(false);

            this.Controls.Add(webBrowser1);

            System.Uri h = GetMainURL();
            webBrowser1.Url = h; // 使浏览器访问指定的网址。
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send ("^c");
        }

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^v");
        }

    }


    /// <summary>利用Interop.SHDocVw（Microsoft Internet Controls），重写WebBrowser。
    /// Interop.SHDocVw的嵌入互操作类型，建议是False，可避免因系统设置不正确所导致的无法实现拦截新窗口功能。
    /// 发现False问题的详细原因：
    /// 1. 已开始用True能实现功能，但电脑突然断电一次后，就必须用False了。
    /// 2. 之后，把用True的exe文件，复制到其它系统（win10）运行，结果是能实现功能。
    /// 3. 把其它系统（win10）用vs2022写的相同代码的，也用True的，在其win10中能实现效果的exe文件，复制到必须False的这台机器上，则无法实现。
    /// MSDN：使用嵌入的互操作类型：从 .NET Framework 4 版开始，您可以指示编译器将互操作程序集中的类型信息嵌入到可执行文件中。 编译器只嵌入您的应用程序所使用的类型信息。 无需将互操作程序集与您的应用程序一起部署。 这是推荐采用的方法。
    /// </summary>
    public class WebBrowser : System.Windows.Forms.WebBrowser
    {
        public delegate void NewWindow3EventHandler(ref bool Cancel, string bstrUrl);


        //发布NewWindow3事件
        public event NewWindow3EventHandler NewWindow3;

        protected override void CreateSink()
        {
            if (this.ActiveXInstance != null)
            {
                var browser = this.ActiveXInstance as SHDocVw.WebBrowser;
                browser.NewWindow3 += WebBrowser_NewWindow3;
            }
            else
            {
                Class_WriteLog.Write(MyGlobal.thisPath + "log\\", @"WebBrowser-CreateSink", "this.ActiveXInstance 为 null。");
                MessageBox.Show(@"无法阻止新窗口打开，详见日志。");
            }
        }

        private void WebBrowser_NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
        {
            var handler = NewWindow3;
            if (handler != null)
                handler(ref Cancel, bstrUrl);
        }
    }
}
