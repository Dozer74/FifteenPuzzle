using System.Windows;
using System.Windows.Controls;

namespace FifteenPuzzle.UserControls
{
    /// <summary>
    ///     Логика взаимодействия для Block.xaml
    /// </summary>
    public partial class Block : UserControl
    {
        private int number;

        public Block()
        {
            InitializeComponent();
        }

        public Block(int x, int y, int number) : this()
        {
            Number = number;
            if (number == 0)
            {
                Border.Visibility = TbNumber.Visibility = Visibility.Hidden;
            }

            TbNumber.Text = number.ToString();
        }
        public int Number
        {
            get { return number; }
            set
            {
                TbNumber.Text = value.ToString();
                Border.Visibility = TbNumber.Visibility = value==0 ? Visibility.Hidden : Visibility.Visible;
                number = value;
            }
        }
    }
}