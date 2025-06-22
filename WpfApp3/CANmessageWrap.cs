using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ixxat.Vci4;
using Ixxat.Vci4.Bal;
using Ixxat.Vci4.Bal.Can;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CAN_Sharp
{
    public class CANmessageWrap
    {
        private TimeSpan dateTime_lp;
        public Brush colorRow { get; set; }
        public CANmessageWrap(ICanMessage v, int canNo, TimeSpan timeSpan)
        {
            long ms, js;
            msg = v;
            dateTime_lp = timeSpan;
            dateTime_now = DateTime.Now;
            nCAN = canNo;
            canString = (canNo == 0) ? "CAN1" : "CAN2";
            if (msg.SelfReceptionRequest)
            {
                colorRow = (canNo == 0) ? Brushes.LightCyan : Brushes.LightGreen;
            }
            else
            {
                colorRow = (canNo == 0) ? Brushes.LightBlue : Brushes.Aquamarine;
            }
            js = dateTime_lp.Ticks - 10000 * (long)(dateTime_lp.Ticks / 10000);
            ms = (dateTime_lp.Ticks - 10000000 * (long)(dateTime_lp.Ticks / 10000000)) / 10000;
            timeString = dateTime_lp.Days.ToString("D2") + " " + dateTime_lp.Hours.ToString("D2") + ":" + dateTime_lp.Minutes.ToString("D2") + ":" + dateTime_lp.Seconds.ToString("D2") + "." + ms.ToString("D3") + " " + js.ToString("D4");
            js = dateTime_now.Ticks - 10000 * (long)(dateTime_now.Ticks / 10000);
            ms = (dateTime_now.Ticks - 10000000 * (long)(dateTime_now.Ticks / 10000000)) / 10000;
            timeString_now = dateTime_now.Year.ToString("D4") + "." + dateTime_now.Month.ToString("D2") + "." + dateTime_now.Day.ToString("D2") + " " + dateTime_now.Hour.ToString("D2") + ":" + dateTime_now.Minute.ToString("D2") + ":" + dateTime_now.Second.ToString("D2") + "." + ms.ToString("D3") + " " + js.ToString("D4");

            for(int i = 0; i < msg.DataLength; i++)
            {
                dataString += msg[i].ToString("X2") + " ";
            }

            idString = msg.Identifier.ToString("X8");
        }

        public ICanMessage msg { get; set; }
        public TimeSpan dateTime_l {
            get { return dateTime_l; }
            set
            {
                dateTime_lp = value;
                long js, ms;
                js = dateTime_lp.Ticks - 10000 * (long)(dateTime_lp.Ticks / 10000);
                ms = (dateTime_lp.Ticks - 10000000 * (long)(dateTime_lp.Ticks / 10000000)) / 10000;
                timeString = dateTime_lp.Days.ToString("D2") + " " + dateTime_lp.Hours.ToString("D2") + ":" + dateTime_lp.Minutes.ToString("D2") + ":" + dateTime_lp.Seconds.ToString("D2") + "." + ms.ToString("D3") + " " + js.ToString("D4");
            }
        }
        public DateTime dateTime_now { get; set; }
        public string timeString { get; set; }
        public string timeString_now { get; set; }
        public string dataString { get; set; }
        public string idString { get; set; }
        public string canString { get; set; }
        public int nCAN { get; set; }
    }
}
