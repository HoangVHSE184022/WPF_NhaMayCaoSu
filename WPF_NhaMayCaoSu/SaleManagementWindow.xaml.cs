﻿using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WPF_NhaMayCaoSu.Repository.Models;
using WPF_NhaMayCaoSu.Service.Services;
using WPF_NhaMayCaoSu.Core.Utils;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for SaleManagementWindow.xaml
    /// </summary>
    public partial class SaleManagementWindow : System.Windows.Window
    {

        private SaleService _service = new();

        public Sale SelectedSale { get; set; } = null;

        public SaleManagementWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Sale x = new();

            x.ProductWeight = Double.Parse(WeightTextBox.Text);
            x.WeightImageUrl = URLWeightTextBox.Text;
            x.Status = short.Parse(StatusTextBox.Text);
            x.RFIDCode = long.Parse(RFIDCodeTextBox.Text);

            CameraService cameraService = new CameraService();
            Camera newestCamera = await cameraService.GetNewestCamera();
            string imageFilePath = string.Empty;


            if (SelectedSale == null)
            {
                imageFilePath = await CaptureImageFromCameraAsync(newestCamera, cameraIndex: 1);

                // Save image to Firebase and set WeightImageUrl
                if (!string.IsNullOrEmpty(imageFilePath))
                {
                    FirebaseService firebaseService = new();
                    string firebaseFileName = Path.GetFileName(imageFilePath);
                    x.WeightImageUrl = await firebaseService.SaveImagePathToDatabaseAsync(imageFilePath, firebaseFileName);
                }

                x.CreatedDate = DateTime.Now;
                x.IsEdited = false;
                x.LastEditedTime = null;
                x.ProductDensity = null;
                x.DensityImageUrl = null;
                MessageBox.Show(Constants.SuccessMessageSaleCreated, Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                await _service.CreateSaleAsync(x);
            }
            else
            {
                imageFilePath = await CaptureImageFromCameraAsync(newestCamera, cameraIndex: 2);

                // Save image to Firebase and set DensityImageUrl
                if (!string.IsNullOrEmpty(imageFilePath))
                {
                    FirebaseService firebaseService = new();
                    string firebaseFileName = Path.GetFileName(imageFilePath);
                    x.DensityImageUrl = await firebaseService.SaveImagePathToDatabaseAsync(imageFilePath, firebaseFileName);
                }

                x.ProductDensity = Double.Parse(DensityTextBox.Text);
                x.DensityImageUrl = URLDensityTextBox.Text;
                x.SaleId = SelectedSale.SaleId;
                x.CreatedDate = SelectedSale.CreatedDate;
                x.IsEdited = true;
                x.LastEditedTime = DateTime.Now;
                MessageBox.Show(Constants.SuccessMessageSaleUpdated, Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                await _service.UpdateSaleAsync(x);
            }

            Close();
        }


        private async Task<string> CaptureImageFromCameraAsync(Camera camera, int cameraIndex)
        {
            string localFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");

            try
            {
                string cameraUrl = cameraIndex == 1 ? camera.Camera1 : camera.Camera2;
                if (!string.IsNullOrEmpty(cameraUrl))
                {
                    using (var capture = new VideoCapture(cameraUrl))
                    {
                        if (capture.IsOpened())
                        {
                            using (var frame = new Mat())
                            {
                                capture.Read(frame);
                                if (!frame.Empty())
                                {
                                    BitmapSource bitmapSource = frame.ToBitmapSource();
                                    using (var stream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                                    {
                                        BitmapEncoder encoder = new JpegBitmapEncoder();
                                        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                                        encoder.Save(stream);
                                    }
                                    MessageBox.Show(string.Format(Constants.SuccessMessageCapturedFrame, cameraIndex), Constants.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show(string.Format(Constants.ErrorMessageCaptureFrameFailed, cameraIndex), Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(string.Format(Constants.ErrorMessageOpenCameraFailed, cameraIndex), Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    throw new Exception(string.Format(Constants.ErrorMessageInvalidCameraUrl, cameraIndex));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Constants.ErrorMessageCaptureImage, cameraIndex, ex.Message), Constants.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }

            return localFilePath;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModeLabel.Content = Constants.ModeLabelAddSale; ;

            if (SelectedSale != null)
            {
                RFIDCodeTextBox.Text = SelectedSale.RFIDCode.ToString();
                WeightTextBox.Text = SelectedSale.ProductWeight.ToString();
                if (SelectedSale.WeightImageUrl != null)
                {
                    URLWeightTextBox.Text = SelectedSale.WeightImageUrl.ToString();
                }
                DensityTextBox.Text = SelectedSale.ProductDensity.ToString();
                if (SelectedSale.DensityImageUrl != null)
                {
                    URLDensityTextBox.Text = SelectedSale.DensityImageUrl.ToString();
                }

                StatusTextBox.Text = SelectedSale.Status.ToString();
                ModeLabel.Content = Constants.ModeLabelEditSale;
            }
        }
    }
}

