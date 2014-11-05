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

namespace TackyRSS
{
    /// <summary>
    /// Interaction logic for updatefrequency.xaml
    /// </summary>
    public partial class updatefrequency : Window
    {
        public string time { get; set; }
        public updatefrequency()
        {
            time = "";
            InitializeComponent();
        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            time = input.Text;

            this.Close();
        }
    }
}
