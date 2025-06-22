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
using Ixxat.Vci4;
using Ixxat.Vci4.Bal;
using Ixxat.Vci4.Bal.Can;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;

namespace CAN_Sharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<CANmessageWrap> msgCollection;
        public App App1;

        public MainWindow()
        {
            InitializeComponent();
            msgCollection = new ObservableCollection<CANmessageWrap>();
            DG1.ItemsSource = msgCollection;
            DG1.DataContext = msgCollection;
            App1 = (App)Application.Current;
            //ret.S = 10;
            App1.uiContext = SynchronizationContext.Current;
            msgCollection.CollectionChanged += Sdfg1;
        }

        private void buttonCANinit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                App1.CAN_Init();
            }
            catch (VciException ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (App1.CAN_Inited)
            {
                buttonCANinit.Foreground = Brushes.Green;
                buttonCANstop.Foreground = Brushes.Red;
                tBlockCANinfo.Text = "CAN Device info\nHardware ID :\t" + App1.UniqueHW + "\nVCIID :\t\t" + App1.VCIID.ToString();
                tBlockCANinfo.Visibility = Visibility.Visible;
                labelCAN1status.Content = "CAN1";
                labelCAN2status.Content = "CAN2";
                if (App1.CANstatusColor[0] == 0)
                {
                    labelCAN1status.Foreground = Brushes.Red;
                }
                else if (App1.CANstatusColor[0] == 1)
                {
                    labelCAN1status.Foreground = Brushes.OrangeRed;
                }
                else if (App1.CANstatusColor[0] == 2)
                {
                    labelCAN1status.Foreground = Brushes.Green;
                }

                if (App1.CANstatusColor[1] == 0)
                {
                    labelCAN2status.Foreground = Brushes.Red;
                }
                else if (App1.CANstatusColor[1] == 1)
                {
                    labelCAN2status.Foreground = Brushes.OrangeRed;
                }
                else if (App1.CANstatusColor[1] == 2)
                {
                    labelCAN2status.Foreground = Brushes.Green;
                }
                labelCAN1status.Visibility = Visibility.Visible;
                labelCAN2status.Visibility = Visibility.Visible;
            }
            else
            {
                buttonCANinit.Foreground = Brushes.Black;
                buttonCANstop.Foreground = Brushes.Black;
                tBlockCANinfo.Visibility = Visibility.Hidden;
                labelCAN1status.Visibility = Visibility.Hidden;
                labelCAN2status.Visibility = Visibility.Hidden;
            }
        }

        public void msgAddToCol(object state)
        {
            CANmessageWrap msg = state as CANmessageWrap;
            msgCollection.Add(msg);
            if (msgCollection.Count > 200)
            {
                msgCollection.Remove(msgCollection.First());
            }
        }

        public void Sdfg1(object sender, NotifyCollectionChangedEventArgs e)
        {
            DG1.ScrollIntoView(msgCollection.Last());
        }

        private void buttonCANstop_Click(object sender, RoutedEventArgs e)
        {
            App1.CAN_Close();

            if (App1.CAN_Inited)
            {
                buttonCANinit.Foreground = Brushes.Green;
                buttonCANstop.Foreground = Brushes.Red;
                tBlockCANinfo.Visibility = Visibility.Visible;
                labelCAN1status.Visibility = Visibility.Visible;
                labelCAN2status.Visibility = Visibility.Visible;
            }
            else
            {
                buttonCANinit.Foreground = Brushes.Black;
                buttonCANstop.Foreground = Brushes.Black;
                tBlockCANinfo.Visibility = Visibility.Hidden;
                labelCAN1status.Visibility = Visibility.Hidden;
                labelCAN2status.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //FilterDefine ws = new FilterDefine();
            AuxWindow ws = new AuxWindow();
            ws.ShowDialog();
        }
    }
}
