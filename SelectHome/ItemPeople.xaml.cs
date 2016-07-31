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
using System.Windows.Shapes;

using MySql.Data.MySqlClient;

namespace SelectHome
{
    /// <summary>
    /// ItemPeople.xaml 的交互逻辑
    /// </summary>
    public partial class ItemPeople : Window
    {
        private action ac;
        private int id;
        public ItemPeople(action ac, int id = 0)
        {
            InitializeComponent();
            this.ac = ac;
            this.id = id;
            //如果想要修改数据时，不要让用户修改Id，那么就在修改数据时隐藏Id栏的显示，加上下面的代码
            //if (ac == action.modify)
            //{
            //Id.Visibility = Visibility.Hidden;
            //IdLabel.Visibility = Visibility.Hidden;
            //}
            //else 
            //{
            //Id.Visibility = Visibility.Visible;
            //IdLabel.Visibility = Visibility.Visible;
            //}
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            string connectStr = "Server=localhost;Database=phome;Uid=root;Pwd=123456";
            MySqlConnection connection = new MySqlConnection(connectStr);
            connection.Open();

            string idNum = Id.Text;
            string cardId = CardId.Text;
            string name = NameText.Text;
            string age = Age.Text;
            string sex = Sex.Text;
            string tel = Tel.Text;

            if ((idNum != "") && (cardId != "") && (name != "") && (age != "") && (sex != "") && (tel != "")) 
            {
                string sqlStr = "";
                if (ac == action.add)
                {
                    sqlStr = String.Format("insert into people(Id,CardId,Name,Age,Sex,Tel)values({0},{1},{2},{3},{4},{5});", idNum, cardId, name, age, sex, tel);
                }
                else if (ac == action.modify)
                {
                    sqlStr = String.Format("update people set Id='{0}', CardId='{1}', Name='{2}', Age='{3}', Sex='{4}', Tel='{5}' where Id='{6}';", idNum, cardId, name, age, sex, tel, id);
                }
                MySqlCommand acCmd = new MySqlCommand(sqlStr, connection);
                acCmd.ExecuteNonQuery();

                this.Close();
            }
        }
    }
}
