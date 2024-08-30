using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows;
using System.Windows.Threading;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Core.Utils;

namespace WPF_NhaMayCaoSu
{
    public partial class CameraEditWindow : System.Windows.Window
    {
        private readonly ICameraService _cameraService;
        private VideoCapture _capture1;
        private VideoCapture _capture2;
        private DispatcherTimer _timer1;
        private DispatcherTimer _timer2;

        public CameraEditWindow(ICameraService cameraService)
        {
            InitializeComponent();
            _cameraService = cameraService;
            //LoadCameraUrls();
        }

        private async void LoadCameraUrls()
        {
            try
            {
                Camera camera = await _cameraService.GetCamera();

                if (camera == null)
                {
                    MessageBox.Show(Constants.ErrorMessageRetrieveCamera, Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                IpCamera1Box.Text = camera.Camera1 ?? string.Empty;
                IpCamera2Box.Text = camera.Camera2 ?? string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Constants.ErrorMessageLoadCameraUrls}: {ex.Message}", Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConnectCamera1Button_Click(object sender, RoutedEventArgs e)
        {
            string cameraUrl = IpCamera1Box.Text;
            _capture1?.Release();
            _capture1 = new VideoCapture(cameraUrl);

            if (!_capture1.IsOpened())
            {
                MessageBox.Show(Constants.ErrorMessageConnectCamera1, Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _timer1 = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            _timer1.Tick += Timer1_Tick;
            _timer1.Start();

            MessageBox.Show(Constants.SuccessMessageConnectCamera1);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (_capture1 != null && _capture1.IsOpened())
            {
                using Mat frame = new Mat();
                _capture1.Read(frame);
                if (!frame.Empty())
                {
                    Camera1Feed.Source = frame.ToBitmapSource();
                }
            }
        }

        private void ConnectCamera2Button_Click(object sender, RoutedEventArgs e)
        {
            string cameraUrl = IpCamera2Box.Text;
            _capture2?.Release();
            _capture2 = new VideoCapture(cameraUrl);

            if (!_capture2.IsOpened())
            {
                MessageBox.Show(Constants.ErrorMessageConnectCamera2, Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _timer2 = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            _timer2.Tick += Timer2_Tick;
            _timer2.Start();

            MessageBox.Show(Constants.SuccessMessageConnectCamera2);
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (_capture2 != null && _capture2.IsOpened())
            {
                using Mat frame = new Mat();
                _capture2.Read(frame);
                if (!frame.Empty())
                {
                    Camera2Feed.Source = frame.ToBitmapSource();
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Camera camera = await _cameraService.GetCamera();

                if (camera == null)
                {
                    MessageBox.Show(Constants.ErrorMessageRetrieveCamera, Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                camera.Camera1 = IpCamera1Box.Text;
                camera.Camera2 = IpCamera2Box.Text;

                await _cameraService.UpdateCamera(camera);

                MessageBox.Show(Constants.SuccessMessageSaveCameraUrls);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Constants.ErrorMessageSaveCameraUrls}: {ex.Message}", Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CaptureCamera1Button_Click(object sender, RoutedEventArgs e)
        {
            if (_capture1 != null && _capture1.IsOpened())
            {
                using Mat frame = new Mat();
                _capture1.Read(frame);
                if (!frame.Empty())
                {
                    CapturedCamera1Image.Source = frame.ToBitmapSource();
                    MessageBox.Show(Constants.SuccessMessageCaptureFrameCamera1);
                }
                else
                {
                    MessageBox.Show(Constants.ErrorMessageCaptureFrameCamera1);
                }
            }
        }

        private void CaptureCamera2Button_Click(object sender, RoutedEventArgs e)
        {
            if (_capture2 != null && _capture2.IsOpened())
            {
                using Mat frame = new Mat();
                _capture2.Read(frame);
                if (!frame.Empty())
                {
                    CapturedCamera2Image.Source = frame.ToBitmapSource();
                    MessageBox.Show(Constants.SuccessMessageCaptureFrameCamera2);
                }
                else
                {
                    MessageBox.Show(Constants.ErrorMessageCaptureFrameCamera2);
                }
            }
        }
    }
}
