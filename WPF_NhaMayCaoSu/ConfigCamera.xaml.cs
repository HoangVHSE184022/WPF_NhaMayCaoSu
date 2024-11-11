using Emgu.CV;
using Emgu.CV.Structure;
using Serilog;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WPF_NhaMayCaoSu.Core.Utils;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Service.Services;

namespace WPF_NhaMayCaoSu
{
    public partial class ConfigCamera : Window
    {
        private readonly IConfigService _cameraService = new ConfigService();

        public Account CurrentAccount { get; set; }

        public ConfigCamera()
        {
            InitializeComponent();
            LoggingHelper.ConfigureLogger();
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

        private async void btnCapture1_Click(object sender, RoutedEventArgs e)
        {
            await CaptureAndDisplayImageAsync(txtUrl1.Text, imgCamera1);
        }

        private async void btnCapture2_Click(object sender, RoutedEventArgs e)
        {
            await CaptureAndDisplayImageAsync(txtUrl2.Text, imgCamera1);
        }

        private async Task CaptureAndDisplayImageAsync(string rtspUrl, System.Windows.Controls.Image imageControl)
        {
            string imagePath = string.Empty;
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // 10-second timeout
            var token = cts.Token; // Capture the token to avoid accessing cts after disposal

            btnCapture1.IsEnabled = false;
            btnCapture2.IsEnabled = false;

            try
            {
                var captureTask = Task.Run(() =>
                {
                    if (token.IsCancellationRequested) return; // Check cancellation before starting

                    using (var capture = new VideoCapture(rtspUrl))
                    {
                        if (token.IsCancellationRequested) return;

                        if (!capture.IsOpened)
                        {
                            if (token.IsCancellationRequested) return;
                            throw new Exception("Cannot connect to the camera. Check RTSP URL.");
                        }

                        using (var frame = new Mat())
                        {
                            capture.Read(frame);

                            if (token.IsCancellationRequested) return;

                            if (!frame.IsEmpty)
                            {
                                var image = frame.ToImage<Bgr, byte>();
                                var bitmap = image.ToBitmap();

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    if (!token.IsCancellationRequested)
                                    {
                                        imageControl.Source = ConvertBitmapToBitmapImage(bitmap);
                                    }
                                });

                                if (!token.IsCancellationRequested)
                                {
                                    imagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), $"capture_{DateTime.UtcNow:yyyyMMdd_HHmmss}.png");
                                    bitmap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                            else
                            {
                                throw new Exception("Cannot capture image from RTSP stream. Frame is empty.");
                            }
                        }
                    }
                }, token);

                if (await Task.WhenAny(captureTask, Task.Delay(10000)) == captureTask)
                {
                    await captureTask; // Await completion or propagate exception if needed
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        MessageBox.Show($"Image saved at: {imagePath}");
                    }
                }
                else
                {
                    // Timeout exceeded
                    MessageBox.Show("Image capture timed out.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"Error capturing image: {ex}");
            }
            finally
            {
                btnCapture1.IsEnabled = true;
                btnCapture2.IsEnabled = true;
                cts.Dispose(); // Ensure cts is disposed only at the end of all operations
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
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn lưu url này không", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            var camera = await _cameraService.GetNewestCameraAsync();
            if (camera == null)
            {
                camera = new Config
                {
                    CameraId = Guid.NewGuid(),
                    Camera1 = txtUrl1.Text,
                    Camera2 = "Chưa có Link",
                    Status = 1
                };
                await _cameraService.CreateCameraAsync(camera);
            }
            else
            {
                camera.Camera1 = txtUrl1.Text;
                await _cameraService.UpdateCameraAsync(camera);
            }

            MessageBox.Show("Link Camera 1 đã được lưu.");
        }

        private async void SaveUrlCamera2_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn lưu url này không", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            var camera = await _cameraService.GetNewestCameraAsync();
            if (camera == null)
            {
                camera = new Config
                {
                    CameraId = Guid.NewGuid(),
                    Camera1 = "Chưa có Link",
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

            MessageBox.Show("Link Camera 2 đã được lưu.");
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadSavedUrlsAsync();

        }
        public void OnWindowLoaded()
        {
            Window_Loaded(this, null);
        }
    }
}
