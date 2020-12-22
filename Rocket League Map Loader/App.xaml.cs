using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using Rocket_League_Map_Loader.Models;

namespace Rocket_League_Map_Loader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string ServerVersionUrl = "https://pastebin.com/raw/tyKGSU4F";
        
        private bool IsUpdateAvailable
        {
            get
            {
                if(string.IsNullOrEmpty(ServerVersionUrl)) 
                    return false;

                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                var serverVersionString = new WebClient().DownloadString(ServerVersionUrl).Split('|')[0];

                if (Version.TryParse(serverVersionString, out var serverVersion))
                    return (serverVersion.Major >= currentVersion.Major &&
                           serverVersion.Minor >= currentVersion.Minor) &&
                            (serverVersion.Major != currentVersion.Minor &&
                                serverVersion.Minor != currentVersion.Minor);

                return false;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            StartupUri = Rocket_League_Map_Loader.Properties.Settings.Default.IsFirstTimeRun
                ? new Uri("Setup.xaml", UriKind.Relative) 
                : new Uri("MainWindow.xaml", UriKind.Relative);

            if(IsUpdateAvailable)
            {
                Update();
                return;
            }

            AppState.RefreshDownloadedMaps();
            AppState.RefreshWorkshopMaps();
            AppState.RefreshLethsMaps();
            AppState.RefreshCommunityMaps();
        }

        private void Update()
        {
            var tempFile = Path.Combine(AppState.TempDirectory, "update.zip");
            var wc = new WebClient();
            var downloadLink = wc.DownloadString(ServerVersionUrl).Split('|')[1];
            var updater = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updater.exe");

            if(!File.Exists(updater))
            {
                MessageBox.Show("Unable to find updater");
                return;
            }

            wc.DownloadFile(downloadLink, tempFile);

            if(!File.Exists(tempFile))
            {
                MessageBox.Show("Failed to download update");
                return;
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                { 
                    FileName = updater,
                    Arguments = $"\"{tempFile}\""
                }
            };

            process.Start();
            Current.Shutdown();
        }
    }
}
