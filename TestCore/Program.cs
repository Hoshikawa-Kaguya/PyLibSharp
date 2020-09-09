using PyLibSharp.Requests;
using System;
using System.Linq;

namespace TestCore
{
    class Program
    {
        static void Main(string[] args)
        {
           Requests.ReqExceptionHandler += Requests_ReqExceptionHandler;
           var data = Requests.Get("https://127.0.0.1:9999",new ReqParams()
           {
               IsUseHtmlMetaEncoding = false,
               UseHandler = true
           });
            Console.WriteLine(data);
            Console.WriteLine(data.Encode);
        }
        private static void Requests_ReqExceptionHandler(object sender, Requests.AggregateExceptionArgs e)
        {
            Console.WriteLine(e.AggregateException.InnerExceptions.First().Message);
        }
    }
}
