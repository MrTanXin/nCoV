using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Parser;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace nCoV
{
    class WebSocket : IDisposable
    {
        /// <summary>
        /// 爬取网站中需要的信息
        /// </summary>
        /// <param name="url">需要被爬取的url</param>
        /// <returns>返回网页内容</returns>
        public string PageDownload(string url)
        {
            try
            {
                using (HttpClient hc = new HttpClient(handler: new HttpClientHandler() { Proxy = new WebProxy() }, disposeHandler: true))
                {

                    var htmlString = hc.GetStringAsync(url).Result;
                    HtmlParser htmlParser = new HtmlParser();
                    var data = htmlParser.ParseDocument(htmlString)
                        .QuerySelectorAll("body")
                        .Select(t => t)
                        .ToList();
                    string metra = "";
                    foreach (var item in data[0].Children)
                    {
                        if (item.Id == "getStatisticsService")
                        {
                            metra = item.TextContent;
                        }
                    }
                    return metra.Length != 0 ? metra : "?Empty Info";
                }
            }
            catch (Exception ex)
            {
                return "?" + ex.Message;
            }
        }

        /// <summary>
        /// 对爬取的页面做切片取出有用的内容
        /// </summary>
        /// <param name="metra">源内容</param>
        /// <param name="count">人数总计</param>
        /// <param name="incr">新增人数</param>
        public void PageHandle(string metra, ref string[] count, ref string[] incr)
        {

            string[] m = metra.Split(',', ':');

            //确诊  例 疑似  例 死亡  例 治愈  例 重症  例

            DateTime time = GetLocalTime(Convert.ToInt64(m[5]), true);


            foreach (var item in m)
            {

                //确诊 0
                //疑似 1
                //重症 2
                //治愈 3
                //死亡 4
                //现存确诊 5
                if (item == "\"confirmedCount\"")
                {
                    count[0] = m[Array.IndexOf(m, item) + 1];
                }
                if (item == "\"suspectedCount\"")
                {
                    count[1] = m[Array.IndexOf(m, item) + 1];
                }
                if (item == "\"seriousCount\"")
                {
                    count[2] = m[Array.IndexOf(m, item) + 1];
                }
                if (item == "\"curedCount\"")
                {
                    count[3] = m[Array.IndexOf(m, item) + 1];
                }
                if (item == "\"deadCount\"")
                {
                    count[4] = m[Array.IndexOf(m, item) + 1];
                }
                if (item == "\"currentConfirmedCount\"")
                {
                    count[5] = m[Array.IndexOf(m, item) + 1];
                }

                //0 --> 确诊增长
                //1 --> 疑似增长
                //2 --> 重症增长
                //3 --> 死亡增长
                //4 --> 治愈增长
                //6 --> 现存确诊增长
                if (item == "\"confirmedIncr\"")
                {
                    incr[0] = m[Array.IndexOf(m, item) + 1];
                }
                if (item == "\"suspectedIncr\"")
                {
                    incr[1] = m[Array.IndexOf(m, item) + 1];
                }
                if (item == "\"seriousIncr\"")
                {
                    incr[2] = m[Array.IndexOf(m, item) + 1];
                }
                if (item == "\"deadIncr\"")
                {
                    incr[3] = m[Array.IndexOf(m, item) + 1];
                }
                if (item == "\"curedIncr\"")
                {
                    incr[4] = m[Array.IndexOf(m, item) + 1];
                }
                if (incr[5] != time.ToString("yyyy-MM-dd HH:mm:ss"))
                {
                    incr[5] = time.ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (item == "\"currentConfirmedIncr\"")
                {
                    incr[6] = m[Array.IndexOf(m, item) + 1];
                }
            }
        }

        /// <summary>
        /// 将UNIX时间戳转为北京时间        
        /// </summary>
        /// <param name="unixTimeStamp">时间戳</param>
        /// <param name="accurateToMilliseconds">精确到毫秒,否为秒</param>
        /// <returns></returns>
        public DateTime GetLocalTime(long unixTimeStamp, bool accurateToMilliseconds)
        {
            DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1, 8, 0, 0));//北京所在东八区
            return (accurateToMilliseconds ? startTime.AddMilliseconds(unixTimeStamp) : startTime.AddSeconds(unixTimeStamp)).ToLocalTime();
        }

        /// <summary>
        /// 析构
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
        }
    }


}
