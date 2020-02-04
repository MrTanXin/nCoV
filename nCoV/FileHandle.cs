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

        public void WriteFile(string s)
        {
            using (StreamWriter sw = new StreamWriter(@"F:\nCoV.txt", true))
            {
                sw.WriteLine(s + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
            }
        }
    }
}
