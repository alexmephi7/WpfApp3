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

namespace CAN_Sharp
{
    /// <summary>
    /// Логика взаимодействия для SelectDevice.xaml
    /// </summary>
    public partial class SelectDevice : Window
    {
        public int selectedIndex;
        public SelectDevice()
        {
            InitializeComponent();
        }

        private void buttonSelect_Click(object sender, RoutedEventArgs e)
        {
            this.selectedIndex = this.lbSd.SelectedIndex;
            this.Close();
        }
    }
}
