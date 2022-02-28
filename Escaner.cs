using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using WIA;
using System.Runtime.InteropServices;

namespace Corte_General.Publico
{
    public static class Escaner
    {
        public static ImageFile scan()
        {
            ImageFile resultado = null;
            try
            {
                // Create a DeviceManager instance
                var deviceManager = new DeviceManager();

                // Create an empty variable to store the scanner instance
                DeviceInfo firstScannerAvailable = null;
                string deviceInfosList = "";
                // Loop through the list of devices to choose the first available
                for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
                {
                    // Skip the device if it's not a scanner. '''
                    if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType && deviceManager.DeviceInfos[i].Type != WiaDeviceType.CameraDeviceType)
                    {
                        continue;
                    }
                    //deviceInfosList += i+" "+deviceManager.DeviceInfos[i].Properties ("Name").Value + "\n";

                    firstScannerAvailable = firstScannerAvailable is null ? deviceManager.DeviceInfos[i] : firstScannerAvailable;

                    //break;
                }
                MessageBox.Show("Lista de scanners:\n" + deviceInfosList + "\nSe usa el primero");
                // Connect to the first available scanner
                var device = firstScannerAvailable.Connect();
                /* Allow to chose device 
                    WIA.CommonDialog wiaDiag = new WIA.CommonDialog();
                    Device d = wiaDiag.ShowSelectDevice(WiaDeviceType.ScannerDeviceType | WiaDeviceType.CameraDeviceType, true, false);
                    device = d;
                */

                // Select the scanner
                var scannerItem = device.Items[1];

                if (false)
                {
                    // Retrieve a image in JPEG format and store it into a variable
                    var imageFile = (ImageFile)scannerItem.Transfer(FormatID: FormatID.wiaFormatJPEG);
                    resultado = imageFile;
                }
                else
                {
                    CommonDialogClass dlg = new CommonDialogClass();
                    object scanResult = dlg.ShowTransfer(scannerItem, WIA.FormatID.wiaFormatJPEG, true);

                    if (scanResult != null)
                    {
                        var imageFile = (ImageFile)scanResult;
                        resultado = imageFile;
                    }
                }
            }
            catch (COMException e)
            {
                // Convert the error code to UINT
                uint errorCode = (uint)e.ErrorCode;

                // See the error codes
                if (errorCode == 0x80210006)
                {
                    MessageBox.Show("Escaner ocupado o no disponible.");
                }
                else if (errorCode == 0x80210064)
                {
                    MessageBox.Show("El proceso de escaneo ha sido cancelado.");
                }
                else if (errorCode == 0x8021000C)
                {
                    MessageBox.Show("Configuracion incorrecta en el dispositivo WIA.");
                }
                else if (errorCode == 0x80210005)
                {
                    MessageBox.Show("Dispositivo desconectado. Asegurate de que este encendido y conectado a la PC.");
                }
                else if (errorCode == 0x80210001)
                {
                    MessageBox.Show("Error desconocido con el dispositivo WIA.");
                }
            }
            return resultado;
        }
        public static byte[] scan_to_byte()
        {
            return (byte[])Escaner.scan().FileData.get_BinaryData();
        }
    }
}
