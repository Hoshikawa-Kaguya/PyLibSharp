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
            var str = Requests.GetAsync("http://www.baidu.com");

            Console.WriteLine("结束");
            Console.WriteLine(str.Result.Text);
            Console.ReadKey();
        }
        private static void Requests_ReqExceptionHandler(object sender, Requests.AggregateExceptionArgs e)
        {
            
            Console.WriteLine(e.AggregateException);
        }
    }
}
