using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Windows;
using System.Windows.Threading;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Repository.Repositories;
using WPF_NhaMayCaoSu.Service.Interfaces;

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
            LoadCameraUrls();
        }

        private async void LoadCameraUrls()
        {
            // Load camera URLs from repository
            Camera camera = await _cameraService.GetCamera();
            IpCamera1Box.Text = camera.Camera1 ?? string.Empty;
            IpCamera2Box.Text = camera.Camera2 ?? string.Empty;
        }

        private void ConnectCamera1Button_Click(object sender, RoutedEventArgs e)
        {
            string cameraUrl = IpCamera1Box.Text;
            _capture1?.Release();
            _capture1 = new VideoCapture(cameraUrl);

            if (!_capture1.IsOpened())
            {
                MessageBox.Show("Unable to connect to Camera 1. Please check the URL.");
                return;
            }

            _timer1 = new DispatcherTimer();
            _timer1.Interval = TimeSpan.FromMilliseconds(30);
            _timer1.Tick += Timer1_Tick;
            _timer1.Start();

            MessageBox.Show("Connected to Camera 1.");
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (_capture1 != null && _capture1.IsOpened())
            {
                Mat frame = new Mat();
                _capture1.Read(frame);
                if (!frame.Empty())
                {
                    Camera1Feed.Source = frame.ToBitmapSource();
                }
                frame.Dispose();
            }
        }

        private void ConnectCamera2Button_Click(object sender, RoutedEventArgs e)
        {
            string cameraUrl = IpCamera2Box.Text;
            _capture2?.Release();
            _capture2 = new VideoCapture(cameraUrl);

            if (!_capture2.IsOpened())
            {
                MessageBox.Show("Unable to connect to Camera 2. Please check the URL.");
                return;
            }

            _timer2 = new DispatcherTimer();
            _timer2.Interval = TimeSpan.FromMilliseconds(30);
            _timer2.Tick += Timer2_Tick;
            _timer2.Start();

            MessageBox.Show("Connected to Camera 2.");
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            if (_capture2 != null && _capture2.IsOpened())
            {
                Mat frame = new Mat();
                _capture2.Read(frame);
                if (!frame.Empty())
                {
                    Camera2Feed.Source = frame.ToBitmapSource();
                }
                frame.Dispose();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var camera = await _cameraService.GetCamera();
                camera.Camera1 = IpCamera1Box.Text;
                camera.Camera2 = IpCamera2Box.Text;

                _cameraService.UpdateCamera(camera);
                MessageBox.Show("Camera URLs saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving camera URLs: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer1?.Stop();
            _timer2?.Stop();
            _capture1?.Release();
            _capture2?.Release();
            base.OnClosed(e);
        }
    }
}
