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

[assembly: Dependency(typeof(AndConnector))]
namespace BlueTest.Droid
{
    public class AndConnector : Connector
    {
        //Variables para el manejo del bluetooth Adaptador y Socket
        private BluetoothAdapter mBluetoothAdapter = null;
        private BluetoothSocket btSocket = null;
        //Streams de lectura I/O
        private Stream outStream = null;
        private Stream inStream = null;
        //MAC Address del dispositivo Bluetooth
        private static string address = "98:D3:21:F4:8B:B6";
        //Unique ID for connection
        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

        private bool CheckBt()
        {
            //Constructing the Bluetooth device 
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (!mBluetoothAdapter.Enable())
            {
                return false;
            }

            return true;
        }

        private bool initiateConnection()
        {
            //Start connecting to arduino device
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(address);
            System.Console.WriteLine("Connecting to device:" + device);

            //We indicate to the adapter that it is no longer visible
            mBluetoothAdapter.CancelDiscovery();
            try
            {
                //We initiate the communication socket with the arduino
                btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);
                //We connect the socket
                btSocket.Connect();
                System.Console.WriteLine("Connection Initiated");
            }
            catch (System.Exception e)
            {
                //In case of generating errors, we close the socket
                Console.WriteLine(e.Message);
                try
                {
                    btSocket.Close();
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("Impossible to connect");
                    return false;
                }
                System.Console.WriteLine("Socket Created");
            }
            
            return true;
        }
        
        //Event to initialize the thread that will listen to the bluetooth requests
        public async Task<String> RecieveAsync()
        {
            //We extract the input stream
           
                inStream = btSocket.InputStream;
                byte[] buffer = new byte[1024];
                int numRead=await inStream.ReadAsync(buffer, 0, buffer.Length);
            if (numRead > 0)
                return System.Text.Encoding.ASCII.GetString(buffer);
            else
                return null;
        }
        

        public bool connect()
        {
            bool BTCHK = CheckBt();

            if (BTCHK == false)
            {
                return false;
            }
            else {
                return initiateConnection();
            }
        }

        public async void Send(string message)
        {
            if (message != "")
            {
                writeData(message);
            }
            else
            {
                return;
            }
        }

        //Method of sending data to the bluetooth
        private async void writeData(string data)
        {
            //We extract the output stream
            try
            {
                outStream = btSocket.OutputStream;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error sending" + e.Message);
            }

            //we create the string that we will send
            String message = data;

            //we convert it into bytes
            byte[] msgBuffer = Encoding.ASCII.GetBytes(message);

            try
            {
                //We write the arrangement that we just generated in the buffer
               await outStream.WriteAsync(msgBuffer, 0, msgBuffer.Length);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Error sending" + e.Message);
            }
        }
    }
}