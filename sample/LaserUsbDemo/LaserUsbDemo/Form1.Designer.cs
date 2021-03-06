﻿
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.BtnDeviceStatus = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.BtnRefresh = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnGetDis = new System.Windows.Forms.Button();
            this.BtnShots = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tv_dis = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tv_amp_t = new System.Windows.Forms.Label();
            this.tv_temp_t = new System.Windows.Forms.Label();
            this.tv_amp = new System.Windows.Forms.Label();
            this.tv_temp = new System.Windows.Forms.Label();
            this.cb_back = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Uart_Rx_Handle);
            // 
            // BtnDeviceStatus
            // 
            this.BtnDeviceStatus.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnDeviceStatus.Location = new System.Drawing.Point(324, 36);
            this.BtnDeviceStatus.Name = "BtnDeviceStatus";
            this.BtnDeviceStatus.Size = new System.Drawing.Size(210, 73);
            this.BtnDeviceStatus.TabIndex = 0;
            this.BtnDeviceStatus.Text = "Connect";
            this.BtnDeviceStatus.UseVisualStyleBackColor = true;
            this.BtnDeviceStatus.Click += new System.EventHandler(this.BtnDeviceStatus_Click);
            // 
            // label1
            // 
            this.label1.AccessibleRole = System.Windows.Forms.AccessibleRole.PageTabList;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(225, 33);
            this.label1.TabIndex = 1;
            this.label1.Text = "Device Status:";
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.IntegralHeight = false;
            this.comboBox1.Location = new System.Drawing.Point(42, 72);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(245, 31);
            this.comboBox1.TabIndex = 2;
            // 
            // BtnRefresh
            // 
            this.BtnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("BtnRefresh.Image")));
            this.BtnRefresh.Location = new System.Drawing.Point(563, 34);
            this.BtnRefresh.Name = "BtnRefresh";
            this.BtnRefresh.Size = new System.Drawing.Size(77, 75);
            this.BtnRefresh.TabIndex = 3;
            this.BtnRefresh.UseVisualStyleBackColor = true;
            this.BtnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(664, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "---------------------------------------------------------------------------------" +
    "-";
            // 
            // BtnGetDis
            // 
            this.BtnGetDis.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnGetDis.Location = new System.Drawing.Point(382, 149);
            this.BtnGetDis.Name = "BtnGetDis";
            this.BtnGetDis.Size = new System.Drawing.Size(152, 51);
            this.BtnGetDis.TabIndex = 5;
            this.BtnGetDis.Text = "Get One Distance";
            this.BtnGetDis.UseVisualStyleBackColor = true;
            this.BtnGetDis.Click += new System.EventHandler(this.BtnGetDis_Click);
            // 
            // BtnShots
            // 
            this.BtnShots.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnShots.Location = new System.Drawing.Point(204, 149);
            this.BtnShots.Name = "BtnShots";
            this.BtnShots.Size = new System.Drawing.Size(146, 51);
            this.BtnShots.TabIndex = 6;
            this.BtnShots.Text = "OFF";
            this.BtnShots.UseVisualStyleBackColor = true;
            this.BtnShots.Click += new System.EventHandler(this.BtnOff_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnStop.Location = new System.Drawing.Point(24, 149);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(146, 51);
            this.BtnStop.TabIndex = 7;
            this.BtnStop.Text = "ON";
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnOn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(18, 243);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(150, 33);
            this.label3.TabIndex = 8;
            this.label3.Text = "Distance:";
            // 
            // tv_dis
            // 
            this.tv_dis.AutoSize = true;
            this.tv_dis.Font = new System.Drawing.Font("Consolas", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tv_dis.Location = new System.Drawing.Point(188, 243);
            this.tv_dis.Name = "tv_dis";
            this.tv_dis.Size = new System.Drawing.Size(0, 33);
            this.tv_dis.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(318, 243);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 33);
            this.label4.TabIndex = 10;
            this.label4.Text = "mm";
            // 
            // tv_amp_t
            // 
            this.tv_amp_t.AutoSize = true;
            this.tv_amp_t.Font = new System.Drawing.Font("Consolas", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tv_amp_t.Location = new System.Drawing.Point(18, 291);
            this.tv_amp_t.Name = "tv_amp_t";
            this.tv_amp_t.Size = new System.Drawing.Size(165, 33);
            this.tv_amp_t.TabIndex = 11;
            this.tv_amp_t.Text = "Amplitude:";
            this.tv_amp_t.Visible = false;
            // 
            // tv_temp_t
            // 
            this.tv_temp_t.AutoSize = true;
            this.tv_temp_t.Font = new System.Drawing.Font("Consolas", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tv_temp_t.Location = new System.Drawing.Point(18, 338);
            this.tv_temp_t.Name = "tv_temp_t";
            this.tv_temp_t.Size = new System.Drawing.Size(195, 33);
            this.tv_temp_t.TabIndex = 12;
            this.tv_temp_t.Text = "Temperature:";
            this.tv_temp_t.Visible = false;
            // 
            // tv_amp
            // 
            this.tv_amp.AutoSize = true;
            this.tv_amp.Font = new System.Drawing.Font("Consolas", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tv_amp.Location = new System.Drawing.Point(188, 291);
            this.tv_amp.Name = "tv_amp";
            this.tv_amp.Size = new System.Drawing.Size(0, 33);
            this.tv_amp.TabIndex = 13;
            // 
            // tv_temp
            // 
            this.tv_temp.AutoSize = true;
            this.tv_temp.Font = new System.Drawing.Font("Consolas", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tv_temp.Location = new System.Drawing.Point(235, 338);
            this.tv_temp.Name = "tv_temp";
            this.tv_temp.Size = new System.Drawing.Size(0, 33);
            this.tv_temp.TabIndex = 14;
            // 
            // cb_back
            // 
            this.cb_back.AutoSize = true;
            this.cb_back.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_back.Location = new System.Drawing.Point(24, 375);
            this.cb_back.Name = "cb_back";
            this.cb_back.Size = new System.Drawing.Size(158, 22);
            this.cb_back.TabIndex = 15;
            this.cb_back.Text = "Back Normal mode";
            this.cb_back.UseVisualStyleBackColor = true;
            this.cb_back.Visible = false;
            this.cb_back.CheckedChanged += new System.EventHandler(this.cb_backMode);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cb_back);
            this.Controls.Add(this.tv_temp);
            this.Controls.Add(this.tv_amp);
            this.Controls.Add(this.tv_temp_t);
            this.Controls.Add(this.tv_amp_t);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tv_dis);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BtnStop);
            this.Controls.Add(this.BtnShots);
            this.Controls.Add(this.BtnGetDis);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BtnRefresh);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnDeviceStatus);
            this.Name = "Form1";
            this.Text = "Laser Usb Demo";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FromClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button BtnDeviceStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button BtnRefresh;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnGetDis;
        private System.Windows.Forms.Button BtnShots;
        private System.Windows.Forms.Button BtnStop;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label tv_dis;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label tv_amp_t;
        private System.Windows.Forms.Label tv_temp_t;
        private System.Windows.Forms.Label tv_amp;
        private System.Windows.Forms.Label tv_temp;
        private System.Windows.Forms.CheckBox cb_back;
    }
}

