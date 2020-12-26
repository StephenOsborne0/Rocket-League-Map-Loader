using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using HtmlAgilityPack;
using RL_Map_Loader.Extensions;
using RL_Map_Loader.Models;

namespace RL_Map_Loader.Helpers
{
    public class GoogleDrive
    {
        public static string Download(string id, string fileName = null)
        {
            var downloadLink = $"https://drive.google.com/u/0/uc?id={id}&export=download";

            try
            {
                var wc = new CookieAwareWebClient();
                var response = wc.DownloadString(downloadLink);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                if (fileName == null)
                {
                    var filenameNode = htmlDoc.GetElementsByClassName("uc-name-size").FirstOrDefault();
                    fileName = filenameNode != null ? filenameNode.FirstChild.InnerText : Path.GetFileName(Path.GetTempFileName());
                }

                if(!Directory.Exists(AppState.TempDirectory))
                    Directory.CreateDirectory(AppState.TempDirectory);

                var downloadPath = Path.Combine(AppState.TempDirectory, fileName);

                var cookies = wc.CookieContainer.List();

                foreach (Cookie cookie in cookies)
                {
                    if(!cookie.Name.StartsWith("download_warning")) 
                        continue;

                    downloadLink += $"&confirm={cookie.Value}"; 
                    break;
                }

                wc.DownloadFile($"{downloadLink}", downloadPath);
                return downloadPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\r\n{ex.InnerException}");
                return null;
            }
        }
    }
}
