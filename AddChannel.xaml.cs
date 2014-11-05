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
    /// Interaction logic for AddChannel.xaml
    /// </summary>
    public partial class AddChannel : Window
    {
        public string url { get; set; }
        public AddChannel()
        {
            url = "";
            InitializeComponent();
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (input.Text == "")
                return;
            else
                url = input.Text;

            this.Close();
        }
    }
}
