using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Parser;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace nCoV
{
    class webSocket:IDisposable
    {
        public string FF(string url ,ref string[] incr)
        {
            try
            {
                using (HttpClient hc = new HttpClient())
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

                    if (metra != null)
                    {
                        string[] m = metra.Split(',',':');

                        
                        //确诊  例 疑似  例 死亡  例 治愈  例 重症  例

                        

                        DateTime time = GetLocalTime(Convert.ToInt64(m[5]), true);

                        //确诊 0
                        //疑似 1
                        //重症 2
                        //治愈 3
                        //死亡 4
                        string[] count = new string[5];
                        foreach (var item in m)
                        {
                            if (item== "\"confirmedCount\"")
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

                            //0 --> 确诊增长
                            //1 --> 疑似增长
                            //2 --> 重症增长
                            //3 --> 死亡增长
                            //4 --> 治愈增长
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

                        }

                        return "" + time.ToString("yyyy-MM-dd HH:mm") + " 确诊 " + count[0] + " 例 疑似 " + count[1] + " 例 死亡 " + count[4] + " 例 治愈 " + count[3] + " 例 重症 " + count[2] + " 例 ";
                    }
                    else
                    {
                        return "?Web Page Changed!";
                    }


                    //string s = metra.Remove(metra.IndexOf("confirmedCount")).Replace(",\"virus\":\"新型冠状病毒 2019 - nCoV\",\"remark1\":\"易感人群: 人群普遍易感。老年人及有基础疾病者感染后病情较重，儿童及婴幼儿也有发病\",\"remark2\":\"潜伏期: 一般为 3~7 天，最长不超过 14 天，潜伏期内存在传染性\",\"remark3\":\"\",\"remark4\":\"\",\"remark5\":\"\",\"generalRemark\":\"疑似病例数来自国家卫健委数据，目前为全国数据，未分省市自治区等\",\"abroadRemark\":\"\"}}catch(e){}","");
                }
            }
            catch (Exception ex)
            {
                return "?" + ex.Message;
            }
        }

        /// <summary>
        /// 将UNIX时间戳转为北京时间        
        /// </summary>
        /// <param name="unixTimeStamp">时间戳</param>
        /// <param name="accurateToMilliseconds">精确到毫秒,佛为秒</param>
        /// <returns></returns>
        public static DateTime GetLocalTime(long unixTimeStamp, bool accurateToMilliseconds)
        {
            DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1, 8, 0, 0));//北京所在东八区
            return (accurateToMilliseconds ? startTime.AddMilliseconds(unixTimeStamp) : startTime.AddSeconds(unixTimeStamp)).ToLocalTime();
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }


}
