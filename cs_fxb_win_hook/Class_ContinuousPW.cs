using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_fxb_win_hook
{
    class ContinuousPW
    {
        // ************************ 字段 ********************


        private string pw; // 密码。

        private string input; // 输入的字符。不手动设置input的大小限制。


        // ************************ 构造函数 ********************


        public ContinuousPW()
        {
            // 初始化字段。 
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
        /// </summary>
        /// <param name="pw">密码。传入string.Empty 或 null，则密码被清空。</param>
        public void SetPw(string pw)
        {
            this.input = string.Empty;//肯定是设置密码之后，再输入密码，所以清空之前的输入。
            if (null == pw)
            {
                pw = string.Empty;
            }
            this.pw = pw;
        }

        /// <summary>输入字符，并核对密码。
        /// <returns>
        /// <para>return -1：密码为空。</para>
        /// <para>return 0：密码错误。</para>
        /// <para>return 1：密码正确。</para>
        /// </returns></summary>
        /// <param name="inchar">输入的字符。</param>
        public int InputPw(string inchar)
        {
            if (string.Empty == this.pw)
            {
                return -1;
            }
            else
            {
                this.input = this.input + inchar;
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


