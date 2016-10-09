using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace AI.Task4
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int GameSize = 8;

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

        bool CheckLine(bool isBlack, int r, int c, int dx, int dy)
        {
            var nr = r + dx;
            var nc = c + dy;

            while (true)
            {
                if (!CheckInBounds(nr, nc)) return false;
                var cur = Board[nr, nc];
                if (cur is EmptyCell) return false;
                var cl = cur as Checker;

                if (cl.IsBlack == isBlack)
                    return true;
                nr += dx; nc += dy;
            }
        }

        void SetLine(bool isBlack, int r, int c, int dx, int dy)
        {
            var nr = r + dx;
            var nc = c + dy;

            while (true)
            {
                if (!CheckInBounds(nr, nc)) return;
                var cur = Board[nr, nc];
                if (cur is EmptyCell) return;
                var cl = cur as Checker;

                if (cl.IsBlack == isBlack)
                    return;
                cl.IsBlack = isBlack;
                nr += dx; nc += dy;
            }
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

        public MainWindow()
        {
            InitializeComponent();

            for (var x = 0; x < GameSize; x++)
                for (var y = 0; y < GameSize; y++)
                    Board[x,y] = new EmptyCell();

            Create(3, 4, true);
            Create(4, 3, true);

            Create(3, 3, false);
            Create(4, 4, false);
        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var obj = sender as UIElement;

            if (obj == null) return;

            var c = Grid.GetColumn(obj);
            var r = Grid.GetRow(obj);

            //MessageBox.Show(c + " " + r);

            if (MakeStep(_isBlackStep, r, c))
                _isBlackStep = !_isBlackStep;

            blacksLabel.Content = Blacks;
            whitesLabel.Content = Whites;
        }
    }

    public class UIChecker
    {
        ResourceDictionary _res;
        Button _btn;
        Checker _checker;

        public Checker Checker
        {
            get
            {
                return _checker;
            }

            set
            {
                _checker = value;
            }
        }

        public UIChecker(int row, int col, bool isBlack, ResourceDictionary r, Grid g)
        {
            _res = r;

            var style = (isBlack ? r["BlackChecker"] : r["WhiteChecker"]) as Style;
            var b = new Button {
                Style = style
            };
            g.Children.Add(b);

            Grid.SetRow(b, row);
            Grid.SetColumn(b, col);

            _btn = b;

            var checker = new Checker(isBlack);
            checker.ColorChanged += Checker_ColorChanged;

            _checker = checker;
        }

        private void Checker_ColorChanged(object sender, CheckerColorChangedEventArgs e)
        {
            _btn.Style = (e.IsBlack ? _res["BlackChecker"] : _res["WhiteChecker"]) as Style;
            _btn.InvalidateVisual();
        }
    }

    public abstract class BoardCell { }

    public sealed class EmptyCell : BoardCell { }

    public class CheckerColorChangedEventArgs : EventArgs
    {
        public CheckerColorChangedEventArgs(bool isBlack)
        {
            IsBlack = isBlack;
        }

        public bool IsBlack { get; }
    }

    public sealed class Checker : BoardCell
    {
        public Checker(bool isBlack)
        {
            IsBlack = isBlack;
        }
        bool isBlack;

        public bool IsBlack
        {
            get
            {
                return isBlack;
            }

            set
            {
                isBlack = value;
                ColorChanged?.Invoke(this, new CheckerColorChangedEventArgs(value));
            }
        }

        public bool IsWhite => !IsBlack;

        public event EventHandler<CheckerColorChangedEventArgs> ColorChanged;
    }

    public unsafe struct BitBoard8x8
    {
        private fixed int board[64];

        public int this[int x, int y]
        {
            get
            {
                fixed(int* b = board)
                {
                    return b[x + 8 * y];
                }
            }
            set
            {
                fixed (int* b = board)
                {
                    b[x + 8 * y] = value;
                }
            }
        }

        public static BitBoard8x8 Copy(ref BitBoard8x8 other)
        {
            var bb = new BitBoard8x8();
            fixed (int* src = other.board)
            {
                for (var i = 0; i < 64; i++)
                    bb.board[i] = src[i];
            }
            return bb;
        }
    }

    public sealed class CheckerBoard
    {
        private BoardCell[,] _board;

        public int Blacks { get; private set; }
        public int Whites { get; private set; }

        public BoardCell this[int x, int y]
        {
            get
            {
                return _board[x, y];
            }
            set
            {
                var old = _board[x,y] as Checker;
                if(old != null)
                {
                    old.ColorChanged -= Nw_ColorChanged;
                    if (old.IsBlack) Blacks--;
                    else Whites--;
                }

                _board[x, y] = value;

                var nw = value as Checker;
                if (nw != null)
                {
                    nw.ColorChanged += Nw_ColorChanged;

                    if (nw.IsBlack) Blacks++;
                    else Whites++;
                }
            }
        }

        private void Nw_ColorChanged(object sender, CheckerColorChangedEventArgs e)
        {
            if (e.IsBlack)
            {
                Blacks++;
                Whites--;
            }
            else
            {
                Whites++;
                Blacks--;
            }
        }

        public CheckerBoard(int width, int height)
        {
            _board = new BoardCell[width,height];
            for (var i = 0; i < width; i++)
                for (var j = 0; j < height; j++)
                    this[i, j] = new EmptyCell();
        }

        bool CheckLine(bool isBlack, int r, int c, int dx, int dy)
        {
            var nr = r + dx;
            var nc = c + dy;

            while (true)
            {
                if (!CheckInBounds(nr, nc)) return false;
                var cur = this[nr, nc];
                if (cur is EmptyCell) return false;
                var cl = cur as Checker;

                if (cl.IsBlack == isBlack)
                    return true;
                nr += dx; nc += dy;
            }
        }

        bool CheckInBounds(int r, int c)
            => r >= 0 && r < _board.GetLength(0) && c >= 0 && c < _board.GetLength(1);

        public bool AvDirections(IntPoint p, bool forBlack, out LinkedList<IntPoint> d)
        {
            var r = p.X;
            var c = p.Y;

            var dirs = new LinkedList<IntPoint>();
            var b = false;

            for (var i = -1; i <= 1; i++)
                for (var j = -1; j <= 1; j++)
                    if (CheckInBounds(r + i, c + j)
                        && this[r + i, c + j] is Checker)
                    {
                        var ch = this[r + i, c + j] as Checker;
                        var cond = ch.IsBlack != forBlack && CheckLine(forBlack, r, c, i, j);

                        if (cond) dirs.AddLast(new IntPoint(i, j));

                        b = b || cond;
                    }
            d = dirs;
            return b;
        }

        public IEnumerable<IntPoint> AvailableTurns(bool forBlack)
        {
            LinkedList<IntPoint> a;

            for (var i = 0; i < _board.GetLength(0); i++)
                for (var j = 0; j < _board.GetLength(1); j++)
                    if (AvDirections(new IntPoint(i, j), forBlack, out a))
                        yield return new IntPoint(i, j);
        }

        public void SetLine(bool isBlack, int r, int c, int dx, int dy)
        {
            var nr = r + dx;
            var nc = c + dy;

            while (true)
            {
                if (!CheckInBounds(nr, nc)) return;
                var cur = this[nr, nc];
                if (cur is EmptyCell) return;
                var cl = cur as Checker;

                if (cl.IsBlack == isBlack)
                    return;
                cl.IsBlack = isBlack;
                nr += dx; nc += dy;
            }
        }
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
    }

}
