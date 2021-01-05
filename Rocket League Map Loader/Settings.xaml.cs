using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RL_Map_Loader
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            AppState.Settings.Reload();

            var properties = AppState.Settings.Properties.Cast<SettingsProperty>().OrderBy(p => p.Name);
            
            foreach (SettingsProperty property in properties)
            {
                if (property.Name == "IsFirstTimeRun")
                    continue;

                PropertyNameStackPanel.Children.Add(new Label
                {
                    Content = property.Name,
                    Margin = new Thickness(20, 0, 0, 0)
                });

                PropertyValueStackPanel.Children.Add(new TextBox
                {
                    Text = AppState.Settings[property.Name].ToString(),
                    Margin = new Thickness(20, 4, 20, 4)
                });
            }
        }

        private void SaveSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach(Label propertyNameLabel in PropertyNameStackPanel.Children)
            {
                var propertyName = propertyNameLabel.Content.ToString();
                var index = PropertyNameStackPanel.Children.IndexOf(propertyNameLabel);
                var propertyValueTextbox = (TextBox)PropertyValueStackPanel.Children[index];
                var propertyValue = propertyValueTextbox.Text;
                AppState.Settings[propertyName] = propertyValue;
            }

            AppState.Settings.Save();
            Close();
        }
    }
}
