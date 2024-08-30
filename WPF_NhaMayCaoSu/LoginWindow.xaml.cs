using System.Windows;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly AccountService _accountService = new();

         public LoginWindow()
         {
             InitializeComponent();
         }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;


            // Call the login service
            try
            {
                Account account = await _accountService.LoginAsync(username, password);
                if (account == null)
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            
            //MainWindow mainWindow = new MainWindow();
            //mainWindow.Show();
            //this.Close();
        }


        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementWindow accountManagementWindow = new AccountManagementWindow();
            accountManagementWindow.Show();
            Close();
        }
    }
}
