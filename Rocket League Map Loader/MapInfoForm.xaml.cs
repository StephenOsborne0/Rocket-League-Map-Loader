using System.Windows;
using Rocket_League_Map_Loader.Models;

namespace Rocket_League_Map_Loader
{
    /// <summary>
    /// Interaction logic for MapInfo.xaml
    /// </summary>
    public partial class MapInfoForm : Window
    {
        private readonly Map _map;

        public MapInfoForm(Map map)
        {
            _map = map;
            InitializeComponent();
            LoadInfo();
        }

        public void LoadInfo()
        {
            NameLabel.Content = _map.Name ?? string.Empty;
            FilepathLabel.Content = _map.MapFilePath ?? string.Empty;
            RecommendedSettingsLabel.Content = _map.RecommendedSettings ?? string.Empty;
            YoutubeVideoUrlLabel.Content = _map.YoutubeUrl ?? string.Empty;
            WebpageLabel.Content = _map.Webpage ?? string.Empty;
            DatePublishedLabel.Content = _map.DatePublished.ToString("dddd MMMM yyyy HH:mm:ss") ?? string.Empty;
            BlogCategoriesLabel.Content = string.Join(", ", _map.BlogCategories);
            TagsLabel.Content = string.Join(", ", _map.Tags);
            ShortDescriptionLabel.Content = _map.ShortDescription ?? string.Empty; ;
            LongDescriptionLabel.Text = _map.LongDescription ?? string.Empty; ;
        }
    }
}
