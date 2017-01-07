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

        public bool StoneClick(object sender, bool secondClick)
        {
            Ellipse ellipse = sender as Ellipse;

            if (!secondClick)
            {
                if(ellipse.Fill.ToString() == transparent.ToString())
                {
                    return secondClick;
                }

                _previousEllipse = sender as Ellipse;
                _previousFill = ellipse.Fill;

                ellipse.Fill = new SolidColorBrush(Colors.Green);
            }
            else
            {
                int uid;
                Int32.TryParse(ellipse.Parent.GetValue(UIElement.UidProperty).ToString(), out uid);

                if (ellipse.Fill.ToString() == red.ToString() || ellipse.Fill.ToString() == black.ToString())
                {
                    return secondClick;
                }
                else if((uid <= 58 && uid >= 50) || (uid <= 68 && uid >= 60))
                {
                    return secondClick;
                }
                else if(ellipse.Name == _previousEllipse.Name)
                {
                    return secondClick;
                }
                else
                {
                    ellipse.Fill = _previousFill;
                    ellipse.Name = _previousEllipse.Name;
                    _previousFill = null;
                    _previousEllipse.Fill = new SolidColorBrush(Colors.Transparent);
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
    }
}
