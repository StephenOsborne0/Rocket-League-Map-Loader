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
                CreateBackupDirectory();
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
                MessageBox.Show($"Error during initial install: {ex.Message}\n\n{ex.StackTrace}");
                MessageBox.Show($"Inner exception: {ex.InnerException.Message}\n\n{ex.InnerException.StackTrace}");
                return false;
            }
        }

        public static void UnprotectHamachi()
        {
            try
            {
                if (!AdminHelper.IsAdmin())
                    throw new Exception("Unable to edit Hamachi firewall rules - need to run as admin");

                var script = "Set-NetFirewallProfile -Profile Public -DisabledInterfaceAliases @('Hamachi')";
                PowerShell.Create().AddScript(script).Invoke();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        public static void BackupUnderpassMap()
        {
            try
            {
                var path = Path.Combine(AppState.CookedPcDirectory, "Labs_Underpass_P.upk");
                FileHelper.BackupFile(path, AppState.LocalBackupDirectory);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to backup underpass map file: {ex.Message}\n\n{ex.StackTrace}", ex);
            }
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
            try
            {
                if(!Directory.Exists(AppState.LocalModsDirectory))
                    Directory.CreateDirectory(AppState.LocalModsDirectory);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create local mods directory: {ex.Message}\n\n{ex.StackTrace}", ex);
            }
        }

        private static void CreateBackupDirectory()
        {
            try
            {
                if (!Directory.Exists(AppState.LocalBackupDirectory))
                    Directory.CreateDirectory(AppState.LocalBackupDirectory);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create local backup directory: {ex.Message}\n\n{ex.StackTrace}", ex);
            }
        }

        private static void CreateRocketLeagueModsDirectory()
        {
            try
            {
                if(!Directory.Exists(AppState.RLModsDirectory))
                    Directory.CreateDirectory(AppState.RLModsDirectory);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create rocket league mods directory: {ex.Message}\n\n{ex.StackTrace}", ex);
            }
        }

        private static void CopyExistingMods()
        {
            try
            {
                if(!Directory.Exists(AppState.LocalModsDirectory))
                    return;

                foreach(var upk in Directory.GetFiles(AppState.RLModsDirectory, "*.upk")
                    .Union(Directory.GetFiles(AppState.RLModsDirectory, "*.udk")))
                    BackupModFile(upk);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to copy existing mod files: {ex.Message}\n\n{ex.StackTrace}", ex);
            }
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
            try
            {
                //Allows bypassing of the workshop textures file for the setup in case
                //user's security blocks google drive cookie access
                var zipFile = Path.Combine(AppState.TempDirectory, "Workshop-textures.zip");

                if(!File.Exists(zipFile))
                {
                    var googleDriveId = "1te3LAFnmeKUemYHiIcmu-tnu_0uF4dSR";
                    zipFile = GoogleDrive.Download(googleDriveId);
                }

                var tempDir = Path.Combine(AppState.TempDirectory, "Workshop Textures");
                FileHelper.ExtractZipFile(zipFile, null, tempDir);

                foreach(var file in Directory.GetFiles(tempDir))
                    FileHelper.MoveFile(file, AppState.CookedPcDirectory, true, AppState.LocalBackupDirectory);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to install workshop textures: {ex.Message}\n\n{ex.StackTrace}", ex);
            }
        }
    }
}
