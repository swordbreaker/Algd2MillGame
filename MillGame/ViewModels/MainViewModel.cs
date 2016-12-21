using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MillGame.Annotations;
using MillGame.Views;

namespace MillGame.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private UserControl _currentPage;

        public UserControl CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public MainViewModel()
        {
            CurrentPage = new MainMenu();
        }

        public void NavigateTo<T>() where T : UserControl, new()
        {
            CurrentPage = new T();
        }

        public void NavigateTo(UserControl control)
        {
            CurrentPage = control;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
