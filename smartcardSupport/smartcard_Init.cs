using System;
using System.Drawing;
using System.Windows.Forms;


/// <summary>
/// Form-Class for Smartcard Init
/// </summary>
namespace smartcardSupport
{
    public partial class smartcard_Init : Form
    {
        public string pin { get; set; }

        /// <summary>
        /// Constructor, load Form
        /// </summary>
        public smartcard_Init()
        {
            InitializeComponent();
            this.Text = "Personalize Smartcard";

            in_pin_1.UseSystemPasswordChar = true;
            in_pin_2.UseSystemPasswordChar = true;

            in_pin_2.BackColor = Color.Red;
            button_ok.Enabled = false;

            in_pin_1.Focus();
        }

        /// <summary>
        /// Method that handles user input "show", show PIN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Method that handles user input "OK", set PIN and close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ok_Click(object sender, EventArgs e)
        {
            this.pin = in_pin_1.Text.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Method that handles user input "Cancel", close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Method that handles user input into text field
        /// check length of input, enable button ok if long enough and both inputs are the same
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Method that handles user input before insert into text field
        /// Check char
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void in_pin_2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
