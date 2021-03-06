﻿using System;
using System.Windows.Input;

namespace MillGame.Commands
{
    public class SimpleCommand : ICommand
    {
        private readonly Action<object> _action;
        private bool _canExecute;

        public SimpleCommand(Action<object> action)
        {
            _action = action;
            _canExecute = true;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public void Disable()
        {
            _canExecute = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Enable()
        {
            _canExecute = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;
    }
}
