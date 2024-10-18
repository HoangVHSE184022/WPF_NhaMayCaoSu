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
    /// Interaction logic for ViewImagePage.xaml
    /// </summary>
    public partial class ViewImagePage : Window
    {
        public string imageUrl;
        public ViewImagePage()
        {
            InitializeComponent();
            LoadImage();
        }
        public void LoadImage()
        {
            if (System.IO.File.Exists(imageUrl))
            {
                ViewImage.Source = new BitmapImage(new Uri(imageUrl, UriKind.RelativeOrAbsolute));
            }
        }
    }
}
