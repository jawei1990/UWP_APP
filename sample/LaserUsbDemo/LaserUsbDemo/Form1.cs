using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace LaserUsbDemo
{
    public partial class Form1 : Form
    {
        Object[] comboBoxData = new object[] {
                            "COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9","COM10",
                            "COM11","COM12","COM13","COM14","COM15","COM16","COM17","COM18","COM19","COM20",
                            "COM21","COM22","COM23","COM24","COM25","COM26","COM27","COM28","COM29","COM30",
                            "COM31","COM32","COM33","COM34","COM35","COM36","COM37","COM38","COM39","COM40",
                            "COM41","COM42","COM43","COM44","COM45","COM46","COM47","COM48","COM49","COM50"};
        bool isDeviceConnected = false;
        string device_port = "";
        string device_port_num = "";
        List <ComPort> SerialPort = new List<ComPort>();
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(comboBoxData);
            getComPort();

            //string ttt = "DIST;19987;AMP;0022;TEMP;1325;VOLT;108";
            //string test = DecodeData(ttt);
        }

        private void getComPort()
        {
            SerialPort = GetSerialPort2();
            string pattern_name = "CH340"; 
            string pattern_port = "COM";

            Regex rgx_name = new Regex(pattern_name);
            Regex rgx_port = new Regex(pattern_port);

            for (int i = 0; i < SerialPort.Count; i++)
            {
                Match match_name = rgx_name.Match(SerialPort[i].description);
                Match match_port = rgx_port.Match(SerialPort[i].description);

                if (match_name.Success)
                {
                    char[] data = SerialPort[i].description.ToCharArray();

                    for (int j = match_port.Index+3; j < data.Length; j++)
                    {
                        if (!string.Equals(data[j].ToString(), ")"))
                            device_port_num += data[j].ToString();
                    }

                    device_port = "COM" + device_port_num;
                    Console.WriteLine("port:" + device_port);

                    // 從 0 開始算
                    comboBox1.SelectedIndex = Int16.Parse(device_port_num) -1 ; 
                    Console.WriteLine("---- found -----:" + SerialPort[i].description);
                }
            }
        }

        struct ComPort
        {
            public string name;
            public string description;

            public ComPort(string name, string description)
            {
                this.name = name;
                this.description = description;
            }
        }

        // Method 2 function
        private List<ComPort> GetSerialPort2()
        {
            Console.WriteLine("------- GetSerialPort2 -----");
            List<ComPort> ports = new List<ComPort>();
            ManagementClass processClass = new ManagementClass("Win32_PnPEntity");

            ManagementObjectCollection Ports = processClass.GetInstances();
           // string device = "No recognized";
            foreach (ManagementObject port in Ports)
            {
                if (port.GetPropertyValue("Name") != null)
                    if (port.GetPropertyValue("Name").ToString().Contains("USB") &&
                        port.GetPropertyValue("Name").ToString().Contains("COM"))
                    {
                        ComPort c = new ComPort();
                        c.name = port.GetPropertyValue("DeviceID").ToString();
                        //               Console.WriteLine("name:" + port.GetPropertyValue("DeviceID").ToString());
                        c.description = port.GetPropertyValue("Caption").ToString();
                        //                Console.WriteLine("description:" + port.GetPropertyValue("Caption").ToString());
                        ports.Add(c);
                    }
            }
            return ports;
        }

        private void CloseSerialPort()
        {
            try
            {
                serialPort1.Close();
                BtnDeviceStatus.Text = "Connect";
                Console.WriteLine("------- CloseSerialPort -----");
                isDeviceConnected = false;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //System.Environment.Exit(1);
            }
        }

        private void OpenSerialPort(string portName)
        {
            try
            {
                serialPort1.BaudRate = 256000;
                serialPort1.Parity = Parity.None;
                serialPort1.StopBits = StopBits.One;
                serialPort1.DataBits = 8;
                serialPort1.Handshake = Handshake.None;
                serialPort1.RtsEnable = true;
                serialPort1.PortName = portName;
                serialPort1.Open();
                BtnDeviceStatus.Text = "Disconnect";
                Console.WriteLine("------- OpenSerialPort -----");
                isDeviceConnected = true;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //System.Environment.Exit(1);
            }
        }

        private void BtnDeviceStatus_Click(object sender, EventArgs e)
        {
            if (isDeviceConnected)
            {
                CloseSerialPort();
            }
            else
            {
                OpenSerialPort(device_port);
            }  
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            if (!isDeviceConnected)
                getComPort();
            else 
            {
                comboBox1.SelectedIndex = Int16.Parse(device_port_num) - 1;
            }
        }

        
        string dis_ddd;
        private async void Uart_Rx_Handle(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            //Console.WriteLine("Data Received:" + indata);
            if (indata.Contains("DIST"))
            {
                switch (btnStatus)
                {
                    case 1:
                        {
                            String dis = DecodeData(indata);
                            dis_ddd = dis;
                            this.BeginInvoke(new InvokeDelegate(HandleSelection));
                            btnStatus = 3;
                        }
                        break;
                    case 2:
                        {
                            String dis = DecodeData(indata);
                            dis_ddd = dis;
                            this.BeginInvoke(new InvokeDelegate(HandleSelection));
                        }
                        break;
                }
            }
        }

        private delegate void InvokeDelegate();

        private void HandleSelection()
        {
            tv_dis.Text = string.Format(dis_ddd);
        }

        // return distance;
        String DecodeData(string indata)
        {
            String ret = "";

            String pattern = ";";
            String[] elements = Regex.Split(indata, pattern);

            Int32 data_int = Int32.Parse(elements[1]);
            ret = data_int.ToString();
           
            return ret;
        }

        int btnStatus = 0; // 0: None, 1: get one data, 2: more data, 3: stop
      //  byte[] getDis = new byte[] {0xAA,0x80,0x00,0x22,0xA2};
        private void BtnGetDis_Click(object sender, EventArgs e)
        {
            //      serialPort1.Write(getDis, 0, 5);
            btnStatus = 1;
        }

        private void BtnShots_Click(object sender, EventArgs e)
        {
            btnStatus = 2;
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            btnStatus = 3;
        }
    }
}
