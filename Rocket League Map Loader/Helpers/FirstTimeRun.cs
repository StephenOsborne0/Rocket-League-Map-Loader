using System;
using System.IO;
using System.Net;
using System.Windows;
using Rocket_League_Map_Loader.Models;

namespace Rocket_League_Map_Loader.Helpers
{
    public class FirstTimeRun
    {
        public static bool Run()
        {
            try
            {
                InstallWorkshopTextures();
                CreateLocalModsDirectory();
                CreateRocketLeagueModsDirectory();
                CopyExistingMods();

                Properties.Settings.Default.IsFirstTimeRun = false;
                Properties.Settings.Default.Save();
                MessageBox.Show("Setup complete");
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error during initial install: {ex.Message}");
                return false;
            }
        }

        public static string SetupBakkesMod()
        {
            var url = "https://download.bakkesmod.com/BakkesModSetup.zip";
            var temp = Path.Combine(Path.GetTempPath(), "Rocket League Map Loader", "BakkesMod.zip");
            new WebClient().DownloadFile(url, temp);

            var tempDir = Path.Combine(AppState.TempDirectory, "BakkesMod");

            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            FileHelper.ExtractZipFile(temp, null, tempDir);
            return tempDir;
        }

        public static void InstallRocketPlugin(string bakkesModPluginDirectory)
        {
            var url = "https://bakkesplugins.com/plugins/download/26";
            var temp = Path.Combine(AppState.TempDirectory, "RocketPlugin.zip");
            new WebClient().DownloadFile(url, temp);

            var tempDir = Path.Combine(AppState.TempDirectory, "Rocket Plugin");

            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            FileHelper.ExtractZipFile(temp, null, tempDir);
            FileHelper.CopyDirectory(tempDir, bakkesModPluginDirectory, true);
        }

        private static void CreateLocalModsDirectory()
        {
            if(!Directory.Exists(AppState.LocalModsDirectory))
                Directory.CreateDirectory(AppState.LocalModsDirectory);
        }

        private static void CreateBackupDirectory()
        {
            if (!Directory.Exists(AppState.LocalBackupDirectory))
                Directory.CreateDirectory(AppState.LocalBackupDirectory);
        }

        private static void CreateRocketLeagueModsDirectory()
        {
            if(!Directory.Exists(AppState.RLModsDirectory))
                Directory.CreateDirectory(AppState.RLModsDirectory);
        }

        private static void CopyExistingMods()
        {
            if(!Directory.Exists(AppState.LocalModsDirectory)) 
                return;

            foreach (var upk in Directory.GetFiles(AppState.RLModsDirectory, "*.upk")) 
                FileHelper.MoveFile(upk, AppState.LocalModsDirectory);

            foreach (var udk in Directory.GetFiles(AppState.RLModsDirectory, "*.udk")) 
                FileHelper.MoveFile(udk, AppState.LocalModsDirectory);
        }

        private static void InstallWorkshopTextures()
        {
            var workshopTexturesStream = ResourceManager.LoadResource("Rocket_League_Map_Loader.Resources.Workshop-textures.zip");

            var tempDir = Path.Combine(AppState.TempDirectory, "Workshop Textures");

            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            FileHelper.ExtractZipFile(workshopTexturesStream, null, tempDir);

            foreach(var file in Directory.GetFiles(tempDir))
                FileHelper.MoveFile(file, AppState.CookedPcDirectory, true, AppState.LocalBackupDirectory);
        }
    }
}
