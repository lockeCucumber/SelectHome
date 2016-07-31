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
    /// Item.xaml 的交互逻辑
    /// </summary>
    public partial class ItemHouse : Window
    {
        private action ac;
        private int id;
        public ItemHouse(action ac, int id = 0)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            string connectStr = "Server=localhost;Database=phome;Uid=root;Pwd=123456";
            MySqlConnection connection = new MySqlConnection(connectStr);
            connection.Open();

            string idNum = Id.Text;
            string adress = Adress.Text;
            string area = Area.Text;
            string pic = Pic.Text;

            if ((idNum != "") && (adress != "") && (area != "") && (pic != ""))
            {
                string sqlStr = "";
                if (ac == action.add)
                {
                    sqlStr = String.Format("insert into house(Id,Adress,Area,Pic)values({0},{1},{2},{3});", idNum, adress, area, pic);
                }
                else if (ac == action.modify)
                {
                    sqlStr = String.Format("update house set Id='{4}', Adress='{0}', Area='{1}', Pic='{2}' where Id='{3}';", adress, area, pic, id, idNum);
                }
                MySqlCommand acCmd = new MySqlCommand(sqlStr, connection);
                acCmd.ExecuteNonQuery();

                this.Close();
            }
        }
    }
}
