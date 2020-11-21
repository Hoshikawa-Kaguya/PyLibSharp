using PyLibSharp.Requests;
using System;
using System.Net.Http;
using System.Text;

namespace TestCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = Requests.GetAsync("https://124.156.146.248/setu/");

            Console.WriteLine("结束");
            Console.WriteLine(str.Result.Json());
            Console.ReadKey();
        }
        private static void Requests_ReqExceptionHandler(object sender, Requests.AggregateExceptionArgs e)
        {
            
            Console.WriteLine(e.AggregateException);
        }
    }
}
