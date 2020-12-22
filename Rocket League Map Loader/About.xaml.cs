using System.Diagnostics;
using System.Windows;

namespace Rocket_League_Map_Loader
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About() => InitializeComponent();

        private void TwitterSocial_OnClick(object sender, RoutedEventArgs e) => 
            Process.Start("https://twitter.com/xChaosMods");

        private void TwitchSocial_OnClick(object sender, RoutedEventArgs e) => 
            Process.Start("https://www.twitch.tv/sykechaos");

        private void YoutubeSocial_OnClick(object sender, RoutedEventArgs e) =>
            Process.Start("https://www.youtube.com/user/ChaosxShotz");

        private void KofiDonateButton_OnClick(object sender, RoutedEventArgs e) =>
            Process.Start("https://ko-fi.com/xchaos");
    }
}
