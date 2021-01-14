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
using System.IO;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace LaserUsbDemo
{
    public partial class Form1 : Form
    {

        private bool[] isDeviceDetect = new bool[10];

        List<ComPort> SerialPort = new List<ComPort>();
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

        public Form1()
        {
            InitializeComponent();
            getComPort();
#if txtFile
            createLogPath();      
#endif
            BtnTmp1.Enabled = false;
            BtnTmp2.Enabled = false;
            BtnTmp3.Enabled = false;
            BtnCal.Enabled = false;

            tv_status.Text = "";
        }

        private void getComPort()
        {
            string device_port_num = "";
            SerialPort = GetAllSerialPort();
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

                    for (int j = match_port.Index + 3; j < data.Length; j++)
                    {
                        if (!string.Equals(data[j].ToString(), ")"))
                        {
                            setTextBoxComPort(data[j].ToString());
                        }
                    }
                }
            }
        }

        private void setTextBoxComPort(String device_port_num)
        {
            if (String.IsNullOrEmpty(tb_device1.Text))
                tb_device1.Text = device_port_num;
            else if (String.IsNullOrEmpty(tb_device2.Text))
                tb_device2.Text = device_port_num;
            else if (String.IsNullOrEmpty(tb_device3.Text))
                tb_device3.Text = device_port_num;
            else if (String.IsNullOrEmpty(tb_device4.Text))
                tb_device4.Text = device_port_num;
            else if (String.IsNullOrEmpty(tb_device5.Text))
                tb_device5.Text = device_port_num;
            else if (String.IsNullOrEmpty(tb_device6.Text))
                tb_device6.Text = device_port_num;
            else if (String.IsNullOrEmpty(tb_device7.Text))
                tb_device7.Text = device_port_num;
            else if (String.IsNullOrEmpty(tb_device8.Text))
                tb_device8.Text = device_port_num;
            else if (String.IsNullOrEmpty(tb_device9.Text))
                tb_device9.Text = device_port_num;
            else if (String.IsNullOrEmpty(tb_device10.Text))
                tb_device10.Text = device_port_num;
        }

        private List<ComPort> GetAllSerialPort()
        {
            Console.WriteLine("------- GetAllSerialPort -----");
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
                        c.description = port.GetPropertyValue("Caption").ToString();
                        ports.Add(c);
                    }
            }
            return ports;
        }


        private void CloseSerialPort(SerialPort serialPort)
        {
            try
            {
                serialPort.Close();
              
                Console.WriteLine("------- CloseSerialPort -----");
               
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //System.Environment.Exit(1);
            }
        }

        private bool OpenSerialPort(SerialPort serialPort,string portName)
        {
            try
            {
                serialPort.BaudRate = 256000;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.DataBits = 8;
                serialPort.Handshake = Handshake.None;
                serialPort.RtsEnable = true;
                serialPort.PortName = portName;
                serialPort.Open(); 
                Console.WriteLine("------- OpenSerialPort -----");
                return true;
            }
            catch (Exception exp)
            {
                return false;       
            }
        }

        private int tempture;
        private int voltage_b;
        private String str_msg;
        private int[] tmp = new int [20];
        private int[] vb = new int [20];
        private void Uart_Rx_Handle(object sender, SerialDataReceivedEventArgs e)
        { 
            SerialPort sp = (SerialPort)sender;
            byte[] rx_data = new byte[30];
            int len = sp.Read(rx_data, 0, 30);

            if (len == 4)
            {
                tempture = (int)(rx_data[1] << 8 | rx_data[0]);
                voltage_b = (int)(rx_data[3] << 8 | rx_data[2]);

                str_msg = "tmp:" + tempture + "," + "Vb:" + voltage_b + " [" + sendDevice + "->" + tmpStatus + "-" + cnt + "]";
                Console.WriteLine(str_msg);
                this.BeginInvoke(new InvokeDelegate(HandleSaveMsg)); 
            }
            else 
            {
                String data = System.Text.Encoding.ASCII.GetString(rx_data, 0, len);
                Console.WriteLine("" + data);
            }
            status_device_cnt();
        }

        private delegate void InvokeDelegate();

        private void HandleSaveMsg()
        {
            String str_temp = "[" + tmpStatus + "-" + cnt + "]";
            //int status,int cnt,int device,String str_status,string tmpture, string volt
            WriteDataToFile(tmpStatus, cnt, sendDevice, str_temp, tempture.ToString(), voltage_b.ToString());
            cnt++;
        }

        private void HandleBtnTmpEnableSelection()
        {
            tv_status.Text = "  OK  ";
            BtnTmp1.Enabled = true;
            BtnTmp2.Enabled = true;
            BtnTmp3.Enabled = true;
            BtnCal.Enabled = true;
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            CloseExcelFile();
        }

        private void checkComPortNull(TextBox device, int device_idx, SerialPort serialPort)
        {
            if (!String.IsNullOrEmpty(device.Text))
            {
                String COM = "COM" + device.Text;
                isDeviceDetect[device_idx] = OpenSerialPort(serialPort, COM);
                Console.WriteLine("Device " + isDeviceDetect[device_idx] +":" + (isDeviceDetect[device_idx] ? "YES" : "NO"));
            }
        }

        private void disconnectDevice(int device_idx, SerialPort serialPort)
        {
            if (isDeviceDetect[device_idx])
            {
                CloseSerialPort(serialPort);
                isDeviceDetect[device_idx] = false;
            }
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;//該值確定是否可以選擇多個檔案
            dialog.Title = "請選擇資料夾";
            dialog.Filter = "xls檔案(*.xls*)|*.xls*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                OpenExcelFile(file);
            }
        }

        private Boolean device_status = true;
        private void btnConnected_Click(object sender, EventArgs e)
        {
            if (device_status)
            {

                checkComPortNull(tb_device1, 0, serialPort1);
                checkComPortNull(tb_device2, 1, serialPort2);
                checkComPortNull(tb_device3, 2, serialPort3);
                checkComPortNull(tb_device4, 3, serialPort4);
                checkComPortNull(tb_device5, 4, serialPort5);
                checkComPortNull(tb_device6, 5, serialPort6);
                checkComPortNull(tb_device7, 6, serialPort7);
                checkComPortNull(tb_device8, 7, serialPort8);
                checkComPortNull(tb_device9, 8, serialPort9);
                checkComPortNull(tb_device10,9, serialPort10);

                if (!isDeviceDetect[0])
                {
                    MessageBox.Show("連接錯誤", "COM PORT 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }  
                else
                {
                    device_status = false;

                    CreateExcelFilePath();   
                    btnConnected.Text = "斷線";
                    Console.WriteLine("建excel");

                    BtnTmp1.Enabled = true;
                    BtnTmp2.Enabled = true;
                    BtnTmp3.Enabled = true;
                    BtnCal.Enabled = true;
                }        
            }
            else
            {
                device_status = true;

                disconnectDevice(0, serialPort1);
                disconnectDevice(1, serialPort2);
                disconnectDevice(2, serialPort3);
                disconnectDevice(3, serialPort4);
                disconnectDevice(4, serialPort5);
                disconnectDevice(5, serialPort6);
                disconnectDevice(6, serialPort7);
                disconnectDevice(7, serialPort8);
                disconnectDevice(8, serialPort9);
                disconnectDevice(9, serialPort10);

                Console.WriteLine("釋放excel");
                CloseExcelFile();
                BtnTmp1.Enabled = false;
                BtnTmp2.Enabled = false;
                BtnTmp3.Enabled = false;
                BtnCal.Enabled = false;
                btnConnected.Text = "連線";
                tv_status.Text = "";
            }   
        }

        private void BtnCal_Click(object sender, EventArgs e)
        {

            BtnTmp1.Enabled = false;
            BtnTmp2.Enabled = false;
            BtnTmp3.Enabled = false;
            Excel.Range range = excelWorksheet.UsedRange;

            int total_col = tmp_1_time + tmp_2_time + tmp_3_time + 1;
            Console.WriteLine("total_col:" + total_col);

            TransData(0, range, total_col, serialPort1);
            TransData(1, range, total_col, serialPort2);
            TransData(2, range, total_col, serialPort3);
            TransData(3, range, total_col, serialPort4);
            TransData(4, range, total_col, serialPort5);
            TransData(5, range, total_col, serialPort6);
            TransData(6, range, total_col, serialPort7);
            TransData(7, range, total_col, serialPort8);
            TransData(8, range, total_col, serialPort9);
            TransData(9, range, total_col, serialPort10);

            BtnTmp1.Enabled = true;
            BtnTmp2.Enabled = true;
            BtnTmp3.Enabled = true;
        }

        private void TransData(int device_idx,Excel.Range range, int times, SerialPort serialPort)
        {
            if (!isDeviceDetect[device_idx])
                return;

            byte[] data = new byte[6];
            int str_temp;
            int str_volt;

            sendCleanDataBuf(serialPort);
            for (int i = 2; i <= times; i++)
            {
                str_temp = (int)(range.Cells[i, 2] as Excel.Range).Value2;
                str_volt = (int)(range.Cells[i, 3] as Excel.Range).Value2;
                byte[] temp = BitConverter.GetBytes(str_temp);
                byte[] vol = BitConverter.GetBytes(str_volt);

                Console.WriteLine("tmp:" + temp[0] + "," + temp[1]);
                Console.WriteLine("vol:" + vol[0] + "," + vol[1]);

                data[0] = 0x54;
                data[1] = temp[0];
                data[2] = temp[1];
                data[3] = vol[0];
                data[4] = vol[1];
                data[5] = 0x0d;
                sendData(serialPort, data);
            }
            sendRunCalculate(serialPort);
        }

        private int tmp_1_time = 0;
        private int tmp_2_time = 0;
        private int tmp_3_time = 0;
        int tmpStatus = 0;
        private void BtnTmp1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(tb_tmp1.Text))
                {
                    tmp_1_time = int.Parse(tb_tmp1.Text);
                    tmpStatus = 1;
                    sendDevice = 1;
                    BtnTmp1.Enabled = false;
                    BtnTmp2.Enabled = false;
                    BtnTmp3.Enabled = false;
                    BtnCal.Enabled = false;
                    tv_status.Text = "Waiting...";
                    status_device_cnt();
                }
            }
            catch (Exception ex)
            {
                BtnTmp1.Enabled = true;
                BtnTmp2.Enabled = true;
                BtnTmp3.Enabled = true;
                BtnCal.Enabled = true;
                MessageBox.Show("溫度1輸入筆數錯誤", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTmp2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(tb_tmp2.Text))
                {
                    tmp_2_time = int.Parse(tb_tmp2.Text);
                    tmpStatus = 2;
                    sendDevice = 1;
                    BtnTmp1.Enabled = false;
                    BtnTmp2.Enabled = false;
                    BtnTmp3.Enabled = false;
                    BtnCal.Enabled = false;
                    tv_status.Text = "Waiting...";
                    status_device_cnt();
                }
            }
            catch (Exception ex)
            {
                BtnTmp1.Enabled = true;
                BtnTmp2.Enabled = true;
                BtnTmp3.Enabled = true;
                BtnCal.Enabled = true;
                MessageBox.Show("溫度2輸入筆數錯誤", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTmp3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(tb_tmp3.Text))
                {
                    tmp_3_time = int.Parse(tb_tmp3.Text);
                    tmpStatus = 3;
                    sendDevice = 1;
                    BtnTmp1.Enabled = false;
                    BtnTmp2.Enabled = false;
                    BtnTmp3.Enabled = false;
                    BtnCal.Enabled = false;
                    tv_status.Text = "Waiting...";
                    status_device_cnt();
                }
            }
            catch (Exception ex)
            {
                BtnTmp1.Enabled = true;
                BtnTmp2.Enabled = true;
                BtnTmp3.Enabled = true;
                BtnCal.Enabled = true;
                MessageBox.Show("溫度3輸入筆數錯誤", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void enableBtnTmp()
        {
            this.BeginInvoke(new InvokeDelegate(HandleBtnTmpEnableSelection));
        }

        private void sendRunCalculate(SerialPort serialPort)
        {
            serialPort.Write("R");
        }

        private void sendCleanDataBuf(SerialPort serialPort)
        {
            serialPort.Write("N");
        }

        private void sendData(SerialPort serialPort, byte[] data)
        {
            serialPort.Write(data, 0, data.Length);
        }

        int sendDevice = 0;
        int cnt = 1; 
        byte[] getVb_Tmp = new byte[] { 0x2D, 0x73, 0x65, 0x61, 0x72, 0x63, 0x68, 0x76, 0x62 , 0x0D};
        private void sendGetVb_Tmp(SerialPort serialPort)
        {
              serialPort.Write(getVb_Tmp, 0, 10);
        //    serialPort.Write("-searchvb\r");
        }

        private void status_device_cnt()
        {
            int times_params = 0;

            if (tmpStatus == 1)
                times_params = tmp_1_time;
            else if (tmpStatus == 2)
                times_params = tmp_2_time;
            else if (tmpStatus == 3)
                times_params = tmp_3_time;

            Thread.Sleep(500);
   
            switch (sendDevice)
            {
                case 1:
                {
                    deal_send_vb_Temp_process(times_params, serialPort1);
                }   
                break;
                case 2:
                {
                    deal_send_vb_Temp_process(times_params, serialPort2);
                }   
                break;
                case 3:
                {
                    deal_send_vb_Temp_process(times_params, serialPort3);
                }  
                break;
                case 4:
                {
                    deal_send_vb_Temp_process(times_params, serialPort4);
                }     
                break;
                case 5:
                {
                    deal_send_vb_Temp_process(times_params, serialPort5);
                }              
                break;
                case 6:
                {
                    deal_send_vb_Temp_process(times_params, serialPort6);
                }
                break;
                case 7:
                {
                    deal_send_vb_Temp_process(times_params, serialPort7);
                }
                break;
                case 8:
                {
                    deal_send_vb_Temp_process(times_params, serialPort8);
                }
                break;
                case 9:
                {
                    deal_send_vb_Temp_process(times_params, serialPort9);
                }
                break;
                case 10:
                {
                    deal_send_vb_Temp_process(times_params, serialPort10);
                }
                break;
            }
        }

        private void deal_send_vb_Temp_process(int times_params, SerialPort serialPort)
        {
            if (cnt <= times_params)
                sendGetVb_Tmp(serialPort);
            else
            {
                if (sendDevice <= 10)
                {
                    if (isDeviceDetect[sendDevice])
                    {
                        sendDevice++;
                        sendGetVb_Tmp(serialPort);
                        cnt = 1;
                    }
                    else
                    {
                        sendDevice = 0;
                        cnt = 1;
                        enableBtnTmp();
                    }
                }
                else
                {
                    sendDevice = 0;
                    cnt = 1;
                    enableBtnTmp();
                }
            }
        }

        string FILENAME_XLS;
        Excel.Application excelApp;
        Excel.Workbook excelWorkbook;
        Excel.Worksheet excelWorksheet;
        public void CreateExcelFilePath()
        {
            if (excelApp == null)
            {
                string DIRNAME = Application.StartupPath + @"\Log\";
                if (!Directory.Exists(DIRNAME))
                    Directory.CreateDirectory(DIRNAME);

                FILENAME_XLS = DIRNAME + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls";
                excelApp = new Excel.Application();
                WriteExcelTitle();
            }
            else 
            {
                DialogResult result = MessageBox.Show("已讀取Excel 資料,是否開啟新檔", "錯誤", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.Yes)
                {
                    // 關閉舊的Excel file
                    CloseExcelFile();

                    // 建立新的File
                    string DIRNAME = Application.StartupPath + @"\Log\";
                    if (!Directory.Exists(DIRNAME))
                        Directory.CreateDirectory(DIRNAME);

                    FILENAME_XLS = DIRNAME + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls";
                    excelApp = new Excel.Application();
                    WriteExcelTitle();
                }
            }
        }

        public void OpenExcelFile(String path)
        {
            excelApp = new Excel.Application();
            excelWorkbook = excelApp.Workbooks.Open(path);
            excelWorksheet = (Excel.Worksheet)excelWorkbook.Sheets.get_Item(1); ;
        }

        private void CloseExcelFile()
        {

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

        public void WriteExcelTitle()
        {
            if (excelApp != null)
            {
                excelWorkbook = excelApp.Workbooks.Add();
                excelWorksheet = (Excel.Worksheet)excelWorkbook.Sheets.Add();

                excelWorksheet.Cells[1, 1] = "Device 1";             
                excelWorksheet.Cells[1, 2] = "Temp";
                excelWorksheet.Cells[1, 3] = "Vb";

                excelWorksheet.Cells[1, 4] = "Device 2";                
                excelWorksheet.Cells[1, 5] = "Temp";
                excelWorksheet.Cells[1, 6] = "Vb";

                excelWorksheet.Cells[1, 7] = "Device 3";               
                excelWorksheet.Cells[1, 8] = "Temp";
                excelWorksheet.Cells[1, 9] = "Vb";

                excelWorksheet.Cells[1, 10] = "Device 4";              
                excelWorksheet.Cells[1, 11] = "Temp";
                excelWorksheet.Cells[1, 12] = "Vb";

                excelWorksheet.Cells[1, 13] = "Device 5";   
                excelWorksheet.Cells[1, 14] = "Temp";
                excelWorksheet.Cells[1, 15] = "Vb";

                excelWorksheet.Cells[1, 16] = "Device 6";
                excelWorksheet.Cells[1, 17] = "Temp";
                excelWorksheet.Cells[1, 18] = "Vb";

                excelWorksheet.Cells[1, 19] = "Device 7";
                excelWorksheet.Cells[1, 20] = "Temp";
                excelWorksheet.Cells[1, 21] = "Vb";

                excelWorksheet.Cells[1, 22] = "Device 8";
                excelWorksheet.Cells[1, 23] = "Temp";
                excelWorksheet.Cells[1, 24] = "Vb";

                excelWorksheet.Cells[1, 25] = "Device 9";
                excelWorksheet.Cells[1, 26] = "Temp";
                excelWorksheet.Cells[1, 27] = "Vb";

                excelWorksheet.Cells[1, 28] = "Device 10";
                excelWorksheet.Cells[1, 29] = "Temp";
                excelWorksheet.Cells[1, 30] = "Vb";
 
                excelApp.ActiveWorkbook.SaveAs(FILENAME_XLS, Excel.XlFileFormat.xlWorkbookNormal);
            }
        }

        public void WriteDataToFile(int status,int cnt,int device,String str_status,string tmpture, string volt)
        {
            
            Int32 col = 1;
            if (status == 1)
                col = 1 + cnt;
            else if (status == 2)
                col = 1 + tmp_1_time + cnt;
            else if (status == 3)
                col = 1 + tmp_1_time + tmp_2_time + cnt;

//            Console.WriteLine("col:" + col + ",status:" + status + ",cnt:" + cnt);
            if (excelWorksheet != null)
            {
                switch (device)
                {
                    case 1:
                        {
                            excelWorksheet.Cells[col, 1] = str_status;
                            excelWorksheet.Cells[col, 2] = tmpture;
                            excelWorksheet.Cells[col, 3] = volt;
                        }
                    break;
                    case 2:
                        {
                            excelWorksheet.Cells[col, 4] = str_status;
                            excelWorksheet.Cells[col, 5] = tmpture;
                            excelWorksheet.Cells[col, 6] = volt;
                        }
                    break;
                    case 3:
                        {
                            excelWorksheet.Cells[col, 7] = str_status;
                            excelWorksheet.Cells[col, 8] = tmpture;
                            excelWorksheet.Cells[col, 9] = volt;
                        }
                    break;
                    case 4:
                        {
                            excelWorksheet.Cells[col, 10] = str_status;
                            excelWorksheet.Cells[col, 11] = tmpture;
                            excelWorksheet.Cells[col, 12] = volt;
                        }
                    break;
                    case 5:
                        {
                            excelWorksheet.Cells[col, 13] = str_status;
                            excelWorksheet.Cells[col, 14] = tmpture;
                            excelWorksheet.Cells[col, 15] = volt;
                        }
                    break;
                    case 6:
                        {
                            excelWorksheet.Cells[col, 16] = str_status;
                            excelWorksheet.Cells[col, 17] = tmpture;
                            excelWorksheet.Cells[col, 18] = volt;
                        }
                        break;
                    case 7:
                        {
                            excelWorksheet.Cells[col, 19] = str_status;
                            excelWorksheet.Cells[col, 20] = tmpture;
                            excelWorksheet.Cells[col, 21] = volt;
                        }
                        break;
                    case 8:
                        {
                            excelWorksheet.Cells[col, 22] = str_status;
                            excelWorksheet.Cells[col, 23] = tmpture;
                            excelWorksheet.Cells[col, 24] = volt;
                        }
                        break;
                    case 9:
                        {
                            excelWorksheet.Cells[col, 25] = str_status;
                            excelWorksheet.Cells[col, 26] = tmpture;
                            excelWorksheet.Cells[col, 27] = volt;
                        }
                        break;
                    case 10:
                        {
                            excelWorksheet.Cells[col, 28] = str_status;
                            excelWorksheet.Cells[col, 29] = tmpture;
                            excelWorksheet.Cells[col, 30] = volt;
                        }
                        break;
                }
               
                excelApp.ActiveWorkbook.Save();
            }
        }

#if txtFile
        string FILENAME_CVS;
        string FILENAME_TXT;
        private void createLogPath()
        {
            Console.WriteLine("-->" + Application.StartupPath);
            string DIRNAME = Application.StartupPath + @"\Log\";
            FILENAME_TXT = DIRNAME + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            FILENAME_CVS = DIRNAME + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
            if (!Directory.Exists(DIRNAME))
                Directory.CreateDirectory(DIRNAME);

            if (!File.Exists(FILENAME_TXT))
            {
                // The File.Create method creates the file and opens a FileStream on the file. You neeed to close it.
                File.Create(FILENAME_TXT).Close();
            }

            if (!File.Exists(FILENAME_CVS))
            {
                // The File.Create method creates the file and opens a FileStream on the file. You neeed to close it.
                File.Create(FILENAME_CVS).Close();
            }
        }
        void WriteToCSV(string FilePath, UserData data)
        {
            using (var file = new StreamWriter(FilePath))
            {
                file.WriteLineAsync($"{data.TempStatus_1},{data.DeviceName_1},{data.Tempture_1},{data.Vb_1}," +
                                       $"{data.TempStatus_2},{data.DeviceName_2},{data.Tempture_2},{data.Vb_2}," +
                                       $"{data.TempStatus_3},{data.DeviceName_3},{data.Tempture_3},{data.Vb_3}," +
                                       $"{data.TempStatus_4},{data.DeviceName_4},{data.Tempture_4},{data.Vb_4}," +
                                       $"{data.TempStatus_5},{data.DeviceName_5},{data.Tempture_5},{data.Vb_5}"
                                       );
            }
        }

        private void WriteLog(String message)
        {    
            using (StreamWriter sw = File.AppendText(FILENAME_TXT))
            {
                Log(message, sw);
            }
        }

        private void Log(string logMessage, TextWriter w)
        {
            w.Write("{0}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            w.WriteLine("    {0}    ", logMessage);  
        }
#endif

    }
}
