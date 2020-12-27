using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using static RL_Map_Loader.Helpers.InternetConnectionHelper;
using Path = System.IO.Path;

namespace RL_Map_Loader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void ExitButton_OnClick(object sender, RoutedEventArgs e) => Environment.Exit(0);

        private void ViewModsFolderButton_OnClick(object sender, RoutedEventArgs e) =>
            Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods"));

        private void AboutButton_OnClick(object sender, RoutedEventArgs e) => new About().ShowDialog();

        private void UPKFileExtractorButton_OnClick(object sender, RoutedEventArgs e) { throw new NotImplementedException(); }

        private void ForceRestartRocketLeagueButton_OnClick(object sender, RoutedEventArgs e)
        {
            var processes = Process.GetProcesses();
            var rocketLeagueProcess = processes.FirstOrDefault(x => x.ProcessName == "RocketLeague");

            if(rocketLeagueProcess == null) 
                return;

            rocketLeagueProcess.Kill();

            while (!rocketLeagueProcess.HasExited)
                Thread.Sleep(100);

            LaunchRocketLeagueButton_OnClick(sender, e);
        }

        private void DownloadAllMapsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show(
                "Warning: As I haven't implemented threading yet, the UI will freeze while all maps download. This will take a long time but you will be notified when it's finished. Continue anyway?",
                "Continue?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (messageBoxResult == MessageBoxResult.No)
                return;

            foreach(var map in AppState.LethsMaps) 
                map.Download();

            LethamyrsMapsUserControl.RefreshChildren();

            MessageBox.Show("All maps downloaded");
        }

        private void ClearCacheButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach(var file in Directory.GetFiles(AppState.MapCacheDirectory)) 
                File.Delete(file);
        }

        private void LaunchRocketLeagueButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(AppState.RocketLeagueDirectory))
                return;

            var executable = Properties.Settings.Default.RocketLeagueExecutableDirectory;

            if(!File.Exists(Properties.Settings.Default.RocketLeagueExecutableDirectory))
            {
                MessageBox.Show("Could not find Rocket League executable");
                return;
            }

            Process.Start(executable);
        }

        private void ShowExternalIpButton_OnClick(object sender, RoutedEventArgs e)
        {
            if(!IsConnectedToTheInternet())
            {
                MessageBox.Show("Not connected to the internet");
                return;
            }

            var externalIp = GetPublicIP();
            MessageBox.Show($"Your external IP address is: {externalIp}");
        }
    }
}
