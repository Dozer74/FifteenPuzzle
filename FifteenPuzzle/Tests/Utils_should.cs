using System.Windows;
using FifteenPuzzle.AI;
using FifteenPuzzle.Helpers;
using NUnit.Framework;

namespace FifteenPuzzle.Tests
{
    [TestFixture]
    internal class Utils_should
    {
        [SetUp]
        public void SetUp()
        {
            Board.Instance.Create(new[,]
            {
                {1, 2, 3},
                {3, 4, 5},
                {7, 8, 9}
            });
        }

        [TestCase(0, 0, Result = true)]
        [TestCase(1, 2, Result = true)]
        [TestCase(3, 1, Result = false)]
        [TestCase(1, -1, Result = false)]
        [TestCase(5, -5, Result = false)]
        public bool check_bounds(int x, int y)
        {
            return Utils.CheckBounds(new Point(x, y));
        }
    }
}