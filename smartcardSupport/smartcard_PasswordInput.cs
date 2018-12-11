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
/// Form-Class for User input masterpassword
/// </summary>
namespace smartcardSupport
{
    public partial class smartcard_PasswordInput : Form
    {
        public string masterPassword { get; set; }

        /// <summary>
        /// Constructor, starts Form
        /// </summary>
        public smartcard_PasswordInput()
        {
            InitializeComponent();
            buttonOK.Enabled = true;
            textBoxMasterPassword.UseSystemPasswordChar = true;
        }

        /// <summary>
        /// Method if User clicks "cancel"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Methos if User clicks "OK"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.masterPassword = textBoxMasterPassword.Text.ToString().ToLower();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Method if user clicks "show"
        /// Show Password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBoxMasterPassword.UseSystemPasswordChar = false;
            }
            else
            {
                textBoxMasterPassword.UseSystemPasswordChar = true;
            }
        }
    }
}
