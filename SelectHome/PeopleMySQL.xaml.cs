using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
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

using System.Collections;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Microsoft.Win32;

namespace SelectHome
{
    /// <summary>
    /// PeopleMySQL.xaml 的交互逻辑
    /// </summary>
    public partial class PeopleMySQL : Window
    {
        private MySqlConnection connection;
        private bool flag;
        public PeopleMySQL(bool flag)
        {
            InitializeComponent();

            this.flag = flag;
            dataGrid2.ContextMenu.IsEnabled = flag;
            if (!flag)
            {
                add.Visibility = Visibility.Hidden;
            }

            string connectStr = "Server=localhost;Database=phome;Uid=root;Pwd=123456";
            connection = new MySqlConnection(connectStr);
            connection.Open();

            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM people";
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);

            ds.Clear();
            DataTable tablePeople = new DataTable();
            adap.Fill(ds, "tablePeople");
            dataGrid2.DataContext = ds;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ItemPeople item = new ItemPeople(action.add);
            item.Show();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            ItemPeople item = new ItemPeople(action.add);
            item.Show();
        }

        private void delete_Click(object sender, RoutedEventArgs e) 
        {
            MessageBoxResult result = MessageBox.Show("确定删除该数据？","提示",MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var a = dataGrid2.SelectedItem as DataRowView;
                int id = (int)(a.Row.ItemArray[0]);
                string sqlStr = String.Format("Delete from people where Id='{0}';",id);
                MySqlCommand minusCmd = new MySqlCommand(sqlStr, connection);
                minusCmd.ExecuteNonQuery();

                a.Delete();
            }
        }

        private void modify_Click(object sender, RoutedEventArgs e)
        {
            var a = dataGrid2.SelectedItem as DataRowView;
            int id = (int)(a.Row.ItemArray[0]);

            ItemPeople item = new ItemPeople(action.modify, id);
            item.Show();
        }
    }
}
