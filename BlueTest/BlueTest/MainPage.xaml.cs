using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BlueTest
{
    public partial class MainPage : ContentPage
    {
        Connector connection;
        bool isConnected;
        public MainPage()
        {
            InitializeComponent();
            connection = DependencyService.Get<Connector>();
            isConnected = false;

        }

        private void clickConnect(object sender, EventArgs e)
        {
            if (connection != null && isConnected == false)
                isConnected = connection.connect();

            if (isConnected == true) {
                MainLable.Text = "Connected";              
            } else {
                MainLable.Text = "Unable To Connect";
            }
            /*
            if (!isConnected)
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        AzimutHeading.Text = await connection.RecieveAsync();
                        await Task.Delay(1000);
                    }
                });
            }
            */
            isConnected = true;
        }

        private async void clickMessage(object sender, EventArgs e)
        {
            string textSended = MainEntry.Text;
            string textRecieved;

            textSended = MainEntry.Text;
            connection.Send(textSended);

            textRecieved = await connection.RecieveAsync();

            if (textRecieved[0] > 9)
            {
                MainLable.Text = textRecieved;
            } 
        }
    }
}
