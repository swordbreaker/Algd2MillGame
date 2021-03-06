﻿using System;
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
            var oldP = new Point
            {
                X = Canvas.GetLeft(target),
                Y = Canvas.GetTop(target)
            };

            var anim1 = new DoubleAnimation(oldP.X, newX, TimeSpan.FromSeconds(1));
            var anim2 = new DoubleAnimation(oldP.Y, newY, TimeSpan.FromSeconds(1));
            target.BeginAnimation(Canvas.LeftProperty, anim1);
            target.BeginAnimation(Canvas.TopProperty, anim2);
        }
    }
}
