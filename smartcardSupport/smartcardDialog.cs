﻿using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using Smartcard_Terminal;

namespace smartcardSupport
{
    public partial class smartcardDialog : Smartcard_Terminal.Smartcard_Terminal
    {
        private smartcardSupportExt _scSupport;
        private smartcard_APDU_Codes _scCodes;

        private String btAdress;
        private String deviceName;
        private Boolean closing = false;

        private Boolean is_Smartcard_App_Connected = false;
        private String smartcardData;
        private String smartcardCode;
        private String lastCommand;
        private String prevCommand;

        private Boolean isSmartcardAuthenticated = false;
        private Boolean masterPassword = false;
        private String scPassword = String.Empty;
        private String smartcardState = String.Empty;
        private String smartcardFileName = String.Empty;
        private String smartcardFileModified = String.Empty;
        private int smartcardFileSize_1 = 0;
        private int smartcardFileSize_2 = 0;

        private Boolean smartcardHasFile = false;

        private String fileName = String.Empty;
        private String filePath = String.Empty;
        private String fileModified = String.Empty;
        private String lastFile = String.Empty;

        private String exportData = String.Empty;
        private String exportLength = String.Empty;
        private int file_1_size = 0;
        private int file_2_size = 0;
        private StringBuilder newKeePassFile;
        private int fileOffset = 0;
        private int readLength = 250;
        private Boolean readFile_1 = true;
        private Boolean readFile_2 = true;
        private String pathForFile = String.Empty;
        private String tmpPUK = String.Empty;

        public Boolean unlockFile = false;
        public Boolean openFile { get; set; }
        public string openFilePath { get; set; }
        public string openFilePW { get; set; }


        public Boolean debug = false;
        Stopwatch sw1 = new Stopwatch();
        Stopwatch sw2 = new Stopwatch();

        private Thread startBTCon;

        //Contructor for Form Class
        public smartcardDialog(int startcode, smartcardSupportExt scSupportExt, String fileName, String filePath, String fileModified, String lastFile)
        {
            InitializeComponent();

            newMessagereceived += new newMessageHandler(newMessage);

            openFile = false;
            openFilePath = String.Empty;
            openFilePW = String.Empty;

            this.fileModified = fileModified;
            this.fileName = fileName;
            this.filePath = filePath;
            this.lastFile = lastFile;

            //Add Event if Form is Closing
            this.FormClosing += new FormClosingEventHandler(smartcardDialog_FormClosing);

            //Get Version number and Set Window title
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            this.Text = "Bluetooth SmartCard Reader Terminal- " + version + "  ||  Database: " + fileName;

            _scCodes = new smartcard_APDU_Codes();
            _scSupport = scSupportExt;

            if (_scSupport.checkUnsavedEntries())
            {
                if (MessageBox.Show("Database has unsaved changes. Save changes (Yes) or continue without saving (No)?", "Export Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.Close();
                }
            }

            btAdress = Get_BT_Address();
            if (btAdress == null)
            {
                msgBoxClose("No Accessible Bluetooth Device! Closing");
            }
            else
            {
                systemLog("Bluetooth enabled");
                pictureBoxBluetoothEnable.Image = Properties.Resources.Apps_Bluetooth_Inactive_icon;
                textBoxConnectedDevice.Text = "";
                barcodePicture.Image = Gen_QRCode(true, 0);

                if (startBluetoothCon())
                {
                    startBTCon = new Thread(startBTListener);
                    startBTCon.Start();
                }
                else
                {
                    systemLog("Error starting Bluetooth Device.");
                }

                
            }
        }

        private void startBTListener()
        {
            if (open_BT_Connection())
            {
                BT_Connection(true);
            }
            else
            {
                BT_Connection(false);
            }
        }

        private delegate void BT_ConnectionDelegate(Boolean success);
        private void BT_Connection(Boolean success)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new BT_ConnectionDelegate(this.BT_Connection), success);
            }
            else
            {
                if (success)
                {
                    systemLog("Successfully Connected");
                    deviceName = Get_BT_Name();
                    textBoxConnectedDevice.Text = deviceName;
                }
                else
                {
                    systemLog("Error establishing connection");
                }
            }
        }

        private void newMessage(object a, newMessageEventArgs e)
        {
            receiveMessage(e.Code, e.Message);
        }

        private String encryptData(String data, Boolean pad)
        {
            String pw = CryptLib.getHashSha256(_scCodes.hexToByteArray(_scCodes.checkLength(scPassword)), 64);
            RijndaelManaged aesAlg = new RijndaelManaged
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None,
                KeySize = 256,
                BlockSize = 128,
                Key = _scCodes.hexToByteArray(pw),
            };
            String paddedData;
            if (pad)
            {
                paddedData = _scCodes.checkLength(scPassword);
                paddedData = paddedData.PadLeft(32, 'F');
            } else
            {
                paddedData = data;
            }
            
            ICryptoTransform encryptor = aesAlg.CreateEncryptor();
            var encData = encryptor.TransformFinalBlock(_scCodes.hexToByteArray(paddedData), 0, _scCodes.hexToByteArray(paddedData).Length);

            //systemLog("Data: " + _scCodes.byteToString(encData));
            return _scCodes.byteToString(encData);
        }

        private String decryptData(String data)
        {
            String pw = CryptLib.getHashSha256(_scCodes.hexToByteArray(_scCodes.checkLength(scPassword)), 64);
            RijndaelManaged aesAlg = new RijndaelManaged
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None,
                KeySize = 256,
                BlockSize = 128,
                Key = _scCodes.hexToByteArray(pw),
            };
            ICryptoTransform decryptor = aesAlg.CreateDecryptor();
            var decData = decryptor.TransformFinalBlock(_scCodes.hexToByteArray(data), 0, _scCodes.hexToByteArray(data).Length);

            //systemLog("Data: " + _scCodes.byteToString(decData));
            return _scCodes.byteToString(decData);
        }

        private delegate void clipboardDelegate(String txt);
        private void clipboard(String txt)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new clipboardDelegate(this.clipboard), txt);
            }
            else
            {
                Clipboard.SetText(txt);
            }
        }

        private delegate void msgBoxCloseDelegate(String txt);
        private void msgBoxClose(String txt)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new msgBoxCloseDelegate(this.msgBoxClose), txt);
            }
            else
            {
                MessageBox.Show(txt, "Smartcard Terminal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                closing = true;
                this.Close();
            }
        }

        private void smartcardDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closing)
            {
                if (MessageBox.Show("Are you sure you want to Exit?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Stop_BT_Connection();
                    return;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                Stop_BT_Connection();
                return;
            }

        }

        /// <summary>
        /// Method for writing log files to the systemlog window
        /// </summary>
        /// <param name="message">String that contains the Log message</param>
        private delegate void systemLogDelegate(String message);
        public void systemLog(String message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new systemLogDelegate(this.systemLog), message);
            }
            else
            {
                String logTime = DateTime.Now.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
                listBoxSystemLog.Items.Insert(0, logTime + " >> " + message);
            }
        }

        public void updateSystemLog(String txt, double percent)
        {
            listBoxSystemLog.Items.RemoveAt(0);
            String logTime = DateTime.Now.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
            String per = (percent * 100).ToString("00.00");
            listBoxSystemLog.Items.Insert(0, logTime + " >> " + txt + " " + per + " %");
        }

        private delegate void receiveMessageDelegate(int code, String data);
        public void receiveMessage(int code, String data)
        {      
            if (this.InvokeRequired)
            {
                this.Invoke(new receiveMessageDelegate(this.receiveMessage), code, data);
            }
            else
            {
                switch (code)
                {
                    case 1:
                        if (Get_BT_is_Connected())
                        {
                            switch (data)
                            {
                                case "application_stop":
                                    btChangeState("disconnected");
                                    break;
                                case "smartcard_discovered":
                                    btChangeState("smartcardDiscovered");
                                    break;
                                case "smartcard_connection_refused":
                                    systemLog("Smartcard connection refused.");
                                    break;
                                case "smartcard_connected":
                                    btChangeState("smartcardConnected");
                                    break;
                                case "smartcard_disconnected":
                                    btChangeState("smartcardDisconnected");
                                    break;
                                default:
                                    systemLog("Error Receiving Message");
                                    break;
                            }
                        }
                        break;
                    case 2:
                        if (Get_BT_is_Connected())
                        {
                            smartcardData = _scCodes.getSmartcardResponse(data)[0];
                            smartcardCode = _scCodes.getSmartcardResponse(data)[1];

                            prevCommand = lastCommand;
                            lastCommand = "";

                            if (debug)
                            {
                                systemLog("Command >" + prevCommand + "< Data: >" + smartcardData + "< Code: >" + smartcardCode + "<");
                            }

                            if (!is_Smartcard_App_Connected && prevCommand.Equals("apduSelectApplet") && smartcardCode.Equals("9000"))
                            {
                                btChangeState("scAppletConnected");
                                smartcardState = smartcardData;
                            }

                            if (is_Smartcard_App_Connected)
                            {
                                if (prevCommand.Equals("card_personalize") && smartcardCode.Equals("9000"))
                                {
                                    initSmartcardResponse(smartcardData);
                                    systemLog("Command >" + prevCommand + "< Successful");
                                    break;
                                }
                                else if (prevCommand.Equals("card_personalize") && !smartcardCode.Equals("9000"))
                                {
                                    systemLog("Smartcard Error >" + smartcardCode + "<");
                                }

                                if (prevCommand.Equals("card_verify") && smartcardCode.Equals("9000"))
                                {
                                    isSmartcardAuthenticated = true;
                                    // get Master PW State
                                    if (smartcardData.Equals("01"))
                                    {
                                        masterPassword = true;
                                    }

                                    systemLog("Card Unlocked");
                                    cardUnlocked(false);

                                    break;
                                }
                                else if (prevCommand.Equals("card_verify") && !smartcardCode.Equals("9000"))
                                {
                                    switch (smartcardCode)
                                    {
                                        case "63C2":
                                            systemLog("Password wrong! 2 Tries remaining.");
                                            verifyPIN(false);
                                            break;
                                        case "63C1":
                                            systemLog("Password wrong! 1 Tries remaining.");
                                            verifyPIN(false);
                                            break;
                                        case "63C0":
                                            systemLog("Password wrong! 0 Tries remaining.");
                                            systemLog("PIN is Locked. Use PUK to unlock.");
                                            pinIsBlocked(false);
                                            break;
                                    }
                                    break;
                                }

                                if (prevCommand.Equals("card_pin_change") && smartcardCode.Equals("9000"))
                                {
                                    systemLog("PIN change successfull");

                                    break;
                                }
                                else if (prevCommand.Equals("card_pin_change") && !smartcardCode.Equals("9000"))
                                {
                                    systemLog("Error changing PIN");
                                    break;
                                }

                                if (prevCommand.Equals("pin_locked") && smartcardCode.Equals("9000"))
                                {
                                    systemLog("PIN successfully reset");
                                    verifyPIN(true);
                                    break;
                                }
                                else if (prevCommand.Equals("pin_locked") && smartcardCode.Equals("9090"))
                                {
                                    pinIsBlocked(true);
                                    return;
                                }
                                else if (prevCommand.Equals("pin_locked") && !smartcardCode.Equals("9000"))
                                {
                                    switch (smartcardCode)
                                    {
                                        case "63C2":
                                            systemLog("PUK wrong! 2 Tries remaining.");
                                            pinIsBlocked(false);
                                            break;
                                        case "63C1":
                                            systemLog("PUK wrong! 1 Tries remaining.");
                                            pinIsBlocked(false);
                                            break;
                                        case "63C0":
                                            systemLog("PUK wrong! 0 Tries remaining.");
                                            systemLog("Smartcard is Locked!!!");
                                            break;
                                    }
                                    break;
                                }

                                if (smartcardData == _scCodes.STATE_INIT && smartcardCode.Equals("9000") && !isSmartcardAuthenticated)
                                {
                                    //Init Card
                                    initSmartcard();
                                    break;
                                }
                                else if (smartcardData == _scCodes.STATE_PIN_LOCKED && smartcardCode.Equals("9000") && !isSmartcardAuthenticated)
                                {
                                    //PIN is locked
                                    btChangeState("scAppletPINBlocked");
                                    pinIsBlocked(false);
                                    break;
                                }
                                else if ((smartcardData == _scCodes.STATE_SECURE_DATA || smartcardData == _scCodes.STATE_SECURE_NO_DATA) && smartcardCode.Equals("9000") && !isSmartcardAuthenticated)
                                {
                                    //Verify PIN
                                    verifyPIN(false);
                                    break;
                                }

                                if (prevCommand.Equals("check_file") && smartcardCode.Equals("9000"))
                                {
                                    smartcardHasFile = true;
                                    String file = _scCodes.dataHexToString(smartcardData);
                                    int cardFileLength = file.Length;

                                    smartcardFileModified = file.Substring(cardFileLength - 24, 19);
                                    smartcardFileName = file.Substring(0, cardFileLength - 25);

                                    if (debug)
                                    {
                                        systemLog("Found File <" + smartcardFileName + "> Modified <" + smartcardFileModified + ">");
                                    }

                                    cardUnlocked(true);
                                    break;
                                }
                                else if (prevCommand.Equals("check_file") && !smartcardCode.Equals("9000"))
                                {
                                    smartcardHasFile = false;
                                    systemLog("Error getting Filename");
                                }

                                if (prevCommand.Equals("card_masterPW_set") && smartcardCode.Equals("9000"))
                                {
                                    systemLog("Master Password successfully uploaded");
                                    masterPassword = true;
                                    cardUnlocked(true);
                                    break;
                                }
                                else if (prevCommand.Equals("card_masterPW_set") && !smartcardCode.Equals("9000"))
                                {
                                    systemLog("Master Password upload failed");
                                    break;
                                }

                                if (prevCommand.Equals("card_masterPW_delete") && smartcardCode.Equals("9000"))
                                {
                                    systemLog("Masster Password delete successfull");
                                    masterPassword = false;
                                    cardUnlocked(true);
                                    break;
                                }
                                else if (prevCommand.Equals("card_masterPW_delete") && !smartcardCode.Equals("9000"))
                                {
                                    systemLog("Masster Password delete failed");
                                    break;
                                }

                                if (prevCommand.Equals("card_masterPW_get") && smartcardCode.Equals("9000"))
                                {
                                    smartcardData = decryptData(smartcardData);

                                    String substr = smartcardData.Substring(0, 2);
                                    int Datalen = int.Parse(substr, NumberStyles.HexNumber);
                                    smartcardData = smartcardData.Substring(smartcardData.Length - Datalen * 2, Datalen * 2);

                                    if (openFile)
                                    {
                                        openFilePW = _scCodes.dataHexToString(smartcardData);
                                        openKDBXFile();
                                        break;
                                    }
                                    else if (unlockFile)
                                    {
                                        openFilePW = _scCodes.dataHexToString(smartcardData);
                                        unlockDB();
                                        break;
                                    }
                                    else
                                    {
                                        systemLog("Master Password copied to Clipboard");
                                        clipboard(_scCodes.dataHexToString(smartcardData));
                                        break;
                                    }
                                }
                                else if (prevCommand.Equals("card_masterPW_get") && !smartcardCode.Equals("9000"))
                                {
                                    systemLog("Getting Master Password Failed");
                                    break;
                                }

                                if (prevCommand.Equals("card_file_size") && smartcardCode.Equals("9000"))
                                {
                                    String FileSize_1 = smartcardData.Substring(0, 4);
                                    String FileSize_2 = smartcardData.Substring(4, 4);

                                    smartcardFileSize_1 = Convert.ToInt32(FileSize_1, 16);
                                    smartcardFileSize_2 = Convert.ToInt32(FileSize_2, 16);

                                    if (debug)
                                    {
                                        systemLog("File 1 size <" + smartcardFileSize_1 + "> File 2 size <" + smartcardFileSize_2 + ">");
                                    }

                                    checkFile();
                                    break;
                                }
                                else if (prevCommand.Equals("card_file_size") && !smartcardCode.Equals("9000"))
                                {
                                    systemLog("Error receiving file size");
                                    break;
                                }

                                if (prevCommand.Equals("card_file_read") && smartcardCode.Equals("9000"))
                                {
                                    importFile(smartcardData);
                                    break;
                                }
                                else if (prevCommand.Equals("card_file_read") && !smartcardCode.Equals("9000"))
                                {
                                    systemLog("Error receiving file data");
                                    break;
                                }

                                if (prevCommand.Equals("card_file_delete") && smartcardCode.Equals("9000"))
                                {
                                    createNewFile();
                                    break;
                                }
                                else if (prevCommand.Equals("card_file_delete") && !smartcardCode.Equals("9000"))
                                {
                                    systemLog("Error deleting data on Smartcard");
                                    break;
                                }

                                if (prevCommand.Equals("card_file_create") && smartcardCode.Equals("9000"))
                                {
                                    systemLog("Uploading File to Smartcard Please wait.");
                                    sw2.Reset();
                                    sw2.Start();
                                    uploadFileToSC();
                                    break;
                                }
                                else if (prevCommand.Equals("card_file_create") && !smartcardCode.Equals("9000"))
                                {
                                    systemLog("Error creating File on Smartcard");
                                    break;
                                }

                                if (prevCommand.Equals("card_file_write") && smartcardCode.Equals("9000"))
                                {
                                    uploadFileToSC();
                                    break;
                                }
                                else if (prevCommand.Equals("card_file_write") && !smartcardCode.Equals("9000"))
                                {
                                    systemLog("Error uploading File on Smartcard");
                                    break;
                                }

                                if (prevCommand.Equals("card_delete_1") && smartcardCode.Equals("9000"))
                                {
                                    deleteAllData(2, "");
                                    break;
                                }
                                else if (prevCommand.Equals("card_delete_1") && !smartcardCode.Equals("9000"))
                                {
                                    switch (smartcardCode)
                                    {
                                        case "63C2":
                                            systemLog("Password wrong! 2 Tries remaining.");
                                            button_Delete_Data.PerformClick();
                                            break;
                                        case "63C1":
                                            systemLog("Password wrong! 1 Tries remaining.");
                                            button_Delete_Data.PerformClick();
                                            break;
                                        case "63C0":
                                            systemLog("Password wrong! 0 Tries remaining.");
                                            systemLog("PIN is Locked. Use PUK to unlock.");
                                            pinIsBlocked(false);
                                            break;
                                    }
                                    systemLog("Error deleting data");
                                    break;
                                }
                                else if (prevCommand.Equals("card_delete_2") && smartcardCode.Equals("9000"))
                                {
                                    deleteAllData(3, "");
                                    break;
                                }
                                else if (prevCommand.Equals("card_delete_2") && smartcardCode.Equals("9000"))
                                {
                                    deleteAllData(4, "");
                                    break;
                                }
                                else if ((prevCommand.Equals("card_delete_1") || prevCommand.Equals("card_delete_2") || prevCommand.Equals("card_delete_3")) && !smartcardCode.Equals("9000"))
                                {
                                    deleteAllData(5, "");
                                    break;
                                }

                                if (prevCommand.Equals("card_reset") && smartcardCode.Equals("9000"))
                                {
                                    if (MessageBox.Show("Smartcard reset successfull.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                                    {
                                        btChangeState("smartcardDisconnected");
                                    }
                                }
                                else if (prevCommand.Equals("card_reset") && !smartcardCode.Equals("9000"))
                                {
                                    switch (smartcardCode)
                                    {
                                        case "63C2":
                                            systemLog("PUK wrong! 2 Tries remaining.");
                                            button_Reset_Card.PerformClick();
                                            break;
                                        case "63C1":
                                            systemLog("PUK wrong! 1 Tries remaining.");
                                            button_Reset_Card.PerformClick();
                                            break;
                                        case "63C0":
                                            systemLog("PUK wrong! 0 Tries remaining.");
                                            systemLog("Smartcard is Locked!!!");
                                            break;
                                    }
                                    systemLog("Error resetting Smartcard");
                                    break;
                                }
                            }
                            else
                            {
                                systemLog("Command >" + prevCommand + "< Failed. With: " + _scCodes.getSmartcardResponse(data)[1]);
                            }
                        }
                        break;
                    case 3:
                        if (Get_BT_is_Connected())
                        {
                            prevCommand = lastCommand;
                            lastCommand = "";

                            if (prevCommand.Equals("card_personalize") && is_Smartcard_App_Connected)
                            {
                                if (data.Equals("scIsConnected"))
                                {
                                    dialogInitResult(true);
                                }
                                else
                                {
                                    dialogInitResult(false);
                                }
                                systemLog("Command >" + prevCommand + "< Successful");
                                break;
                            }

                            if (data.Equals("scIsDisconnected"))
                            {
                                systemLog("Command >" + prevCommand + "< Failed");
                                btChangeState("smartcardDisconnected");
                            }
                        }
                        break;
                }
            }
        }

        public void btChangeState(String state)
        {
            switch (state)
            {
                case "connecting":
                    //barcodeLabel.Visible = true;
                    //barcodeLabel.Text = barcodeLabel_Connecting;
                    break;

                case "connected":
                    barcodePicture.Image = Properties.Resources.barcode2;
                    pictureBoxBluetoothEnable.Image = Properties.Resources.Apps_Bluetooth_Active_icon;
                    textBoxConnectedDevice.Text = deviceName;
                    break;

                case "disconnected":
                    systemLog("Connection Lost!");
                    pictureBoxBluetoothEnable.Image = Properties.Resources.Apps_Bluetooth_Inactive_icon;
                    textBoxConnectedDevice.Text = "";
                    msgBoxClose("Connection Lost! Smartcard Terminal closing.");
                    break;

                case "smartcardDiscovered":
                    //systemLog("Smartcard Discovered");
                    // Enable Buttons
                    break;

                case "smartcardConnected":
                    //systemLog("Smartcard Connected");    
                    connectSCApp();
                    break;

                case "smartcardDisconnected":
                    systemLog("Smartcard disconnected");
                    is_Smartcard_App_Connected = false;
                    isSmartcardAuthenticated = false;
                    scPassword = String.Empty;
                    smartcardState = String.Empty;
                    masterPassword = false;
                    // Disable Buttons
                    button_OpenDatabase.Enabled = false;
                    button_UnlockDatabase.Enabled = false;
                    button_Import_File.Enabled = false;
                    button_Export_File.Enabled = false;
                    button_Get_MPW.Enabled = false;
                    button_Set_MPW.Enabled = false;
                    button_Delete_Data.Enabled = false;
                    button_Reset_Card.Enabled = false;
                    unlockFile = false;
                    openFile = false;
                    openFilePath = String.Empty;
                    openFilePW = String.Empty;
                    break;
                case "scAppletConnected":
                    systemLog("Smartcard connected");
                    //barcodeLabel.Visible = true;
                    //barcodeLabel.Text = barcodeLabel_SmartcardConnected;
                    //_bluetoothClass.isConnected = true;
                    is_Smartcard_App_Connected = true;
                    // Enable Buttons
                    break;
                case "scAppletPINBlocked":
                    systemLog("PIN is Blocked");
                    // Enable Buttons
                    break;
            }
        }

        private void sendAPDU(int code, String data2send, String apdu, Boolean noConvert)
        {
            String dataLength = "";
            String data = "";
            if (noConvert)
            {
                data = data2send;
                dataLength = _scCodes.StringToHex(data.Length / 2);
            }
            
            String sendData = apdu + dataLength + data;
            if (!SendMessage(code, sendData))
            {
                systemLog("Error sending APDU");
            }
        }

        private void sendAPDU(int code, String data2send, String apdu)
        {
            String dataLength = "";
            String data = "";
            if (data2send.Length >= 2)
            {
                data = _scCodes.checkLength(data2send);
                dataLength = _scCodes.StringToHex(data.Length / 2);
            }

            String sendData = apdu + dataLength + data;
            if (!SendMessage(code, sendData))
            {
                systemLog("Error sending APDU");
            }
        }

        private void connectSCApp()
        {
            lastCommand = "apduSelectApplet";
            if (!SendMessage(2, _scCodes.apduSelectApplet))
            {
                systemLog("Error sending APDU");
            }
        }

        private void initSmartcard()
        {
            systemLog("Card init");

            using (var form = new smartcard_Init())
            {
                var result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (!Get_BT_is_Connected())
                    {
                        if (MessageBox.Show("Smartcard Disconnected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            systemLog("Personalization canceled");
                            form.Close();
                        }
                    }
                    else
                    {
                        scPassword = form.pin;
                        lastCommand = "card_personalize";
                        if (!SendMessage(3, ""))
                        {
                            systemLog("Error sending APDU");
                        }
                    }
                }
                else
                {
                    systemLog("Personalization canceled");
                }
                form.Close();
            }
        }

        public void dialogInitResult(Boolean isConnected)
        {
            if (isConnected)
            {
                if (scPassword.Length > 0)
                {
                    if (is_Smartcard_App_Connected)
                    {
                        sendAPDU(2, scPassword, _scCodes.card_personalize);
                        lastCommand = "card_personalize";
                    }
                    else
                    {
                        systemLog("Failed: " + prevCommand);
                    }
                }
                else
                {
                    systemLog("Cancel personalization");
                }
            }
            else
            {
                btChangeState("smartcardDisconnected");
            }
        }

        private void initSmartcardResponse(String puk)
        {
            MessageBox.Show("Smartcard is now Personalized." + Environment.NewLine + "Please note the PUK: " + puk, "Smartcard Terminal", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //Open PIN Input method
            verifyPIN(true);
        }

        private void pinIsBlocked(Boolean correctPUK)
        {
            if (!correctPUK)
            {
                using (var form = new smartcard_pukInput())
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        tmpPUK = form.puk;
                        lastCommand = "pin_locked";
                        sendAPDU(2, encryptData(tmpPUK, true), _scCodes.card_pin_reset_enc, true);
                    }
                    else
                    {
                        systemLog("PUK input canceled");
                    }
                    form.Close();
                }
            }
            else
            {
                using (var form2 = new smartcard_Init())
                {
                    var result2 = form2.ShowDialog();

                    if (result2 == DialogResult.OK)
                    {
                        scPassword = form2.pin;
                        lastCommand = "pin_locked";
                        String dataToSend = encryptData(tmpPUK, true) + encryptData(form2.pin, true);
                        sendAPDU(2, dataToSend, _scCodes.card_pin_reset_enc, true);
                    }
                    else
                    {
                        systemLog("Personalization canceled");
                    }
                    form2.Close();
                }
            }  
        }

        private void verifyPIN(Boolean hasPassword)
        {
            lastCommand = "card_verify";

            if (hasPassword)
            {
                if (!Get_BT_is_Connected())
                {
                    if (MessageBox.Show("Database Saved", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        systemLog("PIN verify canceled");
                    }
                }
                else
                {
                    //sendAPDU(2, scPassword, _scCodes.card_verify);
 
                    sendAPDU(2, encryptData(scPassword, true), _scCodes.card_verify_enc);
                }
            }
            else
            {
                using (var form = new smartcard_pinInput())
                {
                    var result = form.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        if (!Get_BT_is_Connected())
                        {
                            if (MessageBox.Show("Database Saved", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                            {
                                systemLog("PIN verify canceled");
                                form.Close();
                            }
                        }
                        else
                        {
                            scPassword = form.PIN;
                            hasPassword = true;
                            //sendAPDU(2, scPassword, _scCodes.card_verify);
                            sendAPDU(2, encryptData(scPassword, true), _scCodes.card_verify_enc);
                        }
                    }
                    else
                    {
                        systemLog("PIN verify canceled");
                    }
                    form.Close();
                }
            }
        }

        private void cardUnlocked(Boolean response)
        {
            if (response)
            {
                //Enable File buttons
                if (masterPassword)
                {
                    button_Get_MPW.Enabled = true;
                    button_Delete_Data.Enabled = true;
                }
                if (smartcardState == _scCodes.STATE_SECURE_DATA)
                {
                    button_Import_File.Enabled = true;
                }

                button_Set_MPW.Enabled = true;

                if (!filePath.Equals(""))
                {
                    button_Export_File.Enabled = true;
                }
                else
                {
                    button_Export_File.Enabled = false;
                }

                if (smartcardHasFile)
                {
                    DateTime dt_smartcard = DateTime.ParseExact(smartcardFileModified, "yyyy-MM-d_HH-mm-ss", CultureInfo.InvariantCulture);

                    systemLog("File last modiefied: " + dt_smartcard.ToString("g", CultureInfo.CreateSpecificCulture("de-DE")));
                    systemLog("File on Smartcard: " + smartcardFileName + ".kdbx");
                    button_Import_File.Enabled = true;
                    button_Delete_Data.Enabled = true;
                    button_OpenDatabase.Enabled = true;
                }
                else
                {
                    button_Import_File.Enabled = false;
                    button_OpenDatabase.Enabled = false;
                }

                if (!masterPassword && !smartcardHasFile)
                {
                    button_Delete_Data.Enabled = false;
                }

                String f = lastFile.Substring(lastFile.LastIndexOf("\\") + 1);
                f = Path.GetFileNameWithoutExtension(f);
                if (f.Equals(smartcardFileName) && masterPassword)
                {
                    button_UnlockDatabase.Enabled = true;
                }
                else
                {
                    button_UnlockDatabase.Enabled = false;
                }

                button_Reset_Card.Enabled = true;
            }
            else
            {
                if (smartcardState == _scCodes.STATE_SECURE_DATA)
                {
                    lastCommand = "check_file";
                    //getFilename
                    sendAPDU(2, "", _scCodes.card_file_name);
                }
                else
                {
                    cardUnlocked(true);
                }
            }
        }

        private void button_UnlockDatabase_Click(object sender, EventArgs e)
        {
            unlockFile = true;
            lastCommand = "card_masterPW_get";
            sendAPDU(2, "", _scCodes.card_masterPW_get);
        }

        private void button_OpenDatabase_Click(object sender, EventArgs e)
        {
            openFile = true;
            openKDBXFile();
        }

        private void button_Import_File_Click(object sender, EventArgs e)
        {
            sw1.Reset();
            sw1.Start();
            lastCommand = "card_file_size";
            sendAPDU(2, "", _scCodes.card_file_size);
        }

        private void button_Export_File_Click(object sender, EventArgs e)
        {
            sw1.Reset();
            sw1.Start();
            exportData = fileHelper.ZIPFile(fileName, filePath, fileModified);
            fileOffset = 0;
            readLength = 250;

            Boolean import = true;

            if (smartcardState == _scCodes.STATE_SECURE_DATA)
            {
                if (fileName.Equals(smartcardFileName))
                {
                    DateTime dt_smartcard = DateTime.ParseExact(smartcardFileModified, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
                    DateTime dt_local = DateTime.ParseExact(fileModified, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);

                    int compareDate = DateTime.Compare(dt_smartcard, dt_local);

                    String txt;
                    if (compareDate < 0)
                    {
                        txt = "Local File is newer. Continue?";
                    }
                    else if (compareDate == 0)
                    {
                        txt = "Both Files are the same. Export anyway?";
                    }
                    else
                    {
                        txt = "File on Smartcard is newer. Override?";
                    }

                    if (MessageBox.Show("File already exists. " + txt, "Export Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        import = false;
                        return;
                    }
                }
                else
                {
                    if (MessageBox.Show("File " + smartcardFileName + ".kdbx already on Smartcard, override?", "Export Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        import = false;
                        return;
                    }
                }

                if (import)
                {
                    lastCommand = "card_file_delete";
                    sendAPDU(2, "", _scCodes.card_file_delete + "03");
                    return;
                }
            }
            createNewFile();
        }

        private void createNewFile()
        {
            int len = exportData.Length / 2;
            if (len > 60000)
            {
                MessageBox.Show("File is too big. Maximum size is 60KB.", "Export Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                readFile_1 = true;

                if (len > 30000)
                {
                    file_1_size = 30000;
                    file_2_size = len - 30000;
                    readFile_2 = true;
                }
                else
                {
                    file_1_size = len;
                    file_2_size = 0;
                    readFile_2 = false;
                }

                String hexSize_file_1 = _scCodes.StringToHex(file_1_size);
                String hexSize_file_2 = _scCodes.StringToHex(file_2_size);

                if (hexSize_file_1.Length == 2)
                {
                    hexSize_file_1 = "00" + hexSize_file_1;
                }

                if (hexSize_file_2.Length == 2)
                {
                    hexSize_file_2 = "00" + hexSize_file_2;
                }

                if (debug)
                {
                    systemLog("File size 1: " + hexSize_file_1 + ". File size 2: " + hexSize_file_2);
                }

                String sendName = fileName + "-" + fileModified + ".kdbx";
                String sendTMP = hexSize_file_1 + hexSize_file_2 + _scCodes.textStringToHex(sendName);
                lastCommand = "card_file_create";
                fileOffset = 0;
                readLength = 250;
                sendAPDU(2, sendTMP, _scCodes.card_file_create);
            }
        }

        private void uploadFileToSC()
        {
            String tmpSend;
            String off = String.Empty;
            // Export File 1
            if (readFile_1 && fileOffset != file_1_size)
            {
                if ((fileOffset + readLength) > file_1_size)
                {
                    readLength = file_1_size - fileOffset;
                }

                off = _scCodes.StringToHex(fileOffset);
                if (off.Length == 2)
                {
                    off = "00" + off;
                }

                tmpSend = off + exportData.Substring(fileOffset * 2, readLength * 2);
                fileOffset += readLength;

                lastCommand = "card_file_write";

                double per = (double)(fileOffset * 2) / (double)exportData.Length;
                updateSystemLog("Export:", per);

                sendAPDU(2, tmpSend, _scCodes.card_file_write + "01");
                return;
            }
            else if (readFile_1 && fileOffset == file_1_size)
            {
                readFile_1 = false;
                readLength = 250;
                fileOffset = 0;
                off = String.Empty;
            }

            // Export File 2
            if (readFile_2 && fileOffset != file_2_size && !readFile_1)
            {
                if ((fileOffset + readLength) > file_2_size)
                {
                    readLength = file_2_size - fileOffset;
                }

                off = _scCodes.StringToHex(fileOffset);
                if (off.Length == 2)
                {
                    off = "00" + off;
                }
                systemLog("Write: " + (60000 + (fileOffset * 2)) + "with: " + (readLength * 2));
                tmpSend = off + exportData.Substring((60000 + (fileOffset * 2)), (readLength * 2));

                fileOffset += readLength;

                lastCommand = "card_file_write";

                double per = (double)(60000 + (fileOffset * 2)) / (double)exportData.Length;
                updateSystemLog("Export:", per);

                sendAPDU(2, tmpSend, _scCodes.card_file_write + "02");
                return;
            }
            else if (readFile_2 && fileOffset == file_2_size && !readFile_1)
            {
                readFile_2 = false;
            }

            // Export Finish
            if (!readFile_1 && !readFile_2)
            {
                smartcardFileName = fileName;
                smartcardFileModified = fileModified;
                smartcardState = _scCodes.STATE_SECURE_DATA;
                button_Import_File.Enabled = true;
                button_OpenDatabase.Enabled = true;

                sw1.Stop();
                sw2.Stop();
                systemLog("File export complete");
                double time = sw1.Elapsed.TotalSeconds;
                double time2 = sw2.Elapsed.TotalSeconds;
                systemLog("Time: " + time.ToString("0.##") + " s || Data: " + exportData.Length / 2 + " B || Speed: " + ((exportData.Length / 2) / time2).ToString("0.##") + " B/s");
            }
        }

        private void button_Set_MPW_Click(object sender, EventArgs e)
        {
            using (var form = new smartcard_PasswordInput())
            {
                var result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (form.masterPassword.Equals(""))
                    {
                        lastCommand = "card_masterPW_delete";

                        sendAPDU(2, "", _scCodes.card_masterPW_delete);
                    }
                    else
                    {
                        lastCommand = "card_masterPW_set";
                        String password = _scCodes.textStringToHex(form.masterPassword);
                        String password_Length = _scCodes.StringToHex(password.Length / 2);
                        password = password.PadLeft(94, '0');
                        password = password_Length + password;

                        sendAPDU(2, encryptData(password, false), _scCodes.card_masterPW_set_enc, true);
                    }
                }
            }
        }

        private void button_Get_MPW_Click(object sender, EventArgs e)
        {
            lastCommand = "card_masterPW_get";
            sendAPDU(2, "", _scCodes.card_masterPW_get_enc);
        }

        private void button_Delete_Data_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete all Data on the Smartcard?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                deleteAllData(1, scPassword);
            }
        }

        private void deleteAllData(int number, String pin)
        {
            if (number == 1)
            {
                lastCommand = "card_delete_1";
                //sendAPDU(2, pin, _scCodes.card_verify);
                sendAPDU(2, encryptData(pin, true), _scCodes.card_verify_enc);
            }
            else if (number == 2)
            {
                if (smartcardHasFile)
                {
                    lastCommand = "card_delete_2";
                    sendAPDU(2, "", _scCodes.card_file_delete + "03");
                }
                else
                {
                    deleteAllData(3, "");
                }
            }
            else if (number == 3)
            {
                if (masterPassword)
                {
                    lastCommand = "card_delete_3";
                    sendAPDU(2, "", _scCodes.card_masterPW_delete);
                }
                else
                {
                    deleteAllData(4, "");
                }
            }
            else if (number == 4)
            {
                masterPassword = false;
                smartcardHasFile = false;
                button_Import_File.Enabled = false;
                button_OpenDatabase.Enabled = false;
                button_UnlockDatabase.Enabled = false;
                openFile = false;
                unlockFile = false;
                openFilePath = String.Empty;
                openFilePW = String.Empty;
                smartcardFileName = String.Empty;
                smartcardFileModified = String.Empty;
                smartcardState = _scCodes.STATE_SECURE_NO_DATA;
                systemLog("All Data successfully deleted.");
                button_Delete_Data.Enabled = false;
                button_UnlockDatabase.Enabled = false;
                button_Import_File.Enabled = false;
                button_Get_MPW.Enabled = false;
            }
            else
            {
                systemLog("Error deleting Data");
            }
        }

        private void button_Reset_Card_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Really want to reset Card?\n Press YES to reset the Smartcard,\n if you want to change Pin press No.", "Information", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (res == DialogResult.Yes)
            {
                using (var form = new smartcard_pukInput())
                {
                    form.label1.Text = "Please enter PUK to reset Smartcard:";
                    var result = form.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        lastCommand = "card_reset";
                        sendAPDU(2, encryptData(form.puk, true), _scCodes.card_reset_enc);
                    }
                }
            }
            else if (res == DialogResult.No)
            {
                using (var form = new smartcard_Init())
                {
                    var result = form.ShowDialog();
                    form.Text = "Change PIN";
                    form.label1.Text = "Please enter new PIN:";

                    if (result == DialogResult.OK)
                    {
                        lastCommand = "card_pin_change";
                        String pwNew = form.pin;
                        String pwOld = scPassword;

                        while (pwNew.Length < 32)
                        {
                            pwNew += "0";
                        }

                        while (pwOld.Length < 32)
                        {
                            pwOld += "0";
                        }

                        scPassword = form.pin;
                        String oldPW = encryptData(pwOld, true);
                        String newPW = encryptData(pwNew, true);
                        sendAPDU(2, oldPW + newPW, _scCodes.card_pin_change_enc);
                    }
                    else
                    {
                        systemLog("PIN change canceled");
                    }
                }
            }
        }

        private delegate void checkFileDelegate();
        private void checkFile()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new checkFileDelegate(this.checkFile));
            }
            else
            {
                systemLog("Select Folder");
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select Path to save KeePass File";
                    fbd.ShowNewFolderButton = true;
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        string[] files = Directory.GetFiles(fbd.SelectedPath + "\\");
                        
                        Boolean import = true;
                        if (files.Length > 0)
                        {
                            foreach (String f in files)
                            {
                                String file = f.Substring(f.LastIndexOf("\\") + 1);
                                file = Path.GetFileNameWithoutExtension(file);

                                if (file.Equals(smartcardFileName))
                                {
                                    String mod = File.GetLastWriteTime(f).ToString();
                                    DateTime dt_smartcard = DateTime.ParseExact(smartcardFileModified, "yyyy-MM-d_HH-mm-ss", CultureInfo.InvariantCulture);
                                    
                                    DateTime dt_local = DateTime.ParseExact(mod, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                                    int compareDate = DateTime.Compare(dt_smartcard, dt_local);
                                   
                                    String txt;
                                    if (compareDate < 0)
                                    {
                                        txt = "Local File is newer. Override?";
                                    }
                                    else if (compareDate == 0)
                                    {
                                        txt = "Both Files are the same. Import anyway?";
                                    }
                                    else
                                    {
                                        txt = "File on Smartcard is newer. Continue?";
                                    }

                                    openFilePath = f;

                                    if (MessageBox.Show("File already exists. " + txt, "Import Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                                    {
                                        import = false;
                                        if (compareDate == 0)
                                        {
                                            openKDBXFile();
                                        }
                                    }
                                }
                            }
                        }

                        if (import)
                        {
                            pathForFile = fbd.SelectedPath + "\\";
                            newKeePassFile = new StringBuilder();
                            readFile_1 = true;
                            readFile_2 = true;
                            if (smartcardFileSize_2 == 0)
                            {
                                readFile_2 = false;
                            }
                            fileOffset = 0;
                            readLength = 250;
                            systemLog("Importing file, please wait!");
                            sw2.Reset();
                            sw2.Start();
                            importFile("");
                        }
                    }
                }
            }
        }

        private void importFile(String data)
        {
            lastCommand = "card_file_read";

            // Import File 1
            if (fileOffset != smartcardFileSize_1 && readFile_1)
            {
                readLength = 250;

                if ((fileOffset + readLength) > smartcardFileSize_1)
                {
                    readLength = smartcardFileSize_1 - fileOffset;
                }

                String off = _scCodes.StringToHex(fileOffset);
                if (off.Length == 2)
                {
                    off = "00" + off;
                }

                String len = _scCodes.StringToHex(readLength);
                if (len.Length == 2)
                {
                    len = "00" + len;
                }

                if (data.Length > 0)
                {
                    newKeePassFile.Append(data);
                }

                fileOffset = fileOffset + readLength;

                double per = (double)fileOffset / (double)(smartcardFileSize_1 + smartcardFileSize_2);
                updateSystemLog("Import:", per);


                String sendData = off + len;
                sendAPDU(2, sendData, _scCodes.card_file_read + "01");
            }
            else if (fileOffset == smartcardFileSize_1 && readFile_1)
            {
                if (!readFile_2)
                {
                    newKeePassFile.Append(data);
                }
                readFile_1 = false;
                fileOffset = 0;
                readLength = 250;
            }

            // Import File 2
            if (fileOffset != smartcardFileSize_2 && readFile_2 && !readFile_1)
            {
                if ((fileOffset + readLength) > smartcardFileSize_2)
                {
                    readLength = smartcardFileSize_2 - fileOffset;
                }

                String off = _scCodes.StringToHex(fileOffset);
                if (off.Length == 2)
                {
                    off = "00" + off;
                }

                String len = _scCodes.StringToHex(readLength);
                if (len.Length == 2)
                {
                    len = "00" + len;
                }

                newKeePassFile.Append(data);
                fileOffset = fileOffset + readLength;

                double per = (double)(30000 + fileOffset) / (double)(smartcardFileSize_1 + smartcardFileSize_2);
                updateSystemLog("Import:", per);

                String sendData = off + len;
                sendAPDU(2, sendData, _scCodes.card_file_read + "02");
            }
            else if (fileOffset == smartcardFileSize_2 && readFile_2)
            {
                newKeePassFile.Append(data);
                readFile_2 = false;
            }

            // Import Finish
            if (!readFile_1 && !readFile_2)
            {
                sw2.Stop();
                Stopwatch sw3 = new Stopwatch();
                sw3.Start();

                systemLog("Unzipping...");
                fileHelper.unZIPFile(smartcardFileName, pathForFile, newKeePassFile.ToString());
                sw3.Stop();

                if (debug)
                {
                    systemLog("File 1 size: " + smartcardFileSize_1);
                    systemLog("File 2 size: " + smartcardFileSize_2);
                    systemLog("Import complete. Read: " + newKeePassFile.Length);
                    systemLog("Unzip: " + sw3.ElapsedMilliseconds);
                }
                
                openFilePath = pathForFile + smartcardFileName + ".kdbx";
                if (openFile)
                {
                    openKDBXFile();
                }
                else
                {
                    systemLog("Import complete.");
                    sw1.Stop();
                    double time = sw1.Elapsed.TotalSeconds;
                    double time2 = sw2.Elapsed.TotalSeconds;
                    systemLog("Time: " + time.ToString("0.##") + " s || Data: " + (smartcardFileSize_1 + smartcardFileSize_2) + " B || Speed: " + ((smartcardFileSize_1 + smartcardFileSize_2) / time2).ToString("0.##") + " B/s");
                }
            }
            //END IMPORT
        }

        private void unlockDB()
        {
            this.closing = true;
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void openKDBXFile()
        {
            if (openFilePath.Length == 0 && smartcardHasFile)
            {
                lastCommand = "card_file_size";
                sendAPDU(2, "", _scCodes.card_file_size);
                return;
            }
            else if (!smartcardHasFile)
            {
                systemLog("No File to Open");
                return;
            }

            if (openFilePW.Length == 0 && masterPassword)
            {
                lastCommand = "card_masterPW_get";
                sendAPDU(2, "", _scCodes.card_masterPW_get_enc);
                return;
            }

            Stop_BT_Connection();
            
            this.closing = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        //END Class
    }
}