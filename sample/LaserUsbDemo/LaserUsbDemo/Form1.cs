﻿using System;
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
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

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

            tv_amp.Visible = false;
            tv_amp_t.Visible = false;
            tv_temp.Visible = false;
            tv_temp_t.Visible = false;

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

        string FILENAME_XLS;
        Excel.Application excelApp;
        Excel.Workbook excelWorkbook;
        Excel.Worksheet excelWorksheet;
        public void CreateExcelFilePath()
        {
            Console.WriteLine("建Excel");
            string DIRNAME = Application.StartupPath + @"\Log\";
            if (!Directory.Exists(DIRNAME))
                Directory.CreateDirectory(DIRNAME);

            FILENAME_XLS = DIRNAME + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls";
            excelApp = new Excel.Application();
            WriteExcelTitle();
        }

        private void CloseExcelFile()
        {
            Console.WriteLine("關閉 Excel");
            if (excelWorkbook != null)
            {
                excelWorkbook.Close();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelWorksheet);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelWorkbook);
            }

            if (excelApp != null)
            {
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelApp);

            }
            GC.Collect();
            GC.WaitForPendingFinalizers();

            excelWorkbook = null;
            excelApp = null;
        }

        public void WriteDataToFile(int cnt, String dist, string tmpture, string volt)
        {
            int col = cnt + 1;
            excelWorksheet.Cells[col, 1] = cnt.ToString();
            excelWorksheet.Cells[col, 2] = dist;
            excelWorksheet.Cells[col, 3] = tmpture;
            excelWorksheet.Cells[col, 4] = volt;
            excelApp.ActiveWorkbook.Save();
        }

        public void WriteExcelTitle()
        {
            if (excelApp != null)
            {
                excelWorkbook = excelApp.Workbooks.Add();
                excelWorksheet = (Excel.Worksheet)excelWorkbook.Sheets.Add();

                excelWorksheet.Cells[1, 1] = "次數:";
                excelWorksheet.Cells[1, 2] = "Dist";
                excelWorksheet.Cells[1, 3] = "Temp:";
                excelWorksheet.Cells[1, 4] = "Vb:";

                excelApp.ActiveWorkbook.SaveAs(FILENAME_XLS, Excel.XlFileFormat.xlWorkbookNormal);
            }
        }

        private void BtnDeviceStatus_Click(object sender, EventArgs e)
        {
            if (isDeviceConnected)
            {
                CloseSerialPort();
                CloseExcelFile();
            }
            else
            {
                if (string.IsNullOrEmpty(device_port))
                {
                    Console.WriteLine("device not found:" + comboBox1.Text.ToString());
                    OpenSerialPort(comboBox1.Text);
                }
                else 
                {
                    OpenSerialPort(device_port);
                    CreateExcelFilePath();
                }                
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

        private string hexToString(byte[] data, int len)
        {

            string str = BitConverter.ToString(data);

            Console.WriteLine(str);
            String[] tempAry = str.Split('-');
            str = "";
            for (int i = 0; i < len; i++)
            {
                str += tempAry[i];
            }

            return str;
        }

        private const int CNT_MAX = 1000;
        private void Uart_Rx_Handle(object sender, SerialDataReceivedEventArgs e)
        {

     /*       SerialPort sp = (SerialPort)sender;
            byte[] rx_data = new byte[10];
            int len = sp.Read(rx_data,0,10);

            string data = hexToString(rx_data,len);
            Console.WriteLine(data);

            if (data.Contains("CD000005"))
            {
                string str_dis = rx_data[4].ToString() + rx_data[5].ToString() +
                    rx_data[6].ToString() + rx_data[7].ToString();
                text_dis = str_dis.ToString();
                this.BeginInvoke(new InvokeDelegate(HandleSelection));
            }*/

            //Console.WriteLine("Data Received:" + indata);
            
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.WriteLine("Data Received:" + indata);

            if (indata.Contains("DIST"))
            {
                String dist = "";
                String vol = "";
                String temp = "";
                String pattern = ":";
                String[] elements = Regex.Split(indata, pattern);
                Console.WriteLine("len:" + elements.Length);
                dist = elements[1];
                Console.WriteLine("dist:" + dist);
                temp = elements[2];
                Console.WriteLine("temp:" + temp);
                vol = elements[3];
                Console.WriteLine("vol:" + vol);
                WriteDataToFile(cnt,dist,temp,vol);
            }

            if (cnt <= CNT_MAX)
            {
                Console.WriteLine("cnt:" + cnt);
                cnt++;
                testFunc();
            }
        }

        private delegate void InvokeDelegate();

        private void HandleSelection()
        {
            tv_dis.Text = string.Format(text_dis);
 //           tv_amp.Text = string.Format(text_amp);
 //           tv_temp.Text = string.Format(text_temp);
        }

        string text_dis;
        string text_amp;
        string text_temp;
        void DecodeData(string indata)
        {
            String pattern = ":";
            String[] elements = Regex.Split(indata, pattern);

            String str_data = elements[1];

            String pattern_line = "\n";
            String[] elements_line = Regex.Split(str_data, pattern_line);

            Int32 data_int = Int32.Parse(elements_line[0]);
            text_dis = data_int.ToString();

            /*  Int32 data_int = Int32.Parse(elements[1]);
              text_dis = data_int.ToString();

              data_int = Int32.Parse(elements[3]);
              text_amp = data_int.ToString();

              data_int = Int32.Parse(elements[5]);
              text_temp = data_int.ToString();*/
        }

        int btnStatus = 0; // 0: None, 1: get one data, 2: more data, 3: stop
                           //  byte[] getDis = new byte[] {0xCD,0x01,0x00,0x05,0x06};
        byte[] getDis = new byte[] { 0x53};
        private void BtnGetDis_Click(object sender, EventArgs e)
        {
            // serialPort1.Write(getDis, 0, 5);
            serialPort1.Write(getDis, 0, 1);
            btnStatus = 1;
        }

        //       byte[] off = new byte[] { 0xCD, 0x01, 0x00, 0x04, 0x05 };
        byte[] off = new byte[] { 0x44 };
        private void BtnOff_Click(object sender, EventArgs e)
        {
            cnt = 1000000;
            btnStatus = 2;
            serialPort1.Write(off, 0, 1);
        }

    //    byte[] on = new byte[] { 0xCD, 0x01, 0x00, 0x03, 0x04 };
    byte[] on = new byte[] { 0x45 };
        private void BtnOn_Click(object sender, EventArgs e)
        {
            cnt = 1;
            btnStatus = 3;
            //     serialPort1.Write(on, 0, 1);
            serialPort1.WriteLine("E");
            Thread.Sleep(1000);
            serialPort1.WriteLine("S");
        }

        int cnt;
        void testFunc()
        {
            if (cnt <= CNT_MAX)
            {
                serialPort1.WriteLine("E");
                Thread.Sleep(1000);
                serialPort1.WriteLine("S");
            }       
        }


        int DebugCnt = 0;
        bool isDebugMode = false;
        private void FromClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DebugCnt++;
                if (DebugCnt == 3 && !isDebugMode)
                {
                    tv_amp.Visible = true;
                    tv_amp_t.Visible = true;
                    tv_temp.Visible = true;
                    tv_temp_t.Visible = true;
                    cb_back.Visible = true;
                    isDebugMode = true;
                    DebugCnt = 0;
                }
            }
        }

        private void cb_backMode(object sender, EventArgs e)
        {
            tv_amp.Visible = false;
            tv_amp_t.Visible = false;
            tv_temp.Visible = false;
            tv_temp_t.Visible = false;
            cb_back.Checked = false;

            isDebugMode = false;
            DebugCnt = 0;
            cb_back.Visible = false;
        }
    }
}
