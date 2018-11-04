using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace smartcardSupport
{
    public partial class smartcard_pinInput : Form
    {
        public string PIN { get; set; }

        public smartcard_pinInput()
        {
            InitializeComponent();
            buttonOK.Enabled = false;
            inputPIN.UseSystemPasswordChar = true; 
        }

        private void inputPIN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals("\n") && inputPIN.Text.ToString().Length >= 4)
            {
                this.PIN = inputPIN.Text.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void inputPIN_TextChanged(object sender, EventArgs e)
        {
            if (inputPIN.Text.ToString().Length >= 4)
            {
                buttonOK.Enabled = true;
            }
            else
            {
                buttonOK.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button btnSender = sender as Button;
            switch (btnSender.Name)
            {
                case "button1":
                    inputPIN.Text += "1";
                    break;
                case "button2":
                    inputPIN.Text += "2";
                    break;
                case "button3":
                    inputPIN.Text += "3";
                    break;
                case "button4":
                    inputPIN.Text += "4";
                    break;
                case "button5":
                    inputPIN.Text += "5";
                    break;
                case "button6":
                    inputPIN.Text += "6";
                    break;
                case "button7":
                    inputPIN.Text += "7";
                    break;
                case "button8":
                    inputPIN.Text += "8";
                    break;
                case "button9":
                    inputPIN.Text += "9";
                    break;
                case "button10":
                    inputPIN.Text += "0";
                    break;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            inputPIN.Text = inputPIN.Text.Substring(0, inputPIN.Text.Length - 1);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.PIN = inputPIN.Text.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void checkBoxShowPIN_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxShowPIN.Checked)
            {
                inputPIN.UseSystemPasswordChar = false;
            }
            else
            {
                inputPIN.UseSystemPasswordChar = true;
            }
        }

        private void smartcard_pinInput_Shown(object sender, EventArgs e)
        {
            inputPIN.Focus();
        }
    }
}
