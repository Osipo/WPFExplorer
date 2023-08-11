using DevExpress.Mvvm;
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

namespace WpfExplorer.Screens
{
    /// <summary>
    /// Логика взаимодействия для ConnectionFormWindow.xaml
    /// </summary>
    public partial class ConnectionFormWindow : Window
    {
        private string _connStr;
        public ConnectionFormWindow() : this(false)
        {
        }

        public ConnectionFormWindow(bool def)
        {
            InitializeComponent();
            if (def)
            {
                ServerName.Text = "192.168.0.12";
                Catalog.Text = "BESTPRICE_NEW";
                Login.Text = "lead_admin";
                Password.Text = "Wms$erver";
            }
        }

        public String ConnectionString { get { return _connStr; } }

      

        private void Button_Click_Ok(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ServerName.Text))
            {
                MessageBox.Show("Ошибка! Поле ServerName Обязательно!");
                return;
            }
            else if (string.IsNullOrWhiteSpace(Catalog.Text))
            {
                MessageBox.Show("Ошибка! Поле DbName Обязательно!");
                return;
            }
            else if (string.IsNullOrWhiteSpace(Login.Text))
            {
                MessageBox.Show("Ошибка! Поле User Обязательно!");
                return;
            }
            else if (string.IsNullOrWhiteSpace(Password.Text))
            {
                MessageBox.Show("Ошибка! Поле Password Обязательно!");
                return;
            }

            _connStr = "Data Source=" + ServerName.Text;
            if (!string.IsNullOrWhiteSpace(Port.Text)) //Append Port if needed.
                _connStr += ":" + Port.Text;
            _connStr += ";";


            _connStr +=
                       "Initial Catalog=" + Catalog.Text + "; "
                       + "User ID=" + Login.Text + "; "
                       + "Password=" + Password.Text;

            DialogResult = true;
        }

        private void Button_Click_Cancel(object sender, RoutedEventArgs e)
        {
            _connStr = null;
            DialogResult = false;
        }
    }
}
