using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Class for Smartcard commands and converting data
/// </summary>
namespace smartcardSupport
{
    class smartcard_APDU_Codes
    {
        /// <summary>
        /// send : apduSelectApplet
        /// get  : data (Card State) + SW1,SW2 (9000)
        /// </summary>
        public String apduSelectApplet = "00a40400081122334455667788";
        /// <summary>
        /// send : card_personalize + LC (Pin length) + data (Pin)
        /// get  : data (Puk) + SW1,SW2 (9000)
        /// </summary>
        public String card_personalize = "80200001";
        /// <summary>
        /// send : card_verify + LC (Pin length) + data (Pin)
        /// get  : data (Master PW State) + SW1,SW2 (9000)
        /// </summary>
        public String card_verify = "80210100";
        public String card_verify_enc = "80210101";
        /// <summary>
        /// send : card_pin_change + LC (data length) + data (old Pin + new Pin)
        /// get  : SW1,SW2 (9000)
        /// </summary>
        public String card_pin_change = "80220002";
        public String card_pin_change_enc = "80220001";
        /// <summary>
        /// send : card_pin_reset + LC (data length) + data (PUK + new Pin)
        /// get  : SW1,SW2 (9000)
        /// </summary>
        public String card_pin_reset = "80230102";
        public String card_pin_reset_enc = "80230101";
        /// <summary>
        /// send : card_masterPW_set + LC (data length) + data (Master Password)
        /// get  : SW1,SW2 (9000)
        /// </summary>
        public String card_masterPW_set = "80300201";
        public String card_masterPW_set_enc = "80300101";
        /// <summary>
        /// send : card_masterPW_get
        /// get  : data (Master Password) + SW1,SW2 (9000)
        /// </summary>
        public String card_masterPW_get = "80310202";
        public String card_masterPW_get_enc = "80310102";
        /// <summary>
        /// send : card_masterPW_delete
        /// get  : SW1,SW2 (9000)
        /// </summary>
        public String card_masterPW_delete = "80320203";
        /// <summary>
        /// send : card_reset + LC (data length) + data (PUK)
        /// get  : SW1,SW2 (9000)
        /// </summary>
        public String card_reset = "80240000";
        public String card_reset_enc = "80240001";
        /// <summary>
        /// send : card_file_create + LC (data length) + data (filesize 1 + filesize 2 + file name)
        /// get  : SW1,SW2 (9000)
        /// </summary>
        public String card_file_create = "80400301";
        /// <summary>
        /// send : card_file_write + file number + LC (data length) + data (file offset + data)
        /// get  : SW1,SW2 (9000)
        /// </summary>
        public String card_file_write = "804103";
        /// <summary>
        /// send : card_file_read + file number + LC (data length) + data (file offset + file length)
        /// get  : data (file data) + SW1,SW2 (9000)
        /// </summary>
        public String card_file_read = "804203";
        /// <summary>
        /// send : card_file_delete + file number (03 for Both)
        /// get  : SW1,SW2 (9000)
        /// </summary>
        public String card_file_delete = "804303";
        /// <summary>
        /// send : card_file_size
        /// get  : filesize 1 + filesize 2 + SW1,SW2 (9000)
        /// </summary>
        public String card_file_size = "80440304";
        /// <summary>
        /// send : card_file_name
        /// get  : file name + SW1,SW2 (9000)
        /// </summary>
        public String card_file_name = "80450101";


        public String STATE_INIT = "00";
        public String STATE_SECURE_NO_DATA = "01";
        public String STATE_SECURE_DATA = "02";
        public String STATE_PIN_LOCKED = "03";

        private char[] hexArray = "0123456789ABCDEF".ToCharArray();

        /// <summary>
        /// Method that return only Response-Code from Smartcard
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Response-Code</returns>
        public String[] getSmartcardResponse(String response)
        {
            String[] responseCode = new String[2];
            int dataLength = response.Length - 4;

            responseCode[0] = response.Substring(0, dataLength);
            responseCode[1] = response.Substring(dataLength);

            return responseCode;
        }

        /// <summary>
        /// Method for checking length of input
        /// has to be an even length
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Updated string</returns>
        public String checkLength(String data)
        {
            if ((data.Length % 2) != 0)
            {
                return "0" + data;
            }
            else
            {
                return data;
            }
        }

        /// <summary>
        /// Method for converting Byte-Array so string
        /// </summary>
        /// <param name="dataToConvert"></param>
        /// <returns>Data as string</returns>
        public String byteToString(byte[] dataToConvert)
        {
            StringBuilder sb = new StringBuilder();
            foreach(Byte b in dataToConvert)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Method which converts int number to hex number
        /// </summary>
        /// <param name="length">length as int</param>
        /// <returns>Hex length</returns>
        public String StringToHex(int length)
        {
            String hex = length.ToString("X");
            return checkLength(hex);
        }

        /// <summary>
        /// Method that converts String input to Hex-String
        /// </summary>
        /// <param name="input">Data input</param>
        /// <returns>Hex-String</returns>
        public String textStringToHex(String input)
        {
            Byte[] bytes = Encoding.UTF8.GetBytes(input);
            Char[] hexChars = new char[bytes.Length * 2];

            for (int j = 0; j < bytes.Length; j++)
            {
                int v = bytes[j] & 0xFF;
                hexChars[j * 2] = hexArray[v >> 4];
                hexChars[j * 2 + 1] = hexArray[v & 0x0F];
            }

            return new String(hexChars);
        }

        /// <summary>
        /// Method that converts hex-string to string
        /// </summary>
        /// <param name="hexData"></param>
        /// <returns></returns>
        public String dataHexToString(String hexData)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hexData.Length - 1; i += 2)
            {
                String output = hexData.Substring(i, 2);

                int dec = Convert.ToInt32(output, 16);

                sb.Append(Convert.ToChar(Convert.ToUInt32(output, 16)));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts string data to byte-array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] hexToByteArray(String data)
        {
            String hexchars = "0123456789abcdef";
            data = data.Replace(" ", "").ToLower();

            if (data == null)
            {
                return null;
            }

            Byte[] hex = new Byte[data.Length / 2];

            for (int ii = 0; ii < data.Length; ii += 2)
            {
                int i1 = hexchars.IndexOf(data.ElementAt(ii));
                int i2 = hexchars.IndexOf(data.ElementAt(ii + 1));
                hex[ii / 2] = (byte)((i1 << 4) | i2);
            }
            return hex;
        }
    }
}
