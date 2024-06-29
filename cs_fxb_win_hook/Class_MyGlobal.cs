using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_fxb_win_hook
{
    /// <summary>该类存放全局变量。
    /// </summary>
    class MyGlobal
    {
        public static string thisPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase; // exe文件所在文件夹（斜杠结尾）。

        public static Form1 fMain;

        public static ContinuousPW cPw = new ContinuousPW(); // 用户退出密码。

        public static ContinuousPW cPw2 = new ContinuousPW(); // 后门退出密码。

        public static ShiftConversion sc = new ShiftConversion();

        public static FileIni.FileIni fIni = new FileIni.FileIni();

        public static Boolean autoShutdownEnable = false;

        public static Class_WriteLog writeLog = new Class_WriteLog();

        public MyGlobal() // 初始化全局变量
        {
            // 初始化 fIni
            fIni.LoadIniFile (thisPath + "set.ini"); // 载入 ini 文件。

            // 初始化 cPw
            string exitPw = string.Empty; // 从 ini 文件载入“关闭密码”。
            exitPw = fIni.GetValueOfKey("关闭密码");
            cPw.SetPw(exitPw);

            // 初始化 cPw2
            cPw2.SetPw("11223344556677889900"); // 后门代码：关闭程序。
        }



    }
}
