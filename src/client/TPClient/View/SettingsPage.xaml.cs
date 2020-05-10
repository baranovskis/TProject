using System.Windows.Controls;
using TPClient.ViewModel;

namespace TPClient.View
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }
    }
}
