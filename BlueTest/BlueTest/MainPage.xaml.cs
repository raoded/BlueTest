using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlueTest {
    public partial class MainPage : ContentPage {
        Connector connection;
        bool isListening;
        public MainPage() {
            InitializeComponent();
            connection = DependencyService.Get<Connector>();
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
                    Console.Out.Flush();
                    Console.WriteLine(await connection.RecieveAsync());

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

        private void ClickDisconnect(object sender, EventArgs e) {
            if (connection.Disconnect()) {
                MainLable.Text = "Disconnected";
            } else {
                MainLable.Text = "Failed to Disconnect";
            }
        }
    }
}
