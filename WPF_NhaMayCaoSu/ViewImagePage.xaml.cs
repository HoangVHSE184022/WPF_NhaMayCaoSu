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
        }

        public void LoadImage()
        {
            try
            {
                Uri uri;

                if (imageUrl.StartsWith("pack://application:"))
                {
                    uri = new Uri(imageUrl, UriKind.Absolute);
                }
                else if (System.IO.File.Exists(imageUrl))
                {
                    uri = new Uri(imageUrl, UriKind.Absolute);
                }
                else
                {
                    MessageBox.Show("Image not found at " + imageUrl, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.EndInit();

                ViewImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load image: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }

}
