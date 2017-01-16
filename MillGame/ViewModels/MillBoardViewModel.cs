using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MillGame.Annotations;
using MillGame.Commands;
using MillGame.Views;
using MillGame.Models;
using MillGame.Models.Core;
using MillGame.Models.Core.Actions;

namespace MillGame.ViewModels
{
    public class MillBoardViewModel : IView, INotifyPropertyChanged
    {
        // Private Variables
        //private Brush _previousFill = null;
        //private Ellipse _previousEllipse = null;
        public enum BoardState
        {
            Placing, Moving, Jumping
        }
        private bool _take = false;
        //private bool whitePhase = true;
        //private bool playerWhite = true;
        private readonly MillBoard _mBoard;
        //private int redStonesOnBoard = 0;
        //private int blackStonesOnBoard = 0;
        //private int placed = 0;
        //private List<int> redStonesPlace = new List<int>();
        //private List<int> blackStonesPlace = new List<int>();

        private readonly Controller _ctrl;
        //private Placing p;
        //private Moving m;
        //private Taking t;

        private Color _playerColor;
        private Ellipse _selecterEllipse;
        private Color _selectedEllipseColor;
        private ActionPM _previousAction;

        // Colors
        private readonly Color _white = Colors.White;
        private readonly Color _black = Colors.Black;
        private string _message;
        //private readonly SolidColorBrush black = new SolidColorBrush(Colors.Black);
        //private readonly SolidColorBrush red = new SolidColorBrush(Colors.Red);
        //private readonly SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);

        public bool IsGameRunning { get; set; } = false;
        public bool ComputerIsPlaying { get; set; } = false;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public ICommand WhiteCommand { get; set; }
        public ICommand BlackCommand { get; set; }

        public MillBoardViewModel(MillBoard mBoard)
        {
            _mBoard = mBoard;
            _ctrl = new Controller(this);

            WhiteCommand = new SimpleCommand((o) =>
            {
                if(IsGameRunning) return;
                mBoard.HideButtons();
                IsGameRunning = true;
                _playerColor = _white;
                Task.Run(() => _ctrl.StartHumanGame(false));
            });

            BlackCommand = new SimpleCommand((o) =>
            {
                if (IsGameRunning) return;
                _playerColor = _black;
                mBoard.HideButtons();
                IsGameRunning = true;
                ComputerIsPlaying = true;
                Task.Run(() => _ctrl.StartHumanGame(true));
                Message = "Computer is thinking";
            });

            _mBoard.OnBoardRectPressed += MBoardOnOnBoardRectPressed;
            _mBoard.OnStonePressed += MBoardOnOnStonePressed;
        }

        private sbyte GetColorIdex(Color c)
        {
            if (c == _black) return IController.BLACK;
            if (c == _white) return IController.WHITE;
            throw new ArgumentException("Color is not defined");
        }

        private BoardState GetState(sbyte color)
        {
            if (_ctrl.GameTree.CurrentState() == null || _ctrl.GameTree.CurrentState().PlacingPhase(color))
            {
                return BoardState.Placing;
            }
            else if (_ctrl.GameTree.CurrentState().MovingPhase(color))
            {
                return BoardState.Moving;
            }
            else if (_ctrl.GameTree.CurrentState().JumpingPhase(color))
            {
                return BoardState.Placing;
            }
            throw new ArgumentException("State is not defined");
        }

        private void MBoardOnOnStonePressed(object sender, EventArgs eventArgs)
        {
            if(ComputerIsPlaying || !IsGameRunning) return;
            var stone = (Ellipse)sender;
            var color = ((SolidColorBrush)stone.Fill).Color;
            if(color != _white && color != _black) return;
            if (_take)
            {
                if(stone.Tag == null || color == _playerColor) return;
                var status = _ctrl.Play(new Taking(_previousAction, (int) stone.Tag));
                if (status == IController.Status.INVALIDACTION)
                {
                    Message = "Stein in der Mühle";
                    return;
                }
                _take = false;
                Compute();
            }
            else
            {
                if (_selecterEllipse != null)
                {
                    _selecterEllipse.Fill = new SolidColorBrush(_selectedEllipseColor);
                }

                var state = (GetState(GetColorIdex(color)));
                var isPlacingStone = stone.Tag == null && state == BoardState.Placing && color == _playerColor;
                var isMovingStone = stone.Tag != null && state == BoardState.Moving;
                var isJumpingStone = stone.Tag != null && state == BoardState.Jumping;

                if (isPlacingStone || isMovingStone || isJumpingStone)
                {
                    _selecterEllipse = stone;
                    _selectedEllipseColor = color;
                    _selecterEllipse.Fill = new SolidColorBrush(Colors.Green);
                }
            }
        }

        private void MBoardOnOnBoardRectPressed(object sender, EventArgs eventArgs)
        {
            if(_selecterEllipse == null || !IsGameRunning) return;
            var rect = (Rectangle)sender;
            var color = GetColorIdex(_selectedEllipseColor);

            switch (GetState(color))
            {
                case BoardState.Placing:
                    if (_selecterEllipse != null)
                    {
                        _previousAction = new Placing(color, (int) rect.Tag);
                        var status = _ctrl.Play(_previousAction);
                        if (status == IController.Status.CLOSEDMILL)
                        {
                            ComputerIsPlaying = false;
                            _take = true;
                            Message = "You have a Mill take a stone";
                        }
                        else
                        {
                            Compute();
                        }
                    }
                    break;
                case BoardState.Moving:
                case BoardState.Jumping:
                    if (_selecterEllipse != null)
                    {
                        if (_selecterEllipse.Tag == null) return;
                        _previousAction = new Moving(color, (int) _selecterEllipse.Tag, (int) rect.Tag);
                        var status = _ctrl.Play(_previousAction);
                        if (status == IController.Status.CLOSEDMILL)
                        {
                            ComputerIsPlaying = false;
                            _take = true;
                            Message = "You have a Mill take a stone";
                        }
                        else
                        {
                            Compute();
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _selecterEllipse.Fill = new SolidColorBrush(_selectedEllipseColor);
            _selecterEllipse = null;
        }


        private void Compute()
        {
            ComputerIsPlaying = true;
            Task.Run(() => _ctrl.Compute());
            Message = "Computer is thinking";
        }

        /* ####################################################### USER - COMPUTER ####################################################### */

        public void UpdateBoard(State s, Models.Core.Actions.Action a, bool isComputerAction)
        {
            if (isComputerAction)
            {
                Debug.WriteLine(a);
            }

            var taking = a as Taking;
            var moving = a as Moving;
            var placing = a as Placing;

            if (placing != null)
            {
                _mBoard.PlaceStone(placing.EndPosition, placing.Color());
            }

            if (moving != null)
            {
                _mBoard.MoveStone(moving.StartPosition, moving.EndPosition);
            }

            if (taking != null)
            {
                _mBoard.TakeStone(taking.TakePosition);
                if (isComputerAction)
                {
                    var tplacing = taking.Action as Placing;
                    var tmoving = taking.Action as Moving;
                    if(tplacing != null) _mBoard.PlaceStone(taking.Action.EndPosition, taking.Action.Color());
                    if(tmoving != null) _mBoard.MoveStone(tmoving.StartPosition, tmoving.EndPosition);
                }
            }

            ComputerIsPlaying = !isComputerAction;

            if (_ctrl.GetStatus() == IController.Status.FINISHED)
            {
                var winner = (_ctrl.GetWinner() == 1) ? "White" : "Black";
                Message = $"{winner} has won";
                IsGameRunning = false;
                return;
            }

            if (!ComputerIsPlaying) Message = "Your turn";
        }

        public void PrepareBoard()
        {
            // throw new NotImplementedException();
        }

        public void SetComputerName(string name)
        {
            // throw new NotImplementedException();
        }

        public void SetHumanName(string name)
        {
            // throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
