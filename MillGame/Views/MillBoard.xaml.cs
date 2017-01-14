using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using MillGame.ExtentionMethods;
using MillGame.ViewModels;
using MillGame.Models.Core;

namespace MillGame.Views
{
    /// <summary>
    /// Interaction logic for MillBoard.xaml
    /// </summary>
    public partial class MillBoard : UserControl
    {
        public static MillBoardViewModel ViewModel { get; private set; }

        public event EventHandler OnBoardRectPressed;
        public event EventHandler OnStonePressed;

        //public static Controller _ctrl { get; private set; }
        public List<Rectangle> BoardRectangles;
        public Dictionary<int, Ellipse> BoardStones = new Dictionary<int, Ellipse>();
        public Queue<Ellipse> WhitePlacingStones;
        public Queue<Ellipse> BlackPlacingStones;
        //private bool _secondClick = false;

        public MillBoard()
        {
            ViewModel = new MillBoardViewModel(this);

            InitializeComponent();
            BoardRectangles = BoardCanvas.Children.Cast<Rectangle>().ToList();
            WhitePlacingStones = new Queue<Ellipse>(new List<Ellipse>()
            {
                White1, White2, White3, White4, White5, White6, White7, White8, White9
            });
            BlackPlacingStones = new Queue<Ellipse>(new List<Ellipse>()
            {
                Black1, Black2, Black3, Black4, Black5, Black6, Black7, Black8, Black9
            });
        }

        private void Stone_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OnStonePressed?.Invoke(sender, e);

            //if (ViewModel.IsGameRunning)
            //{
            //    _secondClick = ViewModel.StoneClick(sender, _secondClick, true);

            //    if(!_secondClick)
            //    {
            //        _ctrl.Compute();
            //    }
            //}
        }

        public void HideButtons()
        {
            BeginGrid.Visibility = Visibility.Collapsed;
        }

        public void PlaceStone(int pos, sbyte color)
        {
            var stone = color == IController.WHITE ? WhitePlacingStones.Dequeue() : BlackPlacingStones.Dequeue();
            
            BoardStones.Add(pos, stone);
            stone.Tag = pos;
            ((Canvas)stone.Parent).Children.Remove(stone);
            BoardCanvas.Children.Add(stone);
            Canvas.SetLeft(stone, Canvas.GetLeft(BoardRectangles[pos]));
            Canvas.SetTop(stone, Canvas.GetTop(BoardRectangles[pos]));
        }

        public void MoveStone(int from, int to)
        {
            var x = Canvas.GetLeft(BoardRectangles[to]);
            var y = Canvas.GetTop(BoardRectangles[to]);

            var stone = BoardStones[from];
            stone.MoveTo(x, y);
            BoardStones.Remove(from);
            BoardStones.Add(to, stone);
            stone.Tag = to;
        }

        public void TakeStone(int pos)
        {
            var stone = BoardStones[pos];
            BoardStones.Remove(pos);
            BoardCanvas.Children.Remove(stone);
        }

        private void BoardRect_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            OnBoardRectPressed?.Invoke(sender, e);
        }
    }
}
