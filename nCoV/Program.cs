using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace nCoV
{
    class Program
    {
        string[] old = new string[5] { "0", "0", "0", "0", "0" };

        //0 --> 确诊增长
        //1 --> 疑似增长
        //2 --> 重症增长
        //3 --> 死亡增长
        //4 --> 治愈增长
        //5 --> 时间
        string[] incr = new string[6] { "0", "0", "0", "0", "0", "0" };
        
        //确诊 0
        //疑似 1
        //重症 2
        //治愈 3
        //死亡 4
        string[] count = new string[5] { "0", "0", "0", "0", "0" };

        int Heartbeat = 0;

        static void Main(string[] args)
        {
            Program program = new Program();

            while (true)
            {
                program.funct(0);
                Thread.Sleep(10000);
            }
        }

        public void funct(int i)
        {
            if ((DateTime.Now.Hour %4==0) && DateTime.Now.Minute == 0 && DateTime.Now.Second < 11)
            {
                using (FileHandle fh = new FileHandle())
                {
                    string info = "昨天一日确诊增长 " + incr[0] + " 疑似增长 " + incr[1] + " 重症增长 " + incr[2] + " 死亡增长 " + incr[3] + " 治愈增长 " + incr[4] + " ";
                    fh.WriteFile(info);
                }
            }
            if (i == 2)
            {
                Console.WriteLine(DateTime.Now.ToString() + " " + "TimeOut Close！");
                return;
            }
            using (WebSocket webSocket = new WebSocket())
            {
                string s = webSocket.PageDownload("https://ncov.dxy.cn/ncovh5/view/pneumonia_peopleapp?from=timeline&isappinstalled=0");

                if (s[0] == '?')
                {
                    Console.WriteLine(DateTime.Now.ToString() + " " + s.Replace("?", ""));

                    funct(++i);

                    GC.Collect();
                    return;
                }
                else
                {
                    webSocket.PageHandle(s, ref count, ref incr);
                    if (old == count)
                    {
                        Heartbeat++;
                        if (Heartbeat == 10)
                        {
                            Console.WriteLine(DateTime.Now.ToShortTimeString() + " Heartbeat Package！");
                            Heartbeat = 0;
                        }
                    }
                    else
                    {
                        old = count;
                        Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "WriteInfo");
                        using (FileHandle fh = new FileHandle())
                        {
                            string info = Convert.ToDateTime(incr[5]).ToString("yyyy-MM-dd HH:mm") + " 确诊 " + count[0] + " 例 疑似 " + count[1] + " 例 死亡 " + count[4] + " 例 治愈 " + count[3] + " 例 重症 " + count[2] + " 例 ";

                            fh.WriteFile(info);
                        }
                    }
                }
            }
        }
    }
}
