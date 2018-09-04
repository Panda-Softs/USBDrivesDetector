namespace USBDrivesDetector
{
    partial class UsbDetectorForm
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
            this.textBox = new System.Windows.Forms.TextBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.clearButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.labelMapDrive = new System.Windows.Forms.Label();
            this.mappedDriveComboBox = new System.Windows.Forms.ComboBox();
            this.RemoveBtn = new System.Windows.Forms.Button();
            this.disksList = new System.Windows.Forms.ListBox();
            this.Addbutton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Location = new System.Drawing.Point(0, 142);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(862, 156);
            this.textBox.TabIndex = 0;
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "USB Device detector";
            this.notifyIcon.Visible = true;
            this.notifyIcon.Click += new System.EventHandler(this.OnNotifyIconClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.clearButton);
            this.panel1.Controls.Add(this.statusLabel);
            this.panel1.Controls.Add(this.labelMapDrive);
            this.panel1.Controls.Add(this.mappedDriveComboBox);
            this.panel1.Controls.Add(this.RemoveBtn);
            this.panel1.Controls.Add(this.disksList);
            this.panel1.Controls.Add(this.Addbutton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(862, 142);
            this.panel1.TabIndex = 6;
            // 
            // clearButton
            // 
            this.clearButton.AutoSize = true;
            this.clearButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearButton.Location = new System.Drawing.Point(789, 9);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(51, 27);
            this.clearButton.TabIndex = 13;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ForeColor = System.Drawing.Color.Red;
            this.statusLabel.Location = new System.Drawing.Point(258, 15);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(366, 28);
            this.statusLabel.TabIndex = 11;
            this.statusLabel.Text = "[Status]";
            // 
            // labelMapDrive
            // 
            this.labelMapDrive.AutoSize = true;
            this.labelMapDrive.Location = new System.Drawing.Point(10, 15);
            this.labelMapDrive.Name = "labelMapDrive";
            this.labelMapDrive.Size = new System.Drawing.Size(115, 20);
            this.labelMapDrive.TabIndex = 10;
            this.labelMapDrive.Text = "Mapped Drive :";
            // 
            // mappedDriveComboBox
            // 
            this.mappedDriveComboBox.FormattingEnabled = true;
            this.mappedDriveComboBox.Location = new System.Drawing.Point(133, 10);
            this.mappedDriveComboBox.Name = "mappedDriveComboBox";
            this.mappedDriveComboBox.Size = new System.Drawing.Size(99, 28);
            this.mappedDriveComboBox.TabIndex = 9;
            // 
            // RemoveBtn
            // 
            this.RemoveBtn.Location = new System.Drawing.Point(702, 92);
            this.RemoveBtn.Name = "RemoveBtn";
            this.RemoveBtn.Size = new System.Drawing.Size(148, 42);
            this.RemoveBtn.TabIndex = 8;
            this.RemoveBtn.Text = "Remove Drive";
            this.RemoveBtn.UseVisualStyleBackColor = true;
            this.RemoveBtn.Click += new System.EventHandler(this.OnRemoveButtonClick);
            // 
            // disksList
            // 
            this.disksList.DisplayMember = "serialMember";
            this.disksList.FormattingEnabled = true;
            this.disksList.ItemHeight = 20;
            this.disksList.Location = new System.Drawing.Point(12, 46);
            this.disksList.Name = "disksList";
            this.disksList.Size = new System.Drawing.Size(685, 84);
            this.disksList.TabIndex = 7;
            this.disksList.ValueMember = "serialMember";
            // 
            // Addbutton
            // 
            this.Addbutton.Location = new System.Drawing.Point(702, 45);
            this.Addbutton.Name = "Addbutton";
            this.Addbutton.Size = new System.Drawing.Size(148, 44);
            this.Addbutton.TabIndex = 6;
            this.Addbutton.Text = "Add Drive";
            this.Addbutton.UseVisualStyleBackColor = true;
            this.Addbutton.Click += new System.EventHandler(this.OnAddDriveButtonClick);
            // 
            // UsbDetectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 298);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "UsbDetectorForm";
            this.Text = "USB Disks Detector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnUsbDetectorFormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelMapDrive;
        private System.Windows.Forms.ComboBox mappedDriveComboBox;
        private System.Windows.Forms.Button RemoveBtn;
        private System.Windows.Forms.ListBox disksList;
        private System.Windows.Forms.Button Addbutton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button clearButton;
    }
}

