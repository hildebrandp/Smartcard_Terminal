namespace smartcardSupport
{
    partial class smartcard_Init
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.in_pin_1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.in_pin_2 = new System.Windows.Forms.TextBox();
            this.button_ok = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.show_pin = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.MaximumSize = new System.Drawing.Size(560, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(555, 93);
            this.label1.TabIndex = 0;
            this.label1.Text = "Smartcard is not personalized. Only numbers are allowed and PIN must be between 4" +
    " - 32 characters long.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(249, 31);
            this.label2.TabIndex = 1;
            this.label2.Text = "Please enter a PIN:";
            // 
            // in_pin_1
            // 
            this.in_pin_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.in_pin_1.Location = new System.Drawing.Point(18, 137);
            this.in_pin_1.MaxLength = 32;
            this.in_pin_1.Name = "in_pin_1";
            this.in_pin_1.Size = new System.Drawing.Size(549, 38);
            this.in_pin_1.TabIndex = 2;
            this.in_pin_1.TextChanged += new System.EventHandler(this.in_pin_2_TextChanged);
            this.in_pin_1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.in_pin_2_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(18, 182);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(300, 31);
            this.label3.TabIndex = 3;
            this.label3.Text = "Please enter PIN again:";
            // 
            // in_pin_2
            // 
            this.in_pin_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.in_pin_2.Location = new System.Drawing.Point(18, 217);
            this.in_pin_2.MaxLength = 32;
            this.in_pin_2.Name = "in_pin_2";
            this.in_pin_2.Size = new System.Drawing.Size(549, 38);
            this.in_pin_2.TabIndex = 4;
            this.in_pin_2.TextChanged += new System.EventHandler(this.in_pin_2_TextChanged);
            this.in_pin_2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.in_pin_2_KeyPress);
            // 
            // button_ok
            // 
            this.button_ok.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ok.Location = new System.Drawing.Point(237, 261);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(112, 39);
            this.button_ok.TabIndex = 6;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_cancel.Location = new System.Drawing.Point(381, 261);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(186, 39);
            this.button_cancel.TabIndex = 7;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // show_pin
            // 
            this.show_pin.AutoSize = true;
            this.show_pin.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.show_pin.Location = new System.Drawing.Point(18, 265);
            this.show_pin.Name = "show_pin";
            this.show_pin.Size = new System.Drawing.Size(101, 35);
            this.show_pin.TabIndex = 8;
            this.show_pin.Text = "Show";
            this.show_pin.UseVisualStyleBackColor = true;
            this.show_pin.CheckStateChanged += new System.EventHandler(this.show_pin_CheckStateChanged);
            // 
            // smartcard_Init
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 324);
            this.Controls.Add(this.show_pin);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.in_pin_2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.in_pin_1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "smartcard_Init";
            this.Text = "smartcard_Init";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox in_pin_1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox in_pin_2;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.CheckBox show_pin;
    }
}