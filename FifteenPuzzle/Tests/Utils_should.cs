using System.Linq;
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

        [TestCase(0,0, Result = "4 2")]
        [TestCase(2, 2, Result = "6 8")]
        [TestCase(1, 1, Result = "2 8 4 6")]
        public string return_near_cells(int x, int y)
        {
            int[,] board =
            {
                {1, 2, 3},
                {4, 5, 6},
                {7, 8, 0}
            };
            var result =
                Utils.GetNearCell(new Point(x, y), board).Select(s => string.Format("{0}", board[(int) s.X, (int) s.Y]));

            return string.Join(" ", result);
        } 
    }
}