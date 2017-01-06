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

        private readonly SolidColorBrush black = new SolidColorBrush(Colors.Black);
        private readonly SolidColorBrush red = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);

        public MillBoardViewModel()
        {
            
        }

        public bool StoneClick(object sender, MouseEventArgs e, bool secondClick)
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
    }
}
