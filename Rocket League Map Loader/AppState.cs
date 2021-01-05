using System;
using System.Collections.Generic;
using System.IO;
using RL_Map_Loader.Helpers;
using RL_Map_Loader.MapGrabbers;
using RL_Map_Loader.Models;

namespace RL_Map_Loader
{
    public static class AppState
    {
        internal static Properties.Settings Settings { get; set; } = new Properties.Settings();

        public static string RocketLeagueInstallDirectory
        {
            get => Settings.RocketLeagueInstallDirectory ?? Properties.Settings.Default.RocketLeagueInstallDirectory;
            set
            {
                Settings.RocketLeagueInstallDirectory = value;
                Settings.Save();
            }
        }

        public static string BakkesModDataDirectory
        {
            get => Settings.BakkesModDataDirectory ?? Properties.Settings.Default.BakkesModDataDirectory;
            set
            {
                Settings.BakkesModDataDirectory = value;
                Settings.Save();
            }
        }

        public static string RocketLeagueExecutableFilepath
        {
            get => Settings.RocketLeagueExecutableFilepath ?? Properties.Settings.Default.RocketLeagueExecutableFilepath;
            set
            {
                Settings.RocketLeagueExecutableFilepath = value;
                Settings.Save();
            }
        }

        public static string BakkesModExecutableFilepath
        {
            get => Settings.BakkesModExecutableFilepath ?? Properties.Settings.Default.BakkesModExecutableFilepath;
            set
            {
                Settings.BakkesModExecutableFilepath = value;
                Settings.Save();
            }
        }
        public static string HamachiDirectory
        {
            get => Settings.HamachiDirectory ?? Properties.Settings.Default.HamachiDirectory;
            set
            {
                Settings.HamachiDirectory = value;
                Settings.Save();
            }
        }

        public static string HamachiExecutableFilepath
        {
            get => Settings.HamachiExecutableFilepath ?? Properties.Settings.Default.HamachiExecutableFilepath;
            set
            {
                Settings.HamachiExecutableFilepath = value;
                Settings.Save();
            }
        }

        public static string CookedPcDirectory => Path.Combine(RocketLeagueInstallDirectory, "TAGame", "CookedPCConsole");

        public static string TempDirectory => Path.Combine(Path.GetTempPath(), "RL Map Loader");

        public static string LocalModsDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods");

        public static string LocalBackupDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backup");

        public static string RLModsDirectory => Path.Combine(CookedPcDirectory, "rocketplugin");

        public static string SteamWorkshopDirectory => RocketLeagueInstallDirectory.Contains("steamapps") ? GetSteamWorkshopPath() : null;

        public static string MapCacheDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MapCache");

        public static bool RocketLeagueDirectoryIsSteam => RocketLeagueInstallDirectory.Contains("steamapps");

        public static List<Map> DownloadedMaps = new List<Map>();
        public static List<Map> LethsMaps = new List<Map>();
        public static List<Map> WorkshopMaps = new List<Map>();
        public static List<Map> CommunityMaps = new List<Map>();

        public static void RefreshLethsMaps()
        {
            if (!Directory.Exists(MapCacheDirectory))
                Directory.CreateDirectory(MapCacheDirectory);

            LethsMaps = new LethMapGrabber().GetLethamyrsMaps();

            foreach(var map in LethsMaps) 
                map.Save(MapCacheDirectory, $"{map.Name}.json");
        }

        public static void RefreshCommunityMaps()
        {
            if (!Directory.Exists(MapCacheDirectory))
                Directory.CreateDirectory(MapCacheDirectory);

            CommunityMaps = new LethMapGrabber().GetCommunityMaps();

            foreach(var map in CommunityMaps) 
                map.Save(MapCacheDirectory, $"{map.Name}.json");
        }

        public static void RefreshDownloadedMaps()
        {
            var downloadedMaps = new List<Map>();

            if(!Directory.Exists(LocalModsDirectory))
                Directory.CreateDirectory(LocalModsDirectory);

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

            if(!Directory.Exists(SteamWorkshopDirectory))
                return;

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

        public static string GetSteamWorkshopPath()
        {
            var path = Path.Combine(RocketLeagueInstallDirectory, "..", "..", "workshop", "content", "252950");
            return Directory.Exists(path) ? path : null;
        }
    }
}
