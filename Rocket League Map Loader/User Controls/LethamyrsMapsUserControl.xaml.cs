using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Rocket_League_Map_Loader.Models;

namespace Rocket_League_Map_Loader.User_Controls
{
    /// <summary>
    /// Interaction logic for LethamyrsMapsUserControl.xaml
    /// </summary>
    public partial class LethamyrsMapsUserControl : UserControl
    {
        public LethamyrsMapsUserControl()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            LethamyrsMapsStackPanel.Children.Clear();

            foreach(var map in AppState.LethsMaps.OrderBy(x => x.Name))
            {
                var userControl = new MapUserControl(map) { Margin = new Thickness(5, 2, 5, 2) };
                LethamyrsMapsStackPanel.Children.Add(userControl);
            }
        }
    }
}
