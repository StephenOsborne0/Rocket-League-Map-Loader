using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RL_Map_Loader.User_Controls
{
    /// <summary>
    /// Interaction logic for CommunityMapsUserControl.xaml
    /// </summary>
    public partial class CommunityMapsUserControl : UserControl
    {
        public CommunityMapsUserControl()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CommunityMapsStackPanel.Children.Clear();

            foreach (var map in AppState.CommunityMaps.OrderBy(x => x.Name))
            {
                var userControl = new MapUserControl(map) { Margin = new Thickness(5, 2, 5, 2) };
                CommunityMapsStackPanel.Children.Add(userControl);
            }
        }
    }
}
