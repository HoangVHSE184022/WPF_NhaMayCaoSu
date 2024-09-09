using WPF_NhaMayCaoSu.Repository.Models;
using System.Windows;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for BoardListWindow.xaml
    /// </summary>
    public partial class BoardListWindow : Window
    {
        public BoardListWindow()
        {
            InitializeComponent();
        }
        public Account CurrentAccount { get; set; } = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount?.Role?.RoleName != "Admin")
            {
                AddBoardButton.Visibility = Visibility.Collapsed;
                EditBoardButton.Visibility = Visibility.Collapsed;
            }
            //LoadDataGrid();
        }

        private void AddBoardButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditBoardButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
