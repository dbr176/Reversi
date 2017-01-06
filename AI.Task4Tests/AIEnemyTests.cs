using Microsoft.VisualStudio.TestTools.UnitTesting;
using AI.Task4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI.Task4.Tests
{
    [TestClass()]
    public class AIEnemyTests
    {
        [TestMethod()]
        public void UpdatePosTest()
        {
            var board = BitBoard8x8.Create();

            board[4, 3] = 1;
            board[3, 4] = 1;
            board[3, 3] = (-1);
            board[4, 4] = (-1);

            // AI - Белые
            // Текущий ход: Чёрные
            var enemy = new AIEnemy();
            enemy.Eval = (x) => 0;
            enemy.IsBlack = false;
            enemy.Root = new BoardNode(board, true, true);
            
            // Возможные ходы
            var aiAv = enemy.Root.Children.ToArray();
            var av = board.AvailableTurns(true).Select(x => board.Turn(true, x)).ToArray();
            var step = enemy.Root.Children.First().Item2;

            board = board.Turn(true, step);

            // Возможные ходы
            enemy.UpdatePos(board);
            aiAv = enemy.Root.Children.ToArray();
            var shft = enemy.ChooseNext().Value;

            board = board.Turn(false, shft);

        }

        [TestMethod()]
        public void ChooseNextTest()
        {
            Assert.Fail();
        }
    }
}

/*

    Доска: Стартовая позиция
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0-1 1 0 0 0
    0 0 0 1-1 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0

    AI: Возможные ходы Чёрных

    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 1 1 1 0 0 0
    0 0 0 1-1 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0

    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 1 0 0 0 0
    0 0 0 1 1 0 0 0
    0 0 0 1-1 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0

    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0-1 1 0 0 0
    0 0 0 1 1 0 0 0
    0 0 0 0 1 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0

    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0-1 1 0 0 0
    0 0 0 1 1 1 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0

    Доступные ходы: совпадают с AI

----------------------------------
    Доска: После хода чёрных

    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 1 1 1 0 0 0
    0 0 0 1-1 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0

    AI: Возможные ходы

    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0-1 0 0 0 0 0
    0 0 1-1 1 0 0 0
    0 0 0 1-1 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0

    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 1 1 1 0 0 0
    0 0-1-1-1 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0

    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0-1 0 0 0
    0 0 1 1-1 0 0 0
    0 0 0 1-1 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0

----------------------------------
    Доска: После хода белых

    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0-1 0 0 0 0 0
    0 0 1-1 1 0 0 0
    0 0 0 1-1 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0
    0 0 0 0 0 0 0 0

    AI: Возможные ходы

 * */
