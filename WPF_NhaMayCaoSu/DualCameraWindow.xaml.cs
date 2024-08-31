﻿using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows;
using System.Windows.Threading;
using WPF_NhaMayCaoSu.Service.Interfaces;
using WPF_NhaMayCaoSu.Core.Utils;

namespace WPF_NhaMayCaoSu
{
    public partial class DualCameraWindow : System.Windows.Window
    {
        private VideoCapture _capture1;
        private VideoCapture _capture2;
        private Mat _frame1;
        private Mat _frame2;
        private bool _isCapturing;
        private readonly ICameraService _cameraService;

        public DualCameraWindow(ICameraService cameraService)
        {
            InitializeComponent();
            _frame1 = new Mat();
            _frame2 = new Mat();
            _cameraService = cameraService;
            StartCameras();
        }

        private async void StartCameras()
        {
            if (_cameraService == null)
            {
                MessageBox.Show(Constants.ErrorMessageCameraServiceNotInitialized, Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Repository.Models.Camera camera = await _cameraService.GetNewestCamera();

            if (camera == null)
            {
                MessageBox.Show(Constants.ErrorMessageRetrieveNewestCamera, Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _isCapturing = false;
            _capture1?.Release();
            _capture2?.Release();

            _capture1 = new VideoCapture(camera.Camera1);
            _capture2 = new VideoCapture(camera.Camera2);

            if (!_capture1.IsOpened() || !_capture2.IsOpened())
            {
                MessageBox.Show(Constants.ErrorMessageConnectCameras, Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _isCapturing = true;

            DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_isCapturing && _capture1.IsOpened() && _capture2.IsOpened())
            {
                _capture1.Read(_frame1);
                _capture2.Read(_frame2);

                if (!_frame1.Empty())
                {
                    Camera1Image.Source = _frame1.ToBitmapSource();
                }

                if (!_frame2.Empty())
                {
                    Camera2Image.Source = _frame2.ToBitmapSource();
                }
            }
        }

        private void TakePhoto1Button_Click(object sender, RoutedEventArgs e)
        {
            if (_frame1 != null && !_frame1.Empty())
            {
                string filePath = $"Camera1_Capture_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                _frame1.SaveImage(filePath);
                MessageBox.Show(string.Format(Constants.SuccessMessagePhotoSaved, filePath), Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(Constants.ErrorMessageNoFrameAvailableCamera1, Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TakePhoto2Button_Click(object sender, RoutedEventArgs e)
        {
            if (_frame2 != null && !_frame2.Empty())
            {
                string filePath = $"Camera2_Capture_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                _frame2.SaveImage(filePath);
                MessageBox.Show(string.Format(Constants.SuccessMessagePhotoSaved, filePath), Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(Constants.ErrorMessageNoFrameAvailableCamera2, Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            CameraEditWindow editWindow = new CameraEditWindow(_cameraService);
            editWindow.ShowDialog();
            StartCameras(); // Restart cameras after editing URLs
        }

        protected override void OnClosed(EventArgs e)
        {
            _isCapturing = false;
            _capture1?.Release();
            _capture2?.Release();
            _frame1?.Dispose();
            _frame2?.Dispose();
            base.OnClosed(e);
        }
    }
}
