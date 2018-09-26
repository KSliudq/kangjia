namespace kangjia
{
    partial class indexForm2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(indexForm2));
            this.Init_Camera_Timer = new System.Windows.Forms.Timer(this.components);
            this.lblversion = new System.Windows.Forms.Label();
            this.panlversion = new System.Windows.Forms.Panel();
            this.cameraControl = new Camera_NET.CameraControl();
            this.axShockwaveFlash1 = new AxShockwaveFlashObjects.AxShockwaveFlash();
            this.panlversion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axShockwaveFlash1)).BeginInit();
            this.SuspendLayout();
            // 
            // Init_Camera_Timer
            // 
            this.Init_Camera_Timer.Interval = 1000;
            this.Init_Camera_Timer.Tick += new System.EventHandler(this.Init_Camera_Main_Tick);
            // 
            // lblversion
            // 
            this.lblversion.BackColor = System.Drawing.Color.Transparent;
            this.lblversion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblversion.Font = new System.Drawing.Font("宋体", 10F);
            this.lblversion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(14)))), ((int)(((byte)(52)))));
            this.lblversion.Location = new System.Drawing.Point(0, 0);
            this.lblversion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblversion.Name = "lblversion";
            this.lblversion.Size = new System.Drawing.Size(67, 100);
            this.lblversion.TabIndex = 4;
            this.lblversion.Text = "版本号";
            this.lblversion.Visible = false;
            // 
            // panlversion
            // 
            this.panlversion.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panlversion.BackgroundImage")));
            this.panlversion.Controls.Add(this.lblversion);
            this.panlversion.Location = new System.Drawing.Point(25, 82);
            this.panlversion.Margin = new System.Windows.Forms.Padding(2);
            this.panlversion.Name = "panlversion";
            this.panlversion.Size = new System.Drawing.Size(67, 100);
            this.panlversion.TabIndex = 5;
            this.panlversion.Visible = false;
            // 
            // cameraControl
            // 
            this.cameraControl.BackColor = System.Drawing.SystemColors.HotTrack;
            this.cameraControl.BackgroundImage = global::kangjia.Properties.Resources.微信图片_20180914133220;
            this.cameraControl.DirectShowLogFilepath = "";
            this.cameraControl.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.cameraControl.Location = new System.Drawing.Point(185, 99);
            this.cameraControl.Name = "cameraControl";
            this.cameraControl.Size = new System.Drawing.Size(297, 224);
            this.cameraControl.TabIndex = 6;
            // 
            // axShockwaveFlash1
            // 
            this.axShockwaveFlash1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axShockwaveFlash1.Enabled = true;
            this.axShockwaveFlash1.Location = new System.Drawing.Point(0, 0);
            this.axShockwaveFlash1.Name = "axShockwaveFlash1";
            this.axShockwaveFlash1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axShockwaveFlash1.OcxState")));
            this.axShockwaveFlash1.Size = new System.Drawing.Size(664, 485);
            this.axShockwaveFlash1.TabIndex = 1;
            // 
            // indexForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 485);
            this.Controls.Add(this.panlversion);
            this.Controls.Add(this.cameraControl);
            this.Controls.Add(this.axShockwaveFlash1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "indexForm2";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "indexForm2";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.indexForm2_FormClosing);
            this.Load += new System.EventHandler(this.indexForm2_Load);
            this.panlversion.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axShockwaveFlash1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxShockwaveFlashObjects.AxShockwaveFlash axShockwaveFlash1;
        private System.Windows.Forms.Timer Init_Camera_Timer;
        private System.Windows.Forms.Label lblversion;
        private System.Windows.Forms.Panel panlversion;
        private Camera_NET.CameraControl cameraControl;
    }
}