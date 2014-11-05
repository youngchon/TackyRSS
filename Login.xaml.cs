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
using TackyRSS;


namespace TackyRSS
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public string username;
        public string password;
        public bool newuser = false;
        public Login()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (usernamebox.Text != "")
            {
                username = usernamebox.Text;
            }
            else
            {
                MessageBox.Show("Invalid Username");
                return;
            }
            if (passwordbox.Password == "")
            {
                MessageBox.Show("Invalid Password");
                return;
            }
            password = passwordbox.Password;
            this.Close();
        }

        private void register_Click(object sender, RoutedEventArgs e)
        {
            if (usernamebox.Text != "")
            {
                username = usernamebox.Text;
            }
            else
            {
                MessageBox.Show("Invalid Username");
                return;
            }
            if (passwordbox.Password == "")
            {
                MessageBox.Show("Invalid Password");
                return;
            }
            password = passwordbox.Password;
            newuser = true;
            this.Close();
        }


        
    }
}
