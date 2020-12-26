using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RL_Map_Loader.User_Controls
{
    /// <summary>
    /// Interaction logic for DownloadedMapsUserControl.xaml
    /// </summary>
    public partial class DownloadedMapsUserControl : UserControl
    {
        public DownloadedMapsUserControl()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DownloadedMapsStackPanel.Children.Clear();

            foreach (var map in AppState.DownloadedMaps.OrderBy(x => x.Name))
            {
                var userControl = new MapUserControl(map) { Margin = new Thickness(5, 2, 5, 2) };
                DownloadedMapsStackPanel.Children.Add(userControl);
            }
        }
    }
}
