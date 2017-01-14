using System.Windows.Controls;
using MillGame.ViewModels;

namespace MillGame.Views
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenuViewModel ViewModel { get; private set; }

        public MainMenu()
        {
            ViewModel = new MainMenuViewModel();
            InitializeComponent();
        }
    }
}
