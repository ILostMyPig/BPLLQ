using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace cs_fxb_win_hook
{
    class AutoShutdown
    {
        public static Boolean autoShutdownBoo;

        public static void AotuShutdownThread()
        {
            string strBoo = "";
            string strT1 = "";
            try
            {
                strBoo = MyGlobal.fIni.GetValueOfKey("定时关机是否开启（是/否）");
                strT1 = MyGlobal.fIni.GetValueOfKey("定时关机时间（例24:00）");
            }
            catch (KeyNotFoundException ex)
            {
                MessageBox.Show("设置文件中没找到“"+ex.Message +@"”。（set.ini文件）"
                    + Environment.NewLine + "因为缺少相关设置，所以自动关机功能没有启动。");
                
                strBoo = "否";
            }


            // 判断定时关机是否开启,若没有开启则退出该函数。
            if ("否" == strBoo)
            { return; }


            string strT2 = ""; DateTime tim;

            while (autoShutdownBoo)
            {
                tim = DateTime.Now;
                strT2 = tim.ToString("HH:mm");
                if (strT2 == strT1)
                {
                    MyGlobal.autoShutdownEnable = true;
                    autoShutdownBoo = false;
                    MyGlobal.fMain.Close();
                }
                else
                {
                    try
                    {
                        System.Threading.Thread.Sleep(50000);
                    }
                    catch (System.Threading.ThreadInterruptedException)
                    { // 手动关闭程序，会终止此线程，sleep时终止，就会导致此异常。
                        autoShutdownBoo = false;
                    }
                }
            }

        }




    }
}
