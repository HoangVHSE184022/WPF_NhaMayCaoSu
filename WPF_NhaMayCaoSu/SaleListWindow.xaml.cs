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

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for SaleListWindow.xaml
    /// </summary>
    public partial class SaleListWindow : Window
    {
        public SaleListWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddSaleButton_Click(object sender, RoutedEventArgs e)
        {
            SaleManagementWindow saleManagementWindow = new SaleManagementWindow();
            saleManagementWindow.ShowDialog();

        }

        private void EditSaleButton_Click(object sender, RoutedEventArgs e)
        {
            SaleManagementWindow saleManagementWindow = new SaleManagementWindow();
            saleManagementWindow.ShowDialog();
        }
    }
}
