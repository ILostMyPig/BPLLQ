using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileIni
{
    // 对ini文件要求：
    // 每项一行，并以=号分割。
    public class FileIni
    {
        // ************************ 构造函数 ********************


        public FileIni()
        {
            // 初始化字段。
            path = "";
            all = new System.Collections.ArrayList();
        }


        // ************************ 字段 ************************


        public System.Collections.ArrayList all;    // 保存所有项。
        static public string splitText = "=";       // key和value之间的分隔符。
        public string path;                         // 文件的完整名称。


        // ************************ 方法 ************************


        /// <summary>载入ini文件。</summary>
        /// <param name="f">ini文件的完整文件名。</param>
        public void LoadIniFile(string f)
        {
            string[] allLine = System.IO.File.ReadAllLines(f, Encoding.Unicode);
            this.path = f; // 文件读取成功后，再改相关数据。
            all.Clear();
            System.Collections.ArrayList tAL;
            for (int i = 0; i < allLine.GetLength(0); i++)
            {
                if (allLine[i] == string.Empty)
                { // 跳过空行。
                    continue;
                }
                else
                {
                    string[] t = null;
                    try
                    {
                        t = SplitItem(allLine[i]);
                    }
                    catch (System.ArgumentException ex)
                    {
                        string exmsg = "“" + f + "”文件中的第" + (i + 1).ToString() + "行有误。";
                        throw new System.ArgumentException(exmsg, ex);
                    }
                    tAL = new System.Collections.ArrayList();
                    tAL.Add(t[0]);
                    tAL.Add(t[1]);
                    all.Add(tAL);
                }

            }
        }
        
        /// <summary>保存。
        /// <returns>
        /// </returns></summary>
        public void Save()
        {
            string a = "";
            string b = "";
            foreach (System.Collections.ArrayList i in all)
            {
                a = a + b + i[0].ToString() + splitText + i[1].ToString();
                b = System.Environment.NewLine;
            }
            System.IO.File.WriteAllText(this.path, a, Encoding.Unicode);
        }

        /// <summary>将所有项导入ListBox。
        /// </summary>
        /// <param name="lb">System.Windows.Forms.ListBox。</param>
        public void ToListbox(ref System.Windows.Forms.ListBox lb)
        {
            foreach (System.Collections.ArrayList i in all)
            {
                lb.Items.Add(i[0].ToString() + splitText + i[1].ToString());
            }
        }

        /// <summary>增加项，若其键已存在，则更新值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        public void UpdateOrAddItem(string key, string value)
        {
            System.Collections.ArrayList t = null;
            int index = 0;
            try
            {
                index = GetIndexOfKey(key);
                t = (System.Collections.ArrayList)all[index];
                t[1] = value;
            }
            catch (KeyNotFoundException)
            {
                t = new System.Collections.ArrayList();
                t.Add(key);
                t.Add(value);
                all.Add(t);
            }
        }

        /// <summary>给一个键/值/项，删除对应内容相同的项。
        /// </summary>
        /// <param name="str">对应内容。</param>
        /// <param name="which">键/值/项，"key"/"value"/"item"。</param>
        public void RemoveItem(string str, string which)
        {
            for (int i = 0; i < all.Count; i++)
            {
                System.Collections.ArrayList a = (System.Collections.ArrayList)all[i];
                string b = string.Empty;
                switch (which)
                {
                    case "key":
                        b = a[0].ToString();
                        break;
                    case "value":
                        b = a[1].ToString();
                        break;
                    case "item":
                        b = a[0].ToString() + "=" + a[1].ToString();
                        break;
                    default:
                        break;
                }
                if (b == str)
                {
                    all.RemoveAt(i);
                    return;
                }
            }
            throw new System.Collections.Generic.KeyNotFoundException("不存在对应项。");
        }

        /// <summary>将字符串分割成项目名称和项目内容。
        /// <returns>
        /// <para>SplitItem[0]：名称。</para>
        /// <para>SplitItem[1]：内容。</para>
        /// </returns></summary>
        /// <param name="item">_in_，项。</param>  
        static public string[] SplitItem(string item)
        {

            if (null == item)
            {
                throw new System.ArgumentNullException("item 是 null。");
            }
            else if (string.Empty == item)
            {
                throw new System.ArgumentException("item 是空字符串。");
            }
            else
            {
                int xint;
                xint = item.IndexOf(splitText);
                if (-1 == xint)
                {
                    throw new System.ArgumentException("item 中没有 = 号。");
                }
                else
                {
                    string[] kV = new string[2];
                    kV[0] = item.Substring(0, xint);
                    kV[1] = item.Substring(xint + 1);
                    return kV;
                }
            }
        }

        /// <summary>键所在的索引。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns></returns>
        public int GetIndexOfKey(string key)
        {
            for (int i = 0; i < all.Count; i++)
            {
                System.Collections.ArrayList t = (System.Collections.ArrayList)all[i];
                if (t[0].ToString() == key)
                {
                    return i;
                }
            }
            throw new KeyNotFoundException(key);
        }

        /// <summary>查找键的值。<br/>
        /// <returns>
        /// <para>return 值。</para>
        /// </returns></summary>
        /// <param name="key">键。</param>
        public string GetValueOfKey(string key)
        {
            int a = GetIndexOfKey(key);
            System.Collections.ArrayList b = (System.Collections.ArrayList)all[a];
            return b[1].ToString();
        }


    }
}