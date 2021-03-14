using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
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
            MapLoaded += OnThisMapLoaded;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) => UpdateMapLoaded();

        private void OnThisMapLoaded(MapLoadedEventArgs e) => UpdateMapLoaded();

        public void UpdateMapLoaded()
        {
            var currentlyLoadedMap = AppState.CurrentlyLoadedMap;

            if (currentlyLoadedMap == null)
                return;
            
            var mapLoadedIsThisMap = currentlyLoadedMap.Hash == _map.Hash || currentlyLoadedMap.Name == _map.Name;
            LoadButton.Content = mapLoadedIsThisMap ? "Loaded" : "Load";
            LoadButton.Foreground = mapLoadedIsThisMap ? Brushes.Black : Brushes.White;
            LoadButton.IsEnabled = !mapLoadedIsThisMap;
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

            if (!Directory.Exists(AppState.RLModsDirectory))
                Directory.CreateDirectory(AppState.RLModsDirectory);

            FileHelper.EmptyDirectory(AppState.RLModsDirectory);

            var mapDirectory = _map?.Directory;

            if(mapDirectory == null)
            {
                MessageBox.Show("Failed to find map directory");
                return;
            }

            var allMapFiles = FileHelper.FindAllMapFiles(mapDirectory);
            var udks = allMapFiles.FirstOrDefault(m => Path.GetExtension(m) == ".udk");
            var upks = allMapFiles.Where(m => Path.GetExtension(m) == ".upk").ToList();

            if (udks != null)
            {
                //Rename the udk file, leave the rest as is
                foreach(var mapFile in allMapFiles)
                {
                    var destinationFileName = (mapFile == udks)
                        ? "Labs_Underpass_P.upk"
                        : Path.GetFileName(mapFile);
                    var destinationFilePath = Path.Combine(AppState.RLModsDirectory, destinationFileName);
                    File.Copy(mapFile, destinationFilePath, true);
                }
            }
            else if (upks.Count == 1)
            {
                //Rename the only upk file and move it
                var destinationFileName = "Labs_Underpass_P.upk";
                var destinationFilePath = Path.Combine(AppState.RLModsDirectory, destinationFileName);
                File.Copy(upks.First(), destinationFilePath, true);
            }
            else
            {
                //Too many upks for us to tell which one is the main one.
                //Maybe we read the file format to figure it out?
                MessageBox.Show("Unsure of which upk needs renaming. Either copy and rename the files manually or contact me on Discord for help.");
                return;
            }

            foreach (var mapFile in FileHelper.FindAllMapFiles(mapDirectory))
            {
                var destinationFileName = (Path.GetExtension(mapFile) == ".udk" || Path.GetExtension(mapFile) == ".upk")
                    ? "Labs_Underpass_P.upk"
                    : Path.GetFileName(mapFile);
                var destinationFilePath = Path.Combine(AppState.RLModsDirectory, destinationFileName);
                File.Copy(mapFile, destinationFilePath, true);
            }

            var map = Map.TryLoadUnknownMap(FileHelper.FindMapFile(mapDirectory));

            if (map != null)
                OnMapLoaded(new MapLoadedEventArgs(map));

            MessageBox.Show("Map installed. Please restart Rocket League to load the map.");
        }

        //private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        //{
        //    var mbr = MessageBox.Show($"Are you sure you want to delete {_map.Directory}?", "Delete?",
        //        MessageBoxButton.YesNo);

        //    if (mbr == MessageBoxResult.Yes)
        //        Directory.Delete(_map.Directory, true);

        //    AppState.RefreshDownloadedMaps();
        //}

        private void ViewInfoButton_OnClick(object sender, RoutedEventArgs e) => new MapInfoForm(_map).Show();

        private void OnDownloadCompleted(Map.DownloadCompletedEventArgs e)
        {
            if (_map != e.Map)
                return;

            RefreshUserControlUi();
            AppState.RefreshDownloadedMaps();
        }

        //private void OnMapDeleted(Events.MapDeletedEventArgs e)
        //{

        //}

        public delegate void MapLoadedEventHandler(MapLoadedEventArgs e);

        public static event MapLoadedEventHandler MapLoaded;

        private static void OnMapLoaded(MapLoadedEventArgs e) => MapLoaded?.Invoke(e);

        public class MapLoadedEventArgs : EventArgs
        {
            public Map Map;

            public MapLoadedEventArgs(Map map) => Map = map;
        }
    }
}
