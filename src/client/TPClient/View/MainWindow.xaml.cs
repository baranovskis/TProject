using TPClient.Services;
using TPClient.ViewModel;

namespace TPClient.View
{
    public partial class MainWindow
    {
        /// <summary>
        /// Interaction logic for MainWindow.xaml
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainViewModel.Instance;

            // Create navigation service & set start page
            MainViewModel.Instance.Navigation = new NavigationService(MainFrame);
            MainViewModel.Instance.Navigation.Navigate<JokesPage>();
        }
    }
}
