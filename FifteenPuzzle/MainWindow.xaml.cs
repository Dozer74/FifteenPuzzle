using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using FifteenPuzzle.AI;
using FifteenPuzzle.Helpers;
using FifteenPuzzle.UserControls;

namespace FifteenPuzzle
{
    public enum GameState
    {
        Ready,
        Playing,
        Pause
    }

    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Block[,] blocks;
        private int blockSize, renderIndex;
        private int[,] board, savedBoard;
        private Point nullBlockPosition, savedNullBlockPosition;
        private Direction[] solution;
        private GameState state = GameState.Ready;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Создаёт и визуализирует новую доску или (если reset == true) воссоздаёт начальное состояние текущей
        private void CreateBoard(bool reset = false)
        {
            var blocksPerLine = Convert.ToInt32(((ListBoxItem) CbBoardSize.SelectedItem).Tag);

            if (!reset)
            {
                do
                {
                    board = GetRandomBoard(blocksPerLine);
                } while (!Ai.IsSolvable(board));
            }
            else
            {
                //восстанавливаем сохраненное состояние доски
                board = savedBoard;
                nullBlockPosition = savedNullBlockPosition;
            }

            blockSize = (int) (BoardCanvas.Width/blocksPerLine);
            blocks = new Block[blocksPerLine, blocksPerLine];

            for (var i = 0; i < blocksPerLine; i++)
            {
                for (var j = 0; j < blocksPerLine; j++)
                {
                    var block = new Block(j, i, board[i, j]);

                    block.Width = block.Height = blockSize;
                    block.SetValue(Canvas.LeftProperty, blockSize*(double) j);
                    block.SetValue(Canvas.TopProperty, blockSize*(double) i);

                    blocks[j, i] = block;
                    BoardCanvas.Children.Add(block);
                }
            }
        }

        // Формирует случайное расположение пятнашек
        private int[,] GetRandomBoard(int blocksPerLine)
        {
            const int shuffleCount = 100; //число случайных смещений пятнашек

            var startBoard = CreateStartBoard(blocksPerLine);
            nullBlockPosition = new Point(blocksPerLine - 1, blocksPerLine - 1);

            var randomBoard = (int[,]) startBoard.Clone();

            do
            {
                var random = new Random();
                for (var i = 0; i < shuffleCount; i++)
                {
                    var nearCells = Utils.GetNearCell(nullBlockPosition, randomBoard);
                    var newPos = nearCells[random.Next(nearCells.Count())];
                    Utils.SwapOnBoard(nullBlockPosition, newPos, randomBoard);
                    nullBlockPosition = newPos;
                }
            } while (Ai.IsAlreadySolved(randomBoard));

            nullBlockPosition = new Point(nullBlockPosition.Y, nullBlockPosition.X);

            //Сохраняем начальное состояние только что созданной доски
            savedNullBlockPosition = nullBlockPosition;
            savedBoard = randomBoard;

            return randomBoard;
        }

        // Генерирует собранную доску
        private static int[,] CreateStartBoard(int blocksPerLine)
        {
            var startBoard = new int[blocksPerLine, blocksPerLine];

            var k = 1;
            for (var i = 0; i < blocksPerLine; i++)
            {
                for (var j = 0; j < blocksPerLine; j++)
                {
                    startBoard[i, j] = k++;
                }
            }
            startBoard[blocksPerLine - 1, blocksPerLine - 1] = 0;
            return startBoard;
        }

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            switch (state)
            {
                case GameState.Ready: // Стартовое состояние. Доска сгенерирована, игра готова к запуску
                    BtnStart.Content = "Пауза";
                    CbBoardSize.IsEnabled = false;
                    state = GameState.Playing;

                    Ai.BuildSolution(board, out solution);
                    RenderSolution();
                    break;

                case GameState.Playing: // Нажатие на кнопку "Пауза"
                    BtnStart.Content = "Продолжить";
                    BtnStart.IsEnabled = false;
                    state = GameState.Pause;
                    break;
                case GameState.Pause: // Нажатие на кнопку "Продолжить"
                    BtnStart.Content = "Пауза";
                    BtnReset.IsEnabled = false;
                    state = GameState.Playing;

                    RenderSolution();
                    break;
            }
        }

        //Визуализирует сборку пятнашек
        private void RenderSolution()
        {
            if (renderIndex == solution.Length)
            {
                ShowWinForm();
                return;
            }

            var currentStep = solution[renderIndex++];

            sbyte sign = 1;
            var property = Canvas.LeftProperty;
            var otherPoint = new Point(nullBlockPosition.X - 1, nullBlockPosition.Y);

            switch (currentStep)
            {
                case Direction.Up:
                    property = Canvas.TopProperty;
                    otherPoint = new Point(nullBlockPosition.X, nullBlockPosition.Y - 1);
                    break;
                case Direction.Right:
                    sign = -1;
                    otherPoint = new Point(nullBlockPosition.X + 1, nullBlockPosition.Y);
                    break;
                case Direction.Down:
                    property = Canvas.TopProperty;
                    sign = -1;
                    otherPoint = new Point(nullBlockPosition.X, nullBlockPosition.Y + 1);
                    break;
            }

            var otherBlock = Utils.GetBlock(otherPoint, blocks);
            var moveAnimation = BuildAnimation(sign, otherPoint);

            otherBlock.BeginAnimation(property, moveAnimation);
        }

        //Возвращает анимацию движения
        private DoubleAnimation BuildAnimation(int sign, Point otherPoint)
        {
            var animationSpeed = 650 - (int) SlAnimSpeed.Value;
            var moveAnimation = new DoubleAnimation
            {
                By = sign*blockSize,
                Duration = TimeSpan.FromMilliseconds(animationSpeed),
                FillBehavior = FillBehavior.Stop
            };

            moveAnimation.Completed += (o, args) =>
            {
                Utils.SwapBlocks(nullBlockPosition, otherPoint, blocks);
                nullBlockPosition = otherPoint;

                if (state == GameState.Playing)
                {
                    RenderSolution();
                }
                else
                {
                    //Включаем контролы только если игра приостановлена а текущая анимация завершена
                    BtnStart.IsEnabled = CbBoardSize.IsEnabled = BtnReset.IsEnabled = true;
                }
            };

            return moveAnimation;
        }

        //Отображает сообщение о завершении сборки
        private void ShowWinForm()
        {
            var form = new WinForm {Owner = this};
            form.ShowDialog();

            BtnStart.Content = "Готово!";
            BtnStart.IsEnabled = false;
            BtnReset.IsEnabled = CbBoardSize.IsEnabled = true;
        }

        // Сбрасывает значения переменных на начальные
        private void Reset()
        {
            BoardCanvas.Children.Clear();
            BtnReset.IsEnabled = false;
            CbBoardSize.IsEnabled = true;
            solution = null;
            renderIndex = 0;
            state = GameState.Ready;
            BtnStart.Content = "Поехали!";
            BtnStart.IsEnabled = true;
        }

        #region Control events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateBoard();
        }

        private void CbBoardSize_OnDropDownClosed(object sender, EventArgs e)
        {
            Reset();
            CreateBoard();
        }

        private void BtnReset_OnClick(object sender, RoutedEventArgs e)
        {
            Reset();
            CreateBoard(true);
        }

        #endregion
    }
}