using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using QRCoder;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using Microsoft.VisualBasic;

namespace smartcardSupport
{
    public partial class smartcardDialog : Form
    {
        private smartcardSupportExt _scSupport;
        private bluetoothClass _bluetoothClass;
        private Boolean btDevice;
        private String btAdress;
        private String deviceName;

        private qrCodeClass _qrCodeClass;
        public String iv;
        public String key;
        public int pin = 0;

        private smartcard_APDU_Codes _scCodes;

        private String barcodeLabel_noBTdevice = "No Bluetooth Device found";
        private String barcodeLabel_enableBT = "Please Enable Bluetooth";
        private String barcodeLabel_Connecting = "Bluetooth Connecting";
        private String barcodeLabel_Connected = "Bluetooth Connected";
        private String barcodeLabel_SmartcardConnected = "Smartcard Connected";
        private String barcodeLabel_SmartcardDisconnected = "Smartcard Disconnected";

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

        private String fileName = String.Empty;
        private String filePath = String.Empty;
        private String fileModified = String.Empty;

        public Boolean debug = false;

        //Contructor for Form Class
        public smartcardDialog(int startcode, smartcardSupportExt scSupportExt, String fileName, String filePath, String fileModified)
        {
            InitializeComponent();

            this.fileModified = fileModified;
            this.fileName = fileName;
            this.filePath = filePath;

            //Add Event if Form is Closing
            this.FormClosing += new FormClosingEventHandler(smartcardDialog_FormClosing);

            //Get Version number and Set Window title
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            this.Text = "Bluetooth SmartCard Reader Terminal- " + version + "  ||  Database: " + fileName;

            if (debug)
            {
                systemLog("File path: " + filePath);
                systemLog("File name: " + fileName);
                systemLog("File last Modified: " + fileModified);
            }


            _qrCodeClass = new qrCodeClass();
            _scCodes = new smartcard_APDU_Codes();
            _bluetoothClass = new bluetoothClass(this);
            //_scSupport = new scSupportExt;

            btDevice = _bluetoothClass.checkBTDevice();
            if (!btDevice)
            {
                msgBoxClose("No Accessible Bluetooth Device! Closing");
            }
            else
            {
                btAdress = _bluetoothClass.getBluetoothAddress();
                systemLog("Bluetooth enabled");
                pictureBoxBluetoothEnable.Image = Properties.Resources.Apps_Bluetooth_Inactive_icon;
                textBoxConnectedDevice.Text = "";
                barcodePicture.Image = _qrCodeClass.newQRCode(btAdress);

                iv = _qrCodeClass.getSalt();
                key = _qrCodeClass.getKey();
                pin = _qrCodeClass.getPin();

                barcodeLabel.Visible = false;
                _bluetoothClass.startBTListener();
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
                    _bluetoothClass.connectionStop();
                    return;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                _bluetoothClass.connectionStop();
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
                String logTime = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                listBoxSystemLog.Items.Insert(0, logTime + " >> " + message);
            }
        }

        public void keepassConnector(int trigger)
        {
            switch (trigger)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
        }

        public void receiveMessage(int code, String data)
        {
            switch (code)
            {
                case 0:
                    if (pin.Equals(Convert.ToInt32(data)))
                    {
                        _bluetoothClass.isAuthenticated = true;
                        systemLog("Successfully connected");

                        btChangeState("connected");
                        _bluetoothClass.sendMSG(1, "pin_correct");
                    }
                    else
                    {
                        systemLog("Connection failed");
                        _bluetoothClass.sendMSG(1, "pin_wrong");
                        _bluetoothClass.client.Close();
                        msgBoxClose("Connection Failed! Smartcard Terminal closing.");
                    }
                    break;
                case 1:
                    if (_bluetoothClass.isAuthenticated)
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
                    if (_bluetoothClass.isAuthenticated)
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
                                        pinIsBlocked();
                                        break;
                                }
                                break;
                            }

                            if (prevCommand.Equals("pin_locked") && smartcardCode.Equals("9000"))
                            {
                                verifyPIN(true);
                                break;
                            }
                            else if (prevCommand.Equals("pin_locked") && !smartcardCode.Equals("9000"))
                            {
                                switch (smartcardCode)
                                {
                                    case "63C2":
                                        systemLog("PUK wrong! 2 Tries remaining.");
                                        pinIsBlocked();
                                        break;
                                    case "63C1":
                                        systemLog("PUK wrong! 1 Tries remaining.");
                                        pinIsBlocked();
                                        break;
                                    case "63C0":
                                        systemLog("PUK wrong! 0 Tries remaining.");
                                        systemLog("Card is Locked forever!!!");
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
                                pinIsBlocked();
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
                                String file = _scCodes.dataHexToString(smartcardData);
                                int cardFileLength = file.Length;
                                
                                smartcardFileModified = file.Substring(cardFileLength - 24, 19);
                                smartcardFileName = file.Substring(0, cardFileLength - 25); 

                                systemLog("Found File <" + smartcardFileName + "> Modified <" + smartcardFileModified + ">");
                                cardUnlocked(true);
                                break;
                            }
                            else if (prevCommand.Equals("check_file") && !smartcardCode.Equals("9000"))
                            {
                                systemLog("Error getting Filename");
                            }

                            if (prevCommand.Equals("card_masterPW_set") && smartcardCode.Equals("9000"))
                            {
                                systemLog("Masster Password successfully uploaded");
                                masterPassword = true;
                                button_Get_MPW.Enabled = true;
                                break;
                            }
                            else if (prevCommand.Equals("card_masterPW_set") && !smartcardCode.Equals("9000"))
                            {
                                systemLog("Masster Password upload failed");
                                break;
                            }

                            if (prevCommand.Equals("card_masterPW_get") && smartcardCode.Equals("9000"))
                            {
                                systemLog("Master Password: <" + _scCodes.dataHexToString(smartcardData) + ">");
                                break;
                            }
                            else if (prevCommand.Equals("card_masterPW_get") && !smartcardCode.Equals("9000"))
                            {
                                systemLog("Getting Master Password Failed");
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
                    if (_bluetoothClass.isAuthenticated)
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

        public void btChangeState(String state)
        {
            switch (state)
            {
                case "connecting":
                    //barcodeLabel.Visible = true;
                    //barcodeLabel.Text = barcodeLabel_Connecting;
                    break;

                case "connected":
                    //barcodeLabel.Visible = true;
                    //barcodeLabel.Text = barcodeLabel_Connected;
                    pictureBoxBluetoothEnable.Image = Properties.Resources.Apps_Bluetooth_Active_icon;
                    textBoxConnectedDevice.Text = _bluetoothClass.getBTName();
                    break;

                case "disconnected":
                    systemLog("Connection Lost!");
                    pictureBoxBluetoothEnable.Image = Properties.Resources.Apps_Bluetooth_Inactive_icon;
                    textBoxConnectedDevice.Text = "";
                    _bluetoothClass.isAuthenticated = false;
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
                    //barcodeLabel.Visible = true;
                    //barcodeLabel.Text = barcodeLabel_SmartcardDisconnected;
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
                    break;
                case "scAppletConnected":
                    systemLog("Smartcard connected");
                    //barcodeLabel.Visible = true;
                    //barcodeLabel.Text = barcodeLabel_SmartcardConnected;
                    _bluetoothClass.isConnected = true;
                    is_Smartcard_App_Connected = true;
                    // Enable Buttons
                    break;
                case "scAppletPINBlocked":
                    systemLog("PIN is Blocked");
                    // Enable Buttons
                    break;
            }
        }

        private void sendAPDU(int code, String data2send, String apdu)
        {
            String dataLength = "";
            String data = "";
            if (data2send.Length >= 2)
            {
                dataLength = _scCodes.StringToHex(data2send.Length / 2);
                data = _scCodes.checkLength(data2send);
            }   

            String sendData = apdu + dataLength + data;
            _bluetoothClass.sendMSG(code, sendData);
        }

        private void connectSCApp()
        {
            lastCommand = "apduSelectApplet";
            _bluetoothClass.sendMSG(2, _scCodes.apduSelectApplet);
        }

        private void initSmartcard()
        {
            systemLog("Card init");

            using (var form = new smartcard_Init())
            {
                var result = form.ShowDialog();
                
                if (result == DialogResult.OK)
                {
                    if (!_bluetoothClass.isConnected)
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

                        _bluetoothClass.sendMSG(3, "");
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

        private void pinIsBlocked()
        {
            using (var form = new smartcard_pukInput())
            {
                var result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    using (var form2 = new smartcard_Init())
                    {
                        var result2 = form2.ShowDialog();
                        lastCommand = "pin_locked";

                        if (result2 == DialogResult.OK)
                        {
                            scPassword = form2.pin;

                            String dataToSend = form.puk + form2.pin;
                            sendAPDU(2, dataToSend, _scCodes.card_reset);
                        }
                        else
                        {
                            systemLog("Personalization canceled");
                        }
                        form.Close();
                    }
                }
                else
                {
                    systemLog("PUK input canceled");
                }
                form.Close();
            }
        }

        private void verifyPIN(Boolean hasPassword)
        {
            lastCommand = "card_verify";

            if (hasPassword)
            {
                if (!_bluetoothClass.isConnected)
                {
                    if (MessageBox.Show("Database Saved", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        systemLog("PIN verify canceled");
                    }
                }
                else
                {
                    sendAPDU(2, scPassword, _scCodes.card_verify);
                }   
            }
            else
            {
                using (var form = new smartcard_pinInput())
                {
                    var result = form.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        if (!_bluetoothClass.isConnected)
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
                            sendAPDU(2, scPassword, _scCodes.card_verify);
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
                }
                if (smartcardState == _scCodes.STATE_SECURE_DATA)
                {
                    button_Import_File.Enabled = true;
                }
                button_Export_File.Enabled = true;
                button_Set_MPW.Enabled = true;
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

        public void databaseSaved(String filePath)
        {
            if (MessageBox.Show("Database Saved", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
            {
                //Do some stuff
            }
        }

        private void button_UnlockDatabase_Click(object sender, EventArgs e)
        {

        }

        private void button_OpenDatabase_Click(object sender, EventArgs e)
        {

        }

        private void button_Import_File_Click(object sender, EventArgs e)
        {

        }

        private void button_Export_File_Click(object sender, EventArgs e)
        {

        }

        private void button_Set_MPW_Click(object sender, EventArgs e)
        {
            using (var form = new smartcard_PasswordInput())
            {
                var result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    lastCommand = "card_masterPW_set";
                    String password = _scCodes.textStringToHex(form.masterPassword);

                    sendAPDU(2, password, _scCodes.card_masterPW_set);
                }
            }
        }

        private void button_Get_MPW_Click(object sender, EventArgs e)
        {
            lastCommand = "card_masterPW_get";
            sendAPDU(2, "", _scCodes.card_masterPW_get);
        }
    }
}