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
        static string old = "";

        //0 --> 确诊增长
        //1 --> 疑似增长
        //2 --> 重症增长
        //3 --> 死亡增长
        //4 --> 治愈增长
        static string[] incr = new string[5] { "0", "0", "0", "0", "0" };

        static void Main(string[] args)
        {

            while (true)
            {
                funct(0);
                Thread.Sleep(10000);
            }
        }

        static int Heartbeat = 0;

        public static void funct(int i)
        {
            if ((DateTime.Now.Hour %4==0) && DateTime.Now.Minute == 0 && DateTime.Now.Second < 11)
            {
                using (StreamWriter sw = new StreamWriter(@"F:\nCoV.txt", true))
                {
                    sw.WriteLine("昨天一日确诊增长 " + incr[0] + " 疑似增长 " + incr[1] + " 重症增长 " + incr[2] + " 死亡增长 " + incr[3] + " 治愈增长 " + incr[4]+" "+ DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
                }
            }
            if (i == 2)
            {
                Console.WriteLine(DateTime.Now.ToString() + " " + "TimeOut Close！");
                return;
            }
            using (webSocket webSocket = new webSocket())
            {
                string s = webSocket.FF("https://ncov.dxy.cn/ncovh5/view/pneumonia_peopleapp?from=timeline&isappinstalled=0", ref incr);

                if (s[0] == '?')
                {
                    Console.WriteLine(DateTime.Now.ToString() + " " + s.Replace("?", ""));

                    funct(++i);

                    GC.Collect();
                    return;
                }

                if (old == s)
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
                    old = s;
                    Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + "WriteInfo");
                    using (StreamWriter sw = new StreamWriter(@"F:\nCoV.txt", true))
                    {
                        sw.WriteLine(s + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
                    }
                }


            }

        }
    }
}
