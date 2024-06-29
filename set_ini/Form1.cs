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
        public FileIni.FileIni fIni = new FileIni.FileIni();
        string fullFileName = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "set.ini";

        private void FillListBox()
        {
            listBox1.Items.Clear();
            foreach (System.Collections.ArrayList i in fIni.all)
            {
                listBox1.Items.Add(i[0] + "=" + i[1]);
            }
        }

        public Form1()
        {
            InitializeComponent();

            try
            {
                fIni.LoadIniFile(fullFileName); // 载入 ini 文件。                
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载ini文件出错，即将加载默认配置。" +
                    Environment.NewLine + "错误信息：" +
                    Environment.NewLine + ex.ToString());
                fIni.path = fullFileName;
                fIni.UpdateOrAddItem("主页", "https://www.baidu.com/");
                fIni.UpdateOrAddItem("关闭密码", "1q!Q");
                fIni.UpdateOrAddItem("定时关机是否开启（是/否）", "否");
                fIni.UpdateOrAddItem("定时关机时间（例24:00）", "18:05");
                fIni.UpdateOrAddItem("浏览器选择（ie7-doctype/ie8/ie8-doctype/ie9/ie9-doctype/ie10/ie10-doctype/ie11/ie11-doctype）", "ie11-doctype");
            }

            FillListBox();
        }


        private void listBox1_Click(object sender, EventArgs e)
        {
            int a = listBox1.SelectedIndex;
            if (a > -1)
            { // 没有选中任何项时，值为-1。
                string[] b = FileIni.FileIni.SplitItem(listBox1.Items[a].ToString());
                textBox1.Text = b[0];
                textBox2.Text = b[1];
            }
        }

        private void buttonAddUpdate_Click(object sender, EventArgs e)
        {
            fIni.UpdateOrAddItem(textBox1.Text, textBox2.Text);
            FillListBox();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i > -1)
            {
                string item = listBox1.Items[i].ToString();
                try
                {
                    fIni.RemoveItem(item, "item");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("删除“" + item + "”出错。" +
                    Environment.NewLine + "错误信息：" +
                    Environment.NewLine + ex.ToString());
                    return;
                }
            }
            FillListBox();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                fIni.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败。" +
                    Environment.NewLine + "错误信息：" +
                    Environment.NewLine + ex.ToString());
                return;
            }
            MessageBox.Show("保存成功。");

        }
    }
}