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
using MillGame.Models;

namespace MillGame.Views
{
    /// <summary>
    /// Interaction logic for MillBoard.xaml
    /// </summary>
    public partial class MillBoard : UserControl
    {
        public static MillBoardViewModel ViewModel { get; private set; }
        public static Controller _ctrl { get; private set; }
        private bool secondClick = false;
        private int i = 0;

        public MillBoard()
        {
            ViewModel = new MillBoardViewModel(this);
            _ctrl = new Controller(ViewModel);
            ViewModel.SetCtrl(_ctrl);

            InitializeComponent();
        }

        private void MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(ViewModel.IsGameRunning)
            {
                secondClick = ViewModel.StoneClick(sender, secondClick, true);

                if(!secondClick)
                {
                    _ctrl.Compute();
                }
            }
        }

        public void HideButtons()
        {
            BlackButton.Visibility = Visibility.Collapsed;
            WhiteButton.Visibility = Visibility.Collapsed;
        }

        /*
        //GetStone Test
        private void MouseUp2(object sender, MouseButtonEventArgs e)
        {
            object v1 = ViewModel.GetStone("White1");

            System.Console.Write("");
        }
        */
    }
}
