using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_fxb_win_hook
{
    class Class_WriteLog
    {
        public Class_WriteLog()
        { }

        /// <summary>写入日志。按天来分割。
        /// </summary>
        /// <param name="path">_in_，存放日志的文件夹路径(斜杠结尾)。</param>
        /// <param name="classification">_in_，日志分类的名称(类名称-函数名称)。</param>
        /// <param name="log">_in_，自定义信息。</param>
        public void Write(string path, string classification, string log)
        {
            //如果不存在就创建文件夹
            if (System.IO.Directory.Exists(path) == false)
            {
                System.IO.Directory.CreateDirectory(path);
            }

            string fileName = path
                + DateTime.Today.ToString("yyyy-MM-dd")
                + ".log";

            System.IO.FileStream fs;
            // 判断文件是否存在，不存在则创建。
            if (!System.IO.File.Exists(fileName))
            {
                fs = new System.IO.FileStream(
                    fileName,
                    System.IO.FileMode.Create,
                    System.IO.FileAccess.Write
                    ); // 创建文件。
            }
            else
            {
                fs = new System.IO.FileStream(
                    fileName,
                    System.IO.FileMode.Append,
                    System.IO.FileAccess.Write
                    ); // 打开文件,并追加内容。
            }

            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);

            sw.WriteLine("【写入日志的时间】");
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            sw.WriteLine("【所在模块】");
            sw.WriteLine(classification);

            sw.WriteLine("【自定义信息】");
            sw.WriteLine(log);

            sw.Close();
            fs.Close();

        }

        /// <summary>写入日志。按天来分割。
        /// </summary>
        /// <param name="path">_in_，存放日志的文件夹路径(斜杠结尾)。</param>
        /// <param name="classification">_in_，日志分类的名称(类名称-函数名称)。</param>
        /// <param name="log">_in_，自定义信息。</param>
        /// <param name="e">_in_，异常信息。</param>
        public void Write(string path, string classification, string log, Exception e)
        {
            //如果不存在就创建文件夹
            if (System.IO.Directory.Exists(path) == false)
            {
                System.IO.Directory.CreateDirectory(path);
            }

            string fileName = path
                + DateTime.Today.ToString("yyyy-MM-dd")
                + ".log";

            System.IO.FileStream fs;
            // 判断文件是否存在，不存在则创建。
            if (!System.IO.File.Exists(fileName))
            {
                fs = new System.IO.FileStream(
                    fileName,
                    System.IO.FileMode.Create,
                    System.IO.FileAccess.Write
                    ); // 创建文件。
            }
            else
            {
                fs = new System.IO.FileStream(
                    fileName,
                    System.IO.FileMode.Append,
                    System.IO.FileAccess.Write
                    ); // 打开文件,并追加内容。
            }

            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);

            sw.WriteLine("【写入日志的时间】");
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            sw.WriteLine("【所在模块】");
            sw.WriteLine(classification);

            sw.WriteLine("【自定义信息】");
            sw.WriteLine(log);

            sw.WriteLine("【Exception.Data】");
            foreach (System.Collections.DictionaryEntry de in e.Data)
            { sw.WriteLine("The key is '{0}' and the value is: {1}", de.Key, de.Value); }

            sw.WriteLine("【Exception.GetType】");
            sw.WriteLine(e.GetType().Name);

            sw.WriteLine("【Exception.HelpLink】");
            sw.WriteLine(e.HelpLink);

            sw.WriteLine("【Exception.Message】");
            sw.WriteLine(e.Message );

            sw.WriteLine("【Exception.Source】");
            sw.WriteLine(e.Source );

            sw.WriteLine("【Exception.StackTrace】");
            sw.WriteLine(e.StackTrace);

            sw.WriteLine("【Exception.TargetSite】");
            sw.WriteLine(e.TargetSite.ToString ());

            sw.Close();
            fs.Close();

        }


    }
}
