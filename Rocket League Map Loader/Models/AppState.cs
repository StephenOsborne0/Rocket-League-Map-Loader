using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Rocket_League_Map_Loader.Helpers;
using Rocket_League_Map_Loader.MapGrabbers;
using Rocket_League_Map_Loader.User_Controls;

namespace Rocket_League_Map_Loader.Models
{
    public static class AppState
    {
        public static readonly string TempDirectory = Path.Combine(Path.GetTempPath(), "Rocket League Map Loader");
        public static readonly string RocketLeagueDirectory = Properties.Settings.Default.RocketLeagueInstallDirectory;
        public static readonly string CookedPcDirectory = Path.Combine(RocketLeagueDirectory, "TAGame", "CookedPCConsole");
        public static readonly string LocalModsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods");
        public static readonly string LocalBackupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backup");
        public static readonly string RLModsDirectory = Path.Combine(CookedPcDirectory, "Mods");
        public static readonly string SteamWorkshopDirectory = RocketLeagueDirectory.Contains("steamapps") ? GetSteamWorkshopPath() : null;

        public static List<Map> DownloadedMaps = new List<Map>();

        public static List<Map> LethsMaps = new List<Map>();

        public static List<Map> WorkshopMaps = new List<Map>();

        public static List<Map> CommunityMaps = new List<Map>();

        public static void RefreshLethsMaps() => LethsMaps = new LethMapGrabber().GetLethamyrsMaps();

        public static void RefreshCommunityMaps() => CommunityMaps = new LethMapGrabber().GetCommunityMaps();

        public static void RefreshDownloadedMaps()
        {
            var downloadedMaps = new List<Map>();

            foreach (var mapFile in FileHelper.FindAllMapFiles(LocalModsDirectory))
            {
                var map = TryLoadUnknownMap(mapFile);
                
                if (map != null)
                    downloadedMaps.Add(map);
            }

            DownloadedMaps = downloadedMaps;
        }

        public static void RefreshWorkshopMaps()
        {
            var workshopMaps = new List<Map>();

            foreach(var directory in Directory.GetDirectories(SteamWorkshopDirectory))
            {
                var mapFile = FileHelper.FindMapFile(directory);
                var map = TryLoadUnknownMap(mapFile);

                if (map != null)
                    workshopMaps.Add(map);
            }

            WorkshopMaps = workshopMaps;
        }

        public static Map TryLoadUnknownMap(string mapFile)
        {
            try
            {
                var directory = FileHelper.GetFileDirectory(mapFile);
                var extraMapInfo = FileHelper.FindExtraMapInfo(directory);

                Map map;

                if(extraMapInfo != null)
                {
                    var json = File.ReadAllText(extraMapInfo);
                    map = JsonConvert.DeserializeObject<Map>(json);

                    var image = FileHelper.FindMapPreview(directory);

                    if(image != null)
                    {
                        var ms = new MemoryStream(File.ReadAllBytes(image));
                        map.Image = new BitmapImage();
                        map.Image.BeginInit();
                        map.Image.StreamSource = ms;
                        map.Image.EndInit();
                    }
                }
                else
                {
                    map = new Map(mapFile);
                }

                return map;
            }
            catch(Exception ex) { return null; }
        }

        public static string GetSteamWorkshopPath() => Path.Combine(RocketLeagueDirectory, "..", "..", "workshop", "content", "252950");
    }
}
