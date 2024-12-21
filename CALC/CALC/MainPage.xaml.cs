using Shiny.BluetoothLE.Hosting;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text;

namespace CALC
{
    public partial class MainPage : ContentPage
    {

        int code = 0;

        public MainPage()
        {
            InitializeComponent();
            Scanner.GotData += JumpToConnectionScreen;
        }

        private async void OnScanClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new Scanner());
            });
        }

        private async void OnManualClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new Manual());
            });
        }

        private void JumpToConnectionScreen(string ssid, string password, string ip)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Navigation.PushAsync(new Connecting(ssid, password, ip));
            });
        }
    }

}
