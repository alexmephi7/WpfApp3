using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using Ixxat.Vci4;
using Ixxat.Vci4.Bal;
using Ixxat.Vci4.Bal.Can;
using Microsoft.Win32;

namespace CAN_Sharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public bool CAN_Inited;
        CanDev CANDevice;

        ICanChannel can1Channel;
        ICanChannel can2Channel;
        ICanControl can1Control;
        ICanControl can2Control;

        ICanMessageReader[] CANreader;
        ICanMessageWriter[] CANwriter;
        bool[] ThreadRun;
        bool ThreadRunAssembly;

        Thread[] rxThread;
        Thread assemblyThread;

        public int[] CANstatusColor;

        public string UniqueHW;
        public long VCIID;

        public ICanMessage msg1;

        CAN_Sharp.MainWindow zMainWindow;

        Queue<CANmessageWrap>[] mwCol;
        public Queue<CANmessageWrap> mainQ;

        System.Threading.Mutex mutex;

        double dwTimeKoef;

        public int S { get; set; }

        System.Threading.Mutex[] singleThRxQueueMutex;
        public System.Threading.Mutex mainQThRxQueueMutex;

        TimeStampEval timeEval;
        public SynchronizationContext uiContext;

        class TimeStampEval
        {
            public uint uintTimeStamp;
            private double koef;
            private TimeSpan interval;
            private DateTime time;

            public TimeStampEval(uint dwTime, double k)
            {
                uintTimeStamp = dwTime;
                koef = k;
                interval = new TimeSpan(0);
                time = DateTime.Now;
            }

            public TimeSpan EvalTime(uint dwTime)
            {
                long set;
                double y;
                long n;
                y = 10000000d * koef;
                n = (long)((DateTime.Now - time).Ticks / (y * uint.MaxValue));
                if (dwTime >= uintTimeStamp)
                {
                    set = (long)(((dwTime - uintTimeStamp) * y) + (n * y * uint.MaxValue));
                    uintTimeStamp = dwTime;
                    TimeSpan temp = new TimeSpan(set);
                    interval += temp;
                }
                else
                {
                    set = (long)(((uintTimeStamp - dwTime) * y) + (n * y * uint.MaxValue));
                    uintTimeStamp = dwTime;
                    TimeSpan temp = new TimeSpan(set);
                    interval -= temp;
                }
                return interval;
            }
        }

        App()
        {
            CAN_Inited = false;
            rxThread = new Thread[2];
            CANreader = new ICanMessageReader[2];
            CANwriter = new ICanMessageWriter[2];
            ThreadRun = new bool[2] { false, false };
            ThreadRunAssembly = false;
            CANstatusColor = new int[2] { 0, 0 };
            rxThread[0] = null;
            rxThread[1] = null;
            assemblyThread = null;
            //zMainWindow.Z = 10;
            S = 111;
            mwCol = new Queue<CANmessageWrap>[2];
            mwCol[0] = new Queue<CANmessageWrap>();
            mwCol[1] = new Queue<CANmessageWrap>();
            mainQ = new Queue<CANmessageWrap>();
            singleThRxQueueMutex = new System.Threading.Mutex[2];
            singleThRxQueueMutex[0] = new System.Threading.Mutex(false);
            singleThRxQueueMutex[1] = new System.Threading.Mutex(false);
            mainQThRxQueueMutex = new System.Threading.Mutex(false);
            timeEval = null;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            bool createdNew;
            string mutName = "Приложение_809";
            mutex = new System.Threading.Mutex(true, mutName, out createdNew);
            if (!createdNew)
            {
                this.Shutdown();
            }
            //zMainWindow = (CAN_Sharp.MainWindow)MainWindow;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            if (CAN_Inited)
            {
                CAN_Close();
            }
        }

        public void CAN_Init()
        {
            zMainWindow = (CAN_Sharp.MainWindow)Current.MainWindow;
            if (CAN_Inited)
            {
                return;
            }
            else
            {
                try
                {
                    CANDevice = new CanDev();
                    UniqueHW = CANDevice.UniqueHardwareId.ToString();
                    VCIID = CANDevice.VciObjectId;
                    CAN_Inited = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "While initiating CAN device some errors has occur!");
                    return;
                }
            }

            if (CANDevice.BALobject.Resources.Count >= 1)
            {
                can1Channel = CANDevice.openCanChannel(0, 1024, 1024, false);
                if (can1Channel != null)
                {
                    can1Channel.Activate();
                    dwTimeKoef = (float)can1Channel.TimeStampCounterDivisor / can1Channel.ClockFrequency;
                    CANstatusColor[0] = 1;
                }
                can1Control = CANDevice.openCanControl(0);
                if (can1Control != null)
                {
                    can1Control.StartLine();
                    CANstatusColor[0] = 2;
                }
            }

            if (CANDevice.BALobject.Resources.Count >= 2)
            {
                can2Channel = CANDevice.openCanChannel(1, 1024, 1024, false);
                if (can2Channel != null)
                {
                    can2Channel.Activate();
                    if (dwTimeKoef != (float)can2Channel.TimeStampCounterDivisor / can2Channel.ClockFrequency)
                    {
                        throw new Exception("Strange dwTimeKoef");
                    }
                    CANstatusColor[1] = 1;
                }
                can2Control = CANDevice.openCanControl(1);
                if (can2Control != null)
                {
                    can2Control.StartLine();
                    CANstatusColor[1] = 2;
                }
            }

            try
            {
                CANreader[0] = can1Channel.GetMessageReader();
            }
            catch
            {
                MessageBox.Show("CAN1 reader error!");
                CANreader[0] = null;
            }

            try
            {
                CANreader[1] = can2Channel.GetMessageReader();
            }
            catch
            {
                MessageBox.Show("CAN2 reader error!");
                CANreader[1] = null;
            }

            try
            {
                CANwriter[0] = can1Channel.GetMessageWriter();
            }
            catch
            {
                MessageBox.Show("CAN1 writer error!");
                CANwriter[0] = null;
            }

            try
            {
                CANwriter[1] = can2Channel.GetMessageWriter();
            }
            catch
            {
                MessageBox.Show("CAN2 writer error!");
                CANwriter[1] = null;
            }

            createRxThread(0);
            createRxThread(1);
            createAssemblyThread();
        }

        public void CAN_Close()
        {
            stopRxThread(0);
            stopRxThread(1);
            stopAssemblyThread();
            waitRxThreadStop(0);
            waitRxThreadStop(1);
            waitAssemblyThreadStop();

            if (CANreader[0] != null)
            {
                CANreader[0].Dispose();
            }
            if (CANreader[1] != null)
            {
                CANreader[1].Dispose();
            }
            if (CANwriter[0] != null)
            {
                CANwriter[0].Dispose();
            }
            if (CANwriter[1] != null)
            {
                CANwriter[1].Dispose();
            }
            if (can1Control != null)
            {
                can1Control.ResetLine();
                if (can1Channel != null)
                {
                    can1Channel.Deactivate();
                    can1Channel.Dispose();
                    can1Channel = null;
                }
                can1Control.Dispose();
                can1Control = null;
            }

            if (can2Control != null)
            {
                can2Control.ResetLine();
                if (can2Channel != null)
                {
                    can2Channel.Deactivate();
                    can2Channel.Dispose();
                    can2Channel = null;
                }
                can2Control.Dispose();
                can2Control = null;
            }
            if (CANDevice != null)
            {
                CANDevice.BALobject.Dispose();
            }
            CAN_Inited = false;
        }

        public void createRxThread(int n)
        {
            if ((CANreader[n] != null) && (rxThread[n] == null))
            {
                rxThread[n] = new Thread(rxThreadProc);
                ThreadRun[n] = true;
                rxThread[n].Start(n);
            }
        }

        public void createAssemblyThread()
        {
            if (assemblyThread == null)
            {
                assemblyThread = new Thread(queueTimeOrderThread);
                ThreadRunAssembly = true;
                assemblyThread.Start();
            }
        }

        public void stopRxThread(int n)
        {
            ThreadRun[n] = false;
        }

        public void stopAssemblyThread()
        {
            ThreadRunAssembly = false;
        }

        public void waitRxThreadStop(int n)
        {
            while (rxThread[n] != null) ;
        }

        public void waitAssemblyThreadStop()
        {
            while (assemblyThread != null) ;
        }

        public void rxThreadProc(object data)
        {
            int n = (int)data;
            //CANreader[n].ReadMessage(out msg1);
            ICanMessage msg;
            while (ThreadRun[n])
            {
                while (CANreader[n].ReadMessage(out msg))
                {
                    TimeSpan temp = new TimeSpan(0);
                    singleThRxQueueMutex[n].WaitOne();
                    mwCol[n].Enqueue(new CANmessageWrap(msg, n, temp));
                    singleThRxQueueMutex[n].ReleaseMutex();
                }

                Thread.Yield();
            }
            rxThread[n] = null;
        }

        public void queueTimeOrderThread()
        {
            CANmessageWrap tMsg;
            List<CANmessageWrap> qt = new List<CANmessageWrap>();
            while (ThreadRunAssembly)
            {
                for (int l = 0; l < 2; l++)
                {
                    if (singleThRxQueueMutex[l].WaitOne())
                    {
                        if (mwCol[l].Count != 0)
                        {
                            while (mwCol[l].Count != 0)
                            {
                                try
                                {
                                    tMsg = mwCol[l].Dequeue();
                                    qt.Add(tMsg);
                                }
                                catch
                                {
                                    MessageBox.Show("Unexpected catch");
                                    break;
                                }
                            }
                        }
                        singleThRxQueueMutex[l].ReleaseMutex();
                    }
                }

                if (qt.Count != 0)
                {
                    uint min = uint.MaxValue;
                    CANmessageWrap u;
                    u = null;
                    foreach (CANmessageWrap b in qt)
                    {
                        if (min > b.msg.TimeStamp)
                        {
                            min = b.msg.TimeStamp;
                            u = b;
                        }
                    }

                    if (u != null)
                    {
                        if ((DateTime.Now.Ticks - u.dateTime_now.Ticks) > 10000)
                        {
                            TimeSpan temp;
                            if (timeEval == null)
                            {
                                timeEval = new TimeStampEval(u.msg.TimeStamp, dwTimeKoef);
                            }
                            temp = timeEval.EvalTime(u.msg.TimeStamp);
                            u.dateTime_l = temp;
                            if (mainQThRxQueueMutex.WaitOne())
                            {
                                mainQ.Enqueue(u);
                                uiContext.Send(zMainWindow.msgAddToCol, u);
                            }
                            mainQThRxQueueMutex.ReleaseMutex();
                            qt.Remove(u);
                            u = null;
                        }
                    }
                    // msgEvent.RaiseMsgUpdateEvent(this, "3");
                }
                Thread.Yield();
            }
            assemblyThread = null;
        }
    }
}
