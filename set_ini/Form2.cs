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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        /// <summary>取消按钮。
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            ClearTextBox();
            this.Close();
        }

        /// <summary>确认按钮。
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
                Form1 a = (Form1)this.Owner;
                a.listBox1.Items.Add(textBox1.Text + "=" + textBox2.Text);
                ClearTextBox();
            
            button2_Click(sender, e); // 关闭按钮。
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        /// <summary>清空两个textBox控件中的内容。
        /// </summary>
        private void ClearTextBox()
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }
    }
}
