using System;
using System.Collections;
using System.Net;
using System.Reflection;

namespace PyLibSharp.Requests
{
    public static class Utils
    {
        public static CookieCollection GetAllCookies(CookieContainer cookieJar)
        {
            if (!(cookieJar is null) && cookieJar.Count > 0)
            {
                CookieCollection cookieCollection = new CookieCollection();

                Hashtable table = (Hashtable)cookieJar.GetType().InvokeMember("m_domainTable",
                                                                               BindingFlags.NonPublic |
                                                                               BindingFlags.GetField |
                                                                               BindingFlags.Instance,
                                                                               null,
                                                                               cookieJar,
                                                                               new object[] { });

                foreach (var tableKey in table.Keys)
                {
                    String str_tableKey = (string)tableKey;

                    if (str_tableKey[0] == '.')
                    {
                        str_tableKey = str_tableKey.Substring(1);
                    }

                    SortedList list = (SortedList)table[tableKey].GetType().InvokeMember("m_list",
                        BindingFlags.NonPublic |
                        BindingFlags.GetField |
                        BindingFlags.Instance,
                        null,
                        table[tableKey],
                        new object[] { });

                    foreach (var listKey in list.Keys)
                    {
                        String url = "http://" + str_tableKey + (string)listKey;
                        cookieCollection.Add(cookieJar.GetCookies(new Uri(url)));
                    }
                }

                return cookieCollection;
            }
            else
            {
                return new CookieCollection();
            }
        }
    }
}
