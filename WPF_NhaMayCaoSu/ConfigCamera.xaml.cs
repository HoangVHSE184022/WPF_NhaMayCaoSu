using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.IO;

namespace WPF_NhaMayCaoSu
{
    public partial class ConfigCamera : Window
    {
        public ConfigCamera()
        {
            InitializeComponent();
        }

        private void btnCapture1_Click(object sender, RoutedEventArgs e)
        {
            CaptureAndDisplayImage(txtUrl1.Text, imgCamera1);
        }

        private void btnCapture2_Click(object sender, RoutedEventArgs e)
        {
            CaptureAndDisplayImage(txtUrl2.Text, imgCamera2);
        }

        private void CaptureAndDisplayImage(string rtspUrl, System.Windows.Controls.Image imageControl)
        {
            try
            {
                using (VideoCapture capture = new VideoCapture(rtspUrl))
                {
                    using (Mat frame = new Mat())
                    {
                        capture.Read(frame);

                        if (!frame.IsEmpty)
                        {
                            // Convert frame to Image<Bgr, byte>
                            Image<Bgr, byte> image = frame.ToImage<Bgr, byte>();

                            // Convert Image<Bgr, byte> to Bitmap
                            Bitmap bitmap = image.ToBitmap();

                            // Display the image in the Image control
                            imageControl.Source = ConvertBitmapToBitmapImage(bitmap);

                            // Save the image to disk
                            string imagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), $"capture_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                            bitmap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);

                            MessageBox.Show($"Image saved at: {imagePath}");
                        }
                        else
                        {
                            MessageBox.Show("Failed to capture image from the RTSP stream.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
