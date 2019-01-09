using Android.Content;
using Android.Telephony;
using BlueTest.Droid;
using System.Linq;
using Xamarin.Forms;
using static Android.Graphics.ColorSpace;
using Uri = Android.Net.Uri;
using System;
using Android.App;
using Android.Widget;
using Android.OS;
using System.IO;
using Java.Util;
using Android.Bluetooth;
using System.Threading.Tasks;
using System.Text;

[assembly: Dependency(typeof(AndroidBTConnector))]
namespace BlueTest.Droid {
    public class AndroidBTConnector : Connector {
        //Variables para el manejo del bluetooth Adaptador y Socket
        private BluetoothAdapter mBluetoothAdapter;
        private BluetoothSocket btSocket;
        public bool IsConnected { get; protected set; }
        //MAC Address of Bluetooth
        private static string address = "98:D3:21:F4:8B:B6";
        //Unique ID for connection (does not really matter)
        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");



        public AndroidBTConnector() {
            //Constructing the Bluetooth device 
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            IsConnected = false;
        }
        private bool CheckBluetooth() {
            return mBluetoothAdapter.Enable();
        }

        private async Task<bool> InitConnection() {
            //Start connecting to arduino device
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(address);
            System.Console.WriteLine("Connecting to device:" + device);

            //We indicate to the adapter that it is no longer visible
            mBluetoothAdapter.CancelDiscovery();
            try {
                //We initiate the communication socket with the arduino
                btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);

                //We connect the socket
                await btSocket.ConnectAsync();
                System.Console.WriteLine("Connection Initiated");
                IsConnected = true;
            } catch (System.Exception e) {
                //In case of generating errors, we close the socket
                Console.WriteLine(e.Message);
                try {
                    btSocket.Close();
                } catch (System.Exception) {
                    System.Console.WriteLine("Impossible to connect");
                }
                IsConnected = false;
            }

            return IsConnected;
        }

        //Event to initialize the thread that will listen to the bluetooth requests
        public async Task<String> RecieveAsync() {
            //We extract the input stream
            if (IsConnected) {
                Stream inStream = btSocket.InputStream;
                byte[] buffer = new byte[128];
                inStream.Flush();
                int numRead = await inStream.ReadAsync(buffer, 0, buffer.Length);
                if (numRead > 0) {

                    String read = System.Text.Encoding.ASCII.GetString(buffer);
                    return read;

                } else
                    throw new IOException("Could not receive message");
            } else {
                throw new IOException("The Bluetooth adapter is disconnected");
            }
            
        }


        public async Task<bool> ConnectAsync() {
            return CheckBluetooth() && await InitConnection();
        }

        //Method of sending data to the bluetooth
        private async Task WriteData(string data) {
            //We extract the output stream
            Stream outStream = btSocket.OutputStream;

            //we create the string that we will send
            String message = data;

            //we convert it into bytes
            byte[] msgBuffer = Encoding.ASCII.GetBytes(message);

            //We write the arrangement that we just generated in the buffer
            await outStream.WriteAsync(msgBuffer, 0, msgBuffer.Length);


        }

        public async Task SendAsync(string message) {
            if (IsConnected && !string.IsNullOrEmpty(message)) {
                await WriteData(message);
            }
        }

        public bool Disconnect() {
            try {
                btSocket.Close();
                IsConnected = false;
                return true;
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return false;
        }
    }
}