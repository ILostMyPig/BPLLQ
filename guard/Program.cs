using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace guard
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {// 此程序是守护进程，也被称为 B 程序。

            

            string tim =string.Empty ;
            string thisPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string thisGuardPath = thisPath + "guard\\";

            Class1 guardDo=new Class1();

            Boolean boo = true;
            while (boo)
            {
                tim = DateTime.Now.ToString("yyyyMMddHHmmss");

                guardDo.ByGruad(thisGuardPath,tim);// 被守护

                guardDo.ToGruad(thisGuardPath,tim,ref boo);// 守护对方
                
                System.Threading.Thread.Sleep(2000);
            }

            // 无需守护
            tim = DateTime.Now.ToString("yyyyMMddHHmmss");
            string bWritingFullName = thisGuardPath + "BWriting" + tim;
            string con = "请求守护";
            System.IO.File.WriteAllText( // 创建并开始写入报告内容。
                bWritingFullName,
                con,
                System.Text.Encoding.Unicode);
            System.Windows.Forms.Application.DoEvents(); // WriteAllText 反映迟缓，交还控制权给系统处理一波。
            string bReportFullName = thisGuardPath + "BReport" + tim;
            System.IO.File.Move(bWritingFullName, bReportFullName); // 写入完毕后，修改文件名。


            //System.Environment.Exit(0);
        }
    }
}
