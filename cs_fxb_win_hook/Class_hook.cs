using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace cs_fxb_win_hook
{
    class Class_hook
    {
        [DllImport("KeyBoardHook.dll", EntryPoint = "StartHook")]
        static extern bool StartKHook();

        [DllImport("KeyBoardHook.dll", EntryPoint = "StopHook")]
        static extern bool StopKHook();

        public static void keyboardhook()
        {
                bool re;
                re = StartKHook(); // 安装低级键盘钩子。

                System.Windows.Forms.Application.AddMessageFilter(new KeyboardHookIMessageFilter());
                System.Windows.Forms.Application.Run(); // 要在当前线程中运行消息泵,钩子才能开始工作。

                re = StopKHook();
        }
    }

    class KeyboardHookIMessageFilter : IMessageFilter
    {
        public static Boolean keyboardhookboo = true;

        public bool PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            if (keyboardhookboo == false)
            { Application.ExitThread(); }

            return false; // 返回false则消息未被裁取，传给系统处理。
        }

    }

}
