using PyLibSharp.Requests;
using System;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {

            var str = Requests.XHR(@"GET /baidu?tn=monline_3_dg&ie=utf-8&wd=12321312 HTTP/1.1
Host: www.baidu.com
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:82.0) Gecko/20100101 Firefox/82.0
Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8
Accept-Language: zh-CN,zh;q=0.8,zh-TW;q=0.7,zh-HK;q=0.5,en-US;q=0.3,en;q=0.2
Accept-Encoding: gzip, deflate, br
Connection: keep-alive
Referer: https://i.g-fox.cn/rd29.html?engine=baidu_web&q=12321312
Cookie: BAIDUID=0D1F081DFFC8FF3C53817A1405D947C6:FG=1; BIDUPSID=0D1F081DFFC8FF3CC49C20C8A2AE788C; PSTM=1597464138; COOKIE_SESSION=0_0_0_0_1_0_0_0_0_0_0_0_0_0_20_0_1598617462_0_1598617442%7C1%230_0_1598617442%7C1
Upgrade-Insecure-Requests: 1
Pragma: no-cache
Cache-Control: no-cache");

            Console.WriteLine("结束");
            Console.WriteLine(str.Text);
            Console.ReadKey();
        }

    }
}