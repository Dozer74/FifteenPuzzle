using System.Windows;

namespace FifteenPuzzle
{
    /// <summary>
    ///     Логика взаимодействия для WinForm.xaml
    /// </summary>
    public partial class WinForm : Window
    {
        public WinForm()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}