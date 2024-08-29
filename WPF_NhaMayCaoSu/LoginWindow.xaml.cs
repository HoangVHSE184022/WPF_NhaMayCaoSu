using System;
using System.Windows;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly IAccountService _accountService;

        /* public LoginWindow(IAccountService accountService)
         {
             _accountService = accountService;
             InitializeComponent();
         }*/

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;

            /*
            // Call the login service
            Account account = await _accountService.LoginAsync(username, password);

            if (account != null)
            {
                // Successful login
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                // Failed login
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }*/
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }


        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.Show();
        }
    }
}
