using System.Windows;

namespace FifteenPuzzle.AI
{
    /// <summary>
    /// Содержит методы для работы с доской
    /// </summary>
    class Board
    {
        private int[,] board;

        public static int BlocksPerLine { get; private set; }
        public static int BlocksCount { get; private set; }

        private static Board instance;

        private Board(){ }

        public static Board Instance => instance ?? (instance = new Board());

        /// <summary>
        /// Производит инициализацию доски
        /// </summary>
        /// <param name="startBoard">Начальное расположение пятнашек</param>
        public void Create(int[,] startBoard)
        {
            board = startBoard;
            BlocksPerLine = board.GetLength(0);
            BlocksCount = BlocksPerLine*BlocksPerLine;
        }

        /// <summary>
        /// Меняет значения в ячейках p1 и p2 местами
        /// </summary>
        public void Swap(Point p1, Point p2)
        {
            var buf = this[p1];
            this[p1] = this[p2];
            this[p2] = buf;
        }

        public int this[Point p]
        {
            get { return board[(int)p.Y, (int)p.X]; }
            set { board[(int)p.Y, (int)p.X] = value; }
        }

        public int this[int y, int x]
        {
            get { return this[new Point(x, y)]; }
            set { this[new Point(x, y)] = value; }
        }
    }
}
