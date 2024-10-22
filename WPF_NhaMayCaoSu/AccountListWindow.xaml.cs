using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for AccountListWindow.xaml
    /// </summary>
    public partial class AccountListWindow : Window
    {

        private IAccountService _accountService = new AccountService();
        private int _currentPage = 1;
        private int _pageSize = 12;
        private int _totalPages;
        public Account CurrentAccount { get; set; } = null;
        public AccountListWindow()
        {
            InitializeComponent();
        }

        private async void LoadDataGrid()
        {
            int totalAccountCount = await _accountService.GetTotalAccountsCountAsync();
            _totalPages = (int)Math.Ceiling((double)totalAccountCount / _pageSize);

            AccountDataGrid.ItemsSource = null;
            AccountDataGrid.Items.Clear();
            AccountDataGrid.ItemsSource = await _accountService.GetAllAccountsAsync(_currentPage, _pageSize);

            PageNumberTextBlock.Text = $"Trang {_currentPage} trên {_totalPages}";
            PreviousPageButton.IsEnabled = _currentPage > 1;
            NextPageButton.IsEnabled = _currentPage < _totalPages;
        }

        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.CurrentAccount = CurrentAccount;
            accountManagementWindow.ShowDialog();
            LoadDataGrid();

        }

        private void EditAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Account selected = AccountDataGrid.SelectedItem as Account;

            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn 1 tài khoản để chỉnh sửa!", "Vui lòng chọn tài khoản", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.CurrentAccount = CurrentAccount;
            accountManagementWindow.SelectedAccount = selected;
            accountManagementWindow.ShowDialog();
            LoadDataGrid();

        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            Account selected = AccountDataGrid.SelectedItem as Account;

            if (selected == null)
            {
                MessageBox.Show("Vui là chọn 1 tài khoản để xóa.", "Chọn tài khoản", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa tài khoản không", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                if (selected == CurrentAccount)
                {
                    MessageBox.Show("Không thể xóa tài khoản đang dùng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                try
                {
                    await _accountService.DeleteAccountAsync(selected.AccountId);
                    LoadDataGrid();
                    MessageBox.Show("Đã xóa tài khoản thành công", "Thành công", MessageBoxButton.OK);
                }
                catch
                {
                    MessageBox.Show($"Đã xảy ra lỗi khi xóa", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }
        }


        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadDataGrid();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                LoadDataGrid();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentAccount?.Role?.RoleName != "Admin")
            {
                EditAccountButton.Visibility = Visibility.Collapsed;
                AddAccountButton.Visibility = Visibility.Collapsed;
            }
            LoadDataGrid();
        }

        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }
    }
}
