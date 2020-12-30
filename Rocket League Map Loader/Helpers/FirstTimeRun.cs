using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace RL_Map_Loader.Helpers
{
    public class FirstTimeRun
    {
        public static bool Run()
        {
            try
            {
                BackupUnderpassMap();
                InstallWorkshopTextures();
                CreateLocalModsDirectory();
                CreateRocketLeagueModsDirectory();
                CopyExistingMods();

                Properties.Settings.Default.IsFirstTimeRun = false;
                Properties.Settings.Default.Save();
                MessageBox.Show("Setup complete. Please wait whilst maps load...");
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error during initial install: {ex.Message}");
                return false;
            }
        }

        public static void UnprotectHamachi()
        {
            try
            {
                //Must be run as admin else it does fuck all
                var script = "Set-NetFirewallProfile -Profile Public -DisabledInterfaceAliases @('Hamachi')";
                PowerShell.Create().AddScript(script).Invoke();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void BackupUnderpassMap()
        {
            var path = Path.Combine(AppState.CookedPcDirectory, "Labs_Underpass_P.upk");
            FileHelper.BackupFile(path, AppState.LocalBackupDirectory);
        }

        public static string SetupBakkesMod()
        {
            var url = "https://download.bakkesmod.com/BakkesModSetup.zip";
            var temp = Path.Combine(AppState.TempDirectory, "BakkesMod.zip");
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
            MessageBox.Show("Rocket plugin installed");
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

            foreach(var upk in Directory.GetFiles(AppState.RLModsDirectory, "*.upk")
                .Union(Directory.GetFiles(AppState.RLModsDirectory, "*.udk"))) 
                BackupModFile(upk);
        }

        private static void BackupModFile(string fileName)
        {
            var dir = Path.Combine(AppState.LocalModsDirectory, Path.GetFileNameWithoutExtension(fileName));

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            FileHelper.MoveFile(fileName, dir);
        }

        private static void InstallWorkshopTextures()
        {
            var googleDriveId = "1te3LAFnmeKUemYHiIcmu-tnu_0uF4dSR";
            var downloadedFile = GoogleDrive.Download(googleDriveId);
            var tempDir = Path.Combine(AppState.TempDirectory, "Workshop Textures");

            FileHelper.ExtractZipFile(downloadedFile, null, tempDir);

            foreach(var file in Directory.GetFiles(tempDir))
                FileHelper.MoveFile(file, AppState.CookedPcDirectory, true, AppState.LocalBackupDirectory);
        }
    }
}
