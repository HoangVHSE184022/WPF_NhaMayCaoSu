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
    /// Interaction logic for CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window
    {
        public CustomerListWindow()
        {
            InitializeComponent();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CustomerManagementWindow customerManagementWindow = new CustomerManagementWindow();
            customerManagementWindow.ShowDialog();
        }

        private void EditCustomerButton1_Click(object sender, RoutedEventArgs e)
        {
            CustomerManagementWindow customerManagementWindow = new CustomerManagementWindow();
            customerManagementWindow.ShowDialog();
        }
    }
}