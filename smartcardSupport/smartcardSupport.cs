using KeePass.Plugins;
using KeePass.Forms;
using KeePass;
using System;
using System.Windows.Forms;
using System.IO;
using KeePassLib.Keys;
using KeePassLib.Utility;
using KeePassLib.Serialization;
using System.Threading;

/// <summary>
/// KeePass Plugin Class
/// This Class is started with the KeePass Software
/// </summary>
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

        private String openPath = String.Empty;

        /// <summary>
        /// Constructor which creates Menu item for starting the interface
        /// </summary>
        /// <param name="host">KeePass instance</param>
        /// <returns></returns>
        public override bool Initialize(IPluginHost host)
        {
            if (host == null) return false;
            m_host = host;

            this.optionsMenu = new ToolStripMenuItem("Smartcard-Support");
            this.optionsMenu.Click += OnOptions_Click;
            //this.optionsMenu.Image = ; // Add image
            m_host.MainWindow.ToolsMenu.DropDownItems.Add(this.optionsMenu);

            m_host.MainWindow.FileOpened += this.OnFileOpened;
            m_host.MainWindow.FileClosed += this.OnFileClosed;

            
            return true;
        }

        /// <summary>
        /// MEthod which is called when user clicks on menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Event args</param>
        private void OnOptions_Click(object sender, EventArgs e)
        {
            //t = new Thread(() => openSmartcardTerminal());
            //t.Start();
            openSmartcardTerminal();
        }

        /// <summary>
        /// Methed that starts the graphical interface
        /// </summary>
        private void openSmartcardTerminal()
        {
            String lastFile = "";
            IOConnectionInfo ioLastFile = Program.Config.Application.LastUsedFile;
            if (ioLastFile.Path.Length > 0)
            {
                lastFile = ioLastFile.Path;
            }

            using (var form = new smartcardDialog(startCode, this, fileName, filePath, fileModified, lastFile))
            {
                var result = form.ShowDialog();
                String path = form.openFilePath;
                String pw = form.openFilePW;

                if (result == DialogResult.OK)
                {
                    if (form.openFile)
                    {
                        if (path != null)
                        {
                            openPath = path;
                            if (!pw.Equals(String.Empty))
                            {
                                openDatabase(path, pw);
                            }
                            else
                            {
                                openDatabase(path);
                            }
                        }
                    }
                }
                else if (result == DialogResult.Yes)
                {
                    openLastFile(pw);
                }
            }
        }

        /// <summary>
        /// Method for checking if Database have unsaved entries
        /// </summary>
        /// <returns></returns>
        public Boolean checkUnsavedEntries()
        {
            return m_host.Database.Modified;
        }

        /// <summary>
        /// Method for opening Database without password
        /// </summary>
        /// <param name="filePath">filepath of database</param>
        public void openDatabase(String filePath)
        {
            IOConnectionInfo conInfo = IOConnectionInfo.FromPath(filePath);
            Program.MainForm.OpenDatabase(conInfo, null, false); 
        }

        /// <summary>
        /// Method for opening and decypting Database
        /// </summary>
        /// <param name="filePath">filepath of database</param>
        /// <param name="password">password of Database</param>
        public void openDatabase(String filePath, String password)
        {
            CompositeKey cmpKey = new CompositeKey();

            byte[] pbPw = StrUtil.Utf8.GetBytes(password);
            cmpKey.AddUserKey(new KcpPassword(pbPw, Program.Config.Security.MasterPassword.RememberWhileOpen));

            IOConnectionInfo conInfo = IOConnectionInfo.FromPath(filePath);
            Program.MainForm.OpenDatabase(conInfo, cmpKey, false);
        }  

        /// <summary>
        /// Method that opens last used local Database with password
        /// </summary>
        /// <param name="password">passowrd of database</param>
        private void openLastFile(String password)
        {
            CompositeKey cmpKey = new CompositeKey();

            byte[] pbPw = StrUtil.Utf8.GetBytes(password);
            cmpKey.AddUserKey(new KcpPassword(pbPw, Program.Config.Security.MasterPassword.RememberWhileOpen));

            IOConnectionInfo ioLastFile = Program.Config.Application.LastUsedFile;
            if (ioLastFile.Path.Length > 0)
            {
                Program.MainForm.OpenDatabase(ioLastFile, cmpKey, false);
            }
        }

        /// <summary>
        /// Method that returns name of akcive database
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Method which is called wehn KeePass is closed
        /// important to deactivate EventListener
        /// </summary>
        public override void Terminate()
        {
            m_host.MainWindow.FileOpened -= this.OnFileOpened;
            m_host.MainWindow.FileClosed -= this.OnFileClosed;
        }

        /// <summary>
        /// EventListener Method, called if User closes Database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileClosed(object sender, FileClosedEventArgs e)
        {
            if (openPath.Equals(e.IOConnectionInfo.Path))
            {
                if (MessageBox.Show("Delete Database?", "Database closed", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    File.Delete(openPath);
                }
            }
        }

        /// <summary>
        /// EventListener Method, called if user opens database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
