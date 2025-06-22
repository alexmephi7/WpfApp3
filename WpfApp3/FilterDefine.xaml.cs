using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
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
    /// Логика взаимодействия для FilterDefine.xaml
    /// </summary>
    public partial class FilterDefine : Window
    {
        private bool windowInited;
        /*enum DigitUpdateDirection
        {
            init = 0,
            noUpdate,
            idToProtocol,
            protocolToId
        }*/

        private class DigitString
        {
            //DigitUpdateDirection digitUpdateDirection;
            public DigitString(uint value)
            {
                //digitUpdateDirection = DigitUpdateDirection.init;
                IdValue = value & ((1 << 29) - 1);
            }

            private uint privIdValue;

            uint IdValue {
                set
                {
                    privIdValue = value & ((1 << 29) - 1);
                    Id = privIdValue.ToString("X8");
                    CmdData = ((privIdValue >> 28) & 1).ToString("X1");
                    SrcAddr = ((privIdValue >> 22) & ((1 << 6) - 1)).ToString("X2");
                    DstAddr = ((privIdValue >> 16) & ((1 << 6) - 1)).ToString("X2");
                    AttributeHigh = ((privIdValue >> 12) & ((1 << 4) - 1)).ToString("X1");
                    AttributeLow = ((privIdValue >> 8) & ((1 << 4) - 1)).ToString("X1");
                    Code = ((privIdValue >> 0) & ((1 << 8) - 1)).ToString("X2");
                }
                get
                {
                    return privIdValue;
                }
            }

            private string Id { get; set; }
            private string CmdData { get; set; }
            private string SrcAddr { get; set; }
            private string DstAddr { get; set; }
            private string AttributeHigh { get; set; }
            private string AttributeLow { get; set; }
            private string Code { get; set; }

            public string GetId() { return Id; }
            public uint GetIdUint() { return IdValue; }
            public string GetCmdData() { return CmdData; }
            public string GetSrcAddr() { return SrcAddr; }
            public string GetDstAddr() { return DstAddr; }
            public string GetAttributeHigh() { return AttributeHigh; }
            public string GetAttributeLow() { return AttributeLow; }
            public string GetCode() { return Code; }

            public void SetId(uint value)
            {
                IdValue = value;
            }

            public void SetCmdData (uint value)
            {
                IdValue = (privIdValue & ~(((uint)1 << 0) << 28)) | ((value & 1) << 28);
            }

            public void SetSrcAddr(uint value)
            {
                IdValue = (privIdValue & ~((((uint)1 << 6) - 1) << 22)) | ((value & ((1 << 6) - 1)) << 22);
            }

            public void SetDstAddr(uint value)
            {
                IdValue = (privIdValue & ~((((uint)1 << 6) - 1) << 16)) | ((value & ((1 << 6) - 1)) << 16);
            }

            public void SetAttributeHigh(uint value)
            {
                IdValue = (privIdValue & ~((((uint)1 << 4) - 1) << 12)) | ((value & ((1 << 4) - 1)) << 12);
            }

            public void SetAttributeLow(uint value)
            {
                IdValue = (privIdValue & ~((((uint)1 << 4) - 1) << 8)) | ((value & ((1 << 4) - 1)) << 8);
            }

            public void SetCode(uint value)
            {
                IdValue = (privIdValue & ~((((uint)1 << 8) - 1) << 0)) | ((value & ((1 << 8) - 1)) << 0);
            }

        }

        DigitString IdConversion;
        DigitString IdMaskConversion;

        public FilterDefine()
        {
            IdConversion = new DigitString(0x12345678);
            IdMaskConversion = new DigitString(0x87654321);
            windowInited = false;

            InitializeComponent();
            windowInited = true;

            idValue.Text = IdConversion.GetId();
            idCmdData.Text = IdConversion.GetCmdData();
            idSrcAddr.Text = IdConversion.GetSrcAddr();
            idDstAddr.Text = IdConversion.GetDstAddr();
            idAttrHigh.Text = IdConversion.GetAttributeHigh();
            idAttrLow.Text = IdConversion.GetAttributeLow();
            idCode.Text = IdConversion.GetCode();

            idMask.Text = IdMaskConversion.GetId();
            idCmdDataMask.Text = IdMaskConversion.GetCmdData();
            idSrcAddrMask.Text = IdMaskConversion.GetSrcAddr();
            idDstAddrMask.Text = IdMaskConversion.GetDstAddr();
            idAttrHighMask.Text = IdMaskConversion.GetAttributeHigh();
            idAttrLowMask.Text = IdMaskConversion.GetAttributeLow();
            idCodeMask.Text = IdMaskConversion.GetCode();
        }

        private void ValueUpdateById()
        {
            if (windowInited == true)
            {
                try
                {
                    idCmdData.Text = (idCmdData.IsFocused) ? idCmdData.Text : IdConversion.GetCmdData();
                    idSrcAddr.Text = (idSrcAddr.IsFocused) ? idSrcAddr.Text : IdConversion.GetSrcAddr();
                    idDstAddr.Text = (idDstAddr.IsFocused) ? idDstAddr.Text : IdConversion.GetDstAddr();
                    idAttrHigh.Text = (idAttrHigh.IsFocused) ? idAttrHigh.Text : IdConversion.GetAttributeHigh();
                    idAttrLow.Text = (idAttrLow.IsFocused) ? idAttrLow.Text : IdConversion.GetAttributeLow();
                    idCode.Text = (idCode.IsFocused) ? idCode.Text : IdConversion.GetCode();
                }
                catch
                {
                    MessageBox.Show("!");
                }
            }
        }

        private void MaskUpdateById()
        {
            if (windowInited == true)
            {
                try
                {
                    idCmdDataMask.Text = (idCmdDataMask.IsFocused) ? idCmdDataMask.Text : IdMaskConversion.GetCmdData();
                    idSrcAddrMask.Text = (idSrcAddrMask.IsFocused) ? idSrcAddrMask.Text : IdMaskConversion.GetSrcAddr();
                    idDstAddrMask.Text = (idDstAddrMask.IsFocused) ? idDstAddrMask.Text : IdMaskConversion.GetDstAddr();
                    idAttrHighMask.Text = (idAttrHighMask.IsFocused) ? idAttrHighMask.Text : IdMaskConversion.GetAttributeHigh();
                    idAttrLowMask.Text = (idAttrLowMask.IsFocused) ? idAttrLowMask.Text : IdMaskConversion.GetAttributeLow();
                    idCodeMask.Text = (idCodeMask.IsFocused) ? idCodeMask.Text : IdMaskConversion.GetCode();
                }
                catch
                {
                    MessageBox.Show("!");
                }
            }
        }

        private void idValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string g;
            TextBox a = sender as TextBox;
            if ((a.Name == "idCmdData") || (a.Name == "idCmdDataMask"))
            {
                g = "01";
            }
            else
            {
                g = "0123456789abcdefABCDEF";
            }
            bool r = true;
            foreach (char b in g.ToCharArray())
            {
                if (b.ToString() == e.Text)
                {
                    r = false;
                }
            }
            e.Handled = r;
        }

        private void idValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void idValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool r;
            TextBox a = sender as TextBox;
            string g;
            if ((a.Name == "idCmdData") || (a.Name == "idCmdDataMask"))
            {
                g = "01";
            }
            else
            {
                g = "0123456789abcdefABCDEF";
            }
            string h = a.Text;
            int s = a.SelectionStart;
            foreach (char z in h)
            {
                r = false;
                foreach (char b in g.ToCharArray())
                {
                    if (b == z)
                    {
                        r = true;
                    }
                }
                if (r == false)
                {
                    while (h.IndexOf(z) != -1)
                    {
                        h = h.Remove(h.IndexOf(z), 1);
                    }
                }
            }

            //h = (h == "") ? "0" : h;
            a.Text = h;
            a.SelectionStart = s;

            bool i;
            uint zs;
            if (h == "")
            {
                i = true;
                zs = 0;
            }
            else
            {
                i = uint.TryParse(h, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out zs);
            }
            if (i)
            {
                if (windowInited == true)
                {
                    if (a.Name == "idCmdData")
                    {
                        IdConversion.SetCmdData(zs);
                        ValueUpdateById();
                    }
                    else if (a.Name == "idSrcAddr")
                    {
                        IdConversion.SetSrcAddr(zs);
                        ValueUpdateById();
                    }
                    else if (a.Name == "idDstAddr")
                    {
                        IdConversion.SetDstAddr(zs);
                        ValueUpdateById();
                    }
                    else if (a.Name == "idAttrHigh")
                    {
                        IdConversion.SetAttributeHigh(zs);
                        ValueUpdateById();
                    }
                    else if (a.Name == "idAttrLow")
                    {
                        IdConversion.SetAttributeLow(zs);
                        ValueUpdateById();
                    }
                    else if (a.Name == "idCode")
                    {
                        IdConversion.SetCode(zs);
                        ValueUpdateById();
                    }
                    else if (a.Name == "idCmdDataMask")
                    {
                        IdMaskConversion.SetCmdData(zs);
                        MaskUpdateById();
                    }
                    else if (a.Name == "idSrcAddrMask")
                    {
                        IdMaskConversion.SetSrcAddr(zs);
                        MaskUpdateById();
                    }
                    else if (a.Name == "idDstAddrMask")
                    {
                        IdMaskConversion.SetDstAddr(zs);
                        MaskUpdateById();
                    }
                    else if (a.Name == "idAttrHighMask")
                    {
                        IdMaskConversion.SetAttributeHigh(zs);
                        MaskUpdateById();
                    }
                    else if (a.Name == "idAttrLowMask")
                    {
                        IdMaskConversion.SetAttributeLow(zs);
                        MaskUpdateById();
                    }
                    else if (a.Name == "idCodeMask")
                    {
                        IdMaskConversion.SetCode(zs);
                        MaskUpdateById();
                    }

                    if (a.Name == "idValue")
                    {
                        IdConversion.SetId(zs);
                        ValueUpdateById();
                    }

                    if (a.Name == "idMask")
                    {
                        IdMaskConversion.SetId(zs);
                        MaskUpdateById();
                    }

                    try
                    {
                        i = uint.TryParse(idValue.Text, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out zs);
                        if (i)
                        {
                            if (zs != IdConversion.GetIdUint())
                            {
                                idValue.Text = IdConversion.GetIdUint().ToString("X8");
                            }
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        i = uint.TryParse(idMask.Text, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out zs);
                        if (i)
                        {
                            if (zs != IdMaskConversion.GetIdUint())
                            {
                                idMask.Text = IdMaskConversion.GetIdUint().ToString("X8");
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                MessageBox.Show("error");
            }

            if (windowInited == true)
            {
                string res;
                string str = Convert.ToString(IdConversion.GetIdUint(), 2);
                str = str.PadLeft(29, '0');
                res = str.Substring(0, 1) + " ";
                res += str.Substring(1, 6) + " ";
                res += str.Substring(7, 6) + " ";
                res += str.Substring(13, 4) + " ";
                res += str.Substring(17, 4) + " ";
                res += str.Substring(21, 4) + " ";
                res += str.Substring(25, 4);
                bitValue.Content = res;

                str = Convert.ToString(IdMaskConversion.GetIdUint(), 2);
                str = str.PadLeft(29, '0');
                res = str.Substring(0, 1) + " ";
                res += str.Substring(1, 6) + " ";
                res += str.Substring(7, 6) + " ";
                res += str.Substring(13, 4) + " ";
                res += str.Substring(17, 4) + " ";
                res += str.Substring(21, 4) + " ";
                res += str.Substring(25, 4);
                bitMask.Content = res;
            }

            e.Handled = true;
        }

        private void idValue_TextChanged1(object sender, RoutedEventArgs e)
        {
            ValueUpdateById();
            MaskUpdateById();
            //MessageBox.Show("test");
        }
    }
}
