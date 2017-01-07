using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MillGame.Commands;
using MillGame.Views;

namespace MillGame.ViewModels
{
    public class MillBoardViewModel
    {
        private Brush _previousFill = null;
        private Ellipse _previousEllipse = null;
        private String state = "PLACE"; // PLACE, MOVE, TAKE

        private readonly SolidColorBrush black = new SolidColorBrush(Colors.Black);
        private readonly SolidColorBrush red = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);
        private MillBoard mBoard;

        public MillBoardViewModel(MillBoard _mBoard)
        {
            mBoard = _mBoard;
        }

        public void ChangeState(String newState)
        {
            state = newState;
        }

        public bool StoneClick(object sender, bool secondClick)
        {
            Ellipse ellipse = sender as Ellipse;

            if(state == "PLACE")
            {
                secondClick = PlacePhase(ellipse, secondClick);
            }
            else if(state == "MOVE")
            {
                secondClick = MovePhase(ellipse, secondClick);
            }
            else if(state == "TAKE")
            {

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
                    _previousEllipse.Fill = new SolidColorBrush(Colors.Transparent);
                    _previousEllipse.Name = null;
                    _previousEllipse = null;
                    
                }
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
                else if ((uid <= 58 && uid >= 50) || (uid <= 68 && uid >= 60))
                {
                    return secondClick;
                }
                else if (ellipse.Name == _previousEllipse.Name)
                {
                    return secondClick;
                }
                else if(!ValidMove(_previousUid, uid))
                {
                    return secondClick;
                }
                else
                {
                    ellipse.Fill = _previousFill;
                    ellipse.Name = _previousEllipse.Name;
                    _previousFill = null;
                    _previousEllipse.Fill = new SolidColorBrush(Colors.Transparent);
                    _previousEllipse.Name = null;
                    _previousEllipse = null;

                }
            }


            return secondClick = !secondClick;
        }

        public object GetStone(String name)
        {

            return GetStone(mBoard, name);
        }

        private object GetStone(DependencyObject parent, string name)
        {
            if (parent == null) return null;

            object foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for(int i = 0; i < childrenCount;i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                Ellipse childType = child as Ellipse;

                if(childType == null)
                {
                    foundChild = GetStone(child, name);

                    if (foundChild != null) break;
                }
                else if(!string.IsNullOrEmpty(name))
                {
                    var ellipseChild = child as Ellipse;

                    if(ellipseChild != null && ellipseChild.Name == name)
                    {
                        foundChild = child;
                        break;
                    }
                }
                else
                {
                    foundChild = child;
                    break;
                }
            }

            return foundChild;
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
    }
}
