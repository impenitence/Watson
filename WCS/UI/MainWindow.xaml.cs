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

namespace WCS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        FtpFileListener _ftpFileListener;
        public MainWindow()
        {
            InitializeComponent();
            _ftpFileListener = new FtpFileListener(@"C:\Users\ZZH\Desktop\新建文件夹");
        }

        private void ClickEvent(object sender, RoutedEventArgs e)
        {
            //验证功能权限
        }
    }
}
