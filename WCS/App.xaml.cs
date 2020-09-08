using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WCS
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;//设置窗体居中显示
            mainWindow.Topmost = true;

            LogWindow logWindow = new LogWindow();
            logWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;//设置窗体居中显示
            logWindow.Topmost = true;

            if (logWindow.ShowDialog().Value)
                mainWindow.Show();
        }
    }
}
