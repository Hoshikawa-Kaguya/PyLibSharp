using PyLibSharp.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace TestCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var body          = new MultipartFormDataContent();
            var streamContent = new StreamContent(File.Open(@"D:\Temp\无标题.png", FileMode.Open));
            body.Add(streamContent, "file","123.png");
            var req =
                Requests.PostAsync("http://saucenao.com/search.php",
                                   new ReqParams
                                   {
                                       Params = new Dictionary<string, string>()
                                       {
                                           {"api_key","92a805aff18cbc56c4723d7e2d5100c6892fe256" },
                                           {"db","999" },
                                           {"output_type","2" },
                                           {"num_res","16" },
                                       },
                                       PostMultiPart  = body,
                                       PostParamsType = PostType.form_data,
                                       Timeout        = 10000
                                   });

            var res = req.Result.Json();
            Console.WriteLine(res);

            // var str = Requests.Get("https://www.baidu.com", new ReqParams()
            // {
            //     Timeout = 10000,
            //     IsThrowErrorForStatusCode = false,
            //     IsAutoCloseStream = false
            // });
            // byte[] buffer = new byte[50];
            // str.OutputStream.Read(buffer, 0,50);
            // Console.WriteLine(Encoding.UTF8.GetString(buffer));
            //
            // str.OutputStream.Read(buffer, 0, 50);
            // Console.WriteLine(Encoding.UTF8.GetString(buffer));
            //
            // Console.WriteLine("结束");
            // Console.WriteLine(str.StatusCode);
            //
            // foreach (string s in str)
            // {
            //     Console.Write(s);
            // }
            //


            Console.ReadKey();
        }

        private static void Requests_ReqExceptionHandler(object sender, Requests.AggregateExceptionArgs e)
        {
            Console.WriteLine(e.AggregateException);
        }
    }
}