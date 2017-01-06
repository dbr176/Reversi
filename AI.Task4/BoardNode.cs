using System;
using System.Collections.Generic;
using MinMaxTree;

namespace AI.Task4
{

    public sealed class BoardNode : MinMaxNode<BoardNode, BitBoard8x8, IntPoint>
    {
        public BoardNode(BitBoard8x8 board, bool isBlackTurn, bool enemyIsBlack)
        {
            State = board;
            IsBlackTurn = isBlackTurn;
            IsEnemyTurn = enemyIsBlack == isBlackTurn;
            _enemyIsBlack = enemyIsBlack;
            GetTurns = (x, y) => x.AvailableTurns(y);
        }

        private bool _enemyIsBlack;
        public bool IsBlackTurn { get; set; }
        public Func<BitBoard8x8, bool, IEnumerable<IntPoint>> GetTurns { get; set; }

        protected override LinkedList<Tuple<BoardNode, IntPoint>> Unfold()
        {
            var list = new LinkedList<Tuple<BoardNode, IntPoint>>();

            var turns = GetTurns(State, IsBlackTurn);
            foreach(var t in turns)
            {
                var ns = State.Turn(IsBlackTurn, t);
                var node = new BoardNode(ns, !IsBlackTurn, _enemyIsBlack);
                list.AddLast(new Tuple<BoardNode, IntPoint>(node, t));
            }
            
            return list;
        }
    }
}