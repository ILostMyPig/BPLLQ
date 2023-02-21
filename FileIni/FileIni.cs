using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileIni
{
    public class FileIni
    {
        //构造函数

        /// <summary>初始化字段。
        ///</summary>
        public FileIni()
        {
            path = "";
            all = new System.Collections.ArrayList();
        }

        //字段

        /// <summary>项目。
        /// </summary>
        public System.Collections.ArrayList all;

        /// <summary>ini 文件的完整路径。
        /// </summary>
        public string path;

        //方法

        /// <summary>载入现有ini文件。
        /// <returns>
        /// <para>return -1：文件不存在。</para>
        /// <para>return >= 0：返回项目数量。</para>
        /// </returns></summary>
        /// <param name="path">ini文件的完整路径。</param>
        public int LoadIni(
            string path)
        {
            this.path = path;
            if (System.IO.File.Exists(path))
            {
                string[] allLine = System.IO.File.ReadAllLines(path, Encoding.Unicode );
                System.Collections.ArrayList xAL;
                string xstr = "";
                int xint;
                for (int i = 0; i < allLine.GetLength(0); i++)
                {
                    xint = CutApartEntry(ref allLine[i], ref xstr);
                    if (0 == xint)
                    {
                        xAL = new System.Collections.ArrayList();
                        xAL.Add(allLine[i]);
                        xAL.Add(xstr);
                        all.Add(xAL);
                    }
                }
                return all.Count;
            }
            else
            {
                throw new CE_FileIni.NotFoundFileIni("path指定的文件不存在。");
            }

        }

        /// <summary>根据项目名称，查找项目内容。<br/>
        /// <returns>
        /// <para>return -1：该项目不存在。</para>
        /// <para>return >= 0：该项目的引索。</para>
        /// </returns></summary>
        /// <param name="entry">_in_，项目名称。</param>
        /// <param name="content">_out_，项目内容。</param>
        public int GetEntry(
            string entry,
            ref string content)
        {
            content = "";
            System.Collections.ArrayList temp;
            for (int i = 0; i < all.Count; i++)
            {
                temp = (System.Collections.ArrayList)all[i];
                if ((string)temp[0] == entry)
                {
                    content = (string)temp[1];
                    return i;
                }
            }
            return -1;
        }

        /// <summary>增加项目，若该项目已经存在，则更新项目内容。
        /// <returns>
        /// <para>return 0：增加项目。</para>
        /// <para>return 1：更新项目。</para>
        /// </returns></summary>
        /// <param name="entry">_in_，项目名称。</param>
        /// <param name="content">_in_，项目内容。</param>
        public int SetOrAddEntry(
            string entry,
            string content)
        {
            System.Collections.ArrayList temp;
            string x = null;
            int a = GetEntry(entry, ref x);
            if (a >= 0)
            {
                temp = (System.Collections.ArrayList)all[a];
                temp[1] = content;
            }
            else
            {
                temp = new System.Collections.ArrayList();
                temp.Add(entry);
                temp.Add(content);
                all.Add(temp);
            }
            return 0;
        }

        /// <summary>储存项目到载入的文件中。
        /// <returns>
        /// <para>return 0:成功。</para>
        /// <para>return -1:还没有载入ini文件。</para>
        /// </returns></summary>
        public int SaveIni()
        {
            if ("" == this.path)
            {
                return -1;
            }
            string a = "";
            System.Collections.ArrayList temp;
            for (int i = 0; i < all.Count; i++)
            {
                temp = (System.Collections.ArrayList)all[i];
                a = a + (string)temp[0] + (string)temp[1] + System.Environment.NewLine;
            }
            System.IO.File.WriteAllText(this.path, a);
            return 0;
        }

        /// <summary>储存项目到指定的文件中。
        /// <returns>
        /// <para>return 0：成功。</para>
        /// </returns></summary>
        /// <param name="path">_in_，指定的文件。</param>
        public int SaveIni(
            string path)
        {
            string a = "";
            System.Collections.ArrayList temp;
            for (int i = 0; i < all.Count; i++)
            {
                temp = (System.Collections.ArrayList)all[i];
                a = a + (string)temp[0] + (string)temp[1];
            }
            System.IO.File.WriteAllText(path, a);
            return 0;
        }

        /// <summary>将字符串分割成项目名称和项目内容。
        /// <returns>
        /// <para>return -1：没有找到 = 号，所以无法分割。</para>
        /// <para>return -2：entry 为 null，所以无法分割。</para>
        /// <para>return -3：entry 为 string.Empty，所以无法分割。</para>
        /// <para>return 0：成功。</para>
        /// </returns></summary>
        /// <param name="entry">_in_out_，传入字符串，传出项目名称。</param>
        /// <param name="content">_out_，传出项目内容。</param>        
        private int CutApartEntry(
            ref string entry,
            ref string content)
        {
            if (null == entry)
            {
                return -2;
            }
            else if (string.Empty == entry)
            {
                return -3;
            }
            else
            {
                int xint;
                xint = entry.IndexOf("=");
                if (-1 == xint)
                {
                    return -1;
                }
                else
                {
                    content = entry.Substring(xint + 1);
                    entry = entry.Substring(0, xint);
                    return 0;
                }
            }
        }

        /// <summary>更新ini文件，新内容覆盖原内容。
        /// <returns>
        /// <para>return 0：成功。</para>
        /// </returns></summary>
        /// <param name="lb">_in_，传入存放新内容的System.Windows.Forms.ListBox。</param>
        public int CoverIni(
            ref System.Windows.Forms.ListBox lb)
        {
            string a = "";
            string b = "";
            foreach (string str in lb.Items)
            {
                a = a + b + str;
                b = System.Environment.NewLine;
            }
            System.IO.File.WriteAllText(this.path, a,Encoding.Unicode );
            return 0;
        }

        /// <summary>已经载入的ini文件中的内容存入ListBox。
        /// </summary>
        /// <param name="lb">_in_，传入存放内容的System.Windows.Forms.ListBox。</param>
        public void ToListbox(
            ref System.Windows.Forms.ListBox lb)
        {
            for (int i = 0; i < all.Count; i++)
            {
                lb.Items.Add((string)((System.Collections.ArrayList)all[i])[0] + "=" + (string)((System.Collections.ArrayList)all[i])[1]);
            }
        }
    }
}
namespace CE_FileIni
{
    public class NotFoundFileIni : System.ApplicationException
    {
        public NotFoundFileIni() { }
        public NotFoundFileIni(string message) : base(message) { }
        public NotFoundFileIni(string message, Exception inner) : base(message, inner) { }
    }
}