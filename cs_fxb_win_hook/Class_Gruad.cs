using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_fxb_win_hook
{
    class Class2
    {
        public static Boolean boo = true;

        public static void GruadWhile()
        {
            try
            {
                string tim = string.Empty;
                string thisPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string thisGuardPath = thisPath + "guard\\";

                Class2 a = new Class2();

                while (boo)
                {
                    tim = DateTime.Now.ToString("yyyyMMddHHmmss");

                    a.ByGruad(thisGuardPath, tim);// 被守护

                    a.ToGruad(thisGuardPath, tim);// 守护对方

                    System.Threading.Thread.Sleep(2000);
                }

                // 无需守护
                tim = DateTime.Now.ToString("yyyyMMddHHmmss");
                string aWritingFullName = thisGuardPath + "AWriting" + tim;
                string con = "无需守护";
                System.IO.File.WriteAllText(
                    aWritingFullName,
                    con,
                    System.Text.Encoding.Unicode);
                System.Windows.Forms.Application.DoEvents(); // WriteAllText 反映迟缓，交还控制权给系统处理一波。
                string aReportFullName = thisGuardPath + "AReport" + tim;
                System.IO.File.Move(aWritingFullName, aReportFullName); // 写入完毕后，修改文件名。
            }
            catch (Exception e)
            {
                string log = "try块包含整个GruadWhile函数。";
                Class_WriteLog.Write(MyGlobal.thisPath + "log\\", "Class2-GruadWhile", log, e);
            }
        }

        /// <summary>被守护，需要做的事情。
        /// </summary>
        public void ByGruad(string thisGuardPath, string tim)
        {
            string aWritingFullName = thisGuardPath + "AWriting" + tim;
            string con = "请求守护";

            // 先生成新的报告，再删除旧的报告，以保持始终有报告存在，否则被判断为程序不正常运行。
            System.IO.File.WriteAllText( // 创建并开始写入报告内容。
                aWritingFullName,
                con,
                System.Text.Encoding.Unicode);

            System.Windows.Forms.Application.DoEvents(); // WriteAllText 反映迟缓，交还控制权给系统处理一波。

            string aReportFullName = thisGuardPath + "AReport" + tim;
            System.IO.File.Move(aWritingFullName, aReportFullName); // 写入完毕后，修改文件名。

            // 删除旧的 AReport* 文件
            string[] allAReportFiles = System.IO.Directory.GetFiles( // 获取guard目录下 BReport* 文件的列表。
                thisGuardPath,
                "AReport*",
                System.IO.SearchOption.TopDirectoryOnly);
            foreach (string str in allAReportFiles) // 逐个文件对比，若为旧的就删除。
            {// 若 allAReportFiles 为空数组，则 foreach 不执行。
                if (str != aReportFullName)
                {
                    System.IO.File.Delete(str);
                }
            }
        }

        /// <summary>守护对方，需要做的事情。
        /// </summary>
        /// <param name="thisGuardPath"></param>
        public void ToGruad(string thisGuardPath, string tim)
        {
            string[] allBReportFiles = System.IO.Directory.GetFiles( // 获取guard目录下的所有 BReport* 文件。
                        thisGuardPath,
                        "BReport*",
                        System.IO.SearchOption.TopDirectoryOnly);
            string newFullName = "";

            foreach (string str in allBReportFiles) // 挑选最新的 BReport* 文件。
            {
                int re = string.Compare(newFullName, str);
                if (re < 0)
                {
                    newFullName = str;
                }
            }

            if (newFullName == "") // 不存在 BReport* 文件。
            {
                FindException(thisGuardPath, tim, "guard.exe");
            }
            else // if (newFullName == "")
            {
                string con = string.Empty;
                try
                {
                    con = System.IO.File.ReadAllText(newFullName); // 获取最新的 BReport* 文件的内容。
                }
                catch (System.IO.FileNotFoundException)
                {
                    // 可能刚刚被对方删除，再次获取，但不作错误处理，因为对方再次更新文件有2秒间隔。
                    allBReportFiles = System.IO.Directory.GetFiles( // 获取guard目录下的所有 BReport* 文件。
                            thisGuardPath,
                            "BReport*",
                            System.IO.SearchOption.TopDirectoryOnly);
                    newFullName = "";

                    foreach (string str in allBReportFiles) // 挑选最新的 BReport* 文件。
                    {
                        int re = string.Compare(newFullName, str);
                        if (re < 0)
                        {
                            newFullName = str;
                        }
                    }

                    con = System.IO.File.ReadAllText(newFullName); // 获取最新的 BReport* 文件的内容。
                } // try

                if (con == "请求守护")
                {
                    string newFullNameTim = newFullName.Substring(newFullName.Length - 14); // 获取文件名称中的时间部分。
                    double newFullNameTim_double = Microsoft.VisualBasic.Conversion.Val(newFullNameTim);
                    double tim_double = Microsoft.VisualBasic.Conversion.Val(tim);
                    double interval = newFullNameTim_double - tim_double;
                    if (interval > 5) // 最近一次的报告距离当前时间超过5秒。
                    {
                        FindException(thisGuardPath, tim, "guard.exe");
                    }
                    else
                    { }
                }
                else if (con == "无需守护")
                { }
                else
                {
                    System.Windows.Forms.MessageBox.Show(
                        "我是守护系统中的 A 进程，发现 B 进程提交的报告中有无法识别的内容！");
                }
            } // if (newFullName == "")




        }

        /// <summary>被守护进程意外结束后，需要恢复其做出的更改。
        /// </summary>
        public void Restore(string thisGuardPath, string tim)
        {
            // 删除所有 BReport* 文件
            string[] allBReportFiles = System.IO.Directory.GetFiles(
                thisGuardPath,
                "BReport*",
                System.IO.SearchOption.TopDirectoryOnly);
            foreach (string str in allBReportFiles)
            {
                System.IO.File.Delete(str);
            }

            // 删除所有 BWriting* 文件
            string[] allBWritingFiles = System.IO.Directory.GetFiles(
                thisGuardPath,
                "AWriting*",
                System.IO.SearchOption.TopDirectoryOnly);
            foreach (string str in allBWritingFiles)
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
