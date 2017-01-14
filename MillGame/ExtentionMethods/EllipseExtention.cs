using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MillGame.ExtentionMethods
{
    public static class EllipseExtention
    {
        public static void MoveTo(this Ellipse target, double newX, double newY)
        {
            var top = Canvas.GetTop(target);
            var left = Canvas.GetLeft(target);
            var trans = new TranslateTransform();
            target.RenderTransform = trans;
            var anim1 = new DoubleAnimation(top, newY - top, TimeSpan.FromSeconds(2));
            var anim2 = new DoubleAnimation(left, newX - left, TimeSpan.FromSeconds(2));
            trans.BeginAnimation(TranslateTransform.XProperty, anim2);
            trans.BeginAnimation(TranslateTransform.YProperty, anim1);
        }
    }
}
