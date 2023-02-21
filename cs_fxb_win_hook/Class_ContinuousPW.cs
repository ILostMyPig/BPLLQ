using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_fxb_win_hook
{
    class ContinuousPW
    {
        //字段
        /// <summary>密码。
        /// </summary>
        private string pw;

        /// <summary>输入的字符。
        /// </summary>
        private string input;//不手动设置input的大小限制。

        //方法
        /// <summary>构造函数。初始化字段。 
        /// </summary>
        public ContinuousPW()
        {
            pw = string.Empty;
            input = string.Empty;
        }

        /// <summary>清空已输入的字符。
        /// </summary>
        public void ClearInput()
        {
            input = string.Empty; 
        }

        /// <summary>设置密码。
        /// <returns>
        /// return 0：成功。
        /// return -1：pw 为 string.Empty 或 null，密码被清空。
        /// </returns></summary>
        /// <param name="pw">_in_，密码。</param>
        public int SetPw(string pw)
        {
            this.input = string.Empty;//肯定是设置密码之后，再输入密码，所以清空之前的输入。
            if (string.Empty == pw || null == pw)
            {
                this.pw = string.Empty;
                return -1;
            }
            else
            {
                this.pw = pw;
                return 0;
            }

        }

        /// <summary>输入字符，并核对密码。
        /// <returns>
        /// <para>return -1：密码为空。</para>
        /// <para>return 0：密码错误。</para>
        /// <para>return 1：密码正确。</para>
        /// </returns></summary>
        /// <param name="input">_in_，输入的字符。</param>
        public int InputPw(string input)
        {
            if (string.Empty == this.pw)
            {
                return -1;
            }
            else
            {
                this.input = this.input + input;
                if (this.input.Length > this.pw.Length)
                {
                    this.input = this.input.Substring(
                        this.input.Length - this.pw.Length,
                        this.pw.Length
                        );
                }
                if (this.input == this.pw)
                { return 1; }
                else
                { return 0; }


            }


        }

    }
}


