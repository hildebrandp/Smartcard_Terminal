using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


/// <summary>
/// Form-Class for PUK input
/// </summary>
namespace smartcardSupport
{
    public partial class smartcard_pukInput : Form
    {
        public string puk { get; set; }

        /// <summary>
        /// Constructor load Form
        /// </summary>
        public smartcard_pukInput()
        {
            InitializeComponent();

            buttonOK.Enabled = false;
        }

        /// <summary>
        /// Method for user input "OK"
        /// Set Password and close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.puk = textBox1.Text.ToString().ToLower();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Method for User input "Cancel"
        /// Close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Method that handles User input into Text field before it is shown in text field
        /// Chakc if other char than numbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            String allowedChars = "0123456789abcdef";
            if (!char.IsControl(e.KeyChar) && !allowedChars.Contains(e.KeyChar.ToString().ToLower()))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Method that handles user input after insert into text field
        /// Checks input length, if long enough enable button ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            if (textBox1.Text.ToString().Length == 16)
            {
                buttonOK.Enabled = true;
            }
            else
            {
                buttonOK.Enabled = false;
            }
        }
    }
}
