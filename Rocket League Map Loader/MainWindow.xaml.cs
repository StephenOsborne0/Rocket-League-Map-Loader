using System;
using System.Diagnostics;
using System.Windows;
using Path = System.IO.Path;

namespace Rocket_League_Map_Loader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void ExitButton_OnClick(object sender, RoutedEventArgs e) => Environment.Exit(0);

        private void ViewModsFolderButton_OnClick(object sender, RoutedEventArgs e) =>
            Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods"));

        private void AboutButton_OnClick(object sender, RoutedEventArgs e) => new About().ShowDialog();

        private void UPKFileExtractorButton_OnClick(object sender, RoutedEventArgs e) { throw new NotImplementedException(); }
    }
}
