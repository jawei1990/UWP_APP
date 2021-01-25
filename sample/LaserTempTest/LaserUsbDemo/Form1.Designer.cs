
namespace LaserUsbDemo
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_tmp1 = new System.Windows.Forms.TextBox();
            this.tb_tmp2 = new System.Windows.Forms.TextBox();
            this.tb_tmp3 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.BtnTmp1 = new System.Windows.Forms.Button();
            this.BtnTmp2 = new System.Windows.Forms.Button();
            this.BtnTmp3 = new System.Windows.Forms.Button();
            this.BtnCal = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_device1 = new System.Windows.Forms.TextBox();
            this.tb_device2 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tb_device3 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tb_device4 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tb_device5 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnConnected = new System.Windows.Forms.Button();
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.serialPort3 = new System.IO.Ports.SerialPort(this.components);
            this.serialPort4 = new System.IO.Ports.SerialPort(this.components);
            this.serialPort5 = new System.IO.Ports.SerialPort(this.components);
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.tb_device10 = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.tb_device9 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.tb_device8 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tb_device7 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.tb_device6 = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.serialPort6 = new System.IO.Ports.SerialPort(this.components);
            this.serialPort7 = new System.IO.Ports.SerialPort(this.components);
            this.serialPort8 = new System.IO.Ports.SerialPort(this.components);
            this.serialPort9 = new System.IO.Ports.SerialPort(this.components);
            this.serialPort10 = new System.IO.Ports.SerialPort(this.components);
            this.tv_status = new System.Windows.Forms.Label();
            this.btnPath = new System.Windows.Forms.Button();
            this.tv_process = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 256000;
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Uart_Rx_Handle);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(648, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "--------------------------------------------------------------------------------";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("PMingLiU", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(24, 203);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 33);
            this.label4.TabIndex = 6;
            this.label4.Text = "溫度1:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("PMingLiU", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(24, 272);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 33);
            this.label3.TabIndex = 7;
            this.label3.Text = "溫度2:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("PMingLiU", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(24, 346);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 33);
            this.label5.TabIndex = 8;
            this.label5.Text = "溫度3:";
            // 
            // tb_tmp1
            // 
            this.tb_tmp1.Font = new System.Drawing.Font("PMingLiU", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_tmp1.Location = new System.Drawing.Point(146, 196);
            this.tb_tmp1.Name = "tb_tmp1";
            this.tb_tmp1.Size = new System.Drawing.Size(129, 47);
            this.tb_tmp1.TabIndex = 9;
            this.tb_tmp1.Text = "5";
            // 
            // tb_tmp2
            // 
            this.tb_tmp2.Font = new System.Drawing.Font("PMingLiU", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_tmp2.Location = new System.Drawing.Point(146, 266);
            this.tb_tmp2.Name = "tb_tmp2";
            this.tb_tmp2.Size = new System.Drawing.Size(129, 47);
            this.tb_tmp2.TabIndex = 10;
            this.tb_tmp2.Text = "5";
            // 
            // tb_tmp3
            // 
            this.tb_tmp3.Font = new System.Drawing.Font("PMingLiU", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_tmp3.Location = new System.Drawing.Point(146, 337);
            this.tb_tmp3.Name = "tb_tmp3";
            this.tb_tmp3.Size = new System.Drawing.Size(129, 47);
            this.tb_tmp3.TabIndex = 11;
            this.tb_tmp3.Text = "5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("PMingLiU", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(281, 203);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 33);
            this.label6.TabIndex = 12;
            this.label6.Text = "筆";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("PMingLiU", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label7.Location = new System.Drawing.Point(281, 272);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 33);
            this.label7.TabIndex = 13;
            this.label7.Text = "筆";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("PMingLiU", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label8.Location = new System.Drawing.Point(281, 346);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 33);
            this.label8.TabIndex = 14;
            this.label8.Text = "筆";
            // 
            // BtnTmp1
            // 
            this.BtnTmp1.Font = new System.Drawing.Font("PMingLiU", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.BtnTmp1.Location = new System.Drawing.Point(351, 197);
            this.BtnTmp1.Name = "BtnTmp1";
            this.BtnTmp1.Size = new System.Drawing.Size(141, 46);
            this.BtnTmp1.TabIndex = 15;
            this.BtnTmp1.Text = "開始測量";
            this.BtnTmp1.UseVisualStyleBackColor = true;
            this.BtnTmp1.Click += new System.EventHandler(this.BtnTmp1_Click);
            // 
            // BtnTmp2
            // 
            this.BtnTmp2.Font = new System.Drawing.Font("PMingLiU", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.BtnTmp2.Location = new System.Drawing.Point(351, 266);
            this.BtnTmp2.Name = "BtnTmp2";
            this.BtnTmp2.Size = new System.Drawing.Size(141, 46);
            this.BtnTmp2.TabIndex = 16;
            this.BtnTmp2.Text = "開始測量";
            this.BtnTmp2.UseVisualStyleBackColor = true;
            this.BtnTmp2.Click += new System.EventHandler(this.BtnTmp2_Click);
            // 
            // BtnTmp3
            // 
            this.BtnTmp3.Font = new System.Drawing.Font("PMingLiU", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.BtnTmp3.Location = new System.Drawing.Point(351, 334);
            this.BtnTmp3.Name = "BtnTmp3";
            this.BtnTmp3.Size = new System.Drawing.Size(141, 46);
            this.BtnTmp3.TabIndex = 17;
            this.BtnTmp3.Text = "開始測量";
            this.BtnTmp3.UseVisualStyleBackColor = true;
            this.BtnTmp3.Click += new System.EventHandler(this.BtnTmp3_Click);
            // 
            // BtnCal
            // 
            this.BtnCal.Font = new System.Drawing.Font("PMingLiU", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.BtnCal.Location = new System.Drawing.Point(52, 426);
            this.BtnCal.Name = "BtnCal";
            this.BtnCal.Size = new System.Drawing.Size(212, 46);
            this.BtnCal.TabIndex = 19;
            this.BtnCal.Text = "計算資料";
            this.BtnCal.UseVisualStyleBackColor = true;
            this.BtnCal.Click += new System.EventHandler(this.BtnCal_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(86, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 20);
            this.label1.TabIndex = 20;
            this.label1.Text = "裝置1";
            // 
            // tb_device1
            // 
            this.tb_device1.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_device1.Location = new System.Drawing.Point(68, 39);
            this.tb_device1.Name = "tb_device1";
            this.tb_device1.Size = new System.Drawing.Size(103, 31);
            this.tb_device1.TabIndex = 25;
            // 
            // tb_device2
            // 
            this.tb_device2.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_device2.Location = new System.Drawing.Point(188, 39);
            this.tb_device2.Name = "tb_device2";
            this.tb_device2.Size = new System.Drawing.Size(103, 31);
            this.tb_device2.TabIndex = 27;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label9.Location = new System.Drawing.Point(206, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 20);
            this.label9.TabIndex = 26;
            this.label9.Text = "裝置2";
            // 
            // tb_device3
            // 
            this.tb_device3.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_device3.Location = new System.Drawing.Point(312, 39);
            this.tb_device3.Name = "tb_device3";
            this.tb_device3.Size = new System.Drawing.Size(103, 31);
            this.tb_device3.TabIndex = 29;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label10.Location = new System.Drawing.Point(330, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(58, 20);
            this.label10.TabIndex = 28;
            this.label10.Text = "裝置3";
            // 
            // tb_device4
            // 
            this.tb_device4.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_device4.Location = new System.Drawing.Point(432, 39);
            this.tb_device4.Name = "tb_device4";
            this.tb_device4.Size = new System.Drawing.Size(103, 31);
            this.tb_device4.TabIndex = 31;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label11.Location = new System.Drawing.Point(450, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 20);
            this.label11.TabIndex = 30;
            this.label11.Text = "裝置4";
            // 
            // tb_device5
            // 
            this.tb_device5.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_device5.Location = new System.Drawing.Point(550, 39);
            this.tb_device5.Name = "tb_device5";
            this.tb_device5.Size = new System.Drawing.Size(103, 31);
            this.tb_device5.TabIndex = 33;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label12.Location = new System.Drawing.Point(568, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(58, 20);
            this.label12.TabIndex = 32;
            this.label12.Text = "裝置5";
            // 
            // btnConnected
            // 
            this.btnConnected.Font = new System.Drawing.Font("PMingLiU", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnConnected.Location = new System.Drawing.Point(673, 28);
            this.btnConnected.Name = "btnConnected";
            this.btnConnected.Size = new System.Drawing.Size(103, 46);
            this.btnConnected.TabIndex = 34;
            this.btnConnected.Text = "連線";
            this.btnConnected.UseVisualStyleBackColor = true;
            this.btnConnected.Click += new System.EventHandler(this.btnConnected_Click);
            // 
            // serialPort2
            // 
            this.serialPort2.BaudRate = 256000;
            this.serialPort2.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Uart_Rx_Handle);
            // 
            // serialPort3
            // 
            this.serialPort3.BaudRate = 256000;
            this.serialPort3.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Uart_Rx_Handle);
            // 
            // serialPort4
            // 
            this.serialPort4.BaudRate = 256000;
            this.serialPort4.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Uart_Rx_Handle);
            // 
            // serialPort5
            // 
            this.serialPort5.BaudRate = 256000;
            this.serialPort5.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Uart_Rx_Handle);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("PMingLiU", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label13.Location = new System.Drawing.Point(12, 46);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(45, 17);
            this.label13.TabIndex = 35;
            this.label13.Text = "COM";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("PMingLiU", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label14.Location = new System.Drawing.Point(173, 246);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(75, 17);
            this.label14.TabIndex = 36;
            this.label14.Text = "最多20筆";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("PMingLiU", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label15.Location = new System.Drawing.Point(173, 387);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(75, 17);
            this.label15.TabIndex = 37;
            this.label15.Text = "最多20筆";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("PMingLiU", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label16.Location = new System.Drawing.Point(173, 316);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(75, 17);
            this.label16.TabIndex = 38;
            this.label16.Text = "最多20筆";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("PMingLiU", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label17.Location = new System.Drawing.Point(12, 116);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(45, 17);
            this.label17.TabIndex = 49;
            this.label17.Text = "COM";
            // 
            // tb_device10
            // 
            this.tb_device10.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_device10.Location = new System.Drawing.Point(550, 109);
            this.tb_device10.Name = "tb_device10";
            this.tb_device10.Size = new System.Drawing.Size(103, 31);
            this.tb_device10.TabIndex = 48;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label18.Location = new System.Drawing.Point(568, 86);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(67, 20);
            this.label18.TabIndex = 47;
            this.label18.Text = "裝置10";
            // 
            // tb_device9
            // 
            this.tb_device9.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_device9.Location = new System.Drawing.Point(432, 109);
            this.tb_device9.Name = "tb_device9";
            this.tb_device9.Size = new System.Drawing.Size(103, 31);
            this.tb_device9.TabIndex = 46;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label19.Location = new System.Drawing.Point(450, 86);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(58, 20);
            this.label19.TabIndex = 45;
            this.label19.Text = "裝置9";
            // 
            // tb_device8
            // 
            this.tb_device8.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_device8.Location = new System.Drawing.Point(312, 109);
            this.tb_device8.Name = "tb_device8";
            this.tb_device8.Size = new System.Drawing.Size(103, 31);
            this.tb_device8.TabIndex = 44;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label20.Location = new System.Drawing.Point(330, 86);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(58, 20);
            this.label20.TabIndex = 43;
            this.label20.Text = "裝置8";
            // 
            // tb_device7
            // 
            this.tb_device7.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_device7.Location = new System.Drawing.Point(188, 109);
            this.tb_device7.Name = "tb_device7";
            this.tb_device7.Size = new System.Drawing.Size(103, 31);
            this.tb_device7.TabIndex = 42;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label21.Location = new System.Drawing.Point(206, 86);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(58, 20);
            this.label21.TabIndex = 41;
            this.label21.Text = "裝置7";
            // 
            // tb_device6
            // 
            this.tb_device6.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tb_device6.Location = new System.Drawing.Point(68, 109);
            this.tb_device6.Name = "tb_device6";
            this.tb_device6.Size = new System.Drawing.Size(103, 31);
            this.tb_device6.TabIndex = 40;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label22.Location = new System.Drawing.Point(86, 86);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(58, 20);
            this.label22.TabIndex = 39;
            this.label22.Text = "裝置6";
            // 
            // serialPort6
            // 
            this.serialPort6.BaudRate = 256000;
            this.serialPort6.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Uart_Rx_Handle);
            // 
            // serialPort7
            // 
            this.serialPort7.BaudRate = 256000;
            this.serialPort7.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Uart_Rx_Handle);
            // 
            // serialPort8
            // 
            this.serialPort8.BaudRate = 256000;
            this.serialPort8.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Uart_Rx_Handle);
            // 
            // serialPort9
            // 
            this.serialPort9.BaudRate = 256000;
            this.serialPort9.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Uart_Rx_Handle);
            // 
            // serialPort10
            // 
            this.serialPort10.BaudRate = 256000;
            this.serialPort10.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Uart_Rx_Handle);
            // 
            // tv_status
            // 
            this.tv_status.AutoSize = true;
            this.tv_status.Font = new System.Drawing.Font("PMingLiU", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tv_status.Location = new System.Drawing.Point(528, 253);
            this.tv_status.Name = "tv_status";
            this.tv_status.Size = new System.Drawing.Size(259, 60);
            this.tv_status.TabIndex = 50;
            this.tv_status.Text = "Waiting...";
            // 
            // btnPath
            // 
            this.btnPath.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPath.Location = new System.Drawing.Point(312, 426);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(154, 46);
            this.btnPath.TabIndex = 51;
            this.btnPath.Text = "讀取 Excel 資料";
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // tv_process
            // 
            this.tv_process.AutoSize = true;
            this.tv_process.Location = new System.Drawing.Point(547, 334);
            this.tv_process.Name = "tv_process";
            this.tv_process.Size = new System.Drawing.Size(0, 15);
            this.tv_process.TabIndex = 52;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 522);
            this.Controls.Add(this.tv_process);
            this.Controls.Add(this.btnPath);
            this.Controls.Add(this.tv_status);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.tb_device10);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.tb_device9);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.tb_device8);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.tb_device7);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.tb_device6);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.btnConnected);
            this.Controls.Add(this.tb_device5);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.tb_device4);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.tb_device3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tb_device2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tb_device1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnCal);
            this.Controls.Add(this.BtnTmp3);
            this.Controls.Add(this.BtnTmp2);
            this.Controls.Add(this.BtnTmp1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tb_tmp3);
            this.Controls.Add(this.tb_tmp2);
            this.Controls.Add(this.tb_tmp1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Name = "Form1";
            this.Text = "Laser Tempture Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_tmp1;
        private System.Windows.Forms.TextBox tb_tmp2;
        private System.Windows.Forms.TextBox tb_tmp3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button BtnTmp1;
        private System.Windows.Forms.Button BtnTmp2;
        private System.Windows.Forms.Button BtnTmp3;
        private System.Windows.Forms.Button BtnCal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_device1;
        private System.Windows.Forms.TextBox tb_device2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tb_device3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tb_device4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tb_device5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnConnected;
        private System.IO.Ports.SerialPort serialPort2;
        private System.IO.Ports.SerialPort serialPort3;
        private System.IO.Ports.SerialPort serialPort4;
        private System.IO.Ports.SerialPort serialPort5;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tb_device10;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox tb_device9;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox tb_device8;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox tb_device7;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox tb_device6;
        private System.Windows.Forms.Label label22;
        private System.IO.Ports.SerialPort serialPort6;
        private System.IO.Ports.SerialPort serialPort7;
        private System.IO.Ports.SerialPort serialPort8;
        private System.IO.Ports.SerialPort serialPort9;
        private System.IO.Ports.SerialPort serialPort10;
        private System.Windows.Forms.Label tv_status;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.Label tv_process;
    }
}

