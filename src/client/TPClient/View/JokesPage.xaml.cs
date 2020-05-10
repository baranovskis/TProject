using TPClient.ViewModel;

namespace TPClient.View
{
    /// <summary>
    /// Interaction logic for JokesPage.xaml
    /// </summary>
    public partial class JokesPage
    {
        public JokesPage()
        {
            InitializeComponent();
            DataContext = new JokesViewModel();
        }
    }
}
