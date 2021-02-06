using PyLibSharp.Common;
using PyLibSharp.Requests;
using System;
using System.Collections.Generic;

namespace TestCore
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleEx.Print(new Dictionary<string,string>()
            {
                { "11","22"},
                { "12","22"}
            });
            Console.ReadKey();
            // var str = Requests.Get("https://api.yukari.one/setu/", new ReqParams()
            // {
            //     Timeout = 2500,
            //     IsThrowErrorForStatusCode = false,
            // });
            //
            // Console.WriteLine("结束");
            // Console.WriteLine(str.StatusCode);
            //
            // foreach (string s in str)
            // {
            //     Console.Write(s);
            // }
            //
            // Console.ReadKey();
        }

        private static void Requests_ReqExceptionHandler(object sender, Requests.AggregateExceptionArgs e)
        {
            Console.WriteLine(e.AggregateException);
        }
    }
}