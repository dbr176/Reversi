using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AI.Task4
{

    public unsafe struct BitBoard8x8
    {
        private sbyte[] board;

        public static BitBoard8x8 Create()
        {
            var bb = new BitBoard8x8();
            bb.board = new sbyte[64];
            return bb;
        }

        public override string ToString()
        {
            var s = $"";
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                    s += $"{this[j, i]} ";
                s += "\n";
            }
            return s;
        }

        public sbyte this[int x, int y]
        {
            get
            {
                fixed(sbyte* b = board)
                {
                    return b[x + 8 * y];
                }
            }
            set
            {
                fixed (sbyte* b = board)
                {
                    b[x + 8 * y] = value;
                }
            }
        }
        public static BitBoard8x8 Copy(ref BitBoard8x8 other)
        {
            var bb = Create();
            fixed (sbyte* src = other.board)
            {
                for (var i = 0; i < 64; i++)
                    bb.board[i] = src[i];
            }
            return bb;
        }

        bool CheckLine(bool isBlack, int r, int c, int dx, int dy)
        {
            if (dx == 0 && dy == 0) return false;

            var nr = r + dx;
            var nc = c + dy;

            while (true)
            {
                if (!CheckInBounds(nr, nc))
                    return false;
                var cur = this[nr, nc];
                if (cur == 0) return false;

                if (IsBlack(nr, nc) == isBlack)
                    return true;
                nr += dx; nc += dy;
            }
        }

        private bool IsChecker(int x, int y) => this[x, y] != 0;
        private bool IsEmpty(int x, int y) => this[x, y] == 0;
        private bool IsBlack(int x, int y) => this[x, y] == 1;

        bool CheckInBounds(int r, int c)
            => r >= 0 && r < 8 && c >= 0 && c < 8;

        public BitBoard8x8 Turn(bool isBlack, IntPoint p)
        {
            var bb = Copy(ref this);

            var l = new LinkedList<IntPoint>();
            AvailableDirections(p, isBlack, out l);
            foreach (var d in l)
                bb.SetLine(isBlack, p.X, p.Y, d.X, d.Y);
            bb[p.X, p.Y] = (sbyte)(isBlack ? 1 : -1);
            return bb;
        }

        public bool AvailableDirections(IntPoint p, bool forBlack, out LinkedList<IntPoint> d)
        {
            var r = p.X;
            var c = p.Y;

            var dirs = new LinkedList<IntPoint>();
            var b = false;

            for (var i = -1; i <= 1; i++)
                for (var j = -1; j <= 1; j++)
                    if (CheckInBounds(r + i, c + j))
                    if(IsChecker(r + i, c + j))
                    {
                        var ch = IsBlack(r + i, c + j);
                        var cond = ch != forBlack && CheckLine(forBlack, r, c, i, j);

                        if (cond) dirs.AddLast(new IntPoint(i, j));

                        b = b || cond;
                    }
            d = dirs;
            return b;
        }

        public IEnumerable<IntPoint> AvailableTurns(bool forBlack)
        {
            for (var i = 0; i < 8; i++)
                for (var j = 0; j < 8; j++)
                    if (this[i, j] == 0 && AvailableDirections(new IntPoint(i, j), forBlack, out LinkedList<IntPoint> a))
                        yield return new IntPoint(i, j);
        }

        public void SetLine(bool isBlack, int r, int c, int dx, int dy)
        {
            if (dx == 0 && dy == 0) return;

            var nr = r + dx;
            var nc = c + dy;

            while (true)
            {
                if (!CheckInBounds(nr, nc)) return;
                var cur = this[nr, nc];
                if (cur == 0) return;

                if (cur == 1 == isBlack)
                    return;
                this[nr, nc] = (sbyte)(isBlack ? 1 : -1);
                nr += dx; nc += dy;
            }
        }

        public static bool operator == (BitBoard8x8 a, BitBoard8x8 b)
        {
            for (var i = 0; i < 8; i++)
                for (var j = 0; j < 8; j++)
                    if (a[i, j] != b[i, j]) return false;
            return true;
        }

        public override bool Equals(object obj)
            => obj is BitBoard8x8 b && b == this;

        public override int GetHashCode()
            => board.Aggregate(0, (s, x) => s ^ x * s);
        

        public static bool operator != (BitBoard8x8 a, BitBoard8x8 b) => !(a == b);
    }

}