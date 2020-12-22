using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Rocket_League_Map_Loader.Helpers;

namespace Rocket_League_Map_Loader.Models
{
    public class Map
    {
        public string Directory { get; set; }

        public string MapFilePath { get; set; }

        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public string RecommendedSettings { get; set; }

        public string YoutubeUrl { get; set; }

        public EmbedData YoutubeEmbed { get; set; }

        public MapInfo Info { get; set; }

        public string Webpage { get; set; }

        public string Hash => MapFilePath != null ? HashHelper.GenerateHash(MapFilePath) : null;

        public DateTime DatePublished { get; set; }

        [JsonIgnore]
        public BitmapImage Image { get; set; }

        public string DownloadLink { get; set; }

        public bool IsUpk => MapFilePath != null && Path.GetExtension(MapFilePath) == "*.upk";

        public bool IsUdk => MapFilePath != null && Path.GetExtension(MapFilePath) == "*.udk";

        public List<string> BlogCategories { get; set; } = new List<string>();

        public List<string> Tags { get; set; } = new List<string>();

        public Map() { }

        public Map(string mapFilePath)
        {
            Directory = FileHelper.GetFileDirectory(mapFilePath);
            MapFilePath = mapFilePath;
            Name = Path.GetFileNameWithoutExtension(MapFilePath);

            var mapPreview = FileHelper.FindMapPreview(Directory);

            if (mapPreview != null)
                Image = new BitmapImage(new Uri(mapPreview));

            var mapInfo = FileHelper.FindMapInfo(Directory);

            if(mapInfo != null)
            {
                var json = File.ReadAllText(mapInfo);
                Info = JsonConvert.DeserializeObject<MapInfo>(json);
            }
        }

        public bool Download()
        {
            var downloadLink = DownloadLink;

            try
            {
                var wc = new WebClient();
                var html = wc.DownloadString(downloadLink);

                if (html.Contains("Google Drive - Virus scan warning"))
                    TryGetConfirmedDownloadLink(html, ref downloadLink);

                var downloadPath = Path.Combine(AppState.TempDirectory, $"{Name}.zip");
                wc.DownloadFile(downloadLink, downloadPath);

                var mapDirectory = Path.Combine(AppState.LocalModsDirectory, $"{Name}");

                if (!System.IO.Directory.Exists(mapDirectory))
                    System.IO.Directory.CreateDirectory(mapDirectory);

                FileHelper.ExtractZipFile(downloadPath, null, mapDirectory);
                Directory = mapDirectory;
                MapFilePath = FileHelper.FindMapFile(mapDirectory);

                return true;
            }
            catch (Exception ex)
            {
                //Process.Start(downloadLink);
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void TryGetConfirmedDownloadLink(string html, ref string downloadLink)
        {
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);

            var realDownloadLinkNode = htmlDoc.GetElementbyId("uc-download-link");
            var href = realDownloadLinkNode?.GetAttributeValue("href", null);

            if (href != null)
                downloadLink = $"https://drive.google.com{href}";
        }

        public void Save()
        {
            var infoFilepath = FileHelper.FindMapInfo(Directory);
            var infoDirectory = FileHelper.GetFileDirectory(infoFilepath);
            Save(infoDirectory);
        }

        public void Save(string directory)
        {
            var outputFilePath = Path.Combine(directory, "extra-info.json");
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(outputFilePath, json);
        }
    }
}
