using PyLibSharp.Requests;
using System;

namespace TestCore6
{
    class Program
    {
        static void Main(string[] args)
        {
            var req =
                Requests.Get("https://api.vc.bilibili.com/dynamic_svr/v1/dynamic_svr/space_history",new ReqParams()
                                                     {
                                                         Params =
                                                         {
                                                             {"host_uid","123"},
                                                             {"offset_dynamic_id","1"}
                                                         }
                                                     });
            var res = req;
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