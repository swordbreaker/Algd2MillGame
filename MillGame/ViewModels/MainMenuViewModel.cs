using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MillGame.Commands;
using MillGame.Views;

namespace MillGame.ViewModels
{
    public class MainMenuViewModel
    {
        public ICommand PlayCommand { get; private set; }
        public ICommand ServerCommand { get; private set; }

        public MainMenuViewModel()
        {
            PlayCommand = new SimpleCommand(PlayAction);
        }

        private void PlayAction(object o)
        {
            MainWindow.ViewModel.NavigateTo<MillBoard>();
        }
    }
}
