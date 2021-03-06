﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AI.Task4
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int GameSize = 8;

        private void GenerateBoard()
        {

        }

        public CheckerBoard Board { get; set; } = new CheckerBoard(GameSize, GameSize);
        LinkedList<UIChecker> _checkers = new LinkedList<UIChecker>();

        public int Blacks => Board.Blacks;
        public int Whites => Board.Whites;

        public Checker Create(int r, int c, bool isBlack)
        {
            var ch = new UIChecker(r, c, isBlack, Resources, mainGrid);
            _checkers.AddFirst(ch);
            Board[r, c] = ch.Checker;

            return ch.Checker;
        }


        private bool CheckInBounds(int r, int c)
            => r >= 0 && r < GameSize && c >= 0 && c < GameSize;
        

        bool _isBlackStep = true;
        public bool MakeStep(bool isBlackStep, int r, int c)
        {
            if (Board[r, c] is Checker) return false;

            var b = false;
            var dirs = new LinkedList<IntPoint>();
            b = Board.AvDirections(new IntPoint(r, c), isBlackStep, out dirs);

            if (!b) return false;

            Create(r, c, isBlackStep);

            foreach (var d in dirs)
                Board.SetLine(isBlackStep, r, c, d.X, d.Y);
            return true;
        }

        private void InitBoard()
        {
            for(var i = 0; i < GameSize; i++)
            {
                var column = new ColumnDefinition();
                column.Width = new GridLength(64, GridUnitType.Star);
                mainGrid.ColumnDefinitions.Add(column);

                var row = new RowDefinition();
                row.Height = new GridLength(64, GridUnitType.Star);
                mainGrid.RowDefinitions.Add(row);
            }

            for(var x = 0; x < GameSize; x++)
                for(var y = 0; y < GameSize; y++)
                {
                    var cell = new System.Windows.Shapes.Rectangle();
                    mainGrid.Children.Add(cell);
                    Grid.SetColumn(cell, y);
                    Grid.SetRow(cell, x);
                    cell.Style = ((x % 2 == y % 2) ? Resources["BlackCell"] : Resources["WhiteCell"]) as Style;
                }
            
        }

        private void InitCheckers()
        {
            Create(3, 4, true);
            Create(4, 3, true);

            Create(3, 3, false);
            Create(4, 4, false);
        }

        private void InitEnemy()
        {
            _enemy = new AIEnemy();
            _enemy.Eval = (x) => 0;
            _enemy.IsBlack = false;
            _enemy.Root = new BoardNode(Board.ToBitBoard(), true, true);
        }

        AIEnemy _enemy;
        public MainWindow()
        {
            InitializeComponent();

            InitBoard();

            for (var x = 0; x < GameSize; x++)
                for (var y = 0; y < GameSize; y++)
                    Board[x, y] = new EmptyCell();

            InitCheckers();
            InitEnemy();
        }

        private bool PlayerStep(int c, int r)
        {
            var avTurns = Board.AvailableTurns(_isBlackStep);

            if (avTurns.Count() != 0)
            {
                if (MakeStep(_isBlackStep, r, c))
                    _isBlackStep = !_isBlackStep;
                else return false;
            }
            else _isBlackStep = !_isBlackStep;

            return true;
        }

        private void AIStep(int c, int r)
        {
            _enemy.UpdatePos(Board.ToBitBoard());
            var tp = _enemy.ChooseNext();

            if (tp.HasValue)
            {
                var p = tp.Value;
                if (!MakeStep(_isBlackStep, p.X, p.Y))
                {
                    MessageBox.Show("!!!!");
                }
            }
            _isBlackStep = !_isBlackStep;
            _enemy.UpdatePos(Board.ToBitBoard());
        }

        private async void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as UIElement;

            if (obj == null) return;

            var c = Grid.GetColumn(obj);
            var r = Grid.GetRow(obj);

            if (!PlayerStep(c, r)) return;

            await Task.Delay(100);

            AIStep(c, r);

            await Task.Delay(100);

            blacksLabel.Content = Blacks;
            whitesLabel.Content = Whites;
        }
    }

    public class CheckerColorChangedEventArgs : EventArgs
    {
        public CheckerColorChangedEventArgs(bool isBlack)
        {
            IsBlack = isBlack;
        }

        public bool IsBlack { get; }
    }

    public struct IntPoint
    {
        private int y;
        private int x;

        public IntPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X => x;
        public int Y => y;

        public override string ToString() => $"{X};{Y}";
    }
}
