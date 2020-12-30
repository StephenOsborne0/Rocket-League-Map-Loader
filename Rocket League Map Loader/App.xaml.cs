using System;
using System.Net;
using System.Windows;
using AutoUpdaterDotNET;
using static RL_Map_Loader.Properties.Settings;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace RL_Map_Loader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string AutoUpdateUrl = "https://pastebin.com/raw/tyKGSU4F";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.Start(AutoUpdateUrl);
            RunMainApp();
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            switch(args.Error)
            {
                case null when args.IsUpdateAvailable:
                {
                    MessageBoxResult messageBoxResult;

                    if (args.Mandatory.Value)
                    {
                        messageBoxResult =
                            MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. This is required update. Press Ok to begin updating the application.", @"Update Available",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                    }
                    else
                    {
                        messageBoxResult =
                            MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {
                                        args.InstalledVersion
                                    }. Do you want to update the application now?", @"Update Available",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Information);
                    }

                    if (messageBoxResult.Equals(MessageBoxResult.Yes) || messageBoxResult.Equals(MessageBoxResult.OK))
                    {
                        try
                        {
                            if (AutoUpdater.DownloadUpdate(args)) 
                                Current.Shutdown();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }

                    break;
                }

                case null:
                    break;

                case WebException _:
                    MessageBox.Show(
                        @"There is a problem reaching update server. Please check your internet connection and try again later.",
                        @"Update Check Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                default:
                    MessageBox.Show(args.Error.Message,
                        args.Error.GetType().ToString(), MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    break;
            }
        }

        private void RunMainApp()
        {
            var isFirstTimeRun = Default.IsFirstTimeRun;

            StartupUri = isFirstTimeRun
                ? new Uri("Setup.xaml", UriKind.Relative)
                : new Uri("MainWindow.xaml", UriKind.Relative);

            if (!isFirstTimeRun)
                LoadMaps();
        }

        public static void LoadMaps()
        {
            AppState.RefreshDownloadedMaps();
            AppState.RefreshLethsMaps();
            AppState.RefreshWorkshopMaps();
            AppState.RefreshCommunityMaps();
        }
    }
}
