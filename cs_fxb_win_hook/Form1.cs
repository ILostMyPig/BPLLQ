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
        public Form1()
        {
            InitializeComponent();

            System.Uri h = GetMainURL();
            webBrowser1.Url = h; // 使浏览器访问指定的网址。

            int SH = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height; // 设置窗体的尺寸。
            int SW = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            this.Size = new System.Drawing.Size(SW, SH);
        }

        private System.Uri GetMainURL()
        {
            int re = 0;

            string thisPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            string mainURL = string.Empty; // 从 ini 文件载入“主页”。
            re = MyGlobal.fIni.GetEntry("主页", ref mainURL);
            if (-1 == re || mainURL == string.Empty) // ini 文件没有设置主页，则载入错误页面。
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();

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
            this.webBrowser1.NewWindow +=
                new System.ComponentModel.CancelEventHandler(
                    this.webBrowser1_NewWindow);

            this.ResumeLayout(false);

            this.Controls.Add(webBrowser1);

            System.Uri h = GetMainURL();
            webBrowser1.Url = h; // 使浏览器访问指定的网址。
        }

        private void webBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true; // 取消此事件，即取消打开新窗口。

            // 在本窗口加载新窗口的页面。
            System.Windows.Forms.WebBrowser a = (System.Windows.Forms.WebBrowser)sender;
            System.Uri h = new System.Uri(a.StatusText); // 使浏览器访问指定的网址。
            a.Url = h;
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
}
