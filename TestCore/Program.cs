using PyLibSharp.Requests;
using System;
using Range = PyLibSharp.Common.Range;

namespace TestCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var range = Range.range(1, 5, -1)
                      + Range.range(6, 10, 1);
            var range2 = Range.range(1,10,1);
          
            foreach (int i in range)
            {
                Console.WriteLine(i);
            }
            foreach (int i in range2)
            {
                Console.WriteLine(i);
            }
            Console.WriteLine(range2 == range);

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