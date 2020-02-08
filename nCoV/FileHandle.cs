using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCoV
{
    class FileHandle:IDisposable
    {
        public void Dispose()
        {
            GC.Collect();
        }
        /// <summary>
        /// 追加写文件
        /// </summary>
        /// <param name="s">内容</param>
        public void WriteFile(string s)
        {
            using (StreamWriter sw = new StreamWriter(@"F:\nCoV.txt", true))
            {
                sw.WriteLine(s + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
            }
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "WriteInfo");
        }
    }
}
