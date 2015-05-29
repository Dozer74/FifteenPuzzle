using System;
using System.Collections.Generic;
using System.Windows;
using FifteenPuzzle.Helpers;

namespace FifteenPuzzle.AI
{
    /// <summary>
    ///     Содержит методы для поиска и построения решения пятнашек
    /// </summary>
    internal static class Ai
    {
        private static readonly Board Board = Board.Instance;
        private static int minPrevIteration, deepness;
        private static Stack<Direction> wayStack;
        private static int[] goalX, goalY;

        /// <summary>
        ///     Пытается построить последовательность шагов к цели
        /// </summary>
        /// <param name="startBoard">Начальное состояние доски</param>
        /// <param name="result">Содержит последовательность шагов к цели в случае успеха. Иначе - null</param>
        /// <returns>Результат поиска решения</returns>
        public static SolvingResult BuildSolution(int[,] startBoard, out Direction[] result)
        {
            Board.Create(startBoard);

            goalX = new int[Board.BlocksCount];
            goalY = new int[Board.BlocksCount];
            InitGoalArrays();

            wayStack = new Stack<Direction>();
            result = null;

            if (!IsSolvable(startBoard))
                return SolvingResult.Unsolvable;
            if (GetEstimate() == 0)
                return SolvingResult.AlreadyDone;
            if (!IdaStar())
                return SolvingResult.IdaStarError;

            result = wayStack.ToArray();
            return SolvingResult.SolveFound;
        }

        // Проверяет разрешимость пятнашек
        public static bool IsSolvable(int[,] startBoard)
        {
            Board.Create(startBoard);

            var count = 0;
            var transpos = 0;

            var ar = new int[Board.BlocksCount];

            for (var i = 0; i < Board.BlocksPerLine; i++)
            {
                int value;
                if (i%2 == 0)
                {
                    for (var j = 0; j < Board.BlocksPerLine; j++)
                    {
                        value = Board[i, j];
                        if (value > 0)
                        {
                            ar[count] = value;
                            count++;
                        }
                    }
                }
                else
                {
                    for (var j = Board.BlocksPerLine - 1; j >= 0; j--)
                    {
                        value = Board[i, j];
                        if (value > 0)
                        {
                            ar[count] = value;
                            count++;
                        }
                    }
                }
            }
            for (var i = 0; i < count - 1; i++)
            {
                for (var j = i + 1; j < count; j++)
                {
                    if (ar[i] > ar[j]) transpos++;
                }
            }

            return transpos%2 == 1;
        }

        // Проверяет, является ли доска startBoard уже собранной
        public static bool IsAlreadySolved(int[,] startBoard)
        {
            Board.Create(startBoard);
            goalX = new int[Board.BlocksCount];
            goalY = new int[Board.BlocksCount];
            InitGoalArrays();

            return GetEstimate() == 0;
        }

        // Алгоритм поиска IDA*
        private static bool IdaStar()
        {
            const int infinity = int.MaxValue;
            const int maxDeepness = 50;

            var result = false;
            deepness = GetEstimate();

            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (deepness <= maxDeepness)
            {
                minPrevIteration = infinity;
                var x0 = 0;
                var y0 = 0;
                for (var i = 0; i < Board.BlocksPerLine; i++)
                {
                    for (var j = 0; j < Board.BlocksPerLine; j++)
                    {
                        if (Board[i, j] == 0)
                        {
                            x0 = j;
                            y0 = i;
                        }
                    }
                }
                result = RecSearch(0, Direction.Nowhere, new Point(x0, y0));
                deepness = minPrevIteration;
                if (result) break;
            }
            return result;
        }

        /// <summary>
        ///     Рекурсивный поиск в глубину
        /// </summary>
        /// <param name="g">Уже пройденный путь</param>
        /// <param name="previousMove">Предыдущий ход</param>
        /// <param name="currentCell">Текущая клетка</param>
        /// <returns>True, если удалось найти решение, иначе false</returns>
        private static bool RecSearch(int g, Direction previousMove, Point currentCell)
        {
            int[] dx = {0, -1, 0, 1};
            int[] dy = {1, 0, -1, 0};
            var moveDirections = new[] {Direction.Down, Direction.Left, Direction.Up, Direction.Right};

            var oppositeMove = new[] {Direction.Up, Direction.Right, Direction.Down, Direction.Left};

            var h = GetEstimate();
            if (h == 0) return true;

            var f = g + h;
            if (f > deepness)
            {
                if (minPrevIteration > f) minPrevIteration = f;
                return false;
            }

            for (var i = 0; i < 4; i++)
            {
                var newCell = new Point(currentCell.X + dx[i], currentCell.Y + dy[i]);

                if (oppositeMove[i] != previousMove && Utils.CheckBounds(newCell))
                {
                    Board.Swap(currentCell, newCell);
                    var res = RecSearch(g + 1, moveDirections[i], newCell);
                    Board.Swap(currentCell, newCell);
                    if (res)
                    {
                        wayStack.Push(moveDirections[i]);
                        return true;
                    }
                }
            }
            return false;
        }


        // Оценочная функция "Манхеттеновское расстояние"
        private static int GetEstimate()
        {
            var manhattan = 0;
            for (var i = 0; i < Board.BlocksPerLine; i++)
            {
                for (var j = 0; j < Board.BlocksPerLine; j++)
                {
                    var value = Board[i, j];
                    if (value > 0)
                    {
                        manhattan += Math.Abs(i - goalY[value]) + Math.Abs(j - goalX[value]);
                    }
                }
            }
            return manhattan;
        }


        // Производит инициализацию массивов целевых состояний
        private static void InitGoalArrays()
        {
            for (var i = 0; i < Board.BlocksCount - 1; i++)
            {
                goalX[i + 1] = i%Board.BlocksPerLine;
                goalY[i + 1] = i/Board.BlocksPerLine;
            }
            goalX[0] = Board.BlocksPerLine;
            goalY[0] = Board.BlocksPerLine;
        }
    }
}