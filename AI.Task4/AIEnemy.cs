using System;
using System.Linq;
using MinMaxTree;

namespace AI.Task4
{

    public class AIEnemy : MinMaxTree<BoardNode, BitBoard8x8, IntPoint>
    {
        public bool IsBlack { get; set; }

        static int[,] weights = {
            {25, 1, 9, 9, 9, 9, 1, 25},
            { 1,-3, 1, 7, 7, 1,-3, 1 },
            { 9, 1, 7, 5, 5, 7, 1, 9 },
            { 9, 7, 5, 5, 5, 5, 7, 9 },
            { 9, 7, 5, 5, 5, 5, 7, 9 },
            { 9, 5, 7, 5, 5, 7, 1, 9 },
            { 1,-3, 1, 7, 7, 1,-3, 1 },
            {25, 1, 9, 9, 9, 9, 1, 25 }
        };

        public void UpdatePos(BitBoard8x8 current)
        {
            var nroot = Root.Children.FirstOrDefault(x => x.Item1.State == current);

            if (nroot == null)
                Root = new BoardNode(current, !Root.IsBlackTurn, IsBlack);
            else
                Root = nroot.Item1;
        }

        private int EvalPosition(Tuple<BoardNode, IntPoint> n)
        {
            var s = 0;
            var b = n.Item1.State;
            for (var i = 0; i < 8; i++)
                for (var j = 0; j < 8; j++)
                    s += (IsBlack ? 1 : -1) * b[i, j] * weights[i, j];
            return s;
        }

        private int EvalPositionFuture(Tuple<BoardNode, IntPoint> n, int depth, bool isEnemyTurn)
        {
            if (depth == 0)
                return EvalPosition(n);

            var ch = n.Item1.Children;
            if (ch.Count() == 0)
                return isEnemyTurn ? 1000 : -1000;

            if (!isEnemyTurn)
                return n.Item1.Children.Max(x => EvalPositionFuture(x, depth - 1, !isEnemyTurn));

            return n.Item1.Children.Min(x => EvalPositionFuture(x, depth - 1, !isEnemyTurn));

        }

        static int _lookForward = 0;
        public IntPoint? ChooseNext()
        {
            var c = Root.Children;
            IntPoint? max = null;
            var maxV = int.MinValue;

            foreach(var n in c)
            {
                var s = EvalPositionFuture(n, _lookForward, false);
                if (s > maxV)
                {
                    max = n.Item2;
                    maxV = s;
                }
            }

            return max;
        }
    }
}