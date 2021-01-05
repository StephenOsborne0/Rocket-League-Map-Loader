using System.IO;
using System.Windows;
using RL_Map_Loader.Helpers;
using RL_Map_Loader.Models;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace RL_Map_Loader.User_Controls
{
    /// <summary>
    /// Interaction logic for MapUserControl.xaml
    /// </summary>
    public partial class MapUserControl : UserControl
    {
        private readonly Map _map;

        public MapUserControl(Map map)
        {
            _map = map;
            InitializeComponent();
            RefreshUserControlUi();
            Map.DownloadCompleted += OnDownloadCompleted;
            Events.MapDeleted += OnMapDeleted;
        }

        public void RefreshUserControlUi()
        {
            Dispatcher.Invoke(() =>
            {
                DownloadButton.Visibility = _map != null && _map.Directory == null && _map.GoogleDriveId != null ? Visibility.Visible : Visibility.Hidden;
                LoadButton.Visibility = _map?.Directory != null ? Visibility.Visible : Visibility.Hidden;
                //DeleteButton.Visibility = _map?.Directory != null ? Visibility.Visible : Visibility.Hidden;

                MapNameTextbox.Content = _map?.Name ?? "N/A";

                if (_map?.ShortDescription != null)
                {
                    MapDescriptionTextbox.Text = _map.ShortDescription;
                }
                else if (_map?.Info?.desc != null)
                {
                    MapDescriptionTextbox.Text = _map.Info.desc;
                }
                else
                {
                    MapDescriptionTextbox.Text = "N/A";
                }

                MapImageBox.Source = _map?.Image;
                UpkUdkLabel.Content = _map?.MapFilePath != null ? Path.GetExtension(_map.MapFilePath).TrimStart('.') : string.Empty;

                if (_map?.MapFilePath == null)
                    return;

                UpkUdkLabel.Content = Path.GetExtension(_map.MapFilePath).TrimStart('.').ToUpper();
            });
        }

        private void DownloadButton_OnClick(object sender, RoutedEventArgs e) => _map.Download();

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            if(_map?.Directory == null) 
                return;

            var mapFilePath = FileHelper.FindMapFile(_map?.Directory);
            var destinationFilePath = Path.Combine(AppState.RLModsDirectory, "Labs_Underpass_P.upk");

            if(!Directory.Exists(AppState.RLModsDirectory))
                Directory.CreateDirectory(AppState.RLModsDirectory);
            
            File.Copy(mapFilePath, destinationFilePath, true);
            MessageBox.Show("Map installed. Please restart Rocket League to load the map.");
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var mbr = MessageBox.Show($"Are you sure you want to delete {_map.Directory}?", "Delete?",
                MessageBoxButton.YesNo);

            if (mbr == MessageBoxResult.Yes)
                Directory.Delete(_map.Directory, true);

            AppState.RefreshDownloadedMaps();
        }

        private void ViewInfoButton_OnClick(object sender, RoutedEventArgs e) => new MapInfoForm(_map).Show();

        private void OnDownloadCompleted(Map.DownloadCompletedEventArgs e)
        {
            if (_map != e.Map)
                return;

            RefreshUserControlUi();
            AppState.RefreshDownloadedMaps();
        }

        private void OnMapDeleted(Events.MapDeletedEventArgs e)
        {

        }
    }
}
