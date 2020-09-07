using PyLibSharp.Requests;
using System;

namespace TestCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Requests.Get("http://www.baidu.com", new ReqParams()));
        }
    }
}
