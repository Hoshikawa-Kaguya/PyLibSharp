using PyLibSharp.Requests;
using PyLibSharp.Win32Friendly;
using System;

namespace TestCore
{
    class Program
    {
        static void Main(string[] args)
        {
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
            WindowApi.ApiOptions.IsThrowErrorWhenHandlerIsNull = false;

            WindowApi
                .GetWindowByProps(IntPtr.Zero, "Form1").Close();


            Console.ReadKey();
        }

        private static void Requests_ReqExceptionHandler(object sender, Requests.AggregateExceptionArgs e)
        {
            Console.WriteLine(e.AggregateException);
        }
    }
}