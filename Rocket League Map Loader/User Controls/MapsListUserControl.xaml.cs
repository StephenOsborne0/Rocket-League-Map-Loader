using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RL_Map_Loader.Models;

namespace RL_Map_Loader.User_Controls
{
    /// <summary>
    /// Interaction logic for MapsListUserControl.xaml
    /// </summary>
    public partial class MapsListUserControl : UserControl
    {
        internal List<Map> Maps;

        public MapsListUserControl(List<Map> maps)
        {
            Maps = maps ?? throw new InvalidOperationException("User control requires a list of maps");
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        public void RefreshListedMaps(List<Map> maps)
        {
            Maps = maps;
            MapsListStackPanel.Children.Clear();

            foreach (var map in maps.OrderBy(x => x.Name))
            {
                var userControl = new MapUserControl(map) { Margin = new Thickness(5, 2, 5, 2) };
                MapsListStackPanel.Children.Add(userControl);
            }
        }

        public void RefreshChildren()
        {
            foreach(MapUserControl mapUserControl in MapsListStackPanel.Children)
                mapUserControl.RefreshUserControlUi();
        }

        public void WindowLoaded(object sender, RoutedEventArgs e) => RefreshListedMaps(Maps);
    }
}
