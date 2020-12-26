using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using RL_Map_Loader.Helpers;
using RL_Map_Loader.MapGrabbers;
using RL_Map_Loader.Models;

namespace RL_Map_Loader
{
    public static class AppState
    {
        public static readonly string TempDirectory = Path.Combine(Path.GetTempPath(), "RL Map Loader");
        public static readonly string RocketLeagueDirectory = Properties.Settings.Default.RocketLeagueInstallDirectory;
        public static readonly string CookedPcDirectory = Path.Combine(RocketLeagueDirectory, "TAGame", "CookedPCConsole");
        public static readonly string LocalModsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods");
        public static readonly string LocalBackupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backup");
        public static readonly string RLModsDirectory = Path.Combine(CookedPcDirectory, "Mods");
        public static readonly string SteamWorkshopDirectory = RocketLeagueDirectory.Contains("steamapps") ? GetSteamWorkshopPath() : null;
        public static readonly string MapCacheDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MapCache");

        public static List<Map> DownloadedMaps = new List<Map>();
        public static List<Map> LethsMaps = new List<Map>();
        public static List<Map> WorkshopMaps = new List<Map>();
        public static List<Map> CommunityMaps = new List<Map>();

        public static void RefreshLethsMaps()
        {
            LethsMaps = new LethMapGrabber().GetLethamyrsMaps();

            if(!Directory.Exists(MapCacheDirectory))
                Directory.CreateDirectory(MapCacheDirectory);

            foreach(var map in LethsMaps) 
                map.Save(MapCacheDirectory, $"{map.Name}.json");
        }

        public static void RefreshCommunityMaps()
        {
            CommunityMaps = new LethMapGrabber().GetCommunityMaps();

            if (!Directory.Exists(MapCacheDirectory))
                Directory.CreateDirectory(MapCacheDirectory);

            foreach(var map in CommunityMaps) 
                map.Save(MapCacheDirectory, $"{map.Name}.json");
        }

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

                return Map.Load(extraMapInfo, FileHelper.FindMapFile(directory));
            }
            catch(Exception ex) { return null; }
        }

        public static string GetSteamWorkshopPath() => Path.Combine(RocketLeagueDirectory, "..", "..", "workshop", "content", "252950");
    }
}
