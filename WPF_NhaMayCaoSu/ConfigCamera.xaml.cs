using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.IO;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    public partial class ConfigCamera : Window
    {
        private readonly CameraService _cameraService = new();

        public Account CurrentAccount { get; set; }

        public ConfigCamera()
        {
            InitializeComponent();
            LoadSavedUrlsAsync();
        }

        private async Task LoadSavedUrlsAsync()
        {
            var camera = await _cameraService.GetNewestCameraAsync();
            if (camera != null)
            {
                txtUrl1.Text = camera.Camera1;
                txtUrl2.Text = camera.Camera2;
            }
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
                            Image<Bgr, byte> image = frame.ToImage<Bgr, byte>();
                            Bitmap bitmap = image.ToBitmap();
                            imageControl.Source = ConvertBitmapToBitmapImage(bitmap);

                            string imagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), $"capture_{DateTimeOffset.UtcNow:yyyyMMdd_HHmmss}.png");
                            bitmap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);

                            MessageBox.Show($"Hình ảnh đã được lưu tại: {imagePath}");
                        }
                        else
                        {
                            MessageBox.Show("Không thể chụp ảnh từ luồng RTSP.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
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

        private async void SaveUrlCamera1_Click(object sender, RoutedEventArgs e)
        {
            var camera = await _cameraService.GetNewestCameraAsync();
            if (camera == null)
            {
                camera = new Camera
                {
                    CameraId = Guid.NewGuid(),
                    Camera1 = txtUrl1.Text,
                    Camera2 = "Chưa có URL",
                    Status = 1
                };
                await _cameraService.CreateCameraAsync(camera);
            }
            else
            {
                camera.Camera1 = txtUrl1.Text;
                await _cameraService.UpdateCameraAsync(camera);
            }

            MessageBox.Show("URL Camera 1 đã được lưu.");
        }

        private async void SaveUrlCamera2_Click(object sender, RoutedEventArgs e)
        {
            var camera = await _cameraService.GetNewestCameraAsync();
            if (camera == null)
            {
                camera = new Camera
                {
                    CameraId = Guid.NewGuid(),
                    Camera1 = "Chưa có URL",
                    Camera2 = txtUrl2.Text,
                    Status = 1
                };
                await _cameraService.CreateCameraAsync(camera);
            }
            else
            {
                camera.Camera2 = txtUrl2.Text;
                await _cameraService.UpdateCameraAsync(camera);
            }

            MessageBox.Show("URL Camera 2 đã được lưu.");
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
