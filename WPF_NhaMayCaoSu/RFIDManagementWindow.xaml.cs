using System.Windows;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for RFIDManagementWindow.xaml
    /// </summary>
    public partial class RFIDManagementWindow : Window
    {
        public RFIDManagementWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
