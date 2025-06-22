using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ixxat.Vci4;
using Ixxat.Vci4.Bal;
using Ixxat.Vci4.Bal.Can;

namespace CAN_Sharp
{

    class CanDev
    {
        public long VciObjectId { get; }
        Guid DeviceClass { get; }
        Version DriverVersion { get; }
        Version HardwareVersion { get; }
        public object UniqueHardwareId { get; }
        string Description { get; }
        string Manufacturer { get; }
        IVciCtrlInfo[] Equipment { get; }

        IVciDevice Device;
        public IBalObject BALobject;
        private Dictionary<int, ICanSocket> canSockets;
        private Dictionary<int, ICanChannel> canChannels;
        private Dictionary<int, ICanControl> canControls;

        public CanDev()
        {
            BALobject = null;
            canSockets = new Dictionary<int, ICanSocket>();
            canChannels = new Dictionary<int, ICanChannel>();
            canControls = new Dictionary<int, ICanControl>();
            List<IVciDevice> hwId = new List<IVciDevice>();
            IVciDeviceManager deviceMananager = VciServer.Instance().DeviceManager;
            IVciDeviceList devices = deviceMananager.GetDeviceList();

            foreach (IVciDevice d in devices)
            {
                hwId.Add(d);
            }

            if (hwId.Count == 0)
            {
                throw new VciException("No Devices");
            }
            else if (hwId.Count == 1)
            {
                Device = hwId[0];
            }
            else
            {
                SelectDevice w = new SelectDevice();
                w.lbSd.Items.Add(hwId[0].UniqueHardwareId.ToString() + " - VCIID: " + hwId[0].VciObjectId.ToString());
                //w.lbSd.Items.GetItemAt(0);
                w.lbSd.SelectedIndex = 0;
                w.ShowDialog();
                Device = hwId[w.selectedIndex];
                foreach(var item in hwId)
                {
                    if (item != Device) { item.Dispose(); }
                }
            }

            VciObjectId = Device.VciObjectId;
            DeviceClass = Device.DeviceClass;
            HardwareVersion = Device.HardwareVersion;
            UniqueHardwareId = Device.UniqueHardwareId;
            Description = Device.Description;
            Manufacturer = Device.Manufacturer;
            Equipment = Device.Equipment;

            BALobject = Device.OpenBusAccessLayer();

            Device.Dispose();
            devices.Dispose();
            deviceMananager.Dispose();
        }

        public ICanSocket openCanSocket(int n)
        {
            if (n < Equipment.Length)
            {
                try
                {
                    ICanSocket cS;
                    cS = (ICanSocket)BALobject.OpenSocket((byte)n, typeof(ICanSocket));
                    canSockets.Add(n, cS);
                    return cS;
                }
                catch (VciException ex)
                {
                    MessageBox.Show(ex.Message + " CAN " + (n + 1).ToString() + " Cannot open socket");
                    return null;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Index = " + n.ToString() + ", max index value = " + Equipment.Length.ToString());
            }
        }

        public void closeCanSocket(int n)
        {
            canSockets[n].Dispose();
            canSockets.Remove(n);
        }

        public ICanChannel openCanChannel(int n, ushort rxFIFOsize, ushort txFIFOsize, bool exlusive)
        {
            ICanChannel cC;
            if (n < Equipment.Length)
            {
                try
                {
                    cC = (ICanChannel)BALobject.OpenSocket((byte)n, typeof(ICanChannel));
                    canChannels.Add(n, cC);
                }
                catch (VciException ex)
                {
                    MessageBox.Show(ex.Message + " CAN " + (n + 1).ToString() + " Cannot open channel");
                    return null;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Index = " + n.ToString() + ", max index value = " + Equipment.Length.ToString());
            }
            cC.Initialize(rxFIFOsize, txFIFOsize, exlusive);
            //cC.Activate();
            return canChannels[n];
        }

        public void closeCanChannel(int n)
        {
            canChannels[n].Dispose();
            canChannels.Remove(n);
        }

        public ICanControl openCanControl(int n)
        {
            ICanControl cC;
            if (n < Equipment.Length)
            {
                try
                {
                    cC = (ICanControl)BALobject.OpenSocket((byte)n, typeof(ICanControl));
                    canControls.Add(n, cC);
                }
                catch (VciException ex)
                {
                    MessageBox.Show(ex.Message + " CAN " + (n + 1).ToString() + " Control already in use");
                    return null;
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Index = " + n.ToString() + ", max index value = " + Equipment.Length.ToString());
            }
            cC.InitLine(CanOperatingModes.Standard | CanOperatingModes.Extended | CanOperatingModes.ErrFrame, CanBitrate.Cia1000KBit);
            //cC.StartLine();
            return canControls[n];
        }

        /*
        public void closeCanControl(int n)
        {
            canControls[n].Dispose();
            canControls.Remove(n);
        }
        */
        public void Close()
        {
            BALobject.Dispose();
        }
    }
}
