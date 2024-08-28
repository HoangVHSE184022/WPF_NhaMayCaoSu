using System.Windows;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window
    {
        public CustomerListWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CustomerManagementWindow customerManagementWindow = new CustomerManagementWindow();
            customerManagementWindow.ShowDialog();
        }

        private void EditCustomerButton1_Click(object sender, RoutedEventArgs e)
        {
            CustomerManagementWindow customerManagementWindow = new CustomerManagementWindow();
            customerManagementWindow.ShowDialog();
        }
    }
}
