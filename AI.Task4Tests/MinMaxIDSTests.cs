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
    public class MinMaxIDSTests
    {
        [TestMethod()]
        public void NextStepTest()
        {
            var board = BitBoard8x8.Create();

            board[3, 3] = -1;
            board[3, 4] =  1;
            board[4, 3] =  1;
            board[4, 5] = 1;
            board[5, 4] = 1;
            board[5, 5] = 1;
            board[5, 6] = 1;
            board[6, 5] = 1;
            board[6, 6] = -1;
            board[5, 7] = -1;
            board[6, 7] = -1;
            board[7, 7] = -1;

            var aturns = board.AvailableTurns(false).ToArray();

            Assert.AreEqual(10, board.AvailableTurns(false).Count());
            
        }
    }
    
}