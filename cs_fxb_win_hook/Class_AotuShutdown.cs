using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_fxb_win_hook
{
    class AutoShutdown
    {
        public static Boolean autoShutdownBoo;

        public static void AotuShutdownThread()
        {
            try
            {
                int re;

                // 判断定时关机是否开启,若没有开启则退出该函数。
                string strBoo = "";
                re = MyGlobal.fIni.GetEntry("定时关机是否开启（是/否）", ref strBoo);

                if ("否" == strBoo)
                { return; }

                string strT1 = "";
                string strT2 = "";
                re = MyGlobal.fIni.GetEntry("定时关机时间（24:00）", ref strT1);
                DateTime tim;

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
                        {
                            autoShutdownBoo = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string log = "try块包含整个AotuShutdownThread函数。";
                string classification = "AutoShutdown-AotuShutdownThread";
                MyGlobal.writeLog.Write(MyGlobal.thisPath + "log\\", classification, log, e);
            }
        }




    }
}
