using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace smartcardSupport
{
    class bluetoothClass : Form
    {
        //Bluetooth Variables
        private BluetoothRadio myRadio;
        public BluetoothListener btListener;
        public BluetoothClient client;
        private String btAddress;
        private String btName;

        private Guid MyServiceUuid = new Guid("{00001101-0000-1000-8000-00805F9B34FB}");

        private smartcardDialog scDialog;

        public Thread btAccept;
        private StreamWriter wtr_1;
        private StreamReader wtr_2;
        private CryptLib _crypt;
        private String aesSalt;
        private String aesKey;

        public Boolean isAuthenticated = false;
        public Boolean isConnected = false;

        public bluetoothClass(smartcardDialog scDialog)
        {
            this.scDialog = scDialog;
            _crypt = new CryptLib();
        }

        public Boolean checkBTDevice()
        {
            try
            {
                client = new BluetoothClient();
                myRadio = BluetoothRadio.PrimaryRadio;
                if (myRadio == null)
                {
                    scDialog.systemLog("No radio hardware or unsupported software stack");
                    return false;
                }
                RadioMode mode = myRadio.Mode;
                // Warning: LocalAddress is null if the radio is powered-off.
                BluetoothAddress addr = myRadio.LocalAddress;
                btAddress = addr.ToString("C");

                //myRadio.Mode = RadioMode.Discoverable;
                myRadio.Mode = RadioMode.Connectable;

                return true;
            }
            catch (Exception ex)
            {
                scDialog.systemLog("No radio hardware or unsupported software stack");
                return false;
            }
        }

        public String getBluetoothAddress()
        {
            return btAddress;
        }

        public String getBTName()
        {
            return btName;
        }

        public void startBTListener()
        {
            btListener = new BluetoothListener(MyServiceUuid);
            btListener.Authenticate = false;
            btListener.Start(1);

            btAccept = new Thread(new ThreadStart(acceptBT));
            btAccept.Start();
        }

        public void sendMSG(int code, String msg)
        {
            if (client.Connected)
            {
                Stream peerStream = client.GetStream();
                wtr_1 = new StreamWriter(peerStream);

                try
                {
                    string tmp = _crypt.encrypt(msg, aesKey, aesSalt);

                    wtr_1.WriteLine(code + ">>" + tmp);
                    wtr_1.Flush();
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error sending Data: " + e);
                }

            }
        }

        /*
         * Delegate Methods
         */
        private delegate void reciveMSGDelegate(String s);
        void reciveMSG(String msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new reciveMSGDelegate(this.reciveMSG), msg);
            }
            else
            {
                if (msg.Length > 0)
                {
                    String[] stringSeparators = new String[] { ">>" };
                    String[] message = msg.Split(stringSeparators, StringSplitOptions.None);
                    String decryptmsg = _crypt.decrypt(message[1], aesKey, aesSalt);

                    int code = Int32.Parse(message[0]);
                    if (code.Equals(1) && decryptmsg.Equals("smartcard_disconnected"))
                    {
                        isConnected = false;
                    }
                    scDialog.receiveMessage(code, decryptmsg);
                }
            }
        }

        public void connectionStop()
        {
            if (client.Connected)
            {
                sendMSG(1, "application_stop");
            }

            if (btListener != null)
            {
                try
                {
                    btListener.Stop();
                }
                catch (Exception) { }
            }

            if (btAccept != null)
            {
                if (btAccept.IsAlive)
                {
                    btAccept.Abort();
                }
            }
                    
            if (client.Connected)
            {
                client.Close();
            }
        }

        private delegate void connectionrefusedDelegate();
        public void connectionrefused()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new connectionrefusedDelegate(this.connectionrefused));
            }
            else
            {
                scDialog.btChangeState("disconnected");
            }
        }

        private delegate void connectionStateDelegate(int state);
        public void connectionState(int state)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new connectionStateDelegate(this.connectionState), state);
            }
            else
            {
                switch (state)
                {
                    case 0:
                        scDialog.systemLog("Accept Connections");
                        break;
                    case 1:
                        scDialog.systemLog("Connection failed.");
                        break;
                    case 2:
                        scDialog.btChangeState("connecting");
                        //scDialog.systemLog("Connecting to " + client.RemoteMachineName);
                        break;
                }
            }
        }

        void acceptBT()
        {
            connectionState(0);

            try
            {
                client = btListener.AcceptBluetoothClient();
            }
            catch (Exception e)
            {
                connectionState(1);
            }

            if (client.Connected)
            {
                connectionState(2);
                aesSalt = scDialog.iv;
                aesKey = scDialog.key;
                btName = client.RemoteMachineName;
            }

            Stream peerStream = client.GetStream();
            wtr_2 = new StreamReader(peerStream);
            while (true)
            {
                try
                {
                    while (!wtr_2.EndOfStream)
                    {
                        String msg = wtr_2.ReadLine();
                        reciveMSG(msg);
                    }
                }
                catch (Exception e)
                {
                }

                connectionrefused();
                wtr_2.Close();  
            }
        }


    }
}
