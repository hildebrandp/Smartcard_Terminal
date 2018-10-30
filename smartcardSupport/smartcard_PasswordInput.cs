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
    public partial class smartcard_PasswordInput : Form
    {
        public string masterPassword { get; set; }

        public smartcard_PasswordInput()
        {
            InitializeComponent();
            buttonOK.Enabled = false;
            textBoxMasterPassword.UseSystemPasswordChar = true;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.masterPassword = textBoxMasterPassword.Text.ToString().ToLower();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void textBoxMasterPassword_TextChanged(object sender, EventArgs e)
        {
            if (textBoxMasterPassword.Text.ToString().Length > 0)
            {
                buttonOK.Enabled = true;
            }
            else
            {
                buttonOK.Enabled = false;
            }
        }

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
