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

        [DllImport("MouseHook.dll", EntryPoint = "StartHook")]
        static extern bool StartMHook();

        [DllImport("MouseHook.dll", EntryPoint = "StopHook")]
        static extern bool StopMHook();

        

        public static void mousehook()
        {
            try
            {
                bool re;
                re = StartMHook(); // 安装低级鼠标钩子。
                System.Windows.Forms.Application.AddMessageFilter (new MouseHookIMessageFilter());
                System.Windows.Forms.Application.Run(); // 要在当前线程中运行消息泵,钩子才能开始工作。

                re = StopMHook();
            }
            catch (Exception e)
            {
                string log = "try块包含整个mousehook函数。";
                MyGlobal.writeLog.Write(MyGlobal.thisPath + "log\\", "Class_hook-mousehook", log, e);
            }
        }

        public static void keyboardhook()
        {
            try
            {
                bool re;
                re = StartKHook(); // 安装低级键盘钩子。

                System.Windows.Forms.Application.AddMessageFilter(new KeyboardHookIMessageFilter());
                System.Windows.Forms.Application.Run(); // 要在当前线程中运行消息泵,钩子才能开始工作。

                re = StopKHook();
            }
            catch (Exception e)
            {
                string log = "try块包含整个mousehook函数。";
                MyGlobal.writeLog.Write(MyGlobal.thisPath + "log\\", "Class_hook-keyboardhook", log, e);
            }
        }
    }



    class MouseHookIMessageFilter : IMessageFilter
    {
        public static Boolean mousehookboo = true;

        /// <summary>
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            if (mousehookboo == false)
            { Application.ExitThread(); }

            return false; // 返回false则消息未被裁取，传给系统处理。
        }

    }

    class KeyboardHookIMessageFilter : IMessageFilter
    {
        public static Boolean keyboardhookboo = true;

        /// <summary>
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            if (keyboardhookboo == false)
            { Application.ExitThread(); }

            return false; // 返回false则消息未被裁取，传给系统处理。
        }

    }

}
