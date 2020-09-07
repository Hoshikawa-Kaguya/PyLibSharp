using PyLibSharp.Requests;
using System;

namespace TestCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Requests.Get("https://www.qq.com"));
        }
    }
}
