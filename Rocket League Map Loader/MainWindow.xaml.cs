using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using RL_Map_Loader.User_Controls;
using static RL_Map_Loader.Helpers.InternetConnectionHelper;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Label = System.Windows.Controls.Label;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace RL_Map_Loader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MapsListUserControl LethamyrsMapsUserControl { get; set; }

        private MapsListUserControl DownloadedMapsUserControl { get; set; }

        private MapsListUserControl WorkshopMapsUserControl { get; set; }

        private MapsListUserControl CommunityMapsUserControl { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            LethamyrsMapsUserControl = new MapsListUserControl(AppState.LethsMaps);
            DownloadedMapsUserControl = new MapsListUserControl(AppState.DownloadedMaps);
            WorkshopMapsUserControl = new MapsListUserControl(AppState.WorkshopMaps);
            CommunityMapsUserControl = new MapsListUserControl(AppState.CommunityMaps);

            var comingSoonLabel = new Label
            {
                Content = "Coming soon",
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var communityMapsTempGrid = new Grid
            {
                Background = Brushes.Black
            };

            communityMapsTempGrid.Children.Add(comingSoonLabel);

            MapsTabControl.Items.Add(new TabItem { Header = "Lethamyr's maps", Content = LethamyrsMapsUserControl });
            MapsTabControl.Items.Add(new TabItem { Header = "Downloaded maps", Content = DownloadedMapsUserControl });
            MapsTabControl.Items.Add(new TabItem { Header = "Workshop maps", Content = WorkshopMapsUserControl });
            MapsTabControl.Items.Add(new TabItem { Header = "Community maps", Content = communityMapsTempGrid });
        }

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
            var executable = AppState.RocketLeagueExecutableFilepath;

            if (!File.Exists(executable))
            {
                MessageBox.Show("Could not find Rocket League executable");
                return;
            }

            Process.Start(executable);
        }

        private void LaunchBakkesModButton_OnClick(object sender, RoutedEventArgs e)
        {
            var executable = AppState.BakkesModExecutableFilepath;

            if (!File.Exists(executable))
            {
                MessageBox.Show("Could not find BakkesMod executable");
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

        private void OpenSteamWorkshopDownloaderButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://steamworkshopdownloader.io/");
        }

        private void LaunchHamachiButton_OnClick(object sender, RoutedEventArgs e)
        {
            if(!File.Exists(AppState.HamachiExecutableFilepath))
            {
                MessageBox.Show("Could not find Hamachi executable");
                return;
            }

            Process.Start(AppState.HamachiExecutableFilepath);
        }

        private void ImportUnityPackagesButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                DefaultExt = "UPK Files|*.upk|UDK Files|*.udk",
                Multiselect = true
            };

            var dr = openFileDialog.ShowDialog();

            if (dr != System.Windows.Forms.DialogResult.OK)
                return;

            ImportFiles(openFileDialog.FileNames);
        }

        private void ImportFiles(string[] files)
        {
            if (files == null || !files.Any())
                return;

            var dirName = Path.GetFileNameWithoutExtension(files.First());
            var dir = Path.Combine(AppState.LocalModsDirectory, dirName);

            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            foreach(var file in files)
            {
                var filename = Path.GetFileName(file);
                File.Copy(file, Path.Combine(dir, filename));
            }

            RefreshDownloadedMaps();
        }

        private void RefreshDownloadedMaps()
        {
            AppState.RefreshDownloadedMaps();
            DownloadedMapsUserControl.RefreshListedMaps(AppState.DownloadedMaps);
        }

        private void RefreshLethamyrsMaps()
        {
            AppState.RefreshLethsMaps();
            LethamyrsMapsUserControl.RefreshListedMaps(AppState.LethsMaps);
        }

        private void RefreshWorkshopMaps()
        {
            AppState.RefreshWorkshopMaps();
            WorkshopMapsUserControl.RefreshListedMaps(AppState.WorkshopMaps);
        }

        private void RefreshCommunityMaps()
        {
            AppState.RefreshCommunityMaps();
            CommunityMapsUserControl.RefreshListedMaps(AppState.CommunityMaps);
        }

        private void RefreshDownloadedMapsButton_OnClick(object sender, RoutedEventArgs e) => RefreshDownloadedMaps();

        private void RefreshLethamyrsMapsButton_OnClick(object sender, RoutedEventArgs e) => RefreshLethamyrsMaps();

        private void RefreshWorkshopMapsButton_OnClick(object sender, RoutedEventArgs e) => RefreshWorkshopMaps();

        private void RefreshCommunityMapsButton_OnClick(object sender, RoutedEventArgs e) => RefreshCommunityMaps();

        private void ViewInstructionsButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/xchaosmods/Rocket-League-Map-Loader");
        }

        private void ShowSettings_OnClick(object sender, RoutedEventArgs e) => new Settings().ShowDialog();
    }
}
