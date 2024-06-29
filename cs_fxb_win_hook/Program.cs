using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace cs_fxb_win_hook
{
    static class Program
    {
        [DllImport("KeyBoardHook.dll", EntryPoint = "StartHook")]
        static extern bool StartKHook();

        [DllImport("KeyBoardHook.dll", EntryPoint = "StopHook")]
        static extern bool StopKHook();

        [DllImport("Disable_the_Accessibility_Shortcut_Keys.dll", EntryPoint = "StartDisASK")]
        static extern void StartDisASK();

        [DllImport("Disable_the_Accessibility_Shortcut_Keys.dll", EntryPoint = "StopDisASK")]
        static extern void StopDisASK();

        [DllImport("dis_cad_dll.dll")]
        static extern bool EnableDebugPrivilege();

        [DllImport("dis_cad_dll.dll")]
        static extern bool SusWin();

        [DllImport("dis_cad_dll.dll")]
        static extern bool ResWin();

        [DllImport("dis_start.dll", EntryPoint = "StartdisStart")]
        static extern int StartdisStart();

        [DllImport("dis_start.dll", EntryPoint = "StopdisStart")]
        static extern int StopdisStart();

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 使用Sleep原因:系统在登陆时,有一段屏幕由暗到亮的动画.
            // 若电脑运行速度较快，则完全变亮之前就会启动程序。
            // 此程序中挂起winlogin进程的操作会使变亮程序暂停，导致屏幕一直处于暗状态。
            System.Threading.Thread.Sleep(5000);



            bool re;
            int re_int;

            System.Threading.Thread threadKeyboardHook; // 低级键盘钩子线程。
            System.Threading.Thread threadAutoShutdown; // 定时关机线程。
            System.Threading.Thread threadGruad; // 守护操作线程。

            Application.AddMessageFilter(new MyIMessageFilter());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 初始化 MyGlobal。
            try
            {
                cs_fxb_win_hook.MyGlobal initializeGlobal = new cs_fxb_win_hook.MyGlobal();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                string log = "配置文件丢失，请使用“set.exe”程序来重建配置文件。" +
                    Environment.NewLine + "错误信息：" +
                    Environment.NewLine + ex.ToString(); ;
                System.Windows.Forms.MessageBox.Show(log);
                return;
            }

            // 设置浏览器版本。
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.LocalMachine;
            reg = reg.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            reg.DeleteValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe", false);
            switch (MyGlobal.fIni.GetValueOfKey("浏览器选择（ie7-doctype/ie8/ie8-doctype/ie9/ie9-doctype/ie10/ie10-doctype/ie11/ie11-doctype）"))
            {
                case "ie7-doctype":
                    reg.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName+".exe", 7000);
                    break;
                case "ie8":
                    reg.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName+".exe", 8888);
                    break;
                case "ie8-doctype":
                    reg.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName+".exe", 8000);
                    break;
                case "ie9":
                    reg.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName+".exe", 9999);
                    break;
                case "ie9-doctype":
                    reg.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName+".exe", 9000);
                    break;
                case "ie10":
                    reg.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName+".exe", 10001);
                    break;
                case "ie10-doctype":
                    reg.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName+".exe", 10000);
                    break;
                case "ie11":
                    reg.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName+".exe", 11001);
                    break;
                case "ie11-doctype":
                    reg.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName+".exe", 11000);
                    break;
                default:
                    MessageBox.Show("浏览器选择设置错误，将向系统申请ie11-doctype浏览器。");
                    reg.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName+".exe", 11000);
                    break;
            }


            // 启动定时关机线程.
            AutoShutdown.autoShutdownBoo = true;
            threadAutoShutdown = new System.Threading.Thread(AutoShutdown.AotuShutdownThread);
            threadAutoShutdown.Start();

            // 启动守护操作线程.
            threadGruad = new System.Threading.Thread(Class2.GruadWhile);
            threadGruad.Start();

            // 启动低级键盘钩子线程。
            threadKeyboardHook = new System.Threading.Thread(Class_hook.keyboardhook);
            threadKeyboardHook.Start();

            StartDisASK(); // 禁用辅助键。

            #region 挂起winlogin进程。
            re = EnableDebugPrivilege(); // 将当前进程提权。
            re = SusWin(); // 挂起winlogin进程。
            #endregion

            re_int = StartdisStart(); // 隐藏开始按钮和开始栏。

            try
            {
                // 当程序开启时间和自动关机时间相同时，不执行Run。
                if (MyGlobal.autoShutdownEnable == false)
                {
                    MyGlobal.fMain = new Form1();
                    Application.Run(MyGlobal.fMain);
                }
            }
            catch (Exception e)
            {
                string log = "语句“Application.Run(new Form1());”捕获到异常。";
                Class_WriteLog.Write(MyGlobal.thisPath + "log\\", "Program-Main", log, e);
            }

            re_int = StopdisStart(); // 显示开始按钮和开始栏。

            re = ResWin(); // 恢复winlogin进程。

            StopDisASK(); // 启用辅助键。

            Application.Exit();

            // 退出低级键盘钩子线程。
            KeyboardHookIMessageFilter.keyboardhookboo = false;
            threadKeyboardHook.Join();

            // 退出守护操作线程。
            Class2.boo = false;
            threadGruad.Join();

            // 退出定时关机线程,并完成定时关机。
            // Interrupt可以使目标线程在 “等待状态” 
            // 或 “下一次进入等待状态” 时
            // 通过触发异常来中断等待。
            threadAutoShutdown.Interrupt();
            threadAutoShutdown.Join(); // 中断等待后，还要等线程中的剩余代码执行完毕。
            if (MyGlobal.autoShutdownEnable == true)
            { System.Diagnostics.Process.Start("shutdown.exe", "-s -t 0"); }

            //System.Environment.Exit(0);
        }
    }
}
