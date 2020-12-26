using System;
using System.Net;

namespace RL_Map_Loader.Models
{
    public class CookieAwareWebClient : WebClient
    {
        public CookieContainer CookieContainer = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            if (request is HttpWebRequest webRequest)
                webRequest.CookieContainer = CookieContainer;

            return request;
        }
    }
}
