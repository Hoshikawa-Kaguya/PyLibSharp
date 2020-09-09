using PyLibSharp.Requests;
using System;
using System.Threading;

namespace TestCore
{
    class Program
    {
        static void Main(string[] args)
        {
           Requests.ReqExceptionHandler += Requests_ReqExceptionHandler;
           CancellationTokenSource cts=new CancellationTokenSource();
           var data = Requests.GetAsync("https://127.0.0.1:9999",new ReqParams()
           {
               IsUseHtmlMetaEncoding = false,
               UseHandler = true,
               Timeout = 9999
           });
           Console.WriteLine("请求之后");
          // cts.Cancel();
            Console.WriteLine(data.Result);
            Console.WriteLine(data.Result.Encode);
        }
        private static void Requests_ReqExceptionHandler(object sender, Requests.AggregateExceptionArgs e)
        {
            
            Console.WriteLine(e.AggregateException);
        }
    }
}
