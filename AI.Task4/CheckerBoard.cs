using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AI.Task4
{

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

        public BitBoard8x8 ToBitBoard()
        {
            var bb = BitBoard8x8.Create();

            if (_board.GetLength(0) != 8
                || _board.GetLength(1) != 8)
                throw new Exception();

            for (var i = 0; i < _board.GetLength(0); i++)
                for (var j = 0; j < _board.GetLength(1); j++)
                {
                    var th = this[i, j];
                    if (th is EmptyCell)
                        bb[i, j] = 0;
                    else if (th is Checker)
                    {
                        var c = th as Checker;
                        bb[i, j] = (sbyte)(c.IsBlack ? 1 : -1);
                    }
                }

            return bb;
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
                    if (this[i, j] is EmptyCell && AvDirections(new IntPoint(i, j), forBlack, out a))
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

}