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
    /// Interaction logic for AccessKeyWindow.xaml
    /// </summary>
    public partial class AccessKeyWindow : Window
    {
        public AccessKeyWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LicenseKeyLabel.Visibility = Visibility.Collapsed;
        }
    }
}
