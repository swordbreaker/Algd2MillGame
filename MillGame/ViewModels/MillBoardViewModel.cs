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
                _ctrl.StartHumanGame(false);
            });

            BlackCommand = new SimpleCommand((o) =>
            {
                if (IsGameRunning) return;
                _playerColor = _black;
                mBoard.HideButtons();
                IsGameRunning = true;
                ComputerIsPlaying = true;
                _ctrl.StartHumanGame(true);
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
            if(ComputerIsPlaying) return;
            var stone = (Ellipse)sender;
            var color = ((SolidColorBrush)stone.Fill).Color;
            if(color != _white && color != _black) return;
            if (_take)
            {
                if(stone.Tag == null || color == _playerColor) return;
                _ctrl.Play(new Taking(_previousAction, (int) stone.Tag));
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
            if(_selecterEllipse == null) return;
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
                    if (_selecterEllipse != null)
                    {
                        if (_selecterEllipse.Tag == null) return;
                        _previousAction = new Moving(color, (int) _selecterEllipse.Tag, (int) rect.Tag);
                        var status = _ctrl.Play(_previousAction);
                        if (status == IController.Status.CLOSEDMILL)
                        {
                            _take = true;
                            Message = "You have a Mill take a stone";
                        }
                        else
                        {
                            Compute();
                        }
                    }
                    break;
                case BoardState.Jumping:

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

//        public void SetCtrl(Controller _ctrl)
//        {
//            this._ctrl = _ctrl;
//        }

//        // UNUSED
//        public void ChangeState(BoardState newState)
//        {
//            if (_state == BoardState.Placing || _state == BoardState.Moving)
//            {
//                _state = newState;
//            }
//        }

//        // Set Player Color
//        public void ButtonClick(object sender)
//        {
//            Button btn = sender as Button;
//            if (btn.Name == "Black")
//            {
//                playerWhite = false;
//            }
//        }

//        // Click on Stone behaviour
//        public bool StoneClick(object sender, bool secondClick, bool player)
//        {
//            var ellipse = sender as Ellipse;

//            if (_state == BoardState.Placing && !take)
//            {
//                secondClick = PlacePhase(ellipse, secondClick, player);
//            }
//            else if (_state == BoardState.Moving && !take && redStonesOnBoard > 2 && blackStonesOnBoard > 2)
//            {
//                secondClick = MovePhase(ellipse, secondClick);
//            }
//            else if (take)
//            {
//                if (TakePhase(ellipse))
//                {
//                    whitePhase = !whitePhase;
//                    take = false;
//                }
//            }
//            return secondClick;
//        }

//        private bool PlacePhase(Ellipse ellipse, bool secondClick, bool player)
//        {
//            int uid;

//            Int32.TryParse(ellipse.Parent.GetValue(UIElement.UidProperty).ToString(), out uid);

//            if (!secondClick)
//            {
//                // Check valid click
//                if (ellipse.Fill.ToString() == transparent.ToString())
//                {
//                    return secondClick;
//                }
//                else if (!((uid <= 58 && uid >= 50) || (uid <= 68 && uid >= 60)))
//                {
//                    return secondClick;
//                }
//                else if (whitePhase && ellipse.Fill.ToString() != red.ToString())
//                {
//                    return secondClick;
//                }
//                else if (!whitePhase && ellipse.Fill.ToString() != black.ToString())
//                {
//                    return secondClick;
//                }

//                // Save stone
//                _previousEllipse = ellipse;
//                _previousFill = ellipse.Fill;

//                ellipse.Fill = new SolidColorBrush(Colors.Green);
//            }
//            else
//            {
//                // Check valid click
//                if (ellipse.Fill.ToString() == red.ToString() || ellipse.Fill.ToString() == black.ToString())
//                {
//                    return secondClick;
//                }
//                else if ((uid <= 58 && uid >= 50) || (uid <= 68 && uid >= 60))
//                {
//                    return secondClick;
//                }
//                else if (ellipse.Name == _previousEllipse.Name)
//                {
//                    return secondClick;
//                }
//                else
//                {
//                    // Add attributes to the new stone and remove previous position 
//                    ellipse.Fill = _previousFill;
//                    ellipse.Name = _previousEllipse.Name;
//                    _previousFill = null;
//                    _previousEllipse.Fill = transparent;
//                    _previousEllipse.Name = null;
//                    _previousEllipse = null;
//                    placed++;

//                    // Give the information to the computer
//                    // IController.WHITE = 1, IController.BLACK = 0 (sometimes ???)
//                    if (playerWhite && player)
//                    {
//                        p = new Placing(1, uid);
//                        _ctrl.Play(p);
//                    }
//                    else if (!playerWhite && player)
//                    {
//                        p = new Placing(0, uid);
//                        _ctrl.Play(p);
//                    }

//                    // Add boardposition and check for Mill
//                    if (whitePhase)
//                    {
//                        redStonesOnBoard++;
//                        redStonesPlace.Add(uid);
//                        if (Mill(redStonesPlace, uid))
//                        {
//                            take = true;
//                        }
//                        else
//                        {
//                            whitePhase = !whitePhase;
//                        }
//                    }
//                    else
//                    {
//                        blackStonesOnBoard++;
//                        blackStonesPlace.Add(uid);
//                        if (Mill(blackStonesPlace, uid))
//                        {
//                            take = true;
//                        }
//                        else
//                        {
//                            whitePhase = !whitePhase;
//                        }
//                    }
//                }
//            }

//            if (placed == 18) // EDIT FOR TESTING
//            {
//                _state = BoardState.Moving;
//            }

//            return !secondClick;
//        }

//        private bool MovePhase(Ellipse ellipse, bool secondClick)
//        {
//            int uid;
//            Int32.TryParse(ellipse.Parent.GetValue(UIElement.UidProperty).ToString(), out uid);

//            if (!secondClick)
//            {
//                // Check valid click
//                if (ellipse.Fill.ToString() == transparent.ToString())
//                {
//                    return secondClick;
//                }
//                else if (!(uid >= 0 && uid <= 23))
//                {
//                    return secondClick;
//                }
//                else if (whitePhase && ellipse.Fill.ToString() != red.ToString())
//                {
//                    return secondClick;
//                }
//                else if (!whitePhase && ellipse.Fill.ToString() != black.ToString())
//                {
//                    return secondClick;
//                }

//                // Save stone
//                _previousEllipse = ellipse;
//                _previousFill = ellipse.Fill;

//                ellipse.Fill = new SolidColorBrush(Colors.Green);
//            }
//            else
//            {
//                int _previousUid;
//                Int32.TryParse(_previousEllipse.Parent.GetValue(UIElement.UidProperty).ToString(), out _previousUid);

//                // Check valid  move
//                if (ellipse.Fill.ToString() == red.ToString() || ellipse.Fill.ToString() == black.ToString())
//                {
//                    return secondClick;
//                }
//                else if (!(uid >= 0 && uid <= 23))
//                {
//                    return secondClick;
//                }
//                else if (ellipse.Name == _previousEllipse.Name)
//                {
//                    ellipse.Fill = _previousFill;
//                    return !secondClick;
//                }
//                else if (whitePhase && redStonesOnBoard > 3 && !ValidMove(_previousUid, uid))
//                {
//                    return secondClick;
//                }
//                else if (!whitePhase && blackStonesOnBoard > 3 && !ValidMove(_previousUid, uid))
//                {
//                    return secondClick;
//                }
//                else
//                {
//                    // Add attributes to the new stone and remove previous position
//                    ellipse.Fill = _previousFill;
//                    ellipse.Name = _previousEllipse.Name;
//                    _previousFill = null;
//                    _previousEllipse.Fill = transparent;
//                    _previousEllipse.Name = null;
//                    _previousEllipse = null;

//                    // TODO: Give information to the computer

//                    // Add boardposition and check for Mill
//                    if (whitePhase)
//                    {
//                        redStonesPlace.Remove(_previousUid);
//                        redStonesPlace.Add(uid);
//                        if (Mill(redStonesPlace, uid))
//                        {
//                            take = true;
//                        }
//                        else
//                        {
//                            whitePhase = !whitePhase;
//                        }
//                    }
//                    else
//                    {
//                        blackStonesPlace.Remove(_previousUid);
//                        blackStonesPlace.Add(uid);
//                        if (Mill(blackStonesPlace, uid))
//                        {
//                            take = true;
//                        }
//                        else
//                        {
//                            whitePhase = !whitePhase;
//                        }
//                    }

//                    _previousUid = -1;
//                }
//            }

//            return !secondClick;
//        }

//        private bool TakePhase(Ellipse ellipse)
//        {
//            int uid;
//            Int32.TryParse(ellipse.Parent.GetValue(UIElement.UidProperty).ToString(), out uid);

//            // Check valid  click
//            if (whitePhase && ellipse.Fill.ToString() == red.ToString() || ellipse.Fill.ToString() == transparent.ToString())
//            {
//                return false;
//            }
//            else if (!whitePhase && ellipse.Fill.ToString() == black.ToString() || ellipse.Fill.ToString() == transparent.ToString())
//            {
//                return false;
//            }
//            else if (!(uid >= 0 && uid <= 23))
//            {
//                return false;
//            }
//            else if (whitePhase && Mill(blackStonesPlace, uid))
//            {
//                return false;
//            }
//            else if (!whitePhase && Mill(redStonesPlace, uid))
//            {
//                return false;
//            }

//            // Remove stone from board
//            ellipse.Fill = transparent;
//            ellipse.Name = null;
//            ellipse = null;

//            if (whitePhase)
//            {
//                blackStonesOnBoard--;
//            }
//            else
//            {
//                redStonesOnBoard--;
//            }

//            return true;
//        }

//        public object GetStone(String name)
//        {
//            return _mBoard.FindName(name);
//        }

//        private bool ValidMove(int oldPos, int newPos)
//        {
//            if (oldPos == 0 && (newPos == 1 || newPos == 9)) return true;
//            else if (oldPos == 1 && (newPos == 0 || newPos == 2 || newPos == 4)) return true;
//            else if (oldPos == 2 && (newPos == 1 || newPos == 14)) return true;
//            else if (oldPos == 3 && (newPos == 4 || newPos == 10)) return true;
//            else if (oldPos == 4 && (newPos == 1 || newPos == 3 || newPos == 5 || newPos == 7)) return true;
//            else if (oldPos == 5 && (newPos == 4 || newPos == 13)) return true;
//            else if (oldPos == 6 && (newPos == 7 || newPos == 11)) return true;
//            else if (oldPos == 7 && (newPos == 4 || newPos == 6 || newPos == 8)) return true;
//            else if (oldPos == 8 && (newPos == 7 || newPos == 12)) return true;
//            else if (oldPos == 9 && (newPos == 0 || newPos == 10 || newPos == 21)) return true;
//            else if (oldPos == 10 && (newPos == 3 || newPos == 9 || newPos == 11 || newPos == 18)) return true;
//            else if (oldPos == 11 && (newPos == 6 || newPos == 10 || newPos == 15)) return true;
//            else if (oldPos == 12 && (newPos == 8 || newPos == 13 || newPos == 17)) return true;
//            else if (oldPos == 13 && (newPos == 5 || newPos == 12 || newPos == 14 || newPos == 20)) return true;
//            else if (oldPos == 14 && (newPos == 2 || newPos == 13 || newPos == 23)) return true;
//            else if (oldPos == 15 && (newPos == 11 || newPos == 16)) return true;
//            else if (oldPos == 16 && (newPos == 15 || newPos == 17 || newPos == 19)) return true;
//            else if (oldPos == 17 && (newPos == 12 || newPos == 16)) return true;
//            else if (oldPos == 18 && (newPos == 10 || newPos == 19)) return true;
//            else if (oldPos == 19 && (newPos == 16 || newPos == 18 || newPos == 20 || newPos == 22)) return true;
//            else if (oldPos == 20 && (newPos == 13 || newPos == 19)) return true;
//            else if (oldPos == 21 && (newPos == 9 || newPos == 22)) return true;
//            else if (oldPos == 22 && (newPos == 19 || newPos == 21 || newPos == 23)) return true;
//            else if (oldPos == 23 && (newPos == 14 || newPos == 22)) return true;
//            else return false;
//        }

//        private bool Mill(List<int> places, int pos)
//        {
//            if ((pos == 0 || pos == 1 || pos == 2) && (places.Exists(item => item == 0) && places.Exists(item => item == 1) && places.Exists(item => item == 2))) return true;
//            else if ((pos == 3 || pos == 4 || pos == 5) && (places.Exists(item => item == 3) && places.Exists(item => item == 4) && places.Exists(item => item == 5))) return true;
//            else if ((pos == 6 || pos == 7 || pos == 8) && (places.Exists(item => item == 6) && places.Exists(item => item == 7) && places.Exists(item => item == 8))) return true;
//            else if ((pos == 9 || pos == 10 || pos == 11) && (places.Exists(item => item == 9) && places.Exists(item => item == 10) && places.Exists(item => item == 11))) return true;
//            else if ((pos == 12 || pos == 13 || pos == 14) && (places.Exists(item => item == 12) && places.Exists(item => item == 13) && places.Exists(item => item == 14))) return true;
//            else if ((pos == 15 || pos == 16 || pos == 17) && (places.Exists(item => item == 15) && places.Exists(item => item == 16) && places.Exists(item => item == 17))) return true;
//            else if ((pos == 18 || pos == 19 || pos == 20) && (places.Exists(item => item == 18) && places.Exists(item => item == 19) && places.Exists(item => item == 20))) return true;
//            else if ((pos == 21 || pos == 22 || pos == 23) && (places.Exists(item => item == 21) && places.Exists(item => item == 22) && places.Exists(item => item == 23))) return true;
//            else if ((pos == 0 || pos == 9 || pos == 21) && (places.Exists(item => item == 0) && places.Exists(item => item == 9) && places.Exists(item => item == 21))) return true;
//            else if ((pos == 3 || pos == 10 || pos == 18) && (places.Exists(item => item == 3) && places.Exists(item => item == 10) && places.Exists(item => item == 18))) return true;
//            else if ((pos == 6 || pos == 11 || pos == 15) && (places.Exists(item => item == 6) && places.Exists(item => item == 11) && places.Exists(item => item == 15))) return true;
//            else if ((pos == 1 || pos == 4 || pos == 7) && (places.Exists(item => item == 1) && places.Exists(item => item == 4) && places.Exists(item => item == 7))) return true;
//            else if ((pos == 16 || pos == 19 || pos == 22) && (places.Exists(item => item == 16) && places.Exists(item => item == 19) && places.Exists(item => item == 22))) return true;
//            else if ((pos == 8 || pos == 12 || pos == 17) && (places.Exists(item => item == 8) && places.Exists(item => item == 12) && places.Exists(item => item == 17))) return true;
//            else if ((pos == 5 || pos == 13 || pos == 20) && (places.Exists(item => item == 5) && places.Exists(item => item == 13) && places.Exists(item => item == 20))) return true;
//            else if ((pos == 2 || pos == 14 || pos == 23) && (places.Exists(item => item == 2) && places.Exists(item => item == 14) && places.Exists(item => item == 23))) return true;
//            else return false;
//        }

//        // Simulate click from computer
//        public void PlacingStoneHelper(int color, int unusedStones, int uid)
//        {
//            object stone = null;
//            if (color == 0)
//            {
//                stone = GetStone("White" + (unusedStones + 1));
//            }
//            else
//            {
//                stone = GetStone("Black" + (unusedStones + 1));
//            }

//            StoneClick(stone, false, false);
////            System.Threading.Thread.Sleep(4000);
//            stone = ClickPlacingStoneHelper(_mBoard, uid);
//            StoneClick(stone, true, false);
//        }

//        // Get stone from UID
//        public object ClickPlacingStoneHelper(DependencyObject parent, int uid)
//        {
//            if (parent == null) return null;

//            object foundChild = null;
//            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

//            for (int i = 0; i < childrenCount; i++)
//            {
//                var child = VisualTreeHelper.GetChild(parent, i);
//                Canvas childType = child as Canvas;

//                if (childType == null)
//                {
//                    foundChild = ClickPlacingStoneHelper(child, uid);
//                    if (foundChild != null) break;
//                }
//                else
//                {
//                    var canvasChild = child as Canvas;
//                    if (canvasChild != null && canvasChild.Uid == uid.ToString())
//                    {
//                        var e = VisualTreeHelper.GetChild(canvasChild, 0);

//                        foundChild = e;
//                        break;
//                    }
//                }
//            }
//            return foundChild;
//        }

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
                if (s.InMill(placing.EndPosition, placing.Color()) && isComputerAction)
                {
                    ComputerIsPlaying = true;
                    Message = "Computer has a Mill";
                    _ctrl.Compute();
                    return;
                }
            }

            if (moving != null)
            {
                _mBoard.MoveStone(moving.StartPosition, moving.EndPosition);
                if (s.InMill(moving.EndPosition, moving.Color()) && isComputerAction)
                {
                    ComputerIsPlaying = true;
                    Message = "Computer has a Mill";
                    _ctrl.Compute();
                    return;
                }
            }

            if (taking != null)
            {
                _mBoard.TakeStone(taking.TakePosition);
            }

            ComputerIsPlaying = !isComputerAction;

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
