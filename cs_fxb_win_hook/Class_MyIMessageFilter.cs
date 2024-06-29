using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace cs_fxb_win_hook
{
    class MyIMessageFilter : IMessageFilter
    { // 重写消息泵，是为了实现退出密码功能。
        const int WM_KEYUP = 0x0101;

        public MyIMessageFilter()
        {
            sc = new ShiftConversion();
        }

        [DllImport("user32.dll")]
        public static extern System.Int16 GetKeyState(int vKey);

        private ShiftConversion sc;

        /// <summary>截取消息，处理后归还消息。
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool PreFilterMessage(ref System.Windows.Forms.Message m)
        {

            //System.IO.File.AppendAllText( // 记录消息的id和wparam。
            //    MyGlobal.thisPath+"log\\" + "MyIMessageFilter.log",
            //    m.Msg.ToString() + ":" + ((int)m.WParam).ToString() + Environment.NewLine
            //    );

            int vk = (int)m.WParam;

            // WM_CHAR 在输入法打开时不会生成该消息。
            // WM_KEYDOWN 会被输入法改变WParam值为229（“微软拼音新体验输入风格”输入法）。
            // 所以不用这两个消息。
            if (m.Msg == WM_KEYUP) // 若本消息是 WM_KEYUP，则执行密码处理。
            {
                // SHIFT键会被键盘hook拦截，所以要在键盘猴钩子中允许单独按住shift的情况。
                System.Int16 reShift = GetKeyState(16);
                System.Int16 reCapsL = GetKeyState(0x14);

                string x = string.Empty;
                try
                {
                    if ( // 若满足要求，则转换按键。
                        reShift < 0 // shift 按住时,转换所有按键。
                        || (reCapsL > 0 && (vk >= 0x41 && vk <= 0x5a)) // capslock 亮起时，只转换a-z。
                        )
                    {
                        x = sc.GetHoldShift(vk);
                    }
                    else // if ( // 若满足要求，则转换按键。
                    {
                        x = sc.GetFreeShift(vk);
                    }

                }
                catch (ArgumentOutOfRangeException e)
                {
                    string classification = "MyIMessageFilter-PreFilterMessage";
                    string log = "GetHoldShift或GetFreeShift函数抛出异常，但不影响程序运行。";
                    Class_WriteLog.Write(MyGlobal.thisPath + "log\\", classification, log, e);
                }

                // 若输入的不是字符，则视为不是密码，即密码输入中断（未能连续输入）。
                // 此时要清空之前的输入。
                if (x.Length > 0) // 若输入的是字符,则视为输入了密码。
                {
                    if (
                        MyGlobal.cPw.InputPw(x) == 1
                        || MyGlobal.cPw2.InputPw(x) == 1
                        )
                    {
                        MyGlobal.fMain.Close();
                        return false; // 返回false则消息未被截取，传给系统处理。
                    }
                    else
                    {
                        return false; // 返回false则消息未被截取，传给系统处理。
                    }
                }
                else // if (x.Length > 0) // 若输入的是字符,则视为输入了密码。
                {
                    MyGlobal.cPw.ClearInput();
                    MyGlobal.cPw2.ClearInput();

                    return false; // 返回false则消息未被截取，传给系统处理。
                }
            }
            else // if (m.Msg == WM_KEYUP) // 若本消息是 WM_KEYUP，则执行密码处理。
            {
                return false; // 返回false则消息未被截取，传给系统处理。
            }
        }

    }
}
