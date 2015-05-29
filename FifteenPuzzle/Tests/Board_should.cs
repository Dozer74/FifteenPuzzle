using System.Windows;
using FifteenPuzzle.AI;
using NUnit.Framework;

namespace FifteenPuzzle.Tests
{
    [TestFixture]
    internal class Board_should
    {
        [Test]
        public void return_block_count()
        {
            var board = Board.Instance;
            board.Create(new[,]
            {
                {1, 1, 1},
                {2, 2, 2},
                {3, 3, 3}
            });
            Assert.That(Board.BlocksCount, Is.EqualTo(9));
        }

        [Test]
        public void set_value()
        {
            var board = Board.Instance;
            board.Create(new[,]
            {
                {1, 1, 1, 1},
                {2, 2, 2, 2},
                {3, 3, 3, 3},
                {4, 4, 4, 4}
            });

            board[new Point(1, 1)] = 42;
            Assert.That(board[new Point(1, 1)], Is.EqualTo(42));
        }

        [Test]
        public void return_value()
        {
            var board = Board.Instance;
            board.Create(new[,]
            {
                {1, 1, 1},
                {2, 2, 2},
                {3, 3, 3}
            });
            Assert.That(board[new Point(1, 1)], Is.EqualTo(2));
        }

        [Test]
        public void swap_values()
        {
            var board = Board.Instance;
            board.Create(new[,]
            {
                {1, 1, 1, 1},
                {2, 2, 2, 2},
                {3, 3, 3, 3},
                {4, 4, 4, 4}
            });

            var p1 = new Point(0, 0);
            var p2 = new Point(2, 2);

            Board.Instance.Swap(p1, p2);
            Assert.That(Board.Instance[p1], Is.EqualTo(3));
            Assert.That(Board.Instance[p2], Is.EqualTo(1));
        }
    }
}