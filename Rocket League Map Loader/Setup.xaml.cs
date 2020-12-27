using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using RL_Map_Loader.Helpers;
using MessageBox = System.Windows.MessageBox;

namespace RL_Map_Loader
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        private string[] RocketLeagueDirectoryPaths =
        {
            @"C:\Program Files\Steam\steamapps\common\rocketleague",
            @"C:\Program Files (x86)\Steam\steamapps\common\rocketleague",
            @"C:\Program Files\Epic Games\rocketleague",
            @"C:\Program Files (x86)\Epic Games\rocketleague",
            @"E:\SteamLibrary\steamapps\common\rocketleague"
        };

        private string HamachiFilePath = @"C:\Program Files (x86)\LogMeIn Hamachi\hamachi-2-ui.exe";
        private string BakkesModFilePath = @"C:\Program Files\BakkesMod\BakkesMod.exe";

        private string BakkesModPluginDirectory
        {
            get
            {
                var bakkesModPathWin32 = Path.Combine(RocketLeagueDirectoryTextbox.Text, "Binaries", "Win32", "bakkesmod");
                var bakkesModPathWin64 = Path.Combine(RocketLeagueDirectoryTextbox.Text, "Binaries", "Win64", "bakkesmod");

                return Directory.Exists(bakkesModPathWin32) 
                    ? bakkesModPathWin32 :
                    Directory.Exists(bakkesModPathWin64) 
                        ? bakkesModPathWin64 : null;
            }
        }

        private string RocketPluginFilePath =>
            BakkesModPluginDirectory != null
                ? Path.Combine(BakkesModPluginDirectory, "plugins", "RocketPlugin.dll")
                : null;

        public bool RocketLeagueDirectoryIsValid
        {
            get
            {
                var rocketLeagueBinaryPath = Path.Combine(RocketLeagueDirectoryTextbox.Text, "Binaries", "RocketLeague.exe");
                return Directory.Exists(RocketLeagueDirectoryTextbox.Text) && File.Exists(rocketLeagueBinaryPath);
            }
        }

        public bool RocketLeagueDirectoryIsSteam => RocketLeagueDirectoryTextbox.Text.Contains("steamapps");

        public bool IsHamachiInstalled => File.Exists(HamachiFilePath);

        public bool IsBakkesModInstalled => File.Exists(BakkesModFilePath) && BakkesModPluginDirectory != null;

        public bool IsRocketPluginInstalled => File.Exists(RocketPluginFilePath);

        public Setup()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        public void WindowLoaded(object sender, RoutedEventArgs e) => UpdateRocketLeagueDirectoryExists();

        private void HamachiInstalledCallback(object sender, EventArgs e) => UpdateHamachiInstalled();

        private void BakkesModInstalledCallback(object sender, EventArgs e) => UpdateBakkesModInstalled();

        private void RocketPluginInstalledCallback(object sender, EventArgs e) => UpdateRocketPluginInstalled();

        private BitmapImage TrueOrFalseIcon(bool condition) => new BitmapImage(new Uri(condition ? "/Resources/tick.ico" : "/Resources/error.ico", UriKind.Relative));

        private void RocketLeagueDirectoryTextbox_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateRocketLeagueDirectoryExists();

        public void UpdateRocketLeagueDirectoryExists()
        {
            if (CompleteSetupButton != null)
                CompleteSetupButton.IsEnabled = RocketLeagueDirectoryIsValid;

            if (BakkesModButton != null)
                BakkesModButton.IsEnabled = RocketLeagueDirectoryIsValid && RocketLeagueDirectoryIsSteam;

            if (RocketPluginButton != null)
                RocketPluginButton.IsEnabled = RocketLeagueDirectoryIsValid && RocketLeagueDirectoryIsSteam;

            if (RocketLeagueDirectoryExistsIcon != null)
                RocketLeagueDirectoryExistsIcon.Source = TrueOrFalseIcon(RocketLeagueDirectoryIsValid);

            UpdateHamachiInstalled();
            UpdateBakkesModInstalled();
            UpdateRocketPluginInstalled();
        }

        public void UpdateHamachiInstalled()
        {
            if (HamachiDirectoryExistsIcon != null)
                HamachiDirectoryExistsIcon.Source = TrueOrFalseIcon(IsHamachiInstalled);
        }

        public void UpdateBakkesModInstalled()
        {
            if (BakkesModDirectoryExistsIcon != null)
                BakkesModDirectoryExistsIcon.Source = TrueOrFalseIcon(IsBakkesModInstalled);
        }

        public void UpdateRocketPluginInstalled()
        {
            if (RocketPluginDirectoryExistsIcon != null)
                RocketPluginDirectoryExistsIcon.Source = TrueOrFalseIcon(IsRocketPluginInstalled);
        }

        private void AutoFindButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dirs = new Dictionary<string, bool>();

            foreach(var dir in RocketLeagueDirectoryPaths) 
                dirs.Add(dir, Directory.Exists(dir));

            var foundDirectories = dirs.Where(x => x.Value).ToList();

            if(!foundDirectories.Any())
            {
                MessageBox.Show("Failed to find a rocket league directory");
                return;
            }

            if(foundDirectories.Count() > 1)
            {
                var locations = string.Join("\r\n", foundDirectories.Select(x => x.Key));
                MessageBox.Show($"Found {foundDirectories.Count} rocket league directories at the following locations. Please manually input the correct one:\r\n{locations}");
                return;
            }

            RocketLeagueDirectoryTextbox.Text = foundDirectories.First().Key;
        }

        private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            if(folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                RocketLeagueDirectoryTextbox.Text = folderBrowserDialog.SelectedPath;
        }

        private void HamachiButton_OnClick(object sender, RoutedEventArgs e)
        {
            var url = "https://secure.logmein.com/hamachi.msi";
            var temp = Path.Combine(AppState.TempDirectory, "Hamachi installer.msi");
            new WebClient().DownloadFile(url, temp);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo(temp)
            };

            process.Exited += HamachiInstalledCallback;
            process.Start();
        }

        private void BakkesModButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!RocketLeagueDirectoryIsValid)
            {
                MessageBox.Show("Please select a valid rocket league directory first.");
                return;
            }

            var extractionDirectory = FirstTimeRun.SetupBakkesMod();

            var process = new Process
            {
                StartInfo = new ProcessStartInfo(Path.Combine(extractionDirectory, "BakkesModSetup.exe"))
            };

            process.Exited += BakkesModInstalledCallback;
            process.Start();
        }

        private void RocketPluginButton_OnClick(object sender, RoutedEventArgs e)
        {
            if(!RocketLeagueDirectoryIsValid)
            {
                MessageBox.Show("Please select a valid rocket league directory first.");
                return;
            }

            if (BakkesModPluginDirectory == null)
            {
                MessageBox.Show(
                    "Failed to find BakkesMod install directory. Please make sure that you install BakkesMod before Rocket Plugin");
                return;
            }

            FirstTimeRun.InstallRocketPlugin(BakkesModPluginDirectory);
            RocketPluginInstalledCallback(this, new EventArgs());
        }

        private void CompleteSetupButton_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.RocketLeagueInstallDirectory = RocketLeagueDirectoryTextbox.Text;
            Properties.Settings.Default.Save();

            var success = FirstTimeRun.Run();

            if (success)
            {
                App.LoadMaps();
                new MainWindow().Show();
                Close();
            }
        }
    }
}
