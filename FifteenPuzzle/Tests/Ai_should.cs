using FifteenPuzzle.AI;
using FifteenPuzzle.Helpers;
using NUnit.Framework;

namespace FifteenPuzzle.Tests
{
    [TestFixture]
    internal class Ai_Should
    {
        private Direction[] result;

        [Test]
        public void work_on_alredy_solved_task()
        {
            int[,] board =
            {
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {13, 14, 15, 0}
            };

            Assert.AreEqual(true, Ai.IsSolvable(board));
            Assert.AreEqual(true, Ai.IsAlreadySolved(board));
            Assert.AreEqual(SolvingResult.AlreadyDone, Ai.BuildSolution(board, out result));
            Assert.IsNull(result);
        }

        [Test]
        public void work_on_simple_task()
        {
            int[,] board =
            {
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {13, 14, 0, 15}
            };

            Assert.AreEqual(SolvingResult.SolveFound, Ai.BuildSolution(board, out result));
            CollectionAssert.AreEqual(new[] {Direction.Right}, result);
        }

        [Test]
        public void work_on_medium_task()
        {
            int[,] board =
            {
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 15, 11},
                {13, 14, 0, 12}
            };

            Assert.AreEqual(SolvingResult.SolveFound, Ai.BuildSolution(board, out result));
            CollectionAssert.AreEqual(new[] {Direction.Up, Direction.Right, Direction.Down}, result);
        }

        [Test]
        public void work_on_difficult_task()
        {
            int[,] board =
            {
                {0, 1, 2, 3},
                {6, 7, 8, 4},
                {5, 9, 10, 11},
                {13, 14, 15, 12}
            };

            var expected = new[]
            {
                Direction.Right,
                Direction.Right,
                Direction.Right,
                Direction.Down,
                Direction.Left,
                Direction.Left,
                Direction.Left,
                Direction.Down,
                Direction.Right,
                Direction.Right,
                Direction.Right,
                Direction.Down
            };

            Assert.AreEqual(SolvingResult.SolveFound, Ai.BuildSolution(board, out result));
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void work_on_unsolvable_task()
        {
            int[,] board =
            {
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {13, 15, 14, 0}
            };
            Assert.AreEqual(false, Ai.IsSolvable(board));
            Assert.AreEqual(SolvingResult.Unsolvable, Ai.BuildSolution(board, out result));
            Assert.IsNull(result);
        }
    }
}