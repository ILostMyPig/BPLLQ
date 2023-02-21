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
        /// <summary>exe文件所在文件夹（斜杠结尾）。
        /// </summary>
        public static string thisPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        public static Form1 fMain;

        /// <summary>用户退出密码。
        /// </summary>
        public static ContinuousPW cPw = new ContinuousPW();

        /// <summary>后门退出密码。
        /// </summary>
        public static ContinuousPW cPw2 = new ContinuousPW();

        public static ShiftConversion sc = new ShiftConversion();

        public static FileIni.FileIni fIni = new FileIni.FileIni();

        public static Boolean autoShutdownEnable = false;

        public static Class_WriteLog writeLog = new Class_WriteLog();

        public MyGlobal() // 初始化全局变量
        {
            int re;

            // 初始化 fIni
            re = fIni.LoadIni(thisPath + "set.ini"); // 载入 ini 文件。

            // 初始化 cPw
            string exitPw = string.Empty; // 从 ini 文件载入“关闭密码”。
            re = fIni.GetEntry("关闭密码", ref exitPw);
            re = cPw.SetPw(exitPw);

            // 初始化 cPw2
            re = cPw2.SetPw("11223344556677889900");//后门代码：关闭程序。
        }



    }
}
