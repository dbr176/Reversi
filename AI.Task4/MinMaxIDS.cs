using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AI.Task4
{

    public sealed class MinMaxIDS
    {
        public bool IsBlack { get; set; }
        public int Steps { get; set; }
        public BitBoard8x8 StartPos { get; set; }
        public Func<BitBoard8x8, bool, int> EvalNext { get; set; }
        public Func<BitBoard8x8, bool, int> EvalCurr { get; set; }
        public int NoSteps { get; set; } = -1;
        public Func<BitBoard8x8, bool, IEnumerable<IntPoint>> GetTurns { get; set; }
        
        private int Iteration(bool isEnemyTurn, BitBoard8x8 prevState, int step)
        {
            if (step >= Steps && !isEnemyTurn) return EvalCurr(prevState, isEnemyTurn);
            var isBlack = (isEnemyTurn && !IsBlack) || (!isEnemyTurn && IsBlack);
            var turns = GetTurns(prevState, isBlack).ToArray();

            if (turns.Length == 0 && isEnemyTurn) return 10;
            if (turns.Length == 0 && !isEnemyTurn) return -10;

            var nexts = turns.Select(x => prevState.Turn(isBlack, x));

            if (isEnemyTurn)
            {
                return nexts.Select(s => Iteration(!isEnemyTurn, s, step + 1)).Min();
            }
            else
            {
               
                 return 
                    nexts.Select(s => Iteration(!isEnemyTurn, s, step + 1)).Max();
            }
        }

        public IntPoint NextStep()
        {
            var turns = StartPos.AvailableTurns(IsBlack).ToArray();
            var mp = turns[0];
            var mt = int.MinValue;

            for(var i = 0; i < turns.Length; i++)
            {
                var it = Iteration(false, StartPos.Turn(IsBlack, turns[i]), 0);
                if (it > mt)
                    mp = turns[i];
            }
            return mp;
        }
    }

}