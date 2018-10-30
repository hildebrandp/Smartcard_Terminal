using KeePass.Plugins;
using KeePass.Forms;
using KeePassLib.Keys;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace smartcardSupport
{
    public sealed class smartcardSupportExt : Plugin
    {
        private IPluginHost m_host = null;
        ToolStripMenuItem optionsMenu;

        private smartcardDialog scDialog;

        private int startCode = 0;
        private String fileName = String.Empty;
        private String filePath = String.Empty;
        private String fileModified = String.Empty;

        public override bool Initialize(IPluginHost host)
        {
            if (host == null) return false;
            m_host = host;

            this.optionsMenu = new ToolStripMenuItem("Smartcard Terminal");
            this.optionsMenu.Click += OnOptions_Click;
            //this.optionsMenu.Image = ; // Add image
            m_host.MainWindow.ToolsMenu.DropDownItems.Add(this.optionsMenu);

            m_host.MainWindow.FileSaving += this.OnFileSaving;
            m_host.MainWindow.FileOpened += this.OnFileOpened;
            m_host.MainWindow.FileClosed += this.OnFileClosed;

            return true;
        }

        private void OnOptions_Click(object sender, EventArgs e)
        {
            //scDialog = new smartcardDialog();
            using (var form = new smartcardDialog(startCode, this, fileName, filePath, fileModified))
            {
                var result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    
                }
                else
                {
                    
                }
            }
        }

        public override void Terminate()
        {
            this.m_host.MainWindow.FileSaving -= this.OnFileSaving;
            m_host.MainWindow.FileOpened -= this.OnFileOpened;

            fileName = String.Empty;
        }

        private string getFileName(FileSavingEventArgs e)
        {
            string fName = "";
            if (e.Database.IOConnectionInfo.IsLocalFile())
            {
                // local file
                var f = new FileInfo(e.Database.IOConnectionInfo.Path);
                fName = f.Name;

                // remove file extension
                if (!string.IsNullOrEmpty(f.Extension))
                {
                    fName = fName.Substring(0, fName.Length - f.Extension.Length);
                }

                f = null;
            }
            else
            {
                if (MessageBox.Show("Only Local stored Files allowed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    Terminate();
                }
            }

            return fName;
        }

        private void OnFileSaving(object sender, FileSavingEventArgs e)
        {
            // check if SC is connected
            // if connected : getfilename
            // if localFileisnewer : ask if upload to smartcard


#if false
            string fName = "";
            if (e.Database.IOConnectionInfo.IsLocalFile())
            {
                // local file
                var f = new FileInfo(e.Database.IOConnectionInfo.Path);
                fName = f.FullName;

                f = null;
                var form1 = new smartcardDialog(0, fName);
                form1.ShowDialog();
            }
            else
            {
                if (MessageBox.Show("Only Local stored Files allowed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    Terminate();
                }
            } 
#endif
        }

        private void OnFileClosed(object sender, FileClosedEventArgs e)
        {

        }

        private void OnFileOpened(object sender, FileOpenedEventArgs e)
        {
            string fName = "";
            if (e.Database.IOConnectionInfo.IsLocalFile())
            {
                // local file
                var f = new FileInfo(e.Database.IOConnectionInfo.Path);
                filePath = f.FullName;
                fName = f.Name;

                fileModified = f.LastWriteTime.Year + "-" + f.LastWriteTime.Month.ToString("00") + "-"+ f.LastWriteTime.Day.ToString("00") + "_"+ f.LastWriteTime.Hour.ToString("00") + "-"+ f.LastWriteTime.Minute.ToString("00") + "-"+ f.LastWriteTime.Second.ToString("00");
                
                // remove file extension
                if (!string.IsNullOrEmpty(f.Extension))
                {
                    fileName = fName.Substring(0, fName.Length - f.Extension.Length);
                }

                f = null;
            } 
        }
    }
}
