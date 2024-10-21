using System.Windows;
using System.Windows.Media.Imaging;

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
