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

        public MainPage()
        {
            InitializeComponent();
            connection = DependencyService.Get<Connector>();
        }

        private void clickConnect(object sender, EventArgs e)
        {         
            bool connected = false;

            if (connection != null)
                connected = connection.connect();

            if (connected == false) {
                MainLable.Text = "Unable To Connect";
            } else {
                MainLable.Text = "Connected";
            }            
        }

        private async void clickMessage(object sender, EventArgs e)
        {
            string textSended = MainEntry.Text;
            string textRecieved;

            while (textSended != "close")
            {
                textSended = MainEntry.Text;
                connection.Send(textSended);
                textRecieved = await connection.RecieveAsync();
                MainLable.Text = textRecieved;
            }
        }
    }
}
