using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RL_Map_Loader.User_Controls
{
    /// <summary>
    /// Interaction logic for WorkshopMapsUserControl.xaml
    /// </summary>
    public partial class WorkshopMapsUserControl : UserControl
    {
        public WorkshopMapsUserControl()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            WorkshopMapsStackPanel.Children.Clear();

            foreach (var map in AppState.WorkshopMaps.OrderBy(x => x.Name))
            {
                var userControl = new MapUserControl(map) { Margin = new Thickness(5, 2, 5, 2) };
                WorkshopMapsStackPanel.Children.Add(userControl);
            }
        }
    }
}
