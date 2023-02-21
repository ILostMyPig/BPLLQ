using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace set_ini
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            int re = 0;

            string thisPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            try
            {
                re = Global.fIni.LoadIni(thisPath + "set.ini"); // 载入 ini 文件。
            }
            catch (CE_FileIni.NotFoundFileIni)
            {
                MessageBox.Show("set.ini文件丢失，即将载入空配置文件。");                
            }

            // 所有功能的列表。
            string str = "";
            re = Global.fIni.GetEntry("主页", ref str); // 主页
            listBox1.Items.Add("主页" + "=" + str);
            re = Global.fIni.GetEntry("关闭密码", ref str); // 关闭密码
            listBox1.Items.Add("关闭密码" + "=" + str);
            re = Global.fIni.GetEntry("定时关机是否开启（是/否）", ref str); // 定时关机是否开启（是/否）
            listBox1.Items.Add("定时关机是否开启（是/否）" + "=" + str);
            re = Global.fIni.GetEntry("定时关机时间（24:00）", ref str); // 定时关机时间（24:00）
            listBox1.Items.Add("定时关机时间（24:00）" + "=" + str);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>关闭按钮。
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>修改项目按钮。
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            int x = listBox1.Items.Count;
            int y = listBox1.SelectedIndex;

            Form2 a = new Form2();

            string text = listBox1.Items[y].ToString();
            a.textBox1.Text = text.Split(new char[1] { '=' }, 2)[0];
            a.textBox2.Text = text.Split(new char[1] { '=' }, 2)[1];

            a.ShowDialog(this); // 以模式窗口打开 Form2。

            // 等待 Form2 关闭后，才会执行下面的代码。

            // 判断列表中项目的数量，在打开Form2前和关闭Form2后是否一致
            // 如果不一致，说明添加了新项目，则需要删除原来选定的项目，以达到修改的效果。
            // 如果一致，说明取消了修改操作，则保持列表中项目不动。
            if (listBox1.Items.Count > x) // 项目的数量不一致。
            {
                listBox1.Items.RemoveAt(y);
            }
            else
            { }

        }

        /// <summary>保存按钮。
        /// </summary>
        private void button6_Click(object sender, EventArgs e)
        {
            int re;
            re = Global.fIni.CoverIni(ref listBox1);
            MessageBox.Show("保存成功。");
        }
    }
}