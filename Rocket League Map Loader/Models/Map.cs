using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using RL_Map_Loader.Helpers;
using static System.IO.FileMode;

namespace RL_Map_Loader.Models
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

        private string _hash;

        public string Hash
        {
            get
            {
                if(!string.IsNullOrEmpty(_hash))
                    return _hash;

                _hash = MapFilePath != null ? HashHelper.GenerateMD5HashFromFile(MapFilePath) : null;
                return _hash;
            }
        }

        public DateTime DatePublished { get; set; }

        [JsonIgnore]
        public BitmapImage Image { get; set; }

        public string GoogleDriveId { get; set; }

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

        public void Download()
        {
            try
            {
                var downloadPath = GoogleDrive.Download(GoogleDriveId, $"{Name}.zip");
                
                if (downloadPath == null)
                    throw new InvalidOperationException($"Failed to download {Name}");

                var mapDirectory = Path.Combine(AppState.LocalModsDirectory, $"{Name}");

                if (!System.IO.Directory.Exists(mapDirectory))
                    System.IO.Directory.CreateDirectory(mapDirectory);

                FileHelper.ExtractZipFile(downloadPath, null, mapDirectory);
                Directory = mapDirectory;
                MapFilePath = FileHelper.FindMapFile(mapDirectory);
                
                Save();
                OnDownloadCompleted(new DownloadCompletedEventArgs(this));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Save()
        {
            var infoFilepath = FileHelper.FindMapInfo(Directory);
            var infoDirectory = FileHelper.GetFileDirectory(infoFilepath);
            Save(infoDirectory, "extra-info.json");
        }

        public void Save(string directory, string filename)
        {
            var outputFilePath = Path.Combine(directory, filename);
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(outputFilePath, json);
        }

        public void SaveImageSource(string directory, string filename)
        {
            var outputFilePath = Path.Combine(directory, filename);

            if (Image == null)
                return;

            using (var fileStream = new FileStream(outputFilePath, Create))
            {
                Image.StreamSource.CopyTo(fileStream);
            }
        }

        public static Map Load(string mapInfoFile, string mapFilePath = null)
        {
            if(mapInfoFile == null)
            {
                return new Map(mapFilePath)
                {
                    Image = new BitmapImage(new Uri("/Resources/no-image-found.jpg", UriKind.Relative))
                };
            }

            var json = File.ReadAllText(mapInfoFile);
            var map = JsonConvert.DeserializeObject<Map>(json);
            var imageFilePath = Path.Combine(AppState.MapCacheDirectory, $"{map.Name}.jpg");

            map.Image =  new BitmapImage(File.Exists(imageFilePath) 
                ? new Uri(imageFilePath) 
                : new Uri("/Resources/no-image-found.jpg", UriKind.Relative));

            var previouslyDownloaded = AppState.DownloadedMaps.FirstOrDefault(x => x.Name == map.Name);

            if(previouslyDownloaded != null)
                map.Directory = previouslyDownloaded.Directory;

            return map;
        }

        public static Map TryLoadUnknownMap(string mapFile)
        {
            try
            {
                var directory = FileHelper.GetFileDirectory(mapFile);
                var extraMapInfo = FileHelper.FindExtraMapInfo(directory);

                return Map.Load(extraMapInfo, FileHelper.FindMapFile(directory));
            }
            catch (Exception ex) { return null; }
        }

        public delegate void DownloadCompletedEventHandler(DownloadCompletedEventArgs e);

        public static event DownloadCompletedEventHandler DownloadCompleted;

        private static void OnDownloadCompleted(DownloadCompletedEventArgs e) => DownloadCompleted?.Invoke(e);

        public class DownloadCompletedEventArgs : EventArgs
        {
            public Map Map;

            public DownloadCompletedEventArgs(Map map) => Map = map;
        }
    }
}
