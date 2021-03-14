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
        public Setup()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        private string BakkesModDataDirectory
        {
            get
            {
                var roamingAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var bakkesmodDataDirectory = Path.Combine(roamingAppData, "bakkesmod", "bakkesmod");

                var bakkesModDirectory = Directory.Exists(bakkesmodDataDirectory) ? bakkesmodDataDirectory : null;

                SetBakkesModDataDirectoryPath(bakkesModDirectory);
                return bakkesModDirectory;
            }
        }

        private string RocketPluginFilePath =>
            BakkesModDataDirectory != null
                ? Path.Combine(BakkesModDataDirectory, "plugins", "RocketPlugin.dll")
                : null;

        public bool RocketLeagueDirectoryIsValid => Directory.Exists(RocketLeagueDirectoryTextbox.Text) && RocketLeagueExecutableExists;

        public bool RocketLeagueExecutableExists
        {
            get
            {
                SetRocketLeagueExecutablePath();
                return File.Exists(AppState.RocketLeagueExecutableFilepath);
            }
        }

        public void SetBakkesModDataDirectoryPath(string bakkesModDirectory) => AppState.BakkesModDataDirectory = bakkesModDirectory;

        public void SetRocketLeagueExecutablePath()
        {
            var rocketLeagueBinaryPath1 = Path.Combine(RocketLeagueDirectoryTextbox.Text, "Binaries", "RocketLeague.exe");
            var rocketLeagueBinaryPath2 = Path.Combine(RocketLeagueDirectoryTextbox.Text, "Binaries", "Win64", "RocketLeague.exe");
            var rocketLeagueBinaryPath3 = Path.Combine(RocketLeagueDirectoryTextbox.Text, "Binaries", "Win32", "RocketLeague.exe");

            if(File.Exists(rocketLeagueBinaryPath1))
                AppState.RocketLeagueExecutableFilepath = rocketLeagueBinaryPath1;
            else if (File.Exists(rocketLeagueBinaryPath2))
                AppState.RocketLeagueExecutableFilepath = rocketLeagueBinaryPath2;
            else if (File.Exists(rocketLeagueBinaryPath3))
                AppState.RocketLeagueExecutableFilepath = rocketLeagueBinaryPath3;
        }

        public bool IsHamachiInstalled => RegistryHelper.IsExecutableInstalled("hamachi-2-ui.exe");

        public bool IsBakkesModInstalled => RegistryHelper.IsExecutableInstalled("BakkesMod.exe") && BakkesModDataDirectory != null;

        public bool IsRocketPluginInstalled => File.Exists(RocketPluginFilePath);

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            RocketLeagueDirectoryTextbox.Text = TryFindRocketLeagueDirectory();
            UpdateRocketLeagueDirectoryExists();
        }

        public string TryFindRocketLeagueDirectory()
        {
            var path = RegistryHelper.FindExecutableFilePath("RocketLeague.exe");
            var index = path?.IndexOf("rocketleague", StringComparison.Ordinal) + 12;
            return path?.Substring(0, (int)index);
        }

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
                BakkesModButton.IsEnabled = RocketLeagueDirectoryIsValid;

            if (RocketPluginButton != null)
                RocketPluginButton.IsEnabled = RocketLeagueDirectoryIsValid;

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

            var path = RegistryHelper.FindExecutableFilePath("hamachi-2-ui.exe");

            if (path == null) 
                return;

            AppState.HamachiDirectory = path.Substring(0, path.IndexOf("hamachi-2-ui.exe", StringComparison.Ordinal));
            AppState.HamachiExecutableFilepath = path;
        }

        public void UpdateBakkesModInstalled()
        {
            if (BakkesModDirectoryExistsIcon != null)
                BakkesModDirectoryExistsIcon.Source = TrueOrFalseIcon(IsBakkesModInstalled);

            var path = RegistryHelper.FindExecutableFilePath("BakkesMod.exe");

            if (path != null)
                AppState.BakkesModExecutableFilepath = path;
        }

        public void UpdateRocketPluginInstalled()
        {
            if (RocketPluginDirectoryExistsIcon != null)
                RocketPluginDirectoryExistsIcon.Source = TrueOrFalseIcon(IsRocketPluginInstalled);
        }

        private void AutoFindButton_OnClick(object sender, RoutedEventArgs e)
        {
            var path = RegistryHelper.FindExecutableFilePath("RocketLeague.exe");

            if (path == null)
            {
                MessageBox.Show("Failed to find Rocket League directory");
                return;
            }

            RocketLeagueDirectoryTextbox.Text = TryFindRocketLeagueDirectory();
        }

        private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            if(folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                RocketLeagueDirectoryTextbox.Text = folderBrowserDialog.SelectedPath;

            UpdateRocketLeagueDirectoryExists();
        }

        private void HamachiButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
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
            catch(Exception ex) { MessageBox.Show($"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void BakkesModButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!RocketLeagueDirectoryIsValid)
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
            catch(Exception ex) { MessageBox.Show($"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void RocketPluginButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!RocketLeagueDirectoryIsValid)
                {
                    MessageBox.Show("Please select a valid rocket league directory first.");
                    return;
                }

                if (BakkesModDataDirectory == null)
                {
                    MessageBox.Show(
                        "Failed to find BakkesMod install directory. Please make sure that you install BakkesMod before Rocket Plugin");
                    return;
                }

                FirstTimeRun.InstallRocketPlugin(BakkesModDataDirectory);
                RocketPluginInstalledCallback(this, new EventArgs());
            }
            catch (Exception ex) { MessageBox.Show($"{ex.Message}\n{ex.StackTrace}"); }
        }

        private void CompleteSetupButton_OnClick(object sender, RoutedEventArgs e)
        {
            AppState.RocketLeagueInstallDirectory = RocketLeagueDirectoryTextbox.Text;

            if (IsHamachiInstalled)
                FirstTimeRun.UnprotectHamachi();

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
