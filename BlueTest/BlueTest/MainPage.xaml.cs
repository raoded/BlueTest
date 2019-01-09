using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlueTest {
    public partial class MainPage : ContentPage {
        IConnector connection;
        bool isListening;
        public MainPage() {
            InitializeComponent();
            connection = DependencyService.Get<IConnector>();
            isListening = false;
        }

        private async void ClickConnect(object sender, EventArgs e) {
            bool success = false;
            try {
                if (connection != null && connection.IsConnected == false) {
                    success = await connection.ConnectAsync();
                }

                if (success == true) {
                    MainLable.Text = "Connected";
                } else {
                    MainLable.Text = "Unable To Connect (Maybe u are already connected?)";
                }
                //we want to start listen only if we are not already listening
                if (connection.IsConnected && !isListening) {
                    await ListenOnDevice();
                }
            } catch (Exception ex) {
                await DisplayAlert("Exception", ex.Message, "cancel");
            }

        }
        
        private async Task ListenOnDevice() {
            await Task.Run(async () => {
                while (true) {
                    Thread.Sleep(500);
                    /*
                      Device.BeginInvokeOnMainThread(async () => {
                          AzimutHeading.Text ="azimuth:"+ await connection.RecieveAsync();
                      });
                      */
                    string data = await connection.RecieveAsync();
                    if (data != null) {
                        Console.Out.Flush();
                        Console.WriteLine(data);
                    }
                    

                }
            });
            isListening = true;
        }
        

        private async void ClickSend(object sender, EventArgs e) {
            string textSended = MainEntry.Text;

            textSended = MainEntry.Text;
            await connection.SendAsync(textSended);

            //MainLable.Text = await connection.RecieveAsync();
        }

        private async void ClickDisconnect(object sender, EventArgs e) {
            if (await connection.DisconnectAsync()) {
                MainLable.Text = "Disconnected";
            } else {
                MainLable.Text = "Failed to Disconnect";
            }
        }
    }
}
