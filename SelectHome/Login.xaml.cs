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
    public partial class Login : Window
    {
        System.Timers.Timer t;
        MySqlConnection connection;
        List<Record> recordList;
        Random random;
        bool flag;
        public Login(bool flag)
        {
            InitializeComponent();
            
            this.flag = flag;
            if (flag == false) 
            {
                start.Visibility = Visibility.Hidden;
                stop.Visibility = Visibility.Hidden;
                excel.Visibility = Visibility.Hidden;
            }
            
            t = new System.Timers.Timer(200);
            string connectStr = "Server=localhost;Database=phome;Uid=root;Pwd=123456";
            connection = new MySqlConnection(connectStr);
            connection.Open();

            recordList = new List<Record>();
            data.ItemsSource = recordList;

            random = new Random();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                t.Elapsed += new System.Timers.ElapsedEventHandler(flushHouse);
                t.Elapsed += new System.Timers.ElapsedEventHandler(flushPeople);
                t.AutoReset = true;
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void flushHouse(object source, System.Timers.ElapsedEventArgs e)
        {
            //string connectStr = "Server=localhost;Database=phome;Uid=root;Pwd=123456";
            //MySqlConnection connection = new MySqlConnection(connectStr);
            //connection.Open();
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM house";
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            int count = ds.Tables[0].Rows.Count;
            this.home.Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        lock (this) 
                        {
                            Thread.Sleep(15);
                            int i = random.Next(count);
                            home.Text = ds.Tables[0].Rows[i]["Id"].ToString();
                        }
                    }
                    )
                );
        }

        private void flushPeople(object source, System.Timers.ElapsedEventArgs e)
        {
            //string connectStr = "Server=localhost;Database=phome;Uid=root;Pwd=123456";
            //MySqlConnection connection = new MySqlConnection(connectStr);
            //connection.Open();
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM people";
            MySqlDataAdapter adap = new MySqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adap.Fill(ds);
            int count = ds.Tables[0].Rows.Count;
            this.home.Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        lock(this)
                        {
                            Thread.Sleep(15);
                            int i = random.Next(count);
                            people.Text = ds.Tables[0].Rows[i]["Name"].ToString();
                        }
                    }
                    )
                );
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            t.Close();
            Thread.Sleep(200);
            if (home.Text != "")
            {
                int id = int.Parse(home.Text);
                string name = people.Text;

                Record record = new Record(id, name);
                recordList.Add(record);
                data.Items.Refresh();

                string sqlHouseStr = String.Format("Delete from house where Id='{0}';", id);
                MySqlCommand minusHouseCmd = new MySqlCommand(sqlHouseStr, connection);
                minusHouseCmd.ExecuteNonQuery();

                string sqlPeopleStr = String.Format("Delete from people where Name='{0}';", name);
                MySqlCommand minusPeopleCmd = new MySqlCommand(sqlPeopleStr, connection);
                minusPeopleCmd.ExecuteNonQuery();
            }

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            HouseMySQL houseMysql = new HouseMySQL(flag);
            houseMysql.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            PeopleMySQL peopleMysql = new PeopleMySQL(flag);
            peopleMysql.Show();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ExportDataGrid(data);
        }

        public static void ExportDataGrid(DataGrid dGrid)
        {
            SaveFileDialog objSFD = new SaveFileDialog() { DefaultExt = "csv", Filter = "CSV Files (*.csv)|*.csv|Excel XML (*.xml)|*.xml|All files (*.*)|*.*", FilterIndex = 1 };
            if (objSFD.ShowDialog() == true)
            {
                string strFormat = objSFD.SafeFileName.Substring(objSFD.SafeFileName.IndexOf('.') + 1).ToUpper();
                StringBuilder strBuilder = new StringBuilder();
                if (dGrid.ItemsSource == null) return;
                List<string> lstFields = new List<string>();
                if (dGrid.HeadersVisibility == DataGridHeadersVisibility.Column || dGrid.HeadersVisibility == DataGridHeadersVisibility.All)
                {
                    foreach (DataGridColumn dgcol in dGrid.Columns)
                        lstFields.Add(FormatField(dgcol.Header.ToString(), strFormat));
                    BuildStringOfRow(strBuilder, lstFields, strFormat);
                }
                foreach (object data in dGrid.ItemsSource)
                {
                    lstFields.Clear();
                    foreach (DataGridColumn col in dGrid.Columns)
                    {
                        string strValue = "";
                        Binding objBinding = null;
                        if (col is DataGridBoundColumn)
                            objBinding = (Binding)(col as DataGridBoundColumn).Binding;
                        if (col is DataGridTemplateColumn)
                        {
                            //This is a template column... let us see the underlying dependency object
                            DependencyObject objDO = (col as DataGridTemplateColumn).CellTemplate.LoadContent();
                            FrameworkElement oFE = (FrameworkElement)objDO;
                            FieldInfo oFI = oFE.GetType().GetField("TextProperty");
                            if (oFI != null)
                            {
                                if (oFI.GetValue(null) != null)
                                {
                                    if (oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)) != null)
                                        objBinding = oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)).ParentBinding;
                                }
                            }
                        }
                        if (objBinding != null)
                        {
                            if (objBinding.Path.Path != "")
                            {
                                PropertyInfo pi = data.GetType().GetProperty(objBinding.Path.Path);
                                if (pi != null) strValue = pi.GetValue(data, null).ToString();
                            }
                            if (objBinding.Converter != null)
                            {
                                if (strValue != "")
                                    strValue = objBinding.Converter.Convert(strValue, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
                                else
                                    strValue = objBinding.Converter.Convert(data, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
                            }
                        }
                        lstFields.Add(FormatField(strValue, strFormat));
                    }
                    BuildStringOfRow(strBuilder, lstFields, strFormat);
                }
                StreamWriter sw = new StreamWriter(objSFD.OpenFile(), Encoding.GetEncoding("UTF-8"));
                if (strFormat == "XML")
                {
                    //Let us write the headers for the Excel XML
                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    sw.WriteLine("<?mso-application progid=\"Excel.Sheet\"?>");
                    sw.WriteLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                    sw.WriteLine("<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">");
                    sw.WriteLine("<Author>Arasu Elango</Author>");
                    sw.WriteLine("<Created>" + DateTime.Now.ToLocalTime().ToLongDateString() + "</Created>");
                    sw.WriteLine("<LastSaved>" + DateTime.Now.ToLocalTime().ToLongDateString() + "</LastSaved>");
                    sw.WriteLine("<Company>Atom8 IT Solutions (P) Ltd.,</Company>");
                    sw.WriteLine("<Version>12.00</Version>");
                    sw.WriteLine("</DocumentProperties>");
                    sw.WriteLine("<Worksheet ss:Name=\"Silverlight Export\" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                    sw.WriteLine("<Table>");
                }
                sw.Write(strBuilder.ToString());
                if (strFormat == "XML")
                {
                    sw.WriteLine("</Table>");
                    sw.WriteLine("</Worksheet>");
                    sw.WriteLine("</Workbook>");
                }
                sw.Close();
            }
        }
        private static void BuildStringOfRow(StringBuilder strBuilder, List<string> lstFields, string strFormat)
        {
            switch (strFormat)
            {
                case "XML":
                    strBuilder.AppendLine("<Row>");
                    strBuilder.AppendLine(String.Join("\r\n", lstFields.ToArray()));
                    strBuilder.AppendLine("</Row>");
                    break;
                case "CSV":
                    strBuilder.AppendLine(String.Join(",", lstFields.ToArray()));
                    break;
            }
        }
        private static string FormatField(string data, string format)
        {
            switch (format)
            {
                case "XML":
                    return String.Format("<Cell><Data ss:Type=\"String\">{0}</Data></Cell>", data);
                case "CSV":
                    return String.Format("\"{0}\"", data.Replace("\"", "\"\"\"").Replace("\n", "").Replace("\r", ""));
            }
            return data;
        }

    }

    public class Record : INotifyPropertyChanged
    {
        private int _id;
        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        private string _name;
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        public Record(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    public enum action
    {
        add,
        modify
    }
}
