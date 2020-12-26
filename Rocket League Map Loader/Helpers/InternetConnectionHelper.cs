using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace RL_Map_Loader.Helpers
{
    public static class InternetConnectionHelper
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern bool InternetGetConnectedState(ref ConnectionState lpdwFlags, int dwReserved);

        [Flags]
        private enum ConnectionState
        {
            InternetConnectionModem = 0x01,
            InternetConnectionLan = 0x02,
            InternetConnectionProxy = 0x04,
            InternetRasInstalled = 0x10,
            InternetConnectionOffline = 0x20,
            InternetConnectionConfigured = 0x40
        }

        public static bool IsConnectedToTheInternet()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            ConnectionState description = 0;
            return InternetGetConnectedState(ref description, 0);
        }

        public static string GetLocalIP()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                return null;

            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                ?.ToString();
        }

        public static string GetPublicIP()
        {
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org");
            WebResponse response = request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream() ?? throw new WebException("Unable to connect to internet"));
            string responseString = sr.ReadToEnd().Trim();
            var ipAddress = responseString.Split(':')[1].Substring(1).Split('<')[0];
            return ipAddress;
        }
    }
}
