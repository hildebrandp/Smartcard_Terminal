using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

/// <summary>
/// Class for writing and reading Data from file
/// </summary>
namespace smartcardSupport
{
    static class fileHelper
    {
        /// <summary>
        /// Method writes Data to zip-file and unzips file
        /// </summary>
        /// <param name="path">Filepath</param>
        /// <param name="data">Fiule data</param>
        /// <returns>true is successfull</returns>
        public static Boolean unZIPFile(String path, String data)
        {
            try
            {
                smartcard_APDU_Codes smartcard_APDU_Codes = new smartcard_APDU_Codes();

                FileStream fs = File.Create(path + "keepassZIP.zip");
                Byte[] tmp = smartcard_APDU_Codes.hexToByteArray(data);
                fs.Write(tmp, 0, tmp.Length);
                fs.Close();

                ZipFile.ExtractToDirectory(path + "keepassZIP.zip", path);
                File.Delete(path + "keepassZIP.zip");
            } catch (Exception e)
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Method zips File and read data from File
        /// </summary>
        /// <param name="path">Filepath</param>
        /// <param name="data">Fiule data</param>
        /// <returns>Data from File</returns>
        public static String ZIPFile(String filename, String path)
        {
            try
            {
                String tmpPath = path.Substring(0, path.LastIndexOf("\\"));
                ZipArchive zip = ZipFile.Open(tmpPath + "\\" + "keepassZIP.zip", ZipArchiveMode.Create);
                zip.CreateEntryFromFile(path, filename + ".kdbx", CompressionLevel.Optimal);
                zip.Dispose();

                //FileStream fs = new FileStream(tmpPath + "\\" + "keepassZIP.zip", FileMode.Open, FileAccess.Read);
                Byte[] readData = File.ReadAllBytes(tmpPath + "\\" + "keepassZIP.zip");

                smartcard_APDU_Codes smartcard_APDU_Codes = new smartcard_APDU_Codes();
                String send = smartcard_APDU_Codes.byteToString(readData);

                File.Delete(tmpPath + "\\" + "keepassZIP.zip");
                return send;
           
            } catch (Exception e)
            {
                if (MessageBox.Show("Error Zip: " + e, "Export Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning) != DialogResult.OK)
                {
                }
                return "";
            }    
        }
    }
}
