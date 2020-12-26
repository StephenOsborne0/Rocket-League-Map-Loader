using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace RL_Map_Loader.Extensions
{
    public static class CookieExtensions
    {
        public static List<Cookie> List(this CookieContainer container)
        {
            var cookies = new List<Cookie>();

            var table = (Hashtable)container.GetType().InvokeMember("m_domainTable",
                BindingFlags.NonPublic |
                BindingFlags.GetField |
                BindingFlags.Instance,
                null,
                container,
                null);

            foreach (string key in table.Keys)
            {
                var item = table[key];
                var items = (ICollection)item.GetType().GetProperty("Values")?.GetGetMethod().Invoke(item, null);
                cookies.AddRange(from CookieCollection cc in items from Cookie cookie in cc select cookie);
            }

            return cookies;
        }
    }
}
