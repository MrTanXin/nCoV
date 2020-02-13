using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace nCoV
{
    class Program
    {
        #region variable
        string[] old = new string[6] { "-1", "-1", "-1", "-1", "-1", "-1" };

        //0 --> 累计确诊增长
        //1 --> 疑似增长
        //2 --> 重症增长
        //3 --> 死亡增长
        //4 --> 治愈增长
        //5 --> 时间
        //6 --> 现存确诊增长
        string[] incr = new string[7] { "0", "0", "0", "0", "0", "0", "0" };

        //累计确诊 0
        //疑似 1
        //重症 2
        //治愈 3
        //死亡 4
        //现存确诊 5
        string[] count = new string[6] { "0", "0", "0", "0", "0", "0" };

        bool IsWrite = false;

        int Heartbeat = 0;
        #endregion

        /// <summary>
        /// Main Function
        /// </summary>
        public static void Main()
        {
            Program program = new Program();

            while (true)
            {
                if (DateTime.Now.Hour > 12 && program.IsWrite == false)
                {
                    program.IsWrite = true;
                }

                if (program.IsWrite && DateTime.Now.Hour < 12)
                {
                    using (FileHandle fh = new FileHandle())
                    {
                        string info = $"昨天一日 新增确诊增长 {program.incr[6]} 累计确诊增长 {program.incr[0]} 疑似增长 {program.incr[1]} 重症增长 {program.incr[2]} 死亡增长 {program.incr[3]} 治愈增长 {program.incr[4]} ";
                        fh.WriteFile(info);
                    }
                    program.IsWrite = false;
                }

                Task.Factory.StartNew(() => { program.funct(0); });

                Thread.Sleep(10000);
            }
        }

        /// <summary>
        /// Function Call
        /// </summary>
        /// <param name="i">time Count</param>
        public void funct(int i)
        {
            if (i == 2)
            {
                Console.WriteLine(DateTime.Now.ToString() + " " + "TimeOut Close！");
                return;
            }

            using (WebSocket webSocket = new WebSocket())
            {
                object locker = new object();

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
                    if (Enumerable.SequenceEqual(old, count))
                    {
                        lock (locker)
                        {
                            Heartbeat++;
                            if (Heartbeat == 12)
                            {
                                Console.WriteLine(DateTime.Now.ToShortTimeString() + " Heartbeat Package！");
                                Heartbeat = 0;
                            }
                        }
                    }
                    else
                    {
                        Array.Copy(count, old, 6);
                        using (FileHandle fh = new FileHandle())
                        {
                            string info = Convert.ToDateTime(incr[5]).ToString("yyyy-MM-dd HH:mm") + $" 现存确诊 {count[5]} 例 累计确诊 {count[0]} 例 现存疑似 {count[1]} 例 死亡 {count[4]} 例 治愈 {count[3]} 例 现存重症 {count[2]} 例 ";
                            fh.WriteFile(info);
                        }
                    }
                }
            }
        }
    }
}
