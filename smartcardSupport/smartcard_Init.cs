using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace smartcardSupport
{
    public partial class smartcard_Init : Form
    {
        public string pin { get; set; }

        public smartcard_Init()
        {
            InitializeComponent();

            in_pin_1.UseSystemPasswordChar = true;
            in_pin_2.UseSystemPasswordChar = true;

            in_pin_2.BackColor = Color.Red;
            button_ok.Enabled = false;

            in_pin_1.Focus();
        }

        private void show_pin_CheckStateChanged(object sender, EventArgs e)
        {
            if (show_pin.Checked)
            {
                in_pin_1.UseSystemPasswordChar = false;
                in_pin_2.UseSystemPasswordChar = false;
            }
            else
            {
                in_pin_1.UseSystemPasswordChar = true;
                in_pin_2.UseSystemPasswordChar = true;
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            this.pin = in_pin_1.Text.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void in_pin_2_TextChanged(object sender, EventArgs e)
        {
            if (in_pin_2.Text.ToString().Length >= 4)
            {
                if (in_pin_1.Text.Equals(in_pin_2.Text))
                {
                    in_pin_2.BackColor = Color.Green;
                    in_pin_2.Update();
                    button_ok.Enabled = true;
                }
                else
                {
                    in_pin_2.BackColor = Color.Red;
                    in_pin_2.Update();
                    button_ok.Enabled = false;
                }
            }
            else
            {
                button_ok.Enabled = false;
            }
        }

        private void in_pin_2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
