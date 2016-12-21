using MahApps.Metro.Controls;
using MillGame.ViewModels;

namespace MillGame.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static MainViewModel ViewModel { get; private set; }

        public MainWindow()
        {
            ViewModel = new MainViewModel();
            InitializeComponent();
        }
    }
}
