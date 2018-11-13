namespace smartcardSupport
{
    partial class smartcardDialog
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
            this.barcodePicture = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxConnectedDevice = new System.Windows.Forms.TextBox();
            this.pictureBoxBluetoothEnable = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxSystemLog = new System.Windows.Forms.ListBox();
            this.button_Import_File = new System.Windows.Forms.Button();
            this.button_Export_File = new System.Windows.Forms.Button();
            this.button_UnlockDatabase = new System.Windows.Forms.Button();
            this.button_OpenDatabase = new System.Windows.Forms.Button();
            this.button_Set_MPW = new System.Windows.Forms.Button();
            this.button_Get_MPW = new System.Windows.Forms.Button();
            this.button_Delete_Data = new System.Windows.Forms.Button();
            this.button_Reset_Card = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.barcodePicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBluetoothEnable)).BeginInit();
            this.SuspendLayout();
            // 
            // barcodePicture
            // 
            this.barcodePicture.Location = new System.Drawing.Point(30, 30);
            this.barcodePicture.Name = "barcodePicture";
            this.barcodePicture.Size = new System.Drawing.Size(400, 400);
            this.barcodePicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.barcodePicture.TabIndex = 0;
            this.barcodePicture.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(436, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(284, 37);
            this.label1.TabIndex = 3;
            this.label1.Text = "Connected Device:";
            // 
            // textBoxConnectedDevice
            // 
            this.textBoxConnectedDevice.Cursor = System.Windows.Forms.Cursors.No;
            this.textBoxConnectedDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxConnectedDevice.Location = new System.Drawing.Point(443, 70);
            this.textBoxConnectedDevice.Name = "textBoxConnectedDevice";
            this.textBoxConnectedDevice.ReadOnly = true;
            this.textBoxConnectedDevice.Size = new System.Drawing.Size(463, 44);
            this.textBoxConnectedDevice.TabIndex = 4;
            // 
            // pictureBoxBluetoothEnable
            // 
            this.pictureBoxBluetoothEnable.Image = global::smartcardSupport.Properties.Resources.Apps_Bluetooth_Inactive_icon;
            this.pictureBoxBluetoothEnable.Location = new System.Drawing.Point(923, 30);
            this.pictureBoxBluetoothEnable.Name = "pictureBoxBluetoothEnable";
            this.pictureBoxBluetoothEnable.Size = new System.Drawing.Size(100, 100);
            this.pictureBoxBluetoothEnable.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxBluetoothEnable.TabIndex = 5;
            this.pictureBoxBluetoothEnable.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(436, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(194, 37);
            this.label2.TabIndex = 7;
            this.label2.Text = "System Log:";
            // 
            // listBoxSystemLog
            // 
            this.listBoxSystemLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxSystemLog.FormattingEnabled = true;
            this.listBoxSystemLog.HorizontalScrollbar = true;
            this.listBoxSystemLog.ItemHeight = 20;
            this.listBoxSystemLog.Location = new System.Drawing.Point(443, 157);
            this.listBoxSystemLog.Name = "listBoxSystemLog";
            this.listBoxSystemLog.Size = new System.Drawing.Size(580, 264);
            this.listBoxSystemLog.TabIndex = 8;
            // 
            // button_Import_File
            // 
            this.button_Import_File.Enabled = false;
            this.button_Import_File.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Import_File.Location = new System.Drawing.Point(277, 436);
            this.button_Import_File.Name = "button_Import_File";
            this.button_Import_File.Size = new System.Drawing.Size(203, 49);
            this.button_Import_File.TabIndex = 9;
            this.button_Import_File.Text = "Import File";
            this.button_Import_File.UseVisualStyleBackColor = true;
            this.button_Import_File.Click += new System.EventHandler(this.button_Import_File_Click);
            // 
            // button_Export_File
            // 
            this.button_Export_File.Enabled = false;
            this.button_Export_File.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Export_File.Location = new System.Drawing.Point(277, 491);
            this.button_Export_File.Name = "button_Export_File";
            this.button_Export_File.Size = new System.Drawing.Size(203, 49);
            this.button_Export_File.TabIndex = 10;
            this.button_Export_File.Text = "Export File";
            this.button_Export_File.UseVisualStyleBackColor = true;
            this.button_Export_File.Click += new System.EventHandler(this.button_Export_File_Click);
            // 
            // button_UnlockDatabase
            // 
            this.button_UnlockDatabase.Enabled = false;
            this.button_UnlockDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_UnlockDatabase.Location = new System.Drawing.Point(30, 436);
            this.button_UnlockDatabase.Name = "button_UnlockDatabase";
            this.button_UnlockDatabase.Size = new System.Drawing.Size(241, 49);
            this.button_UnlockDatabase.TabIndex = 11;
            this.button_UnlockDatabase.Text = "Unlock Database";
            this.button_UnlockDatabase.UseVisualStyleBackColor = true;
            this.button_UnlockDatabase.Click += new System.EventHandler(this.button_UnlockDatabase_Click);
            // 
            // button_OpenDatabase
            // 
            this.button_OpenDatabase.Enabled = false;
            this.button_OpenDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_OpenDatabase.Location = new System.Drawing.Point(30, 491);
            this.button_OpenDatabase.Name = "button_OpenDatabase";
            this.button_OpenDatabase.Size = new System.Drawing.Size(241, 49);
            this.button_OpenDatabase.TabIndex = 12;
            this.button_OpenDatabase.Text = "Open Database";
            this.button_OpenDatabase.UseVisualStyleBackColor = true;
            this.button_OpenDatabase.Click += new System.EventHandler(this.button_OpenDatabase_Click);
            // 
            // button_Set_MPW
            // 
            this.button_Set_MPW.Enabled = false;
            this.button_Set_MPW.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Set_MPW.Location = new System.Drawing.Point(486, 436);
            this.button_Set_MPW.Name = "button_Set_MPW";
            this.button_Set_MPW.Size = new System.Drawing.Size(262, 49);
            this.button_Set_MPW.TabIndex = 13;
            this.button_Set_MPW.Text = "Set Master Password";
            this.button_Set_MPW.UseVisualStyleBackColor = true;
            this.button_Set_MPW.Click += new System.EventHandler(this.button_Set_MPW_Click);
            // 
            // button_Get_MPW
            // 
            this.button_Get_MPW.Enabled = false;
            this.button_Get_MPW.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Get_MPW.Location = new System.Drawing.Point(486, 491);
            this.button_Get_MPW.Name = "button_Get_MPW";
            this.button_Get_MPW.Size = new System.Drawing.Size(262, 49);
            this.button_Get_MPW.TabIndex = 14;
            this.button_Get_MPW.Text = "Get Master Password";
            this.button_Get_MPW.UseVisualStyleBackColor = true;
            this.button_Get_MPW.Click += new System.EventHandler(this.button_Get_MPW_Click);
            // 
            // button_Delete_Data
            // 
            this.button_Delete_Data.Enabled = false;
            this.button_Delete_Data.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Delete_Data.Location = new System.Drawing.Point(754, 436);
            this.button_Delete_Data.Name = "button_Delete_Data";
            this.button_Delete_Data.Size = new System.Drawing.Size(203, 49);
            this.button_Delete_Data.TabIndex = 15;
            this.button_Delete_Data.Text = "Delete all Data";
            this.button_Delete_Data.UseVisualStyleBackColor = true;
            this.button_Delete_Data.Click += new System.EventHandler(this.button_Delete_Data_Click);
            // 
            // button_Reset_Card
            // 
            this.button_Reset_Card.Enabled = false;
            this.button_Reset_Card.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Reset_Card.Location = new System.Drawing.Point(754, 491);
            this.button_Reset_Card.Name = "button_Reset_Card";
            this.button_Reset_Card.Size = new System.Drawing.Size(203, 49);
            this.button_Reset_Card.TabIndex = 16;
            this.button_Reset_Card.Text = "Reset Card";
            this.button_Reset_Card.UseVisualStyleBackColor = true;
            this.button_Reset_Card.Click += new System.EventHandler(this.button_Reset_Card_Click);
            // 
            // smartcardDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1049, 556);
            this.Controls.Add(this.button_Reset_Card);
            this.Controls.Add(this.button_Delete_Data);
            this.Controls.Add(this.button_Get_MPW);
            this.Controls.Add(this.button_Set_MPW);
            this.Controls.Add(this.button_OpenDatabase);
            this.Controls.Add(this.button_UnlockDatabase);
            this.Controls.Add(this.button_Export_File);
            this.Controls.Add(this.button_Import_File);
            this.Controls.Add(this.listBoxSystemLog);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBoxBluetoothEnable);
            this.Controls.Add(this.textBoxConnectedDevice);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.barcodePicture);
            this.Name = "smartcardDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "smartcardDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.smartcardDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.barcodePicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBluetoothEnable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox barcodePicture;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxConnectedDevice;
        private System.Windows.Forms.PictureBox pictureBoxBluetoothEnable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBoxSystemLog;
        private System.Windows.Forms.Button button_Import_File;
        private System.Windows.Forms.Button button_Export_File;
        private System.Windows.Forms.Button button_UnlockDatabase;
        private System.Windows.Forms.Button button_OpenDatabase;
        private System.Windows.Forms.Button button_Set_MPW;
        private System.Windows.Forms.Button button_Get_MPW;
        private System.Windows.Forms.Button button_Delete_Data;
        private System.Windows.Forms.Button button_Reset_Card;
    }
}