using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MillGame.ViewModels;

namespace MillGame.Views
{
    /// <summary>
    /// Interaction logic for MillBoard.xaml
    /// </summary>
    public partial class MillBoard : UserControl
    {
        public static MillBoardViewModel ViewModel { get; private set; }
        private bool secondClick = false;

        public MillBoard()
        {
            ViewModel = new MillBoardViewModel(this);
            InitializeComponent();
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            secondClick = ViewModel.StoneClick(sender, secondClick);
        }

        /* //GetStone Test
        private void MouseUp2(object sender, MouseButtonEventArgs e)
        {
            object v1 = ViewModel.GetStone("White1");

            System.Console.Write("");
        }
        */
    }
}
