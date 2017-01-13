using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using MillGame.Commands;
using MillGame.Views;

namespace MillGame.ViewModels
{
    public class MillBoardViewModel
    {
        private Brush _previousFill = null;
        private Ellipse _previousEllipse = null;
        private String state = "PLACE"; // PLACE, MOVE
        private bool take = false;
        private bool whitePhase = true;
        private bool playerWhite = true;
        private MillBoard mBoard;
        private int redStonesOnBoard = 0;
        private int blackStonesOnBoard = 0;
        private int placed = 0;
        private List<int> redStonesPlace = new List<int>();
        private List<int> blackStonesPlace = new List<int>();

        private readonly SolidColorBrush black = new SolidColorBrush(Colors.Black);
        private readonly SolidColorBrush red = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);

        public MillBoardViewModel(MillBoard _mBoard)
        {
            mBoard = _mBoard;
        }

        public void ChangeState(String newState)
        {
            if(state == "PLACE" || state == "MOVE")
            {
                state = newState;
            }
        }

        public void ButtonClick(object sender)
        {
            Button btn = sender as Button;
            if(btn.Name == "Black")
            {
                playerWhite = false;
            }
        }

        public bool StoneClick(object sender, bool secondClick)
        {
            Ellipse ellipse = sender as Ellipse;

            //if(whitePhase != playerWhite) return !secondClick; // Comment out to test         

            if(state == "PLACE" && !take)
            {
                secondClick = PlacePhase(ellipse, secondClick);
            }
            else if(state == "MOVE" && !take && redStonesOnBoard > 2 && blackStonesOnBoard > 2)
            {
                secondClick = MovePhase(ellipse, secondClick);
            }
            else if(take)
            {
                if(TakePhase(ellipse))
                {
                    whitePhase = !whitePhase;
                    take = false;
                }
                
            }

            return secondClick;
        }

        private bool PlacePhase(Ellipse ellipse, bool secondClick)
        {
            int uid;
            Int32.TryParse(ellipse.Parent.GetValue(UIElement.UidProperty).ToString(), out uid);

            if (!secondClick)
            {
                if (ellipse.Fill.ToString() == transparent.ToString())
                {
                    return secondClick;
                }
                else if(!((uid <= 58 && uid >= 50) || (uid <= 68 && uid >= 60)))
                {
                    return secondClick;
                }
                else if(whitePhase && ellipse.Fill.ToString() != red.ToString())
                {
                    return secondClick;
                }
                else if(!whitePhase && ellipse.Fill.ToString() != black.ToString())
                {
                    return secondClick;
                }                

                _previousEllipse = ellipse;
                _previousFill = ellipse.Fill;

                ellipse.Fill = new SolidColorBrush(Colors.Green);
            }
            else
            {
                if (ellipse.Fill.ToString() == red.ToString() || ellipse.Fill.ToString() == black.ToString())
                {
                    return secondClick;
                }
                else if ((uid <= 58 && uid >= 50) || (uid <= 68 && uid >= 60))
                {
                    return secondClick;
                }
                else if (ellipse.Name == _previousEllipse.Name)
                {
                    return secondClick;
                }
                else
                {
                    ellipse.Fill = _previousFill;
                    ellipse.Name = _previousEllipse.Name;
                    _previousFill = null;
                    _previousEllipse.Fill = transparent;
                    _previousEllipse.Name = null;
                    _previousEllipse = null;
                    placed++;

                    if(whitePhase)
                    {
                        redStonesOnBoard++;
                        redStonesPlace.Add(uid);
                        if (Mill(redStonesPlace, uid))
                        {
                            take = true;
                        }
                        else
                        {
                            whitePhase = !whitePhase;
                        }
                    }
                    else
                    {
                        blackStonesOnBoard++;
                        blackStonesPlace.Add(uid);
                        if (Mill(blackStonesPlace, uid))
                        {
                            take = true;
                        }
                        else
                        {
                            whitePhase = !whitePhase;
                        }
                    }
                }
            }

            if(placed == 18) // EDIT FOR TESTING
            {
                state = "MOVE";
            }

            return secondClick = !secondClick;
        }

        private bool MovePhase(Ellipse ellipse, bool secondClick)
        {
            int uid;
            Int32.TryParse(ellipse.Parent.GetValue(UIElement.UidProperty).ToString(), out uid);

            if (!secondClick)
            {
                if (ellipse.Fill.ToString() == transparent.ToString())
                {
                    return secondClick;
                }
                else if(!(uid >= 0 && uid <= 23))
                {
                    return secondClick;
                }
                else if(whitePhase && ellipse.Fill.ToString() != red.ToString())
                {
                    return secondClick;
                }
                else if(!whitePhase && ellipse.Fill.ToString() != black.ToString())
                {
                    return secondClick;
                }

                _previousEllipse = ellipse;
                _previousFill = ellipse.Fill;

                ellipse.Fill = new SolidColorBrush(Colors.Green);
            }
            else
            {
                int _previousUid;
                Int32.TryParse(_previousEllipse.Parent.GetValue(UIElement.UidProperty).ToString(), out _previousUid);

                if (ellipse.Fill.ToString() == red.ToString() || ellipse.Fill.ToString() == black.ToString())
                {
                    return secondClick;
                }
                else if (!(uid >= 0 && uid <= 23))
                {
                    return secondClick;
                }
                else if (ellipse.Name == _previousEllipse.Name)
                {
                    ellipse.Fill = _previousFill;
                    return !secondClick;
                }
                else if(whitePhase && redStonesOnBoard > 3 && !ValidMove(_previousUid, uid))
                {
                    return secondClick;
                }
                else if(!whitePhase && blackStonesOnBoard > 3 && !ValidMove(_previousUid, uid))
                {
                    return secondClick;
                }
                else
                {
                    ellipse.Fill = _previousFill;
                    ellipse.Name = _previousEllipse.Name;
                    _previousFill = null;
                    _previousEllipse.Fill = transparent;
                    _previousEllipse.Name = null;
                    _previousEllipse = null;

                    if(whitePhase)
                    {
                        redStonesPlace.Remove(_previousUid);
                        redStonesPlace.Add(uid);
                        if(Mill(redStonesPlace, uid))
                        {
                            take = true;
                        }
                        else
                        {
                            whitePhase = !whitePhase;
                        }
                    }
                    else
                    {
                        blackStonesPlace.Remove(_previousUid);
                        blackStonesPlace.Add(uid);
                        if (Mill(blackStonesPlace, uid))
                        {
                            take = true;
                        }
                        else
                        {
                            whitePhase = !whitePhase;
                        }
                    }

                    _previousUid = -1;
                }
            }

            return secondClick = !secondClick;
        }

        private bool TakePhase(Ellipse ellipse)
        {
            int uid;
            Int32.TryParse(ellipse.Parent.GetValue(UIElement.UidProperty).ToString(), out uid);

            if (whitePhase && ellipse.Fill.ToString() == red.ToString() || ellipse.Fill.ToString() == transparent.ToString())
            {
                return false;
            }
            else if (!whitePhase && ellipse.Fill.ToString() == black.ToString() || ellipse.Fill.ToString() == transparent.ToString())
            {
                return false;
            }
            else if (!(uid >= 0 && uid <= 23))
            {
                return false;
            }
            else if(whitePhase && Mill(blackStonesPlace, uid))
            {
                return false;
            }
            else if(!whitePhase && Mill(redStonesPlace, uid))
            {
                return false;
            }

            ellipse.Fill = transparent;
            ellipse.Name = null;
            ellipse = null;

            if(whitePhase)
            {
                blackStonesOnBoard--;
            }
            else
            {
                redStonesOnBoard--;
            }

            return true;
        }

        public object GetStone(String name)
        {
            return mBoard.FindName(name);
        }

        private bool ValidMove(int oldPos, int newPos)
        {
            if (oldPos == 0 && (newPos == 1 || newPos == 9)) return true;
            else if (oldPos == 1 && (newPos == 0 || newPos == 2 || newPos == 4)) return true;
            else if (oldPos == 2 && (newPos == 1 || newPos == 14)) return true;
            else if (oldPos == 3 && (newPos == 4 || newPos == 10)) return true;
            else if (oldPos == 4 && (newPos == 1 || newPos == 3 || newPos == 5 || newPos == 7)) return true;
            else if (oldPos == 5 && (newPos == 4 || newPos == 13)) return true;
            else if (oldPos == 6 && (newPos == 7 || newPos == 11)) return true;
            else if (oldPos == 7 && (newPos == 4 || newPos == 6 || newPos == 8)) return true;
            else if (oldPos == 8 && (newPos == 7 || newPos == 12)) return true;
            else if (oldPos == 9 && (newPos == 0 || newPos == 10 || newPos == 21)) return true;
            else if (oldPos == 10 && (newPos == 3 || newPos == 9 || newPos == 11 || newPos == 18)) return true;
            else if (oldPos == 11 && (newPos == 6 || newPos == 10 || newPos == 15)) return true;
            else if (oldPos == 12 && (newPos == 8 || newPos == 13 || newPos == 17)) return true;
            else if (oldPos == 13 && (newPos == 5 || newPos == 12 || newPos == 14 || newPos == 20)) return true;
            else if (oldPos == 14 && (newPos == 2 || newPos == 13 || newPos == 23)) return true;
            else if (oldPos == 15 && (newPos == 11 || newPos == 16)) return true;
            else if (oldPos == 16 && (newPos == 15 || newPos == 17 || newPos == 19)) return true;
            else if (oldPos == 17 && (newPos == 12 || newPos == 16)) return true;
            else if (oldPos == 18 && (newPos == 10 || newPos == 19)) return true;
            else if (oldPos == 19 && (newPos == 16 || newPos == 18 || newPos == 20 || newPos == 22)) return true;
            else if (oldPos == 20 && (newPos == 13 || newPos == 19)) return true;
            else if (oldPos == 21 && (newPos == 9 || newPos == 22)) return true;
            else if (oldPos == 22 && (newPos == 19 || newPos == 21 || newPos == 23)) return true;
            else if (oldPos == 23 && (newPos == 14 || newPos == 22)) return true;
            else return false;
        }

        private bool Mill(List<int> places, int pos)
        {
            if ((pos == 0 || pos == 1 || pos == 2) && (places.Exists(item => item == 0) && places.Exists(item => item == 1) && places.Exists(item => item == 2))) return true;
            else if ((pos == 3 || pos == 4 || pos == 5) && (places.Exists(item => item == 3) && places.Exists(item => item == 4) && places.Exists(item => item == 5))) return true;
            else if ((pos == 6 || pos == 7 || pos == 8) && (places.Exists(item => item == 6) && places.Exists(item => item == 7) && places.Exists(item => item == 8))) return true;
            else if ((pos == 9 || pos == 10 || pos == 11) && (places.Exists(item => item == 9) && places.Exists(item => item == 10) && places.Exists(item => item == 11))) return true;
            else if ((pos == 12 || pos == 13 || pos == 14) && (places.Exists(item => item == 12) && places.Exists(item => item == 13) && places.Exists(item => item == 14))) return true;
            else if ((pos == 15 || pos == 16 || pos == 17) && (places.Exists(item => item == 15) && places.Exists(item => item == 16) && places.Exists(item => item == 17))) return true;
            else if ((pos == 18 || pos == 19 || pos == 20) && (places.Exists(item => item == 18) && places.Exists(item => item == 19) && places.Exists(item => item == 20))) return true;
            else if ((pos == 21 || pos == 22 || pos == 23) && (places.Exists(item => item == 21) && places.Exists(item => item == 22) && places.Exists(item => item == 23))) return true;
            else if ((pos == 0 || pos == 9 || pos == 21) && (places.Exists(item => item == 0) && places.Exists(item => item == 9) && places.Exists(item => item == 21))) return true;
            else if ((pos == 3 || pos == 10 || pos == 18) && (places.Exists(item => item == 3) && places.Exists(item => item == 10) && places.Exists(item => item == 18))) return true;
            else if ((pos == 6 || pos == 11 || pos == 15) && (places.Exists(item => item == 6) && places.Exists(item => item == 11) && places.Exists(item => item == 15))) return true;
            else if ((pos == 1 || pos == 4 || pos == 7) && (places.Exists(item => item == 1) && places.Exists(item => item == 4) && places.Exists(item => item == 7))) return true;
            else if ((pos == 16 || pos == 19 || pos == 22) && (places.Exists(item => item == 16) && places.Exists(item => item == 19) && places.Exists(item => item == 22))) return true;
            else if ((pos == 8 || pos == 12 || pos == 17) && (places.Exists(item => item == 8) && places.Exists(item => item == 12) && places.Exists(item => item == 17))) return true;
            else if ((pos == 5 || pos == 13 || pos == 20) && (places.Exists(item => item == 5) && places.Exists(item => item == 13) && places.Exists(item => item == 20))) return true;
            else if ((pos == 2 || pos == 14 || pos == 23) && (places.Exists(item => item == 2) && places.Exists(item => item == 14) && places.Exists(item => item == 23))) return true;
            else return false;
        }
    }
}
