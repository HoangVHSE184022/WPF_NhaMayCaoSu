﻿using System.Windows;
using System.Windows.Controls;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SaveBoardWindow : Window
    {
        public string SelectedBoardName { get; private set; }

        public SaveBoardWindow()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (BoardNameComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                SelectedBoardName = selectedItem.Content.ToString();
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một tên cho board.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}