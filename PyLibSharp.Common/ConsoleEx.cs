using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace PyLibSharp.Common
{
    public static class ConsoleEx
    {
        private static bool HasToStringMethod(this object obj)
            => obj.GetType()
                  .GetMethods(BindingFlags.Public
                            | BindingFlags.DeclaredOnly |
                              BindingFlags.Instance)
                  .Where(i => i.DeclaringType != typeof(object))
                  .Where(i => !i.GetParameters().Any())
                  .Any(i => i.Name == "ToString");

        private static bool PrintWhenHasToString(this object obj)
        {
            if (obj.HasToStringMethod())
            {
                Console.Write(obj.ToString());
                return true;
            }

            return false;
        }

        public static void Print(object obj)
        {
            //如果对象已显式实现ToString，则直接调用
            if (obj.PrintWhenHasToString()) return;

            if (obj is IEnumerable lst)
            {
                Console.Write("{");
                IEnumerator enumerator = lst.GetEnumerator();
                object      previous   = null;
                while (enumerator.MoveNext())
                {
                    if (previous != null)
                    {
                        Print(previous);
                        Console.Write(",");
                    }

                    previous = enumerator.Current;
                }

                Print(previous);
                Console.Write("}");
            }
        }
    }
}