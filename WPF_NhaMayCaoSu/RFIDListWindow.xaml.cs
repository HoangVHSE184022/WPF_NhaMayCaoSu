﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_NhaMayCaoSu
{
    /// <summary>
    /// Interaction logic for RFIDListWindow.xaml
    /// </summary>
    public partial class RFIDListWindow : Window
    {
        public RFIDListWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddRFIDButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDManagementWindow rfidManagementWindow = new RFIDManagementWindow();
            rfidManagementWindow.ShowDialog();
        }

        private void EditRFIDButton_Click(object sender, RoutedEventArgs e)
        {
            RFIDManagementWindow rfidManagementWindow = new RFIDManagementWindow();
            rfidManagementWindow.ShowDialog();
        }
    }
}