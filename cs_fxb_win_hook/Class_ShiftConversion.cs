using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_fxb_win_hook
{

    /// <summary>按住和释放shift键时，输入字符的转换。
    /// </summary>
    class ShiftConversion
    {
        //字段
        /// <summary>以Virtual-Key Code为引索，
        /// [引索,0]为原内容，[引索,1]为按住shift时的内容。
        /// </summary>
        private string[,] vks = new string[256, 2];

        //方法
        /// <summary>构造函数，初始化vks[,]的值。
        ///</summary>
        public ShiftConversion()
        {
            //初始化。
            for (int i = 0; i < 256; i++)
            {
                vks[i, 0] = string.Empty;
                vks[i, 1] = string.Empty;
            }

            //a-z。
            for (int i = 65; i <= 90; i++)
            {
                vks[i, 0] = new string((char)(i + 32), 1);
                vks[i, 1] = new string((char)(i), 1);
            }

            //num0-num9。
            for (int i = 96; i <= 105; i++)
            {
                vks[i, 0] = new string((char)(i - 48), 1);
                vks[i, 1] = new string((char)(i - 48), 1);
            }

            //0-9。
            char[] xcha0 = { ')', '!', '@', '#', '$', '%', '^', '&', '*', '(' };
            for (int i = 48; i <= 57; i++)
            {
                vks[i, 0] = new string((char)(i), 1);
                vks[i, 1] = new string(xcha0[i - 48], 1);
            }

            //符号键。
            string[] xstr1 = { "\t", " ", "`", "-", "=", "[", "]", "\'", ";", "'", ",", ".", "/", "/", "*", "-", "+", "." };
            string[] xstr2 = { "\t", " ", "~", "_", "+", "{", "}", "|", ":", "\"", "<", ">", "?", "/", "*", "-", "+", "." };
            int[] xint0 = { 9, 32, 192, 189, 187, 219, 221, 220, 186, 222, 188, 190, 191, 111, 106, 107, 109, 110 };
            for (int i = xint0.GetLowerBound(0); i <= xint0.GetUpperBound(0); i++)
            {
                vks[xint0[i], 0] = xstr1[i];
                vks[xint0[i], 1] = xstr2[i];
            }



        }

        /// <summary>获取没按Shift时的字符。
        /// <returns>
        /// <para>return 字符：没按Shift时的字符。</para>
        /// <para>return String.Empty：该键没有字符。</para>
        /// </returns></summary>
        /// <param name="i">虚拟键码。</param>
        public string GetFreeShift(int i)
        {
            if (i < 0 || i > 255) { throw new CE_ShiftConversion.ArgumentOutOfRangeException("参数i的取值范围是[0,255]，但是传入的值是" + i.ToString() + "。"); }
            return vks[i, 0];
        }

        /// <summary>获取按住Shift时的字符。
        /// <returns>
        /// <para>return 字符：按住Shift时的字符。</para>
        /// <para>return String.Empty：该键没有可显示字符。</para>
        /// </returns></summary>
        /// <param name="i">虚拟键码。</param>
        public string GetHoldShift(int i)
        {
            if (i < 0 || i > 255) { throw new CE_ShiftConversion.ArgumentOutOfRangeException("参数i的取值范围是[0,255]，但是传入的值是" + i.ToString() + "。"); }
            return vks[i, 1];
        }

        /// <summary>获取按住Shift时的字符。
        /// <returns>
        /// <para>return 字符：按住Shift时的字符。</para>
        /// <para>return String.Empty：该键在按住Shift时没有转换。</para>
        /// </returns></summary>
        /// <param name="a">不按住Shift时的字符。</param>
        public string GetHoldShift(string a)
        {
            if (a.Length != 1) { throw new CE_ShiftConversion.ArgumentOutOfRangeException("参数a中的字符数必须是1，但传入的值中的字符数是" + a.Length.ToString() + "。"); }

            string b = string.Empty;
            for (int i = vks.GetLowerBound(0); i <= vks.GetUpperBound(0); i++)
            {
                if (vks[i, 0] == a && vks[i, 0] != vks[i, 1])
                {
                    b = b + vks[i, 1];
                }
            }
            return b;
        }
    }
}
namespace CE_ShiftConversion
{
    public class ArgumentOutOfRangeException : System.ApplicationException
    {
        public ArgumentOutOfRangeException() { }
        public ArgumentOutOfRangeException(string message) : base(message) { }
        public ArgumentOutOfRangeException(string message, Exception inner) : base(message, inner) { }
    }
}