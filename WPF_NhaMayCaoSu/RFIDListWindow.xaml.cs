using System.Windows;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for RFIDListWindow.xaml
    /// </summary>
    public partial class RFIDListWindow : Window
    {
        public RFIDListWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddRFIDButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDManagementWindow rfidManagementWindow = new RFIDManagementWindow();
            rfidManagementWindow.ShowDialog();
        }

        private void EditRFIDButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDManagementWindow rfidManagementWindow = new RFIDManagementWindow();
            rfidManagementWindow.ShowDialog();
        }
    }
}
