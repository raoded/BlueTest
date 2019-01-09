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
using System.Collections.Concurrent;
using System.Threading;

[assembly: Dependency(typeof(AndroidBTConnector))]
namespace BlueTest.Droid {
    public class AndroidBTConnector : IConnector {
        private const int SAMPLING_TIME = 500;
        private const int QUEUE_SIZE = 100;
        private const int BUFFER_SIZE = 128;
        private BluetoothAdapter mBluetoothAdapter;
        private BluetoothSocket btSocket;
        private volatile bool isListening;
        private ConcurrentQueue<string> dataItems;
        public bool IsConnected { get; protected set; }

        //MAC Address of Bluetooth
        private static readonly string address = "98:D3:21:F4:8B:B6";
        //Unique ID for connection (does not really matter)
        private static readonly UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");



        public AndroidBTConnector() {
            Console.WriteLine("Constructing the Bluetooth device");
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            IsConnected = false;
            isListening = false;
            dataItems = new ConcurrentQueue<string>();
        }

        public async Task<bool> ConnectAsync() {
            return CheckBluetooth() && await InitConnection();
        }
        public async Task<bool> DisconnectAsync() {
            Console.WriteLine("Trying to disconnect");
            return await Task.Run(() => {
                try {
                    btSocket.Close();
                    IsConnected = false;
                    isListening = false;
                    Console.WriteLine("Disconnected from Bluetooth");
                    return true;
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                }
                return false;
            });
        }

        public async Task<string> RecieveAsync() {
            ListenToDevice();
            return await Task.Run(() => {
                string data=null;
                /*
                if (dataItems.TryDequeue(out data)) {
                    Console.WriteLine("consuming data from queue:" + data);
                }
                */
                dataItems.TryDequeue(out data);
                return data;

            });
        }

        public async Task SendAsync(string message) {
            if (IsConnected && !string.IsNullOrEmpty(message)) {
                await WriteData(message);
            }
        }

        public void ListenToDevice() {
            if (!isListening) {
                Console.WriteLine("Starting to listen to device");
                isListening = true;
                Task.Run(async () => {
                    while (isListening) {

                        string number = "";
                        string recived = "";
                        try {
                            recived = await ReadData();
                            for (int i = 0; i < recived.Length; i++) {
                                if (recived[i] == '(') {
                                    number = "";
                                    continue;
                                } else if (recived[i] == ')') {
                                    //Console.WriteLine("producing number to queue:" + number);
                                    dataItems.Enqueue(number);
                                    continue;
                                }
                                number += recived[i];
                            }
                            //dataItems.CompleteAdding();
                            Thread.Sleep(SAMPLING_TIME);
                        } catch (IOException ex) {
                            Console.WriteLine(ex);
                        }
                    }

                });
            }


        }

        //Event to initialize the thread that will listen to the bluetooth requests
        private async Task<string> ReadData() {
            //We extract the input stream
            if (IsConnected) {

                Stream inStream = btSocket.InputStream;
                byte[] buffer = new byte[BUFFER_SIZE];

                //inStream.Flush();
                int numRead = await inStream.ReadAsync(buffer, 0, buffer.Length);

                if (numRead > 0) {
                    return System.Text.Encoding.ASCII.GetString(buffer);
                } else
                    throw new IOException("Could not receive message");
            } else {
                throw new IOException("The Bluetooth adapter is disconnected");
            }

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
                Console.WriteLine("Fail to connect");
                Console.WriteLine(e.Message);
                await DisconnectAsync();
            }

            return IsConnected;
        }

    }
}