using Bll;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// LogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LogWindow : Window
    {
        #region Fields

        string _userId;
        string _password;

        #endregion
        public LogWindow()
        {
            InitializeComponent();
            
        }

        public string HaveAuthority(User user)
        {
            return string.Empty;
        }

        private string GetUserInfo()
        {
            string errText = string.Empty;
            if ((_userId=Cbb_UserId.Text.Trim()).Length <= 0)
            {
                errText = "user id is empty";
            }
            else if ((_password = Tb_UserPwd.Text.Trim()).Length <= 0)
            {
                errText = "password is empty";
            }
            return errText;
        }

        /// <summary>
        /// 按键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickEvent(object sender, RoutedEventArgs e)
        {
            string errText = string.Empty;
            //验证用户名&密码
            if ((errText = GetUserInfo()).Length > 0)
            {
                MessageBox.Show(errText, "ERROR");
                return;
            }

            DataSet ds = DbInf.LogOnSystem(_userId, _password, ref errText);
            if (errText.Length > 0)
            {
                MessageBox.Show(errText, "ERROR");
                return;
            }

            User.CurrUser = new User(ds);

            //验证权限
            if ((errText = HaveAuthority(User.CurrUser)).Length>0)
            {
                MessageBox.Show(errText, "ERROR");
                System.Environment.Exit(0);
            }

            DialogResult = true;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
