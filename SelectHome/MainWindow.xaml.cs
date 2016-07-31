using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using Microsoft.Win32;

namespace SelectHome
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnection connection;
        bool flag;
        public MainWindow()
        {
            InitializeComponent();
            string connectStr = "Server=localhost;Database=phome;Uid=root;Pwd=123456";
            connection = new MySqlConnection(connectStr);
            connection.Open();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string loginName = name.Text;
            string loginPsd = psd.Text;

            string sqlStr = String.Format("select count(*) from administrator where Name='{0}' and Psd='{1}';", loginName, loginPsd);
            MySqlCommand adminCmd = new MySqlCommand(sqlStr, connection);
            int ansAdmin = Int16.Parse(adminCmd.ExecuteScalar().ToString());

            string sqlStrUser = String.Format("select count(*) from user where Name='{0}' and Psd='{1}';", loginName, loginPsd);
            MySqlCommand userCmd = new MySqlCommand(sqlStrUser, connection);
            int ansUser = Int16.Parse(userCmd.ExecuteScalar().ToString());

            if (ansAdmin > 0)
            {
                flag = true;
                Login login = new Login(flag);
                login.Show();
                this.Close();
            }
            else if (ansUser > 0)
            {
                flag = false;
                Login login = new Login(flag);
                login.Show();
                this.Close();
            }
            else 
            {
                MessageBox.Show("用户不存在或密码错误！");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string loginName = name.Text;
            string loginPsd = psd.Text;

            if (loginName != "" && loginPsd!= "")
            {
                string sqlStr = String.Format("insert into user(Name,Psd)values('{0}','{1}');", loginName, loginPsd);
                MySqlCommand acCmd = new MySqlCommand(sqlStr, connection);
                acCmd.ExecuteNonQuery();
                MessageBox.Show("注册成功");
                name.Text = "";
                psd.Text = "";
            }
        }

    }

}
