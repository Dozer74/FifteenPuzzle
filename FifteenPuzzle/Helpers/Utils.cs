using System.Collections.Generic;
using System.Windows;
using FifteenPuzzle.AI;
using FifteenPuzzle.UserControls;

namespace FifteenPuzzle.Helpers
{
    internal static class Utils
    {
        /// <summary>
        ///     Проверяет, находится ли точка в пределах доски
        /// </summary>
        public static bool CheckBounds(Point point)
        {
            return point.X >= 0 && point.X < Board.BlocksPerLine && point.Y >= 0 && point.Y < Board.BlocksPerLine;
        }

        /// <summary>
        ///     Меняет местами значения в точках p1 и p2 на доске board
        /// </summary>
        public static void SwapOnBoard(Point p1, Point p2, int[,] board)
        {
            var buf = board[(int) p1.X, (int) p1.Y];
            board[(int) p1.X, (int) p1.Y] = board[(int) p2.X, (int) p2.Y];
            board[(int) p2.X, (int) p2.Y] = buf;
        }

        /// <summary>
        ///     Меняет местами номера на блоках в точках p1 и p2 в массиве blocks
        /// </summary>
        public static void SwapBlocks(Point p1, Point p2, Block[,] blocks)
        {
            var b1 = GetBlock(p1, blocks);
            var b2 = GetBlock(p2, blocks);

            var n = b1.Number;
            b1.Number = b2.Number;
            b2.Number = n;
        }

        /// <summary>
        ///     Возвращает блок с координатами точки p из массива blocks
        /// </summary>
        public static Block GetBlock(Point p, Block[,] blocks)
        {
            return blocks[(int) p.X, (int) p.Y];
        }

        /// <summary>
        ///     Возвращает соседние с point точки на доске board
        /// </summary>
        public static List<Point> GetNearCell(Point point, int[,] board)
        {
            var nearCells = new List<Point>();
            if (point.X - 1 >= 0) nearCells.Add(new Point(point.X - 1, point.Y));
            if (point.X + 1 < board.GetLength(0)) nearCells.Add(new Point(point.X + 1, point.Y));
            if (point.Y - 1 >= 0) nearCells.Add(new Point(point.X, point.Y - 1));
            if (point.Y + 1 < board.GetLength(1)) nearCells.Add(new Point(point.X, point.Y + 1));

            return nearCells;
        }
    }
}