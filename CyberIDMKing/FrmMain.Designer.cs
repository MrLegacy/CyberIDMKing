namespace CyberIDMKing
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.btnReset = new System.Windows.Forms.Button();
            this.btnRegister = new System.Windows.Forms.Button();
            this.tmrWindow = new System.Windows.Forms.Timer(this.components);
            this.nIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chbAutoResetTrial = new System.Windows.Forms.CheckBox();
            this.picLoading = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(23, 37);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(113, 37);
            this.btnReset.TabIndex = 0;
            this.btnReset.Text = "Reset Trial";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(151, 39);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(109, 35);
            this.btnRegister.TabIndex = 1;
            this.btnRegister.Text = "Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // tmrWindow
            // 
            this.tmrWindow.Enabled = true;
            this.tmrWindow.Interval = 1000;
            this.tmrWindow.Tick += new System.EventHandler(this.tmrWindow_Tick);
            // 
            // nIcon
            // 
            this.nIcon.BalloonTipText = "Working here...";
            this.nIcon.BalloonTipTitle = "CyberIDMKing";
            this.nIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("nIcon.Icon")));
            this.nIcon.Visible = true;
            this.nIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.nIcon_MouseClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.picLoading);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.chbAutoResetTrial);
            this.groupBox1.Controls.Add(this.btnReset);
            this.groupBox1.Controls.Add(this.btnRegister);
            this.groupBox1.Location = new System.Drawing.Point(9, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(582, 179);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Internet download manager";
            // 
            // chbAutoResetTrial
            // 
            this.chbAutoResetTrial.AutoSize = true;
            this.chbAutoResetTrial.Location = new System.Drawing.Point(23, 129);
            this.chbAutoResetTrial.Name = "chbAutoResetTrial";
            this.chbAutoResetTrial.Size = new System.Drawing.Size(125, 21);
            this.chbAutoResetTrial.TabIndex = 2;
            this.chbAutoResetTrial.Text = "Auto Reset Trial";
            this.chbAutoResetTrial.UseVisualStyleBackColor = true;
            this.chbAutoResetTrial.CheckedChanged += new System.EventHandler(this.chbAutoResetTrial_CheckedChanged);
            // 
            // picLoading
            // 
            this.picLoading.Image = global::CyberIDMKing.Properties.Resources.loading;
            this.picLoading.Location = new System.Drawing.Point(278, 37);
            this.picLoading.Name = "picLoading";
            this.picLoading.Size = new System.Drawing.Size(41, 37);
            this.picLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLoading.TabIndex = 4;
            this.picLoading.TabStop = false;
            this.picLoading.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CyberIDMKing.Properties.Resources.cybertik;
            this.pictureBox1.Location = new System.Drawing.Point(365, 17);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(174, 150);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 198);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Akram A. AL-Shamiri 2024";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 227);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.Text = "CyberIDMKing v1.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Timer tmrWindow;
        private System.Windows.Forms.NotifyIcon nIcon;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chbAutoResetTrial;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox picLoading;
        private System.Windows.Forms.Label label1;
    }
}

