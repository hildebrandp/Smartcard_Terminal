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
    public partial class smartcard_pukInput : Form
    {
        public string puk { get; set; }

        public smartcard_pukInput()
        {
            InitializeComponent();

            buttonOK.Enabled = false;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.puk = textBox1.Text.ToString().ToLower();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            String allowedChars = "0123456789abcdef";
            if (!char.IsControl(e.KeyChar) && !allowedChars.Contains(e.KeyChar.ToString().ToLower()))
            {
                e.Handled = true;
            }
        }

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
