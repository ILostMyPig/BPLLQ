using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

namespace guard
{
    class Class1
    {
        [DllImport("dis_cad_dll.dll")]
        static extern bool EnableDebugPrivilege();

        [DllImport("dis_cad_dll.dll")]
        static extern bool SusWin();

        [DllImport("dis_cad_dll.dll")]
        static extern bool ResWin();

        /// <summary>被守护，需要做的事情。
        /// </summary>
        public void ByGruad(string thisGuardPath, string tim)
        {
            string bWritingFullName = thisGuardPath + "BWriting" + tim;
            string con = "请求守护";

            // 先生成新的报告，再删除旧的报告，以保持始终有报告存在，否则被判断为程序不正常运行。
            System.IO.File.WriteAllText( // 创建并开始写入报告内容。
                bWritingFullName,
                con,
                System.Text.Encoding.Unicode);

            System.Windows.Forms.Application.DoEvents(); // System.IO.File.WriteAllText 反映迟缓，稍等一下，再继续处理。

            string bReportFullName = thisGuardPath + "BReport" + tim;
            System.IO.File.Move(bWritingFullName, bReportFullName); // 写入完毕后，修改文件名。

            // 删除旧的 BReport* 文件
            string[] allBReportFiles = System.IO.Directory.GetFiles( // 获取guard目录下 BReport* 文件的列表。
                thisGuardPath,
                "BReport*",
                System.IO.SearchOption.TopDirectoryOnly);
            foreach (string str in allBReportFiles) // 逐个文件对比，若为旧的就删除。
            {// 若 allBReportFiles 为空数组，则 foreach 不执行。
                if (str != bReportFullName)
                {
                    System.IO.File.Delete(str);
                }
            }
        }

        /// <summary>守护对方，需要做的事情。
        /// </summary>
        /// <param name="thisGuardPath"></param>
        public void ToGruad(string thisGuardPath, string tim,ref Boolean boo)
        {
            string[] allAReportFiles = System.IO.Directory.GetFiles( // 获取guard目录下的所有 AReport* 文件。
                        thisGuardPath,
                        "AReport*",
                        System.IO.SearchOption.TopDirectoryOnly);
            string newFullName = "";

            foreach (string str in allAReportFiles) // 挑选最新的 AReport* 文件。
            {
                int re = string.Compare(newFullName, str);
                if (re < 0)
                {
                    newFullName = str;
                }
            }

            if (newFullName == "") // 不存在 AReport* 文件。
            {
                FindException(thisGuardPath, tim, "cs_fxb_win_hook.exe");
            }
            else // if (newFullName == "")
            {
                string con = string.Empty;
                try
                {
                    con = System.IO.File.ReadAllText(newFullName); // 获取最新的 AReport* 文件的内容。
                }
                catch (System.IO.FileNotFoundException)
                {
                    // 可能刚刚被对方删除，再次获取，但不作错误处理，因为对方再次更新文件有2秒间隔。
                    allAReportFiles = System.IO.Directory.GetFiles( // 获取guard目录下的所有 AReport* 文件。
                        thisGuardPath,
                        "AReport*",
                        System.IO.SearchOption.TopDirectoryOnly);
                    newFullName = "";

                    foreach (string str in allAReportFiles) // 挑选最新的 AReport* 文件。
                    {
                        int re = string.Compare(newFullName, str);
                        if (re < 0)
                        {
                            newFullName = str;
                        }
                    }

                    con = System.IO.File.ReadAllText(newFullName); // 获取最新的 AReport* 文件的内容。
                }
                
                if (con == "请求守护")
                {
                    string newFullNameTim = newFullName.Substring(newFullName.Length - 14); // 获取文件名称中的时间部分。
                    double newFullNameTim_double = Microsoft.VisualBasic.Conversion.Val(newFullNameTim);
                    double tim_double = Microsoft.VisualBasic.Conversion.Val(tim);
                    double interval = newFullNameTim_double - tim_double;
                    if (interval > 5) // 最近一次的报告距离当前时间超过5秒。
                    {
                        FindException(thisGuardPath, tim, "cs_fxb_win_hook.exe");
                    }
                    else
                    { }
                }
                else if (con == "无需守护")
                {
                    string[] allFiles = System.IO.Directory.GetFiles(
                        thisGuardPath,
                        "*",
                        System.IO.SearchOption.TopDirectoryOnly);
                    foreach (string item in allFiles)
                    {
                        System.IO.File.Delete(item);
                    }
                    boo = false; // 停止主线程的while循环，以结束程序。
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(
                        "我是守护系统中的 B 进程，发现 A 进程提交的报告中有无法识别的内容！");
                }
            } // if (newFullName == "")

        }

        /// <summary>被守护进程意外结束后，需要恢复其做出的更改。
        /// </summary>
        public void Restore(string thisGuardPath, string tim)
        {
            // 恢复winlogin进程
            bool re;
            re = EnableDebugPrivilege(); // 将当前进程提权。
            re = ResWin(); // 恢复winlogin进程。

            // 删除所有 AReport* 文件
            string[] allAReportFiles = System.IO.Directory.GetFiles(
                thisGuardPath,
                "AReport*",
                System.IO.SearchOption.TopDirectoryOnly);
            foreach (string str in allAReportFiles)
            {
                System.IO.File.Delete(str);
            }

            // 删除所有 AWriting* 文件
            string[] allAWritingFiles = System.IO.Directory.GetFiles(
                thisGuardPath,
                "AWriting*",
                System.IO.SearchOption.TopDirectoryOnly);
            foreach (string str in allAWritingFiles)
            {
                System.IO.File.Delete(str);
            }
        
        }

        /// <summary>重启被保护的进程。
        /// </summary>
        /// <param name="thisGuardPath"></param>
        /// <param name="tim"></param>
        /// <param name="proName"></param>
        public void FindException(string thisGuardPath, string tim, string proName)
        {
            // 若被守护的进程还存在，则结束它。
            System.Diagnostics.Process[] pro = System.Diagnostics.Process.GetProcesses();//获取所有进程
            for (int i = 0; i < pro.Length; i++) //遍历所有获取的进程
            {
                if (pro[i].ProcessName.ToLower() == proName) //判断此进程是否是要查找的进程
                {
                    pro[i].Kill();//结束进程
                }
                else
                { }
            }

            Restore(thisGuardPath, tim);

            string thisPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            System.Diagnostics.Process.Start(thisPath + proName);
        }
    }
}
